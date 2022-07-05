using System;
using Control;
using UnityEngine;

namespace Characters
{
	public class CharacterMovement : MonoBehaviour
	{
		[SerializeField] private float moveSpeed;
		[SerializeField] private float minX = -9.9f;
		[SerializeField] private float maxX = 2.5f;
		[SerializeField] private GameObject flipper;
		[SerializeField] private float moveFrequency = 5f;

		private Vector3 targetLocation;
		private float moveTimer;
		private Animator animator;
		public event Action<CharacterMovement> OnReachedDestination;
		private static readonly int DoMove = Animator.StringToHash("doMove");
		public float GetMinX() => minX;
		public float GetMaxX() => maxX;
		private Character character;
		private CharacterHealth characterHealth;
		private CharacterCombat characterCombat;

		private void Start()
		{
			animator = GetComponentInChildren<Animator>();
			SetStartPosition();
//			character = GetComponent<Character>();
			characterHealth = GetComponent<CharacterHealth>();
			characterCombat = GetComponent<CharacterCombat>();
		}

		private void OnEnable()
		{
			FightController.OnFightOver += FightOver;
		}


		private void FightOver(Character arg1, Character arg2)
		{
			if (arg1 != character && arg2 != character) return;
			moveTimer = 0;
			//RandomMove();
			animator.SetTrigger(DoMove);
		}

		private void OnDisable()
		{
			FightController.OnFightOver -= FightOver;
		}

		private void Update()
		{
			if (characterHealth.GetIsDead()) return;
			moveTimer += Time.deltaTime;
			if (moveTimer > moveFrequency && Vector3.Distance(transform.position, targetLocation) < 0.2f &&
			    !characterCombat.GetIsFighting())
			{
				moveTimer = 0;
				RandomMove();
			}

			UpdateLocation();
		}

		private void UpdateLocation()
		{
			if (Vector3.Distance(transform.position, targetLocation) < 0.2f)
			{
				targetLocation = transform.position;
				OnReachedDestination?.Invoke(this);
				return;
			}

			animator.SetTrigger(DoMove);
			Move();
		}


		public void RequestMove()
		{
			moveTimer = 0;
			RandomMove();
		}

		private void SetStartPosition() => transform.position = new Vector3(UnityEngine.Random.Range(minX, maxX),
			transform.parent.transform.position.y, 0);

		private void RandomMove() => SetDestination(new Vector3(UnityEngine.Random.Range(minX, maxX),
			transform.parent.transform.position.y, 0));

		private void SetDestination(Vector3 pos) => targetLocation = pos;

		public void Flip(bool isRight) => flipper.transform.localScale = isRight ? Vector3.one : new Vector3(-1, 1, 1);

		public void RequestMove(Vector3 position)
		{
			position = new Vector3(position.x, transform.parent.transform.position.y, position.z);
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
	}
}