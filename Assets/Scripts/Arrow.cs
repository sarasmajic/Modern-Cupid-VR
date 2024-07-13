using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Arrow : MonoBehaviour
{
    private Rigidbody _rigidbody;

    public float Damage { get; set; } 
    
    void Start() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update() {
        if (_rigidbody.velocity.magnitude > 0f) {
            transform.rotation = Quaternion.LookRotation(_rigidbody.velocity);
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Floor") || other.gameObject.CompareTag("Enemy"))
            Destroy(gameObject);
    }
}
