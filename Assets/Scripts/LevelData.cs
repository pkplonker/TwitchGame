//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

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
		levels = new int[maxLevel];
		for (int i = 0; i < maxLevel; i++)
		{
			levels[i] = (int) (Mathf.Floor(100 * (Mathf.Pow(i, 1.5f))));
		}
	}
}