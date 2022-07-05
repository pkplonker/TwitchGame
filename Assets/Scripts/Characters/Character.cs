using System;
using UI;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace Characters
{
	public class Character : NetworkBehaviour, INetworkSerializable
	{
		[SerializeField] private Animator animator;
		SpriteRenderer spriteRenderer;
		[SerializeField] private CharacterUI characterUI;
		private static readonly int DoMove = Animator.StringToHash("doMove");
		private string userName;
	
		public string GetUserName() => userName;
		
		private CharacterStats characterStats;

		public CharacterStats GetCharacterStats() => characterStats;
		private CharacterHealth characterHealth;
		public void Init( string userName, CharacterStats characterStats)
		{
			spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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
			animator.runtimeAnimatorController = Resources.Load(classs.GetAnimationControllerPath()) as RuntimeAnimatorController;
			spriteRenderer.sprite = classs.sprite;
			characterStats.SetCurrentClass(classs);
			SaveState();
		}


		public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
		{
			serializer.SerializeValue(ref userName);

			
		}
	}
}