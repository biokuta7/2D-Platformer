using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelPerfectMainCamera : MonoBehaviour {

    [SerializeField]
    public int PPU = 16;
    [SerializeField]
    public int screenHeight = 240;
    [SerializeField]
    public float orthoSize = 7.5f;

    public void UpdateOrthoSize()
    {
        orthoSize = (screenHeight / PPU) * .5f;
    }

    public void ApplyOrthoSize()
    {
        Camera.main.orthographicSize = orthoSize;
    } 

}
