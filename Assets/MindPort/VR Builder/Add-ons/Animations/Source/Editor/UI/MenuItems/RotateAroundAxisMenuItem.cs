using VRBuilder.Core.Behaviors;
using VRBuilder.Editor.UI.StepInspector.Menu;
using VRBuilder.Animations.Behaviors;

namespace VRBuilder.Editor.Animations.UI.Behaviors
{
    /// <inheritdoc />
    public class RotateAroundAxisMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Animation/Rotate Around Axis";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new RotateAroundAxisBehavior();
        }
    }
}
