using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunDataDisplay : MonoBehaviour {

	public Text nameText;
	public Holster holster;
	float timer = 30;
	Text text;

	private new string name = "";

	void Start() {
		text = GetComponent<Text> ();
	}

	private void LateUpdate() {
		if (holster != null && holster.currentGun != null) {
			UpdateText ();
		}
	}

	private void UpdateText() {

		timer -= Time.deltaTime;

		if (name != holster.currentGun.name.ToUpper ()) {
			timer = 1;
			name = holster.currentGun.name.ToUpper ();
		}

		Color color = Color.white;

		if (timer > 0) {
			//text.color = Mathf.RoundToInt (Time.time * 10) % 2 == 0 ? Color.white : Color.black;
			color.a = Mathf.Clamp01(timer/.5f);
		}

		nameText.color = color;

		string shotColor = "white";

		//transform.localPosition = new Vector3 (0, startY - Mathf.Clamp01(timer * 10) * 20, 0);

		string ammoData = "<color=" + shotColor + ">: " + holster.currentGun.GetAmmo () + " / " + holster.currentGun.maxAmmo + " :</color>";
		string gunData = (timer > 0? 
			"<color=green>[A]\n</color>" + 
			holster.GetWeaponInfoDisplay () + 
			"<color=green>[S]</color>" : "");

		text.text = ammoData;
		nameText.text = gunData;
		nameText.rectTransform.localPosition = new Vector3 (-125, -40 + holster.GetCurrentWeaponID () * 5, 0);
	}

}
