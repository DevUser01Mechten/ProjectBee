using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
	public Texture2D mainMenuBackground;
	public GUIStyle customButtonStyle;
	
	private float screenWidth;
	private float screenHeight;
	private float buttonWidth;
	private float buttonHeight;
	
	void Start()
	{
		screenWidth = Screen.width;
		screenHeight = Screen.height;
		buttonWidth = screenWidth*0.5f;
		buttonHeight = screenHeight*0.15f;
	}
	
	
	void OnGUI()
	{
		GUI.DrawTexture(new Rect(0f, 0f, screenWidth, screenHeight), mainMenuBackground, ScaleMode.ScaleToFit);
	
	
		if (GUI.Button(new Rect((screenWidth-buttonWidth)/2, screenHeight*0.2f, buttonWidth, buttonHeight), "TRY AGAIN", customButtonStyle))
		{
			Debug.Log ("TRY BUTTON CLICKED!");
		}
	
	
		if (GUI.Button(new Rect((screenWidth-buttonWidth)/2, screenHeight*0.4f, buttonWidth, buttonHeight), "TRY AGAIN", customButtonStyle))
		{
			Debug.Log ("TRY BUTTON CLICKED!");
		}
	}
	
}
