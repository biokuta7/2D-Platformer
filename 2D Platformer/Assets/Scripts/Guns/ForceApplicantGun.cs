using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceApplicantGun : Gun {

	public Player p;
	public float amount;
	public bool onlyY;

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
