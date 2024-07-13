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
    /// Sets an integer parameter on an animator to the specified value.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/animations-add-on")]
    public class SetAnimatorIntParameterBehavior : Behavior<SetAnimatorIntParameterBehavior.EntityData>
    {
        /// <summary>
        /// The <see cref="SetAnimatorIntParameterBehavior"/> behavior's data.
        /// </summary>
        [DisplayName("Set Animator Integer")]
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
            public int ParameterValue { get; set; }

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
                Data.AnimatorProperty.Value.SetInteger(Data.ParameterName, Data.ParameterValue);
            }

            public override void FastForward()
            {
            }
        }

        [JsonConstructor, Preserve]
        public SetAnimatorIntParameterBehavior() : this("", "", 0)
        {
        }

        public SetAnimatorIntParameterBehavior(IAnimatorProperty animatorProperty, string parameterName, int parameterValue) : this(ProcessReferenceUtils.GetNameFrom(animatorProperty), parameterName, parameterValue)
        {
        }

        public SetAnimatorIntParameterBehavior(string animatorPropertyName, string parameterName, int parameterValue)
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
