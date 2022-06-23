using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Character : MonoBehaviour
{
	[SerializeField] private Animator animator;
	private Vector3 targetLocation;
	[SerializeField] private float moveSpeed;
	private static readonly int DoMove = Animator.StringToHash("doMove");
	private static readonly int DoDie = Animator.StringToHash("doDie");
	private static readonly int DoStop = Animator.StringToHash("doStop");

	private void Awake()
	{
		targetLocation = transform.position;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.D))
		{
			Debug.Log("die");
			animator.SetTrigger(DoDie);
		}

		if (Input.GetKeyDown(KeyCode.M))
		{
			Debug.Log("move");
			animator.SetTrigger(DoMove);
		}

		if (Input.GetKeyDown(KeyCode.T))
		{
			targetLocation = new Vector3(UnityEngine.Random.Range(-9.9f, 2.5f), 0f, 0f);
		}

		UpdateLocation();
	}

	private void UpdateLocation()
	{
		if (Vector3.Distance(transform.position, targetLocation) < 0.2f)
		{
			targetLocation = transform.position;
			animator.SetTrigger(DoStop);

			return;
		}

		animator.SetTrigger(DoMove);
		Move();
	}

	private void Move()
	{
		if (targetLocation.x > transform.position.x) transform.localEulerAngles = Vector3.zero;
		else transform.localEulerAngles = new Vector3(0, 180, 0);
		transform.position = Vector3.MoveTowards(transform.position, targetLocation, moveSpeed * Time.deltaTime);
	}
}