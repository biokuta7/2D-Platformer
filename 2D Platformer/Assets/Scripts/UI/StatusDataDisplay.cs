using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusDataDisplay : MonoBehaviour {

	public Player player;
	private Text text;

	int fuelPercent = 0;

	void Start () {
		text = GetComponent<Text> ();		
	}

	private void LateUpdate() {
		if (player != null) {
			TextUpdate ();
		}
	}

	private void TextUpdate () {
		fuelPercent += 3;

		float fuelDecimal = player.GetFuelPercentage () * 100;

		if (fuelPercent > fuelDecimal && fuelDecimal < 100) {
			fuelPercent = Mathf.RoundToInt(fuelDecimal);
		}

		Color fuelColor = Color.green;

		float alpha = .3f;

		if (fuelPercent < 10) {
			fuelColor = Mathf.RoundToInt (Time.time * 10) % 2 == 0 ? Color.red : Color.black;
			fuelColor.a = alpha;
		} else if (fuelPercent < 30) {
			fuelColor = Color.yellow;
			fuelColor.a = alpha;
		} else if (fuelPercent < 100) {
			fuelColor = Color.white;
			fuelColor.a = alpha;
		} else if (fuelPercent < 150) {
			fuelColor = Mathf.RoundToInt (Time.time * 20) % 2 == 0 ? Color.white : Color.black;
			fuelColor.a = alpha;
		} else if (fuelPercent < 200) {
			fuelColor = Color.white;
			fuelColor.a = alpha;
		} else {
			fuelColor = Color.clear;
		}

		Vector3 fuelPosition = Camera.main.WorldToScreenPoint (player.transform.position + Vector3.up) * 2;



		text.transform.position = (fuelPosition);
		text.color = fuelColor;
		text.text = (Mathf.Clamp(fuelPercent, 0, 100)) + "% FUEL";
	}
}
