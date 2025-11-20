using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour {
	Rigidbody rb;
	public float MaxForce;
	public float MaxTorque;
	public Vector3 ForceBase;
	public Vector3 TorqueBase;
	public int maxStall;
	public float endY;
	public bool DEBUG_RESET = false;
	public float gravityFactor;

	Vector3 spawn;
	float minY;
	int stallTime = 0;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	public event EventHandler Reset;

	protected virtual void OnReset(EventArgs e) {
		Reset?.Invoke(this, e);
	}

	void Start() {
		rb = GetComponent<Rigidbody>();
		spawn = transform.position;
		Drop();
	}

	void Drop() {
		rb.AddForce(ForceBase * Random.Range(-MaxForce, MaxForce), ForceMode.VelocityChange);
		rb.AddTorque(TorqueBase * Random.Range(-MaxTorque, MaxTorque), ForceMode.VelocityChange);
		minY = transform.position.y;
		stallTime = 0;
		OnReset(EventArgs.Empty);
	}

	// Update is called once per frame
	void FixedUpdate() {
		rb.AddForce(Physics.gravity * gravityFactor, ForceMode.Acceleration);
		if (transform.position.y <= endY) {
			DEBUG_RESET = true; //TODO handle ball result.
		} else if (transform.position.y < minY) {
			minY = transform.position.y;
			stallTime = 0;
		} else {
			stallTime++;
			if (stallTime >= maxStall) {
				DEBUG_RESET = true; //TODO handle ball stall.
			}
		}
		if (DEBUG_RESET) {
			transform.position = spawn;
			rb.linearVelocity = new Vector3(0, 0, 0);
			rb.angularVelocity = Vector3.zero;
			DEBUG_RESET = false;
			Drop();
		}
	}
}
