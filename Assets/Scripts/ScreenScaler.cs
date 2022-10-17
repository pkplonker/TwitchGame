 //
 // Copyright (C) 2022 Stuart Heath. All rights reserved.
 //

 using System;
 using UnityEngine;

    /// <summary>
    ///ScreenScaler full description
    /// </summary>
    
public class ScreenScaler : MonoBehaviour
{
	private const uint targetWidth = 1920;
	private const uint targetHeight = 1080;

	private void Awake()
	{
		var cam = Camera.main;
		var deviceScreenResolution = new Vector2(Screen.width, Screen.height);
		var aspect = deviceScreenResolution.x / deviceScreenResolution.y;
		
		transform.localScale = new Vector3(Screen.width / (float)targetWidth, Screen.height / (float)targetHeight, 1);
		cam.aspect = aspect;
		var camHeight = 100.0f * cam.orthographicSize*2.0f;
		var camWidth = camHeight * cam.aspect;
		

	}
}
