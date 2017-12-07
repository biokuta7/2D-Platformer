using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackgroundBehaviour : MonoBehaviour {

    public float speed;

    SpriteRenderer spriteRenderer;
    Camera cam;

    private void Start()
    {
        cam = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {

        spriteRenderer.material.mainTextureOffset = new Vector2(cam.transform.position.x * speed, cam.transform.position.y * speed);

    }

}
