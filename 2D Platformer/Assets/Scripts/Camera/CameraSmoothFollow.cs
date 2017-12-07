using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmoothFollow : MonoBehaviour {

    public bool killPlayerWhenOutOfBounds = true;
    public float dampingTime;
    public Transform target;

    public Rect inputLimits;

    Rect limits;
    public bool drawLimits = true;

    private Rect fullLimits;

    private Vector3 velocity;
    private Vector3 currentPosition;
    private Player player;

	private Vector3 shakeOffset;
	private float magnitude;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        limits = new Rect(
            inputLimits.x - inputLimits.width / 2f,
            inputLimits.y - inputLimits.height / 2f,
            inputLimits.width,
        inputLimits.height

            );

        fullLimits = new Rect(limits.x - 10, limits.y - 7.5f, limits.width + 20f, limits.height + 15f);
    }

    private void OnDrawGizmos()
    {
        if(drawLimits)
        {

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(limits.center, new Vector3(limits.width, limits.height, 10));
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(fullLimits.center, new Vector3(fullLimits.width, fullLimits.height, 10));

        }
    }

    private void LateUpdate()
    {

		ScreenShaking ();

        //Kill player when out of bounds

        if (killPlayerWhenOutOfBounds && !fullLimits.Contains(
            player.transform.position    
                )
            )
        {
            player.Die();
        }

        //Move camera

        Vector3 targetPosition = new Vector3(target.position.x, target.position.y);

        currentPosition = Vector3.SmoothDamp(currentPosition, targetPosition, ref velocity, dampingTime);

        float size = 16;

        transform.position = new Vector3(
            Mathf.Round((currentPosition.x * size)) / size,
            Mathf.Round((currentPosition.y * size)) / size
            );

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, limits.xMin, limits.xMax),
            Mathf.Clamp(transform.position.y, limits.yMin, limits.yMax), -10);

		transform.position += shakeOffset;
        
    }

	public void SetMagnitude(float m) {
		magnitude = m;
	}

	private void ScreenShaking() {

		if (magnitude > 0) {
			magnitude -= Time.deltaTime;
		} else {
			magnitude = 0;
		}

		shakeOffset = new Vector3 (Mathf.Sin (Time.time * 100) * magnitude, Random.Range (-1.0f, 1.0f) * magnitude, 0) * .25f;

	}

}