//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using Control;
using StuartHeathTools;
using TwitchIntegration;
using UnityEngine;

namespace Characters
{
	public class CharacterManager : GenericUnitySingleton<CharacterManager>
	{
		[SerializeField] private GameObject characterPrefab;
		[SerializeField] private List<Character> characters = new List<Character>();
		[SerializeField] private Commands commands;
		public static event Action<Character, Character> OnFightRequested;
		public List<Character> GetCharacters() => characters;

		private void OnEnable()
		{
			ActiveMembers.OnMemberJoin += MemberJoin;
			ActiveMembers.OnMemberLeave += MemberLeave;
			IRCParser.OnPRIVMSG += ParseMessage;
		}

		private void OnDisable()
		{
			ActiveMembers.OnMemberJoin -= MemberJoin;
			ActiveMembers.OnMemberLeave -= MemberLeave;
			IRCParser.OnPRIVMSG += ParseMessage;
		}

		private void ParseMessage(string sender, string message)
		{
			if (message.Contains(commands.GetMoveCommand()))
			{
				foreach (var c in characters.Where(c => c.GetUserName() == sender))
				{
					c.RequestMove();
					return;
				}
			}
			else if (message.Contains(commands.GetFightCommand()))
			{
				HandleFightRequest(sender, message);
			}
		}

		private void HandleFightRequest(string sender, string message)
		{
			var requestedFighter2 = "";
			//parse message to get 2nd player
			if (message.Contains(commands.GetFightCommand() + " "))
			{
				var l = (commands.GetFightCommand() + " ").Length;
				if (l > message.Length)
				{
					Debug.LogError("Math error");
				}

				requestedFighter2 = message.Remove(0, l);
			}


			var fighter1 = "";
			var fighter2 = "";
			foreach (var c in characters)
			{
				if (c.GetUserName() == sender) fighter1 = c.GetUserName();
				else if (c.GetUserName() == requestedFighter2) fighter2 = c.GetUserName();
			}

			if (string.IsNullOrWhiteSpace(fighter1) || string.IsNullOrEmpty(fighter2)) return;
			OnFightRequested?.Invoke(GetCharacterByUserName(fighter1),GetCharacterByUserName(fighter2));
		}

		private void MemberJoin(string username)
		{
			foreach (var character in characters.Where(character => character.GetUserName() == username))
			{
				return;
			}

			Character c = Instantiate(characterPrefab, transform).GetComponent<Character>();
			characters.Add(c);
			c.Init(this, username);
		}

		private Character GetCharacterByUserName(string un)
		{
			foreach (var character in characters.Where(character => character.GetUserName() == un))
			{
				return character;
			}

			return null;
		}

		private void MemberLeave(string username)
		{
			if (characters.Count == 0) return;
			foreach (var character in characters.Where(character => character.GetUserName() == username))
			{
				if(character.RequestDestroy()) characters.Remove(character);
			
				return;
			}
		}
	}
}