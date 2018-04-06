using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debugger : MonoBehaviour {

	public InputField field;

	public static Debugger instance;

	private string[] playercommands = new string[] {
		"refuel ---------------- set fuel back to 100%.",
		"addxp # --------------- add # xp to current weapon.",
		"addgun [ID] ----------- adds gun of specified ID to inventory. Use 'help weapons' command to find ID's.",
		"setgodmode [on/off] --- activates or deactivates god mode. God mode activates invincibility, unlimited ammo, unlimited fuel.",
		"refillammo ------------ refills current weapon ammo.",
		"refillammo all -------- refill all weapon ammo.",
		"teleport #1 #2 -------- teleports player to position (#1, #2).",
		"hurt # ---------------- hurts the player # health points." };

	private string weaponIDs = 
		"NEMESIS, MACHINE GUN, MISSILES, POLAR STAR";

	private bool active = false;

	public bool IsActive() {
		return active;
	}

	private void Awake() {
		instance = this;
	}

	private void Update() {

		if (Input.GetKeyDown (KeyCode.BackQuote)) {
			active = !active;
			WorldState.instance.Pause ();
		}

		if (active) {
			field.gameObject.SetActive (true);
			field.ActivateInputField ();
		} else {
			field.text = "";
			field.gameObject.SetActive (false);
		}

	}

	public void TerminalInput(string input) {

		string[] plots = input.ToLower().Split (new char[] { ' ', '.' }, System.StringSplitOptions.None);

		switch (plots[0]) {

		default:
			Debug.Log ("Not a command! Use 'help basic' for commands.");
			break;
		case "":
			break;

		case "help":
			switch (plots [1]) {
			default:
				Debug.Log ("Not a command!");
				break;
			case "basic":
				Debug.Log ("Use 'help player' for player commands. Use 'help weapons' for weapon ID's.");
				break;
			case "player":
				foreach (string s in playercommands) {
					Debug.Log (s);
				}
				break;
			case "weapons":
				Debug.Log (weaponIDs);
				break;
			}
			break;
		case "player":

			switch (plots [1]) {
			default:
				Debug.Log ("Not a command!");
				break;
			case "refuel":
				Player.instance.Refuel ();
				Debug.Log ("Fuel Refilled!");
				break;
			case "addxp":

				int value;
				int.TryParse (plots [2], out value);
				Player.instance.GetHolster ().AddXP (value);

				Debug.Log ("Added " + value + " EXP to gun " + Player.instance.GetHolster().currentGunItem.ToString());

				break;
			case "addgun":

				string name = plots [2];

				if (plots.Length > 3) {
					name += "_" + plots [3];
				}

				string path = "Data/Guns/" + name + "";

				Debug.Log ("attempting to load gun " + name + "...\n"+path);

				Gun g = Resources.Load(path) as Gun;

				bool alreadyInInventory = !Player.instance.GetHolster ().AddGunToInventory (g);

				Debug.Log (alreadyInInventory ? g.name + " is already in inventory!" : g.name + " has been added to inventory.");

				break;
			case "setgodmode":

				if (plots.Length > 2) {
					bool godmode = (plots [2].Equals ("true") || plots[2].Equals("on") || plots[2].Equals("active") ? true : false);

					Player.instance.SetGodMode (godmode);

				} else {
					Player.instance.SetGodMode ();
				}

				if (Player.instance.stats.godMode) {
					Debug.Log ("God Mode enabled.");
				} else {
					Debug.Log ("God Mode disabled.");
				}

				break;
			case "hurt":
				int hpToLose = int.Parse(plots[2]);

				Player.instance.GetHurt(hpToLose);

				break;

			case "refillammo":
				Player.instance.GetHolster ().RefillAmmo();
				break;
			case "refillammo all":
				Player.instance.GetHolster ().RefillAllAmmo();
				break;
			case "teleport":

				if (plots.Length > 3) {

					int x = int.Parse(plots [2]);
					int y = int.Parse(plots [3]);

					Vector2 pos = new Vector2(x,y);
					Player.instance.SetPosition(pos);

					Debug.Log ("Successfully teleported to {" + x + ", " + y + "}");

				}

				break;
			}


			break;


		}

	}

}