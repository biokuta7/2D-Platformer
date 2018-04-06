using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ContactDamageSurface : Surface {

	public float damageOnHit = 3.0f;

	void OnCollisionStay2D(Collision2D col) {

		if (col.gameObject.CompareTag ("Player")) {
			Player.instance.GetHurt (damageOnHit);
		}

	}

}
