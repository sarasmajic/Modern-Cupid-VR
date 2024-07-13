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
    /// Sets a trigger parameter on an animator.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/animations-add-on")]
    public class SetAnimatorTriggerParameterBehavior : Behavior<SetAnimatorTriggerParameterBehavior.EntityData>
    {
        /// <summary>
        /// The <see cref="SetAnimatorTriggerParameterBehavior"/> behavior's data.
        /// </summary>
        [DisplayName("Set Animator Trigger")]
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

                    return $"Trigger parameter {parameterName} on animator {animatorProperty}";
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
                Data.AnimatorProperty.Value.SetTrigger(Data.ParameterName);
            }

            public override void FastForward()
            {
            }
        }

        [JsonConstructor, Preserve]
        public SetAnimatorTriggerParameterBehavior() : this("", "")
        {
        }

        public SetAnimatorTriggerParameterBehavior(IAnimatorProperty animatorProperty, string parameterName) : this(ProcessReferenceUtils.GetNameFrom(animatorProperty), parameterName)
        {
        }

        public SetAnimatorTriggerParameterBehavior(string animatorPropertyName, string parameterName)
        {
            Data.AnimatorProperty = new ScenePropertyReference<IAnimatorProperty>(animatorPropertyName);
            Data.ParameterName = parameterName;
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }
    }
}
