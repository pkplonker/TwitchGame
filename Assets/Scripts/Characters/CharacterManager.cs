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
		public static List<Character> characters { get; private set; } = new List<Character>();
		[SerializeField] private Commands commands;
		[SerializeField] private LevelData levelData;
		[SerializeField] private CharacterClassContainer characterClassContainer;
		public static event Action<Character, Character> OnFightRequested;
		private List<Character> pendingDestroys = new List<Character>();
		[SerializeField] private List<Transform> markers;
		const string dir = "/CharacterData/";


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
			sender = sender.ToLower();
			message = message.ToLower();
			if (message.Contains(commands.GetMoveCommand()))
			{
				foreach (var c in characters.Where(c => c.GetUserName() == sender))
				{
					var s = message.Replace(commands.GetMoveCommand() + " ", "");
					if (Int32.TryParse(s, out int result))
					{
						if (result >= 1 && result <= markers.Count + 1)
						{
							c.GetComponent<CharacterMovement>().RequestMove(markers[result - 1].position);
							Debug.Log(("moving to requested position " + result).WithColor(Color.green));
							return;
						}
					}

					Debug.Log("moving to random position".WithColor(Color.red));

					c.GetComponent<CharacterMovement>().RequestMove();
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
					throw new Exception("Math error");
				}

				requestedFighter2 = message.Remove(0, l);
				if (requestedFighter2[0] == '@') requestedFighter2 = requestedFighter2.Remove(0, 1);
			}

			var fighter1 = "";
			var fighter2 = "";
			foreach (var c in characters)
			{
				if (c.GetUserName() == sender) fighter1 = c.GetUserName();
				else if (c.GetUserName() == requestedFighter2) fighter2 = c.GetUserName();
			}

			

			if (string.IsNullOrWhiteSpace(fighter1))
			{
				TwitchCore.Instance.PRIVMSGTToTwitch(
					$"@{sender}, You're not an active player in this game. Please join the game first. You can use the command !join");
				return;
			}
			if (string.IsNullOrEmpty(fighter2))
			{
				TwitchCore.Instance.PRIVMSGTToTwitch(
					$"@{sender}, I currently cannot find the requested opponent. Try checking your spelling.");
				return;
			}
			OnFightRequested?.Invoke(GetCharacterByUserName(fighter1), GetCharacterByUserName(fighter2));
		}

		private void MemberJoin(string username)
		{
			if (characters.Any(character => character.GetUserName() == username)) return;
			var c = Instantiate(characterPrefab, transform).GetComponent<Character>();
			characters.Add(c);
			c.Init(username, GenerateCharacterStats(username));
		}

		private CharacterStats GenerateCharacterStats(string userName)
		{
			Debug.Log(Application.persistentDataPath);
			var path = Application.persistentDataPath + dir + userName + ".txt";

			Directory.CreateDirectory(Application.persistentDataPath + dir);
			if (!File.Exists(path))
			{
				var s = new CharacterStats(userName, levelData, characterClassContainer);
				s.Save();
				return s;
			}

			var json = File.ReadAllText(path);
			var sd = JsonUtility.FromJson<CharacterSaveData>(json);
			var cs = new CharacterStats(userName, levelData, characterClassContainer);
			cs.Load(sd);
			return cs;
		}

		public List<CharacterStats> GetAllCharacterStatData()
		{
			var path = Application.persistentDataPath + dir;
			var files = Directory.GetFiles(path);
			List<CharacterStats> stats = new List<CharacterStats>();
			foreach (var file in files)
			{
				try
				{
					var json = File.ReadAllText(file);
					var sd = JsonUtility.FromJson<CharacterSaveData>(json);
					var s = new CharacterStats(sd.userName, levelData, characterClassContainer);
					s.Load(sd);
					stats.Add(s);
				}
				catch (Exception e)
				{
					Debug.LogError(e);
					Debug.LogError(e.StackTrace);
					throw;
				}
			}

			return stats;
		}


		public static Character GetCharacterByUserName(string un) =>
			characters.FirstOrDefault(character => character.GetUserName().ToLower() == un.ToLower());

		private void MemberLeave(string username)
		{
			if (characters.Count == 0) return;
			foreach (var character in characters.Where(character =>
				         character.GetUserName().ToLower() == username.ToLower()))
			{
				if (character.RequestDestroy()) characters.Remove(character);
				else pendingDestroys.Add(character);
				return;
			}
		}

		public void SaveAllCharacters()
		{
			foreach (var c in characters) c.SaveState();
		}
	}
}