using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroyer : MonoBehaviour {

	public float time;

	private void Update() {
		time -= Time.deltaTime;

		if (time < 0f) {
			Destroy (gameObject);
		}
	}

}
