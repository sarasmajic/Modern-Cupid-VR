using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PullInteractable : XRBaseInteractable
{
	public static event Action<float> PullActionReleased;

	public Transform start, end;
	public GameObject notch;
	public float PullAmount { get; private set; } = 0.0f;

	private LineRenderer lineRenderer;
	private IXRSelectInteractor pullingInteractor = null;

	protected override void Awake() {
		base.Awake();
		lineRenderer = GetComponent<LineRenderer>();
	}

	public void SetPullInteractor(SelectEnterEventArgs args) {
		pullingInteractor = args.interactorObject;
	}
	
	// This signals the release action to all subscribed objects
	public void Release() {
		PullActionReleased?.Invoke(PullAmount);
		pullingInteractor = null;
		PullAmount = 0f;
		notch.transform.localPosition = new Vector3(notch.transform.localPosition.x, notch.transform.localPosition.y, 0f);
		UpdateString();
	}

	public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase) {
		base.ProcessInteractable(updatePhase);
		if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic) {
			if (isSelected) {
				Vector3 pullPosition = pullingInteractor.transform.position;
				PullAmount = CalculatePull(pullPosition);
				UpdateString();
			}
		}
	}

	private float CalculatePull(Vector3 pullPosition) {
		Vector3 pullDirection = pullPosition - start.position;
		Vector3 targetDirection = end.position - start.position;
		float maxLength = targetDirection.magnitude;
		
		targetDirection.Normalize();
		float pullValue = Vector3.Dot(pullDirection, targetDirection) / maxLength;
		return Mathf.Clamp(pullValue, 0, 1);
	}

	private void UpdateString() {
		Vector3 linePosition = Vector3.forward *
		                       Mathf.Lerp(start.transform.localPosition.z, end.transform.localPosition.z, PullAmount);
		notch.transform.localPosition = new Vector3(notch.transform.localPosition.x, notch.transform.localPosition.y,
						linePosition.z);
		lineRenderer.SetPosition(1, linePosition);
	}
}