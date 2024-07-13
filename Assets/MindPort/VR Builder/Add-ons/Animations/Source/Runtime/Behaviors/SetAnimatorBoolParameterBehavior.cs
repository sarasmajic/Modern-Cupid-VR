using Newtonsoft.Json;
using System.Collections;
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
    /// Sets a boolean parameter on an animator to the specified value.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/animations-add-on")]
    public class SetAnimatorBoolParameterBehavior : Behavior<SetAnimatorBoolParameterBehavior.EntityData>
    {
        /// <summary>
        /// The <see cref="SetAnimatorBoolParameterBehavior"/> behavior's data.
        /// </summary>
        [DisplayName("Set Animator Boolean")]
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
            [DisplayName("Parameter value")]
            public bool ParameterValue { get; set; }

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

                    return $"Set parameter {parameterName} to {ParameterValue} on animator {animatorProperty}";
                }
            }
        }

        private class ActivatingProcess : StageProcess<EntityData>
        {
            public ActivatingProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Start()
            {
            }

            /// <inheritdoc />
            public override IEnumerator Update()
            {
                yield return null;
            }

            /// <inheritdoc />
            public override void End()
            {
                Data.AnimatorProperty.Value.SetBool(Data.ParameterName, Data.ParameterValue);
            }

            public override void FastForward()
            {
            }
        }

        [JsonConstructor, Preserve]
        public SetAnimatorBoolParameterBehavior() : this("", "", false)
        {
        }

        public SetAnimatorBoolParameterBehavior(IAnimatorProperty animatorProperty, string parameterName, bool parameterValue) : this(ProcessReferenceUtils.GetNameFrom(animatorProperty), parameterName, parameterValue)
        {
        }

        public SetAnimatorBoolParameterBehavior(string animatorPropertyName, string parameterName, bool parameterValue)
        {
            Data.AnimatorProperty = new ScenePropertyReference<IAnimatorProperty>(animatorPropertyName);
            Data.ParameterName = parameterName;
            Data.ParameterValue = parameterValue;
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }
    }
}
