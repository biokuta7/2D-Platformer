using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceleratingProjectile : Projectile {

	public float initialVelocity = 0;

	float acceleration;
	float maxSpeed;

	public override void Spawn (Vector3 pos, Vector3 dir)
	{
		base.Spawn (pos, dir);
		speed = initialVelocity;
		maxSpeed = -speed + (2 * range) / time;
		acceleration = (maxSpeed-speed) / time;
	}

	public override void Update ()
	{
		speed += acceleration * Time.deltaTime;
		base.Update ();
	}

}
