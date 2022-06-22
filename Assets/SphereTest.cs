using System.Collections;
using System.Collections.Generic;
using StuartHeathTools;
using TwitchIntegration;
using UnityEngine;

public class SphereTest : MonoBehaviour
{
	[SerializeField] private float moveAmount = 0.5f;
	private void OnEnable() => TwitchCore.OnMessageReceived += HandleNewTwitchMessage;

	private void OnDisable() => TwitchCore.OnMessageReceived -= HandleNewTwitchMessage;

	private void HandleNewTwitchMessage(string sender, string message)
	{
		switch (message)
		{
			case "!up":
				transform.position += Vector3.up * moveAmount;
				break;
			case "!down":
				transform.position += Vector3.down * moveAmount;
				break;
			case "!left":
				transform.position += Vector3.left * moveAmount;
				break;
			case "!right":
				transform.position += Vector3.right * moveAmount;
				break;
			default:
				Debug.Log("Wrong command".WithColor(Color.red));
				break;
		}
		
	}
}