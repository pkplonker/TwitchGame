//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using UnityEditor;
using UnityEngine;
using StuartHeathTools;
namespace Characters
{
	/// <summary>
	///CharacterStats full description
	/// </summary>
	[CreateAssetMenu(fileName = "New Character Stats", menuName = "Character Stats")]
	public class CharacterStats : ScriptableObject
	{
		public string userName;
		public int currentLevel = 1;
		public int currentXP = 0;
		[SerializeField] private LevelData levelData;
		public event Action<int> OnLevelUp;

		private void OnEnable()
		{
			levelData = ScriptableObjectFuncs.GetAllInstances<LevelData>()[0];
		}

		public void EarnXP(int amount)
		{
			currentXP += amount;
			if (currentXP > levelData.levels[currentLevel])
			{
				LevelUp();
			}
		}

		private void LevelUp()
		{
			if (currentLevel >= levelData.maxLevel) return;
			currentLevel++;
			OnLevelUp?.Invoke(currentLevel);
		}
	}
}