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
		

		[SerializeField] private CharacterUI characterUI;

		private static readonly int DoMove = Animator.StringToHash("doMove");
		private static readonly int DoStop = Animator.StringToHash("doStop");

		private string userName;
	
		public string GetUserName() => userName;
		
		private CharacterStats characterStats;

		public CharacterStats GetCharacterStats() => characterStats;
		private CharacterHealth characterHealth;
		public void Init( string userName, CharacterStats characterStats)
		{
			animator = GetComponentInChildren<Animator>();
			this.userName = userName;
			
			gameObject.name = userName;
			this.characterStats = characterStats;
			characterUI.SetName(userName);
			ChangeClass(characterStats.characterClass);
			characterHealth = GetComponent<CharacterHealth>();

		}



		
		public void DestroyObject() => Destroy(gameObject);
		public void SaveState() => characterStats.Save();
		[SerializeField] SpriteRenderer spriteRenderer;
		
		
		
		public bool RequestDestroy()
		{
			if (GetComponent<CharacterCombat>().GetIsFighting()) return false;
			SaveState();
			Invoke(nameof(DestroyObject), 0.1f);
			return true;
		}
		
		
		
	

		public void ChangeClass(CharacterClass classs)
		{
			if (classs == null) characterStats.LoadDefaultClass();
			animator.runtimeAnimatorController = classs.GetAnimationController();
			spriteRenderer.sprite = classs.sprite;
			characterStats.SetCurrentClass(classs);
			SaveState();
		}


	
	}
}