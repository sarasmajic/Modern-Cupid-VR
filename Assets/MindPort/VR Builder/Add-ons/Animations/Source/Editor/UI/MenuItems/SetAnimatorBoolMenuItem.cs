using VRBuilder.Core.Behaviors;
using VRBuilder.Editor.UI.StepInspector.Menu;
using VRBuilder.Animations.Behaviors;

namespace VRBuilder.Editor.Animations.UI.Behaviors
{
    /// <inheritdoc />
    public class SetAnimatorBoolMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Animation/Set Animator Boolean";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new SetAnimatorBoolParameterBehavior();
        }
    }
}
