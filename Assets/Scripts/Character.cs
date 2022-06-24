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
	[SerializeField] private float minX = -9.9f;
	[SerializeField] private float maxX = 2.5f;
	private CharacterManager characterManager;
	private static readonly int DoMove = Animator.StringToHash("doMove");
	private static readonly int DoDie = Animator.StringToHash("doDie");
	private static readonly int DoStop = Animator.StringToHash("doStop");
	private string userName;
	[SerializeField] private CharacterUI characterUI;
	public event Action<float, float> OnTakeDamage;
	[SerializeField] private Canvas canvas;
	[SerializeField] private GameObject model;
	[SerializeField] private GameObject flipper;

	[SerializeField] private float moveFrequency = 5f;
	private float moveTimer;
	public string GetUserName() => userName;

	public void Init(CharacterManager characterManager, string userName)
	{
		this.characterManager = characterManager;
		this.userName = userName;
		characterUI.SetName(userName);
		SetStartPosition();
		targetLocation = transform.position;
		gameObject.name = userName;
	}

	private void SetStartPosition()
	{
		transform.position = new Vector3(UnityEngine.Random.Range(minX, maxX), 0, 0);
		Debug.Log("setting position to" + transform.position);
	}

	private void Update()
	{
		moveTimer += Time.deltaTime;
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
			targetLocation = new Vector3(UnityEngine.Random.Range(minX, maxX), 0f, 0f);
		}

		if (moveTimer > 5f && Vector3.Distance(transform.position, targetLocation) < 0.2f)
		{
			moveTimer = 0;
			SetDestination(new Vector3(UnityEngine.Random.Range(minX, maxX), 0, 0));
		}

		UpdateLocation();
	}

	private void SetDestination(Vector3 pos)
	{
		targetLocation = pos;
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
		Flip(targetLocation.x > transform.position.x);
		transform.position = Vector3.MoveTowards(transform.position, targetLocation, moveSpeed * Time.deltaTime);
	}

	private void Flip(bool isRight)
	{
		flipper.transform.localScale = isRight ? Vector3.one : new Vector3(-1, 1, 1);
	}
}