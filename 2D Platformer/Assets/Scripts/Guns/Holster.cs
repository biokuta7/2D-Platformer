using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class Holster : MonoBehaviour {

	public List<Gun> inventory;
    public Gun currentGun;
	public ParticleSystem muzzleFlash;

	public int objectPoolLayer = 10;

	private int currentGunID;
	private Vector3 muzzlePosition;
	private Vector3 direction;
	private float timer;
	private bool shot;
    SpriteRenderer spriteRenderer;
	Player player;

	private List<GameObject> bullets;

	private void Start() {

		bullets = new List<GameObject> ();

		player = GetComponentInParent<Player> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();

		if (inventory.Count > 0) {
			EquipGun (inventory [0]);
		}

	}

	void OnDrawGizmos() {

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (transform.position + muzzlePosition, .125f);

	}

	public bool Shot() { return shot; }

	private GameObject GetNextBullet() {

		for (int i = 0; i < bullets.Count; i++) {
			if (bullets [i].layer == objectPoolLayer) {
				return bullets [i];
			}
		}

		return null;

	}

	private void EquipGun(Gun g)
    {

		currentGun = g;

		for (int i = 0; i < bullets.Count; i++) {
			Destroy (bullets [i]);
		}

		bullets.Clear ();

		for (int i = 0; i < g.bulletCount; i++) {

			GameObject c = Instantiate (g.bullet, Vector3.down * 100f, Quaternion.identity);
			c.layer = objectPoolLayer;
			c.name = currentGun.gunName.ToUpper() + " BULLET ID " + i;
			bullets.Add (c);

		}


    }

	private void Update() {
		Animation();
		SwitchingWeapons ();
		Shooting ();

	}

	private void Shooting() {

		shot = false;

		timer += Time.deltaTime;

		if (

			(currentGun.ammoMode.Equals (AmmoMode.UNLIMITED) || currentGun.GetAmmo () > 0) &&
			
			(Input.GetButtonDown ("Fire2")
			||
			(timer > currentGun.fireDelay && (currentGun.fireMode.Equals (FireMode.AUTO) && Input.GetButton ("Fire2"))))) {

			shot = true;
			timer = 0;

			GameObject g = GetNextBullet ();
			if (g != null) {
				muzzleFlash.transform.localPosition = muzzlePosition;
				muzzleFlash.Emit (1);
				currentGun.Shoot (transform.position + muzzlePosition, direction, g);
			}
		} else if (currentGun.ammoMode.Equals (AmmoMode.RECHARGE) && timer > currentGun.fireDelay) {

			timer = 0;
			currentGun.AddAmmo (1);

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

			float xPos = (direction.x) * (.125f - currentGun.handleOffset.y);
			float yPos = (direction.y) * (currentGun.handleOffset.x + currentGun.handleOffset.z + .125f +  
				(player.GetHeadBob()? .0625f : 0f));


			spriteRenderer.flipY = player.LookingDown ();
			spriteRenderer.sprite = currentGun.gunSpriteUp;
			spriteRenderer.flipX = (player.FacingRight () && player.LookingDown ()) || (!player.FacingRight () && !player.LookingDown ());
			transform.localPosition = new Vector3(xPos, yPos, 0.0f);

			muzzlePosition = new Vector3(Mathf.Sign(xPos) * (currentGun.muzzlePosition.y + currentGun.muzzlePosition.z), Mathf.Sign(yPos) * currentGun.muzzlePosition.x, 0f);

		} else {

			float xPos = (direction.x) * (currentGun.handleOffset.x + .125f);
			float yPos = currentGun.handleOffset.y + -.375f + (player.GetHeadBob()? .0625f : 0);

			spriteRenderer.flipY = false;
			spriteRenderer.sprite = currentGun.gunSpriteRight;
			spriteRenderer.flipX = !player.FacingRight ();
			transform.localPosition = new Vector3(xPos, yPos, 0.0f);
		
			muzzlePosition = new Vector3(Mathf.Sign(xPos) * currentGun.muzzlePosition.x, currentGun.muzzlePosition.y, 0f);
		
		}


	}
		

}
