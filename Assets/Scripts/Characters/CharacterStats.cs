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
		public int currentXP = 0;
		public LevelData levelData;
		public static event Action<int, CharacterStats> OnLevelUp;

		public CharacterStats(string userName, LevelData levelData)
		{
			this.userName = userName;
			this.levelData = levelData;
		}

		public void EarnXP(int amount)
		{
			currentXP += amount;
			if (currentXP > levelData.levels[currentLevel])
			{
				LevelUp();
			}
			Save();
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
			var path = Application.persistentDataPath + dir + userName+".txt";
			if (!Directory.Exists(Application.persistentDataPath + dir)) Directory.CreateDirectory(dir);
			var json = JsonUtility.ToJson(new CharacterSaveData(this));
			File.WriteAllText(path, json);
			Debug.Log(("Saved"+userName+" Character data to "+ dir+userName));
		}

		public void Load(CharacterSaveData sd)
		{
			currentLevel = sd.level;
			currentXP = sd.xp;
			userName = sd.userName;
		}
	}
}