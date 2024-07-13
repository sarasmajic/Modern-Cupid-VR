using VRBuilder.Core.Behaviors;
using VRBuilder.Editor.UI.StepInspector.Menu;
using VRBuilder.Animations.Behaviors;

namespace VRBuilder.Editor.Animations.UI.Behaviors
{
    /// <inheritdoc />
    public class FollowPathMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Animation/Follow Path";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new FollowPathBehavior();
        }
    }
}
