using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Items/Force Applicant Gun")]
public class ForceApplicantGun : Gun {

	private Player p;
	public float amount;
	public bool onlyY;

	public override void Init() {
		base.Init ();
		p = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();

	}

	public override void Shoot (Vector3 pos, Vector3 direction, GameObject g)
	{
		base.Shoot (pos, direction, g);

		Vector3 force = direction * -1.0f * amount;

		if (onlyY) {
			force.x = 0;
		}

		p.ApplyForce (force);

	}

}
