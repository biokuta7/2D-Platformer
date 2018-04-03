using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTargetBehaviour : MonoBehaviour {

	public float range = 5;
	Player p;

	private void Start() {
		p = GetComponentInParent<Player> ();
	}

	private void Update()
	{

		Vector3 targetPosition = p.transform.position + new Vector3(
			p.FacingRight() ? 1 : -1,
			Controller2D.Direction2Vector(p.GetDirection()).y) * range;

		transform.position = targetPosition;

	}
}
