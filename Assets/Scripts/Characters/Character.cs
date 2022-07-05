using System;
using UI;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Characters
{
	public class Character : MonoBehaviour
	{
		[SerializeField] private Animator animator;
		[SerializeField] private float moveSpeed;
		[SerializeField] private float minX = -9.9f;
		[SerializeField] private float maxX = 2.5f;
		[SerializeField] private CharacterUI characterUI;
		[SerializeField] private GameObject flipper;
		[SerializeField] private float moveFrequency = 5f;
		private Vector3 targetLocation;
		private bool isDead = false;
		public float GetMinX() => minX;
		public float GetMaxX() => maxX;

		private static readonly int DoMove = Animator.StringToHash("doMove");
		private static readonly int DoStop = Animator.StringToHash("doStop");

		private static readonly int DoAttack = Animator.StringToHash("doAttack");
		private string userName;
		
		public event Action<Character> OnReachedDestination;
		private bool isFighting = false;
		private float moveTimer;
		public string GetUserName() => userName;
		
		private CharacterStats characterStats;

		public CharacterStats GetCharacterStats() => characterStats;

		public void Init( string userName, CharacterStats characterStats)
		{
			animator = GetComponentInChildren<Animator>();
			this.userName = userName;
			SetStartPosition();
			targetLocation = transform.position;
			gameObject.name = userName;
			this.characterStats = characterStats;
			characterUI.SetName(userName);
			ChangeClass(characterStats.characterClass);
			
		}

		private void Update()
		{
			if (isDead) return;
			moveTimer += Time.deltaTime;
			if (moveTimer > moveFrequency && Vector3.Distance(transform.position, targetLocation) < 0.2f && !isFighting)
			{
				moveTimer = 0;
				RandomMove();
			}
			UpdateLocation();
		}

		private void SetStartPosition() => transform.position = new Vector3(UnityEngine.Random.Range(minX, maxX), transform.parent.transform.position.y, 0);
		private void RandomMove() => SetDestination(new Vector3(UnityEngine.Random.Range(minX, maxX), transform.parent.transform.position.y, 0));
		private void SetDestination(Vector3 pos) => targetLocation = pos;

		public void Flip(bool isRight) => flipper.transform.localScale = isRight ? Vector3.one : new Vector3(-1, 1, 1);
		public void DestroyObject() => Destroy(gameObject);
		public void SaveState() => characterStats.Save();
		[SerializeField] SpriteRenderer spriteRenderer;
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

		public void StartFight(Vector3 position)
		{
			position = new Vector3(position.x, transform.parent.transform.position.y, position.z);
			RequestMove(position);
			isFighting = true;
		}

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
		
		public bool RequestDestroy()
		{
			if (isFighting) return false;
			SaveState();
			Invoke(nameof(DestroyObject), 0.1f);
			return true;
		}
		
		public void Attack(Character opp)
		{
			if (isDead) return;
			animator.SetTrigger(DoAttack);
		}
		
		public void ResetAfterFight()
		{
			moveTimer = 0;
			isDead = false;
			isFighting = false;
			RandomMove();
			animator.SetTrigger(DoMove);
		}

		public void ChangeClass(CharacterClass classs)
		{
			if (classs == null) characterStats.LoadDefaultClass();
			animator.runtimeAnimatorController = classs.GetAnimationController();
			spriteRenderer.sprite = classs.sprite;
			characterStats.SetCurrentClass(classs);
			SaveState();
		}

		public void SetIsDead(bool b) => isDead=b;
	}
}