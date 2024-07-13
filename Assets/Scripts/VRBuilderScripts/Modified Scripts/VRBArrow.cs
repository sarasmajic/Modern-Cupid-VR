using System;
using System.Collections;
using UnityEngine;

public class VRBArrow : MonoBehaviour
{
	public float speed = 10f;
	public Transform tip;

	[SerializeField]
	private GameObject trail;

	[SerializeField]
	private GameObject destructionParticles;

	private Rigidbody _rigidbody;
	private bool _inAir = false;
	private Vector3 _lastPosition = Vector3.zero;

	private void Awake() {
		_rigidbody = GetComponent<Rigidbody>();
		PullInteractable.PullActionReleased += Release;
		Stop();
	}

	private void FixedUpdate() {
		if (_inAir) {
			_lastPosition = tip.position;
		}
	}

	private void OnCollisionEnter(Collision other) {
		if (_inAir) {
			Debug.Log("We hit something");
			destructionParticles.SetActive(true);
			Stop();
		}
	}

	private void OnDestroy() {
		PullInteractable.PullActionReleased -= Release;
	}

	private void Release(float value) {
		trail.SetActive(true);
		PullInteractable.PullActionReleased -= Release;
		gameObject.transform.parent = null;
		_inAir = true;
		SetPhysics(true);

		Vector3 force = transform.forward * value * speed;
		_rigidbody.AddForce(force, ForceMode.Impulse);

		StartCoroutine(RotateWithVelocity());

		_lastPosition = tip.position;
	}

	private IEnumerator RotateWithVelocity() {
		yield return new WaitForFixedUpdate();
		while (_inAir) {
			Quaternion newRotation = Quaternion.LookRotation(_rigidbody.velocity, transform.up);
			transform.rotation = newRotation;
			yield return null;
		}
	}


	private void Stop() {
		trail.GetComponent<ParticleSystem>().Stop(true);
		_inAir = false;
		SetPhysics(false);
	}

	private void SetPhysics(bool usePhysics) {
		_rigidbody.useGravity = usePhysics;
		_rigidbody.isKinematic = !usePhysics;
	}
}