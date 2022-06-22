using System;
using System.Collections;
using System.Collections.Generic;
using StuartHeathTools;
using UnityEngine;

public class Collision : MonoBehaviour
{
	private void OnCollisionEnter(UnityEngine.Collision other)
	{
		Destroy(other.gameObject);
		Debug.Log("BEAR ATE A COOKIE".WithColor(Color.cyan));
	}
	
}
