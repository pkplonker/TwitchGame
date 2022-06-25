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
	public event Action<Character> OnReachedDestination;

	[SerializeField] private Canvas canvas;
	[SerializeField] private GameObject model;
	[SerializeField] private GameObject flipper;

	[SerializeField] private float moveFrequency = 5f;
	private bool isFighting = false;
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
		if (moveTimer > moveFrequency && Vector3.Distance(transform.position, targetLocation) < 0.2f && !isFighting)
		{
			moveTimer = 0;
			RandomMove();
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
			OnReachedDestination?.Invoke(this);
			return;
		}

		animator.SetTrigger(DoMove);
		Move();
	}

	public void RandomMove()
	{
		SetDestination(new Vector3(UnityEngine.Random.Range(minX, maxX), 0, 0));
	}

	public void RequestMove()
	{
		moveTimer = 0;
		RandomMove();
	}

	public void StartFight(Vector3 position)
	{
		RequestMove(position);
		isFighting = true;
	}

	public void RequestMove(Vector3 position)
	{
		moveTimer = 0;
		SetDestination(position);
	}

	private void Move()
	{
		var position = transform.position;
		Flip(targetLocation.x > position.x);
		position = Vector3.MoveTowards(position, targetLocation, moveSpeed * Time.deltaTime);
		transform.position = position;
	}

	public void Flip(bool isRight)
	{
		flipper.transform.localScale = isRight ? Vector3.one : new Vector3(-1, 1, 1);
	}

	public bool RequestDestroy()
	{
		if (isFighting) return false;
		Invoke(nameof(DestroyObject), 0.1f);
		return true;
	}

	public void DestroyObject() => Destroy(gameObject);
}