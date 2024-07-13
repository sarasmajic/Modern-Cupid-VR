using Newtonsoft.Json;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Scripting;
using VRBuilder.Animations.Properties;
using VRBuilder.Core;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;

namespace VRBuilder.Animations.Behaviors
{
    /// <summary>
    /// Sets a float parameter on an animator to the specified value.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/animations-add-on")]
    public class SetAnimatorFloatParameterBehavior : Behavior<SetAnimatorFloatParameterBehavior.EntityData>
    {
        /// <summary>
        /// The <see cref="SetAnimatorFloatParameterBehavior"/> behavior's data.
        /// </summary>
        [DisplayName("Set Animator Float")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            /// <summary>
            /// Object with the animator property.
            /// </summary>
            [DataMember]
            [DisplayName("Animator")]
            public ScenePropertyReference<IAnimatorProperty> AnimatorProperty { get; set; }

            /// <summary>
            /// Name of the parameter to be changed.
            /// </summary>
            [DataMember]
            [DisplayName("Parameter name")]
            public string ParameterName { get; set; }

            /// <summary>
            /// New value for the selected parameter.
            /// </summary>
            [DataMember]
            [DisplayName("Target value")]
            public float TargetValue { get; set; }

            /// <summary>
            /// Timeframe during which the value is progressively changed.
            /// </summary>
            [DataMember]
            [DisplayName("Duration (in seconds)")]
            public float Duration { get; set; }

            /// <summary>
            /// Determines how fast the value changes at a given time. The curve is normalized, the duration of the animation can be set in the <see cref="Duration"/> field.
            /// </summary>
            [DataMember]
            [DisplayName("Animation curve")]
            public AnimationCurve AnimationCurve { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name
            {
                get
                {
                    string animatorProperty = AnimatorProperty.IsEmpty() ? "[NULL]" : AnimatorProperty.Value.SceneObject.GameObject.name;
                    string parameterName = string.IsNullOrEmpty(ParameterName) ? "[EMPTY]" : ParameterName;

                    return $"Set parameter {parameterName} to {TargetValue} on animator {animatorProperty}";
                }
            }
        }

        private class ActivatingProcess : StageProcess<EntityData>
        {
            private float startingTime;
            private float initialValue;

            public ActivatingProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Start()
            {
                initialValue = Data.AnimatorProperty.Value.GetFloat(Data.ParameterName);

                startingTime = Time.time;
            }

            /// <inheritdoc />
            public override IEnumerator Update()
            {
                float progress;

                while ((Time.time - startingTime) < Data.Duration)
                {
                    progress = Mathf.Clamp01(Data.AnimationCurve.Evaluate((Time.time - startingTime) / Data.Duration * Data.AnimationCurve.keys.Last().time));

                    Data.AnimatorProperty.Value.SetFloat(Data.ParameterName, Mathf.Lerp(initialValue, Data.TargetValue, progress));

                    yield return null;
                }
            }

            /// <inheritdoc />
            public override void End()
            {
                float progress = Mathf.Clamp01(Data.AnimationCurve.Evaluate(Data.AnimationCurve.keys.Last().time));
                Data.AnimatorProperty.Value.SetFloat(Data.ParameterName, Mathf.Lerp(initialValue, Data.TargetValue, progress));
            }

            public override void FastForward()
            {
            }
        }

        [JsonConstructor, Preserve]
        public SetAnimatorFloatParameterBehavior() : this("", "", 0.0f, 0.0f, null)
        {
            Data.AnimationCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
        }

        public SetAnimatorFloatParameterBehavior(IAnimatorProperty animatorProperty, string parameterName, float targetValue, float duration, AnimationCurve animationCurve) : this(ProcessReferenceUtils.GetNameFrom(animatorProperty), parameterName, targetValue, duration, animationCurve)
        {
        }

        public SetAnimatorFloatParameterBehavior(string animatorPropertyName, string parameterName, float targetValue, float duration, AnimationCurve animationCurve)
        {
            Data.AnimatorProperty = new ScenePropertyReference<IAnimatorProperty>(animatorPropertyName);
            Data.ParameterName = parameterName;
            Data.TargetValue = targetValue;
            Data.Duration = duration;
            Data.AnimationCurve = animationCurve;
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }
    }
}
