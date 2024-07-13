using Newtonsoft.Json;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Scripting;
using VRBuilder.Core;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;

namespace VRBuilder.Animations.Behaviors
{
    /// <summary>
    /// Animates an object's transform over time to a new position, rotation and/or scale.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/animations-add-on")]
    public class AnimateTransformBehavior : Behavior<AnimateTransformBehavior.EntityData>
    {
        /// <summary>
        /// The "follow path" behavior's data.
        /// </summary>
        [DisplayName("Animate Transform")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            /// <summary>
            /// Target scene object to be transformed.
            /// </summary>
            [DataMember]
            [DisplayName("Object")]
            public SceneObjectReference Target { get; set; }

            /// <summary>
            /// Object defining the target transform values.
            /// </summary>
            [DataMember]
            [DisplayName("Final transform provider")]
            public SceneObjectReference TargetTransform { get; set; }

            /// <summary>
            /// Duration of the transition. If duration is equal or less than zero, the new transform will be applied immediately.
            /// </summary>
            [DataMember]
            [DisplayName("Duration (in seconds)")]
            public float Duration { get; set; }

            /// <summary>
            /// Determines the position of the object at a given time. The curve is normalized, the duration of the animation can be set in the <see cref="Duration"/> field.
            /// </summary>
            [DataMember]
            [DisplayName("Position curve")]
            public AnimationCurve PositionCurve { get; set; }

            /// <summary>
            /// Determines the rotation of the object at a given time. The curve is normalized, the duration of the animation can be set in the <see cref="Duration"/> field.
            /// </summary>
            [DataMember]
            [DisplayName("Rotation curve")]
            public AnimationCurve RotationCurve { get; set; }

            /// <summary>
            /// Determines the scale of the object at a given time. The curve is normalized, the duration of the animation can be set in the <see cref="Duration"/> field.
            /// </summary>
            [DataMember]
            [DisplayName("Scale curve")]
            public AnimationCurve ScaleCurve { get; set; }

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
                    string targetTransform = TargetTransform.IsEmpty() ? "[NULL]" : TargetTransform.Value.GameObject.name;
                    return $"Animate {target} to {targetTransform}";
                }
            }
        }

        private class ActivatingProcess : StageProcess<EntityData>
        {
            private float startingTime;
            private float repeats = 0;
            private Vector3 initialPosition;
            private Quaternion initialRotation;
            private Vector3 initialScale;

            public ActivatingProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Start()
            {
                startingTime = Time.time;
                initialPosition = Data.Target.Value.GameObject.transform.localPosition;
                initialRotation = Data.Target.Value.GameObject.transform.localRotation;
                initialScale = Data.Target.Value.GameObject.transform.localScale;

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

                if (movingTransform == null || movingTransform.Equals(null) || Data.TargetTransform.Equals(null))
                {
                    string warningFormat = "The process scene object's game object is null, transition movement is not completed, behavior activation is forcefully finished.";
                    warningFormat += "Target object unique name: {0}, Transform provider unique name: {1}";
                    Debug.LogWarningFormat(warningFormat, Data.Target.UniqueName, Data.TargetTransform.UniqueName);
                    yield break;
                }

                while (repeats < Data.Repeats)
                {
                    while (Time.time - startingTime < Data.Duration)
                    {
                        ProcessAnimation(movingTransform, GetAnimationProgress(Data.PositionCurve, false), GetAnimationProgress(Data.RotationCurve, false), GetAnimationProgress(Data.ScaleCurve, false));
                        yield return null;
                    }

                    if (Data.PingPong)
                    {
                        startingTime = Time.time;
                        while (Time.time - startingTime < Data.Duration)
                        {
                            ProcessAnimation(movingTransform, GetAnimationProgress(Data.PositionCurve, true), GetAnimationProgress(Data.RotationCurve, true), GetAnimationProgress(Data.ScaleCurve, true));
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
                float endPositionTime = Data.PingPong ? 0 : Data.PositionCurve.keys.Last().time;
                float endRotationTime = Data.PingPong ? 0 : Data.RotationCurve.keys.Last().time;
                float endScaleTime = Data.PingPong ? 0 : Data.ScaleCurve.keys.Last().time;

                ProcessAnimation(Data.Target.Value.GameObject.transform, Data.PositionCurve.Evaluate(endPositionTime), Data.RotationCurve.Evaluate(endRotationTime), Data.ScaleCurve.Evaluate(endScaleTime));

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

            private void ProcessAnimation(Transform movingTransform, float positionProgress, float rotationProgress, float scaleProgress)
            {
                Vector3 targetPosition = Data.TargetTransform.Value.GameObject.transform.position;
                Quaternion targetRotation = Data.TargetTransform.Value.GameObject.transform.rotation;
                Vector3 targetScale = Data.TargetTransform.Value.GameObject.transform.localScale;

                if(movingTransform.parent != null)
                {
                    targetPosition = movingTransform.parent.InverseTransformPoint(targetPosition);
                    targetRotation = Quaternion.Inverse(movingTransform.parent.rotation) * targetRotation;
                }

                movingTransform.localPosition = Vector3.Lerp(initialPosition, targetPosition, positionProgress);
                movingTransform.localRotation = Quaternion.Lerp(initialRotation, targetRotation, rotationProgress);
                movingTransform.localScale = Vector3.Lerp(initialScale, targetScale, scaleProgress);
            }

            private float GetAnimationProgress(AnimationCurve curve, bool isReversed)
            {
                if (isReversed)
                {
                    return Mathf.Clamp01(curve.Evaluate((Data.Duration - (Time.time - startingTime)) / Data.Duration * curve.keys.Last().time));
                }
                else
                {
                    return Mathf.Clamp01(curve.Evaluate((Time.time - startingTime) / Data.Duration * curve.keys.Last().time));
                }
            }
        }

        [JsonConstructor, Preserve]
        public AnimateTransformBehavior() : this("", "", 0f, null, null, null)
        {
            Data.Duration = 1;
            Data.PositionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            Data.RotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            Data.ScaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            Data.Repeats = 1;
        }

        public AnimateTransformBehavior(ISceneObject target, ISceneObject transformProvider, float duration, AnimationCurve positionCurve, AnimationCurve rotationCurve, AnimationCurve scaleCurve, bool pingPong = false, int repeats = 1) : this(ProcessReferenceUtils.GetNameFrom(target), ProcessReferenceUtils.GetNameFrom(transformProvider), duration, positionCurve, rotationCurve, scaleCurve, pingPong, repeats)
        {
        }

        public AnimateTransformBehavior(string targetName, string transformProviderName, float duration, AnimationCurve positionCurve, AnimationCurve rotationCurve, AnimationCurve scaleCurve, bool pingPong = false, int repeats = 1)
        {
            Data.Target = new SceneObjectReference(targetName);
            Data.TargetTransform = new SceneObjectReference(transformProviderName);
            Data.Duration = duration;
            Data.PositionCurve = positionCurve;
            Data.RotationCurve = rotationCurve;
            Data.ScaleCurve = scaleCurve;
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
