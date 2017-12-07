using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireMode {
	AUTO,
	SEMIAUTO
}

public enum AmmoMode {

	UNLIMITED,
	LIMITED,
	RECHARGE

}

public class Gun : MonoBehaviour {

	public string gunName;
    public Sprite gunSpriteRight;
	public Sprite gunSpriteUp;
	public Vector3 handleOffset;
	public Vector3 muzzlePosition;
	public FireMode fireMode = FireMode.SEMIAUTO;
	public AmmoMode ammoMode = AmmoMode.UNLIMITED;
	public GameObject bullet;
	public int bulletCount;
	public float fireDelay = .25f;

	public int maxAmmo;
	private int ammo;

	private void Start() {
		AddAmmo (1000);
	}

	public int GetAmmo() { return ammo; }

	public void AddAmmo(int a) {
		ammo = Mathf.Clamp (ammo + a, 0, maxAmmo);
	}

	public virtual void Shoot(Vector3 pos, Vector3 direction, GameObject g) {

		if (!ammoMode.Equals (AmmoMode.UNLIMITED)) {

			ammo -= 1;

		}

		g.layer = 0;

		Projectile p = g.GetComponent<Projectile> ();
		if (p != null) {
			p.Spawn (pos, direction);
		}
	}

}
