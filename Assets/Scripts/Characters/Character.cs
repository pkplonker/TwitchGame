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
		[SerializeField] private DamagePopup damagePopup;

		[SerializeField] private float moveFrequency = 5f;
		private Vector3 targetLocation;
		public float GetMinX() => minX;
		public float GetMaxX() => maxX;

		private static readonly int DoMove = Animator.StringToHash("doMove");
		private static readonly int DoDie = Animator.StringToHash("doDie");
		private static readonly int DoStop = Animator.StringToHash("doStop");
		private static readonly int DoHit = Animator.StringToHash("doHit");
		private static readonly int DoAttack = Animator.StringToHash("doAttack");
		private string userName;
		public event Action<Character, float, float> OnHealthChanged;
		public event Action<float> OnTakeDamage;

		public event Action<Character> OnDeath;
		public event Action<Character> OnReachedDestination;
		private bool isFighting = false;
		private float moveTimer;
		public string GetUserName() => userName;
		private int currentHealth;
		[SerializeField] int maxHealth = 100;
		private bool isDead;
		private CharacterStats characterStats;

		public CharacterStats GetCharacterStats() => characterStats;
		public void SetCharacterStats(CharacterStats stats) => characterStats = stats;

		public void Init(CharacterManager characterManager, string userName, CharacterStats characterStats)
		{
			this.userName = userName;
			SetStartPosition();
			targetLocation = transform.position;
			gameObject.name = userName;
			currentHealth = maxHealth;
			this.characterStats = characterStats;
			characterUI.SetName(userName);
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
		public bool GetIsDead() => isDead;
		public void SaveState() => characterStats.Save();

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


		public void TakeDamage(int amount)
		{
			if (isDead) return;
			animator.SetTrigger(DoHit);
			var popup = Instantiate(damagePopup, transform);
			popup.SetDamageText(amount);
			OnTakeDamage?.Invoke(amount);
			currentHealth -= amount;
			if (currentHealth <= 0)
			{
				currentHealth = 0;
				OnHealthChanged?.Invoke(this, currentHealth, maxHealth);
				Die();
				return;
			}

			OnHealthChanged?.Invoke(this, currentHealth, maxHealth);
		}

		private void Die()
		{
			if (!isDead) animator.SetTrigger(DoDie);
			isDead = true;
			OnDeath?.Invoke(this);
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
			currentHealth = maxHealth;
			isFighting = false;
			RandomMove();
			animator.SetTrigger(DoMove);
			OnHealthChanged?.Invoke(this, currentHealth, maxHealth);
		}
	}
}