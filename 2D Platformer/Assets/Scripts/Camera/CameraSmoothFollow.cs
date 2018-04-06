using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmoothFollow : MonoBehaviour {

    public bool killPlayerWhenOutOfBounds = true;
    public float dampingTime;
    public Transform target;

	public static CameraSmoothFollow instance;

    private Vector3 velocity;
    private Vector3 currentPosition;
    
	private Vector3 shakeOffset;
	private float magnitude;

	private void Awake() {
		instance = this;
	}

    private void LateUpdate()
    {

		ScreenShaking ();

		if (target != null) {
			Move ();
		}
        
    }

	public void SetTarget(Transform t) {
		target = t;
	}

	private void Move() {
		//Move camera

		Vector3 targetPosition = new Vector3(target.position.x, target.position.y);

		currentPosition = Vector3.SmoothDamp(currentPosition, targetPosition, ref velocity, dampingTime);

		float size = 16;

		transform.position = new Vector3(
			Mathf.Round((currentPosition.x * size)) / size,
			Mathf.Round((currentPosition.y * size)) / size,
			-10
		);

		transform.position += shakeOffset;
	}

	public void SetMagnitude(float m) {
		magnitude = m;
	}

	private void ScreenShaking() {

		if (magnitude > 0) {
			magnitude -= Time.deltaTime;
		} else {
			magnitude = 0;
		}

		shakeOffset = new Vector3 (Mathf.Sin (Time.time * 100) * magnitude, Random.Range (-1.0f, 1.0f) * magnitude, 0) * .25f;

	}

}