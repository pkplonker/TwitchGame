//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.IO;
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
		[SerializeField] private static List<Character> characters = new List<Character>();
		[SerializeField] private Commands commands;
		[SerializeField] private LevelData levelData;
		public static event Action<Character, Character> OnFightRequested;
		private List<Character> pendingDestroys = new List<Character>();
		[SerializeField] private List<Transform> markers;

	

		private void Start() => InvokeRepeating(nameof(AttemptDestroy), 1f, 2f);

		private void AttemptDestroy()
		{
			if (pendingDestroys.Count == 0) return;

			for (var i = pendingDestroys.Count - 1; i >= 0; i--)
			{
				if (!pendingDestroys[i].RequestDestroy()) continue;
				characters.Remove(pendingDestroys[i]);
				pendingDestroys.RemoveAt(i);
			}
		}

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
			else if (message.Contains(commands.GetFightCommand())) HandleFightRequest(sender, message);
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
			OnFightRequested?.Invoke(GetCharacterByUserName(fighter1), GetCharacterByUserName(fighter2));
		}

		private void MemberJoin(string username)
		{
			if (characters.Any(character => character.GetUserName() == username))
			{
				return;
			}

			var c = Instantiate(characterPrefab, transform).GetComponent<Character>();
			characters.Add(c);
			c.Init(this, username, GenerateCharacterStats(username));
		}

		private CharacterStats GenerateCharacterStats(string userName)
		{
			const string dir = "/CharacterData/";
			var path = Application.persistentDataPath + dir + userName + ".txt";

			Directory.CreateDirectory(Application.persistentDataPath + dir);
			if (!File.Exists(path))
			{
				Debug.Log(("creating new characterStats for " + userName).WithColor(Color.magenta));
				var s = new CharacterStats(userName, levelData);
				s.Save();
				return s;
			}

			Debug.Log(("Loading existing characterStats for " + userName).WithColor(Color.magenta));
			var json = File.ReadAllText(path);
			CharacterSaveData sd = JsonUtility.FromJson<CharacterSaveData>(json);
			var cs = new CharacterStats(userName, levelData);
			cs.Load(sd);
			return cs;
		}

		public static Character GetCharacterByUserName(string un) =>
			characters.FirstOrDefault(character => character.GetUserName() == un);


		private void MemberLeave(string username)
		{
			if (characters.Count == 0) return;
			foreach (var character in characters.Where(character => character.GetUserName() == username))
			{
				if (character.RequestDestroy()) characters.Remove(character);
				else pendingDestroys.Add(character);
				return;
			}
		}

		public void SaveAllCharacters()
		{
			foreach (var c in characters)
			{
				c.SaveState();
			}
		}
	}
}