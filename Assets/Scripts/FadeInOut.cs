﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeInOut : MonoBehaviour {

    private float fadeSpeed = 1.5f;
    private bool sceneStaring = true;
    private GUITexture tex;

	void Start () {
		tex = gameObject.GetComponent<GUITexture>();
        tex.pixelInset = new Rect(0, 0, Screen.width, Screen.height);
	}
	
	
	void Update () {
        if (sceneStaring)
        {
            StartScene();
        }
	}

    private void FadeToClear()
    {
        tex.color = Color.Lerp(tex.color, Color.clear, fadeSpeed * Time.deltaTime);
    }

    private void FadeToBlack()
    {
        tex.color = Color.Lerp(tex.color, Color.black, fadeSpeed * Time.deltaTime);
    }

    private void StartScene()
    {
        FadeToClear();
        if (tex.color.a <= 0.05f)
        {
            tex.color = Color.clear;
            tex.enabled = false;
            sceneStaring = false;
        }
    }

    public void EndScene()
    {
        FadeToBlack();
        if (tex.color.a >= 0.95f)
        {
            SceneManager.LoadScene("Game");
        }
    }
}
