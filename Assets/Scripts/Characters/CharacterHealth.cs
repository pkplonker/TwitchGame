using System;
using System.Collections;
using Control;
using UI;
using UnityEngine;

namespace Characters
{
	public class CharacterHealth : MonoBehaviour
	{
		[SerializeField] private DamagePopup damagePopup;
		[SerializeField] int maxHealth = 100;


		private Character character;
		private Animator animator;

		private static readonly int DoDie = Animator.StringToHash("doDie");
		private static readonly int DoHit = Animator.StringToHash("doHit");
		private bool isDead = false;

		private int currentHealth;

		public event Action<float> OnTakeDamage;
		public event Action<CharacterHealth, float, float> OnHealthChanged;
		public event Action<CharacterHealth> OnDeath;
		private void Awake() => currentHealth = maxHealth;

		private void Start()
		{
			character = GetComponent<Character>();
			animator = GetComponentInChildren<Animator>();
		}

		private void OnEnable()=>FightController.OnFightOver += FightOver;
		

		private void OnDisable()=>FightController.OnFightOver -= FightOver;
		

		private void FightOver(Character arg1, Character arg2)=>StartCoroutine(AwaitDeathFlagRemoval());
			
			
		

		private IEnumerator AwaitDeathFlagRemoval()
		{
			yield return new WaitForSeconds(2);
			isDead = false;
			currentHealth = maxHealth;
			OnHealthChanged?.Invoke(this, currentHealth, maxHealth);
		}

		public bool TakeDamage(int amount)
		{
			if (isDead) return false;
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
				return true;
			}

			OnHealthChanged?.Invoke(this, currentHealth, maxHealth);
			return false;
		}

		private void Die()
		{
			Debug.Log("req death");
			if (!isDead) animator.SetTrigger(DoDie);
			isDead = true;
			OnDeath?.Invoke(this);
		}
		public void SetIsDead(bool b) => isDead=b;

		public bool GetIsDead() => isDead;
	
	}
}