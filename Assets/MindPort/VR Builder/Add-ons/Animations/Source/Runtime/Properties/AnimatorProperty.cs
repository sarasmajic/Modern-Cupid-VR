using UnityEngine;
using VRBuilder.Core.Properties;

namespace VRBuilder.Animations.Properties
{
    /// <summary>
    /// Property that handles animations by changing parameters on an Animator component.
    /// </summary>
    public class AnimatorProperty : ProcessSceneObjectProperty, IAnimatorProperty
    {
        /// <summary>
        /// The animator component on this game object.
        /// </summary>
        public Animator Animator
        {
            get
            {
                if(animator == null)
                {
                    animator = GetComponent<Animator>();
                }

                if(animator == null)
                {
                    Debug.LogError($"Animator property on object {gameObject.name} could not find an animator.");
                }

                return animator;
            }
        }

        private Animator animator;

        /// <inheritdoc/>
        public void SetTrigger(string parameterName)
        {
            Animator.SetTrigger(parameterName);
        }

        /// <inheritdoc/>
        public void SetBool(string parameterName, bool value)
        {
            Animator.SetBool(parameterName, value);
        }

        /// <inheritdoc/>
        public void SetInteger(string parameterName, int value)
        {
            Animator.SetInteger(parameterName, value);
        }

        /// <inheritdoc/>
        public void SetFloat(string parameterName, float value)
        {
            Animator.SetFloat(parameterName, value);            
        }

        /// <inheritdoc/>
        public bool GetBool(string parameterName)
        {
            return Animator.GetBool(parameterName);
        }

        /// <inheritdoc/>
        public int GetInteger(string parameterName)
        {
            return Animator.GetInteger(parameterName);
        }

        /// <inheritdoc/>
        public float GetFloat(string parameterName)
        {
            return Animator.GetFloat(parameterName);
        }
    }
}