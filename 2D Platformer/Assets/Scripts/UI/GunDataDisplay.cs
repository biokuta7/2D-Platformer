using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunDataDisplay : MonoBehaviour {

	public Text nameText;
	public Text dataText;

	public Slider XPSlider;
	public Slider HPSlider;

	float timer = 30;
	Text text;

	private new string name = "";
	private Player player;
	private Holster holster;
	void Start() {
		player = Player.instance;
		holster = player.GetHolster ();
		text = GetComponent<Text> ();
	}

	private void LateUpdate() {
		if (holster != null && holster.currentGunItem != null) {
			UpdateText ();
		}
	}

	private void UpdateText() {

		if (player.stats.dead) {
			nameText.text = "";
			dataText.text = "";
			text.text = "";
			return;
		}

		timer -= Time.deltaTime;

		string targetName = holster.currentGunItem.gun.name.ToUpper ();

		if (name != targetName) {
			timer = 1;
			name = targetName;
		}

		Color color = Color.white;

		if (timer > 0) {
			//text.color = Mathf.RoundToInt (Time.time * 10) % 2 == 0 ? Color.white : Color.black;
			color.a = Mathf.Clamp01(timer/.5f);
		}

		XPSlider.value = holster.GetXPPercent ();
		HPSlider.value = player.GetHealthPercentage ();

		nameText.color = color;
		dataText.color = color;
		string shotColor = "white";

		//transform.localPosition = new Vector3 (0, startY - Mathf.Clamp01(timer * 10) * 20, 0);

		string ammoString = "<color=" + shotColor + ">: " + holster.currentGunItem.ammo + " / " + holster.currentGunItem.gun.maxAmmo + " :</color>";
		string nameString = (timer > 0? 
			"<color=green>[A]\n</color>" + 
			holster.GetWeaponInfoDisplay () + 
			"<color=green>[S]</color>" : "");

		string dataString = (timer > 0?
			"LVL: " + holster.currentGunItem.level : "");
		
		text.text = ammoString;
		nameText.text = nameString;
		dataText.text = dataString;
		nameText.rectTransform.localPosition = new Vector3 (-125, -40 + holster.GetCurrentWeaponID () * 5, 0);
	}

}
