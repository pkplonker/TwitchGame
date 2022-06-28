//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using System.IO;
using UnityEngine;

namespace Characters
{
	/// <summary>
	///CharacterStats full description
	/// </summary>
	[Serializable]
	public class CharacterStats
	{
		public string userName;
		public int currentLevel = 1;
		public long currentXP = 0;
		public LevelData levelData;
		public int wins = 0;
		public int loses = 0;
		public CharacterClass characterClass;
		public int currentWinStreak;
		public int bestWinStreak;
		public static event Action<int, CharacterStats> OnLevelUp;

		public CharacterStats(string userName, LevelData levelData, CharacterClassContainer ccc)
		{
			this.userName = userName;
			this.levelData = levelData;
			characterClass = ccc.classes[0];
		}

		public void EarnXP(long amount)
		{
			currentXP += amount;
			while (CheckForLevelUp())
			{
			}

			OnLevelUp?.Invoke(currentLevel, this);

			Save();
		}

		private bool CheckForLevelUp()
		{
			if (currentXP <= levelData.levels[currentLevel]) return false;
			LevelUp();
			return true;
		}

		private void LevelUp()
		{
			if (currentLevel >= levelData.maxLevel) return;
			currentLevel++;
			OnLevelUp?.Invoke(currentLevel, this);
		}

		public void Save()
		{
			const string dir = "/CharacterData/";
			var path = Application.persistentDataPath + dir + userName + ".txt";
			if (!Directory.Exists(Application.persistentDataPath + dir)) Directory.CreateDirectory(dir);
			var json = JsonUtility.ToJson(new CharacterSaveData(this));
			File.WriteAllText(path, json);
		}

		public void Load(CharacterSaveData sd)
		{
			currentLevel = sd.level;
			currentXP = sd.xp;
			userName = sd.userName;
			wins = sd.wins;
			loses = sd.loses;
			characterClass = sd.characterClass;
			bestWinStreak = sd.bestWinStreak;
			currentWinStreak = sd.currentWinStreak;
		}

		public void SetCurrentClass(CharacterClass characterClass) => this.characterClass = characterClass;

		public int GetNextLevel()
		{
			var level = currentLevel++;
			if (currentLevel - 1 == levelData.maxLevel)
			{
				return -1;
			}

			return level;
		}

		public long ExperienceRequiredForNextLevel() => levelData.levels[currentLevel] - currentXP;

		public void Lose()
		{
			loses++;
			currentWinStreak = 0;
		}

		public void Win()
		{
			wins++;
			currentWinStreak++;
			if (currentWinStreak > bestWinStreak) bestWinStreak = currentWinStreak;
		}
	}
}