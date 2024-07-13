using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class Persistent : MonoBehaviour
{
	private Persistent instance;

	private void Awake() {
		if (instance == null && GameObject.FindGameObjectsWithTag("Persistent").Length == 1) {
			instance = this;
			DontDestroyOnLoad(this);
		}
		else {
			Destroy(gameObject);
		}
	}
}