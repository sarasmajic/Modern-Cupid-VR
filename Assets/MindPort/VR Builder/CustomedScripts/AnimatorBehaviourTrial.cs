using UnityEngine;

public class AnimateCauldron : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        // Get the Animator component of the specific object
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Example: Activate the "Open" trigger when a button is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Set the "Open" trigger to activate the corresponding animation or state transition
            animator.SetTrigger("Open");
        }

        // Example: Reset the "Open" trigger and activate the "Close" trigger
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Reset the "Open" trigger
            animator.ResetTrigger("Open");
            // Set the "Close" trigger to activate the corresponding animation or state transition
            animator.SetTrigger("Close");
        }
    }
}