﻿using UnityEngine;

[ExecuteInEditMode]
public class SimpleBlit : MonoBehaviour {

	public RenderTexture bl;

	void OnRenderImage(RenderTexture src, RenderTexture dst) {
		if (bl != null) {
			Graphics.Blit (bl, dst);
		}
	}

}
