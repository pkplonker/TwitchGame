using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTest : MonoBehaviour
{
	private float startX;
	private void Start()
	{
		startX = transform.position.x;
	}

	private void Update()
	{
		transform.position = new Vector3(startX + Mathf.PingPong(Time.time, 2f), transform.position.y);
	}
}