using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunDataDisplay : MonoBehaviour {

	public Holster holster;
	float timer = 30;
	Text text;

	private float startY;
	string gunName = "";

	void Start() {
		startY = transform.localPosition.y;
		text = GetComponent<Text> ();
	}

	void LateUpdate() {

		timer -= Time.deltaTime;

		if (gunName != holster.currentGun.gunName.ToUpper ()) {
			timer = 1;
			gunName = holster.currentGun.gunName.ToUpper ();
		}

		Color color = Color.white;

		if (timer > 0) {
			//text.color = Mathf.RoundToInt (Time.time * 10) % 2 == 0 ? Color.white : Color.black;
			color.a = Mathf.Clamp01(timer/.5f);
		} else {
			text.color = Color.white;
		}

		text.color = color;

		string shotColor = "white";

		transform.localPosition = new Vector3 (0, startY - Mathf.Clamp01(timer * 10) * 20, 0);

		string ammoData = "<color=" + shotColor + ">: " + holster.currentGun.GetAmmo () + " / " + holster.currentGun.maxAmmo + " :</color>";
		string gunData = (timer > 0 ? gunName : "");

		text.text =  gunData + "\n" + ammoData;
	}

}
