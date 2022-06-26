//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using Characters;
using UnityEngine;

/// <summary>
///LevelData full description
/// </summary>
[CreateAssetMenu(fileName = "Level Data", menuName = "Level Data")]
public class LevelData : ScriptableObject
{
	public int[] levels;
	public int maxLevel;
	
	private void OnEnable()
	{
		levels = new int[maxLevel];
		CalculateLevelArray();
	}

	public void CalculateLevelArray()
	{
		levels = new int[maxLevel+1];
		for (int i = 0; i < maxLevel+1; i++)
		{
			levels[i] = (int) (Mathf.Floor(100 * (Mathf.Pow(i, 1.5f))));
		}
	}

	public long GetXPForLevel(CharacterStats character, int level)
	{
		return levels[level];
	}
}