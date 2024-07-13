using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;
using VRBuilder.Core;
using VRBuilder.Core.Behaviors;
using System.Linq;
using VRBuilder.Core.Properties;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace VRBuilder.Animations.Behaviors
{
    /// <summary>
    /// Moves an object along a path. The object rotates to follow the path.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/animations-add-on")]
    public class FollowPathBehavior : Behavior<FollowPathBehavior.EntityData>
    {
        /// <summary>
        /// The "follow path" behavior's data.
        /// </summary>
        [DisplayName("Follow Path")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            /// <summary>
            /// Target scene object to be moved.
            /// </summary>
            [DataMember]
            [DisplayName("Object")]
            public SceneObjectReference Target { get; set; }

            /// <summary>
            /// Path the target is meant to follow.
            /// </summary>
            [DataMember]
            [DisplayName("Path")]
            public ScenePropertyReference<IPathProperty> Path { get; set; }

            /// <summary>
            /// If enabled, the object will keep its relative position to the path instead of being teleported on it when the animation starts.
            /// </summary>
            [DataMember]
            [DisplayName("Keep relative position")]
            public bool KeepRelativePosition { get; set; }

            /// <summary>
            /// If enabled, the object will keep its relative rotation to the path instead of having its forward vector pointing forward along the path.
            /// </summary>
            [DataMember]
            [DisplayName("Keep relative rotation")]
            public bool KeepRelativeRotation { get; set; }

            /// <summary>
            /// Duration of the transition. If duration is equal or less than zero, the object will be instantaneously moved at the end of the path.
            /// </summary>
            [DataMember]
            [DisplayName("Duration (in seconds)")]
            public float Duration { get; set; }

            /// <summary>
            /// Determines how fast the object moves at a given time. The curve is normalized, the duration of the animation can be set in the <see cref="Duration"/> field.
            /// </summary>
            [DataMember]
            [DisplayName("Velocity curve")]
            public AnimationCurve Velocity { get; set; }

            /// <summary>
            /// If enabled, the animation will play backwards.
            /// </summary>
            [DataMember]
            [DisplayName("Reverse")]
            public bool Reverse { get; set; }

            /// <summary>
            /// If enabled, the animation will repeat in reverse, returning the object to the starting position.
            /// </summary>
            [DataMember]
            [DisplayName("Ping pong")]
            public bool PingPong { get; set; }

            /// <summary>
            /// Number of times the animation should be repeated.
            /// </summary>
            [DataMember]
            [DisplayName("Repeats")]
            public int Repeats { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name
            {
                get
                {
                    string target = Target.IsEmpty() ? "[NULL]" : Target.Value.GameObject.name;
                    string path = Path.IsEmpty() ? "[NULL]" : Path.Value.SceneObject.GameObject.name;
                    return $"Move {target} along {path}";
                }
            }
        }

        private class ActivatingProcess : StageProcess<EntityData>
        {
            private float startingTime;
            private float repeats = 0;
            private Vector3 initialPosition;
            private Quaternion initialRotation;

            public ActivatingProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Start()
            {
                startingTime = Time.time;
                initialPosition = Data.Target.Value.GameObject.transform.position;
                initialRotation = Data.Target.Value.GameObject.transform.rotation;

                Rigidbody movingRigidbody = Data.Target.Value.GameObject.GetComponent<Rigidbody>();
                if (movingRigidbody != null)
                {
                    movingRigidbody.velocity = Vector3.zero;
                    movingRigidbody.angularVelocity = Vector3.zero;
                }
            }

            /// <inheritdoc />
            public override IEnumerator Update()
            {
                Transform movingTransform = Data.Target.Value.GameObject.transform;

                if (movingTransform == null || movingTransform.Equals(null) || Data.Path.Equals(null))
                {
                    string warningFormat = "The process scene object's game object is null, transition movement is not completed, behavior activation is forcefully finished.";
                    warningFormat += "Target object unique name: {0}, Path unique name: {1}";
                    Debug.LogWarningFormat(warningFormat, Data.Target.UniqueName, Data.Path.UniqueName);
                    yield break;
                }

                while (repeats < Data.Repeats)
                {
                    while (Time.time - startingTime < Data.Duration)
                    {
                        float progress = GetAnimationProgress(Data.Reverse);
                        ProcessAnimation(progress, movingTransform);
                        yield return null;
                    }

                    if (Data.PingPong)
                    {
                        startingTime = Time.time;
                        while (Time.time - startingTime < Data.Duration)
                        {
                            float progress = GetAnimationProgress(!Data.Reverse);
                            ProcessAnimation(progress, movingTransform);
                            yield return null;
                        }
                    }

                    repeats++;
                    startingTime = Time.time;
                }
            }

            /// <inheritdoc />
            public override void End()
            {
                float endValue = Data.PingPong ^ Data.Reverse ? 0 : Data.Velocity.keys.Last().time;
                ProcessAnimation(Data.Velocity.Evaluate(endValue), Data.Target.Value.GameObject.transform);

                Rigidbody movingRigidbody = Data.Target.Value.GameObject.GetComponent<Rigidbody>();
                if (movingRigidbody != null)
                {
                    movingRigidbody.velocity = Vector3.zero;
                    movingRigidbody.angularVelocity = Vector3.zero;
                }
            }

            public override void FastForward()
            {
            }

            private void ProcessAnimation(float progress, Transform movingTransform)
            {
                if (Data.KeepRelativePosition)
                {
                    movingTransform.position = initialPosition + Data.Path.Value.GetPoint(progress) - Data.Path.Value.GetPoint(0);
                }
                else
                {
                    movingTransform.position = Data.Path.Value.GetPoint(progress);
                }

                if (Data.KeepRelativeRotation)
                {
                    Quaternion deltaRotation = Quaternion.LookRotation(Data.Path.Value.GetDirection(progress)) * Quaternion.Inverse(Quaternion.LookRotation(Data.Path.Value.GetDirection(0)));
                    movingTransform.rotation = deltaRotation * initialRotation;
                }
                else
                {
                    movingTransform.rotation = Quaternion.LookRotation(Data.Path.Value.GetDirection(progress));
                }
            }

            private float GetAnimationProgress(bool isReversed)
            {
                if (isReversed)
                {
                    return Mathf.Clamp01(Data.Velocity.Evaluate((Data.Duration - (Time.time - startingTime)) / Data.Duration * Data.Velocity.keys.Last().time));
                }
                else
                {
                    return Mathf.Clamp01(Data.Velocity.Evaluate((Time.time - startingTime) / Data.Duration * Data.Velocity.keys.Last().time));
                }
            }
        }

        [JsonConstructor, Preserve]
        public FollowPathBehavior() : this("", "", 0f, null)
        {
            Data.Duration = 1;
            Data.Velocity = AnimationCurve.EaseInOut(0, 0, 1, 1);
            Data.Repeats = 1;
        }

        public FollowPathBehavior(ISceneObject target, IPathProperty path, float duration, AnimationCurve velocity, bool keepRelativePosition = false, bool keepRelativeRotation = false, bool reverse = false, bool pingPong = false, int repeats = 1) : this(ProcessReferenceUtils.GetNameFrom(target), ProcessReferenceUtils.GetNameFrom(path), duration, velocity, keepRelativePosition, keepRelativeRotation, reverse, pingPong, repeats)
        {
        }

        public FollowPathBehavior(string targetName, string pathName, float duration, AnimationCurve velocity, bool keepRelativePosition = false, bool keepRelativeRotation = false, bool reverse = false, bool pingPong = false, int repeats = 1)
        {
            Data.Target = new SceneObjectReference(targetName);
            Data.Path = new ScenePropertyReference<IPathProperty>(pathName);
            Data.Duration = duration;
            Data.Velocity = velocity;
            Data.KeepRelativePosition = keepRelativePosition;
            Data.KeepRelativeRotation = keepRelativeRotation;
            Data.Reverse = reverse;
            Data.PingPong = pingPong;
            Data.Repeats = repeats;
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }
    }
}
