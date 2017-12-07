using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public Sprite[] rightSprites;
	public Sprite[] upSprites;

	public float time;
	public float range;
	public GameObject defaultDeathParticles;
	public bool stillUseDefaultParticlesOnHit = false;
	private GameObject changedParticles;

	public LayerMask hitmask;
	public float screenShakeOnHitMagnitude = 0.0f;

	private float lifeTime;
	protected float speed;
	private Vector3 direction;
	private bool boof;

	private static CameraSmoothFollow cam;
	private static int ID = 0;

	private void Awake() {
		if (ID <= 0) {
			cam = Camera.main.GetComponent<CameraSmoothFollow> ();
			ID++;
		}
	}

	SpriteRenderer spriteRenderer;

	public virtual void Update() {
		if (gameObject.layer != 10) {
			Life ();
			CheckForCollisions ();
			Move (speed * Time.deltaTime);
			if (boof) {
				SwitchOffSprites ();
			}
		}
	}

	private void SwitchOffSprites() {

		if (direction.x != 0) {

			spriteRenderer.sprite = rightSprites[Random.Range(0, rightSprites.Length)];
		}

		if (direction.y != 0) {

			spriteRenderer.sprite = upSprites[Random.Range(0, upSprites.Length)];
		}

	}

	public void Life() {
		lifeTime -= Time.deltaTime;
		if (lifeTime <= 0) {
			Die ();
		}
	}

	public void Move(float delta) {
		transform.position += direction * delta;
	}

	public virtual void Spawn(Vector3 pos, Vector3 dir) {

		gameObject.layer = 0;
		spriteRenderer = GetComponent<SpriteRenderer> ();
		speed = range / time;
		lifeTime = time;

		boof = rightSprites.Length > 1 || upSprites.Length > 1;

		direction = dir;

		transform.position = pos;

		if (direction.x != 0) {
			spriteRenderer.sprite = rightSprites[0];
			spriteRenderer.flipX = direction.x < 0;
		} else if (direction.y != 0) {
			spriteRenderer.sprite = upSprites[0];
			spriteRenderer.flipY = direction.y < 0;
		}
	}

	private void CheckForCollisions() {
		RaycastHit2D hit = Physics2D.Raycast (transform.position, direction, (speed * Time.deltaTime), hitmask);

		if (hit) {

			Surface s = hit.collider.GetComponent<Surface> ();

			if (screenShakeOnHitMagnitude > 0) {
				cam.SetMagnitude (screenShakeOnHitMagnitude);
			}

			if (s != null) {
				changedParticles = s.hitParticles;
			}

			transform.position = hit.point;
			Die ();
		}


	}

	public void Die() {

		if (!stillUseDefaultParticlesOnHit && changedParticles != null) {
			Instantiate (changedParticles, transform.position, Quaternion.identity);
			changedParticles = null;
		} else {
			Instantiate (defaultDeathParticles, transform.position, Quaternion.identity);
		}

		transform.position = Vector3.down * 500f;

		gameObject.layer = 10;

	}

}
