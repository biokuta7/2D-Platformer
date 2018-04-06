using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class Holster : MonoBehaviour {

	[System.Serializable]
	public class GunItem
	{
		public Gun gun;
		[HideInInspector]
		public int level = 1;
		[HideInInspector]
		public float XP = 0;
		[HideInInspector]
		public int ammo = 0;

		public new string ToString() {
			return "LVL: " + level + "\nXP: " + XP;
		}

		public GunItem(Gun _gun) {
			gun = _gun;
			level = 1;
			XP = 0;
			ammo = gun.maxAmmo;
		}

	}

	public List<GunItem> inventory;
    public GunItem currentGunItem;
	public ParticleSystem muzzleFlash;

	private int currentGunID;
	private Vector3 muzzlePosition;
	private Vector3 direction;
	private float timer;
	private bool shot;
    SpriteRenderer spriteRenderer;
	Player player;

	private void Start() {
		player = Player.instance;
		spriteRenderer = GetComponent<SpriteRenderer> ();

		if (inventory.Count > 0) {
			EquipGun (inventory [0]);
		}

		InitInventory ();
	}

	private void Update() {
		if (!WorldState.paused) {
			Animation ();
			SwitchingWeapons ();
			Shooting ();
		}
	}

	public SpriteRenderer GetSpriteRenderer() {return spriteRenderer;}
	public bool Shot() { return shot; }
	public void AddAmmo(int amount) {currentGunItem.ammo = Mathf.Clamp (currentGunItem.ammo + amount, 0, currentGunItem.gun.maxAmmo);}

	public void RefillAmmo() {currentGunItem.ammo = currentGunItem.gun.maxAmmo;}

	public void RefillAllAmmo() {
		foreach (GunItem g in inventory) {
			g.ammo = g.gun.maxAmmo;
		}
	}

	public float GetXPPercent() { return currentGunItem.gun.GetXPPercent (currentGunItem.level, currentGunItem.XP); }
	public int GetCurrentWeaponID() { return currentGunID; }
	public bool AddGunToInventory(Gun g) {
		foreach (GunItem item in inventory) {
			if (item.gun.name.Equals (g.name)) {
				return false;
			}
		}

		GunItem gunItem = new GunItem (g);
		inventory.Add (gunItem);
		inventory [inventory.Count - 1].gun.Init ();
		return true;
	}

	public void AddXP(float amount) {
		currentGunItem.XP = Mathf.Clamp(currentGunItem.XP + amount, 0, currentGunItem.gun.GetMaxXP());
		currentGunItem.level = currentGunItem.gun.GetLevel (currentGunItem.XP);
	}


	public string GetWeaponInfoDisplay() {

		string ret = "";

		for (int i = 0; i < inventory.Count; i++) {

			bool current = currentGunID == i;

			if (current) {
				ret += "<color=red>";
			}

			ret += inventory [i].gun.name.ToUpper ();

			if (current) {
				ret += "</color>";
			}

			ret += "\n";


		}

		return ret;

	}


	private void InitInventory() {
		foreach (GunItem g in inventory) {
			g.gun.Init ();
		}

		RefillAllAmmo ();
	}
	private void EquipGun(GunItem g) {currentGunItem = g;}
	private void Shooting() {
		shot = false;

		timer += Time.deltaTime;

		if (

			(currentGunItem.gun.ammoMode.Equals (AmmoMode.UNLIMITED) || currentGunItem.ammo > 0) &&
			
			(Input.GetButtonDown ("Fire2") || (timer > currentGunItem.gun.GetCurrentGunLevel().fireDelay && (currentGunItem.gun.fireMode.Equals (FireMode.AUTO) && Input.GetButton ("Fire2"))))) {

			GameObject g = ObjectPooler.instance.SpawnFromPool (currentGunItem.gun.GetGunTag(currentGunItem.level), transform.position + muzzlePosition, Quaternion.identity);

			if (g != null) {

				shot = true;
				timer = 0;

				if (!player.stats.godMode) {
					AddAmmo(-1);
				}

				muzzleFlash.transform.localPosition = muzzlePosition;
				muzzleFlash.Emit (1);
				currentGunItem.gun.Shoot (transform.position + muzzlePosition, direction, g);
			
			}
		} else if (currentGunItem.gun.ammoMode.Equals (AmmoMode.RECHARGE) && timer > currentGunItem.gun.GetCurrentGunLevel().fireDelay) {

			timer = 0;
			AddAmmo (1);
		}

	}

	private void SwitchingWeapons() {

		int switchInput =
			Input.GetButtonDown("Fire3")? -1 : (
				Input.GetButtonDown("Fire4")? 1 : 0);

		if(switchInput != 0) {

			currentGunID += switchInput;

			if(currentGunID < 0) {
				currentGunID = inventory.Count - 1;
			} else if(currentGunID > inventory.Count - 1) {
				currentGunID = 0;
			}

			EquipGun(inventory[currentGunID]);

		}

	}

	private void Animation() {

		direction = (player.GetLookValue () > 0 || player.LookingDown ()) ?
			new Vector3 (
			0f, (player.GetLookValue () > 0 || player.LookingDown ()) ? (player.LookingDown () ? -1.0f : 1.0f) : 0) :
			new Vector3( (player.FacingRight () ? 1.0f : -1.0f), 0f, 0f);
			

		if (direction.y != 0) {

			float xPos = (direction.x) * (.125f - currentGunItem.gun.handleOffset.y);
			float yPos = (direction.y) * (currentGunItem.gun.handleOffset.x + currentGunItem.gun.handleOffset.z + .125f +  
				(player.GetHeadBob()? .0625f : 0f));


			spriteRenderer.flipY = player.LookingDown ();
			spriteRenderer.sprite = currentGunItem.gun.gunSpriteUp;
			spriteRenderer.flipX = (player.FacingRight () && player.LookingDown ()) || (!player.FacingRight () && !player.LookingDown ());
			transform.localPosition = new Vector3(xPos, yPos, 0.0f);

			muzzlePosition = new Vector3(Mathf.Sign(xPos) * (currentGunItem.gun.muzzlePosition.y + currentGunItem.gun.muzzlePosition.z), Mathf.Sign(yPos) * currentGunItem.gun.muzzlePosition.x, 0f);

		} else {

			float xPos = (direction.x) * (currentGunItem.gun.handleOffset.x + .125f);
			float yPos = currentGunItem.gun.handleOffset.y + -.375f + (player.GetHeadBob()? .0625f : 0);

			spriteRenderer.flipY = false;
			spriteRenderer.sprite = currentGunItem.gun.gunSpriteRight;
			spriteRenderer.flipX = !player.FacingRight ();
			transform.localPosition = new Vector3(xPos, yPos, 0.0f);
		
			muzzlePosition = new Vector3(Mathf.Sign(xPos) * currentGunItem.gun.muzzlePosition.x, currentGunItem.gun.muzzlePosition.y, 0f);
		
		}


	}

	void OnDrawGizmos() {

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (transform.position + muzzlePosition, .125f);

	}
		

}
