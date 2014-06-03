using UnityEngine;
using System.Collections;

public class beeGUI : MonoBehaviour {
	public float score = 0.0f;
	public int numLifes = 1;
	public GUIStyle myStyle;

	int screenW;
	int screenH;

	int lableW;
	int lableH;
	void Start()
	{
		screenW = Screen.width;
		screenH = Screen.height;

		lableW = screenW / 8;
		lableH = screenH / 10;
		score = 0.0f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		score += (Time.deltaTime* 10);
	}

	void OnGUI()
	{
		GUI.Label (new Rect (screenW - (lableW * 1.5f), lableH * 1.5f, lableW, lableH), "Score: " + (int)score, myStyle);
	}
}
