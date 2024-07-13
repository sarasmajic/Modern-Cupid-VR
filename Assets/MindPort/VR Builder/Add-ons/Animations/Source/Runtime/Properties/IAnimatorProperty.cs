using UnityEngine;
using VRBuilder.Core.Properties;

namespace VRBuilder.Animations.Properties
{
    /// <summary>
    /// Property for a game object that can be animated through parameters.
    /// </summary>
    public interface IAnimatorProperty : ISceneObjectProperty
    {
        /// <summary>
        /// The animator controlled by this property.
        /// </summary>
        Animator Animator { get; }

        /// <summary>
        /// Triggers an animation.
        /// </summary>        
        void SetTrigger(string parameterName);

        /// <summary>
        /// Sets a boolean parameter.
        /// </summary>
        void SetBool(string parameterName, bool value);

        /// <summary>
        /// Gets a boolean parameter.
        /// </summary>
        bool GetBool(string parameterName);

        /// <summary>
        /// Sets a integer parameter.
        /// </summary>
        void SetInteger(string parameterName, int value);

        /// <summary>
        /// Gets a integer parameter.
        /// </summary>
        int GetInteger(string parameterName);

        /// <summary>
        /// Sets a float parameter.
        /// </summary>
        void SetFloat(string parameterName, float value);

        /// <summary>
        /// Gets a float parameter.
        /// </summary>
        float GetFloat(string parameterName);   
    }
}