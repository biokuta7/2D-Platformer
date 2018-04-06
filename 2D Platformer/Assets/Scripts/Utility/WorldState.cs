using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour {

	public static bool paused;

	public static WorldState instance;

	private void Awake() {
		instance = this;
	}

	public static void Pause(bool p) {

		paused = p;

		if (paused) {
			Time.timeScale = 0.0f;
		} else {
			Time.timeScale = 1.0f;
		}

	}

	public void Pause() {Pause (!paused);}

	public void InvokePause(float seconds) {

		Invoke ("Pause", seconds);

	}

}
