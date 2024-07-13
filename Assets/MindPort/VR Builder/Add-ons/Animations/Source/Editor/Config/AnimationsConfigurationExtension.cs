using System;
using System.Collections.Generic;
using VRBuilder.Editor.Configuration;
using VRBuilder.Editor.UI.Behaviors;

namespace VRBuilder.Editor.Animations.Configuration
{
    public class AnimationsConfigurationExtension : IEditorConfigurationExtension
    {
        public IEnumerable<Type> RequiredMenuItems
        {
            get
            {
                return new Type[0];
            }
        }

        public IEnumerable<Type> DisabledMenuItems
        {
            get
            {
                return new[] { typeof(MoveObjectMenuItem) };
            }
        }
    }
}
