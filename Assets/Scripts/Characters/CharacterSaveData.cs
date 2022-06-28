using System;

namespace Characters
{
	[Serializable]
	public class CharacterSaveData
	{
		public string userName;
		public long xp;
		public int level;
		public int wins;
		public int loses;
		public CharacterClass characterClass;
		public int currentWinStreak;
		public int bestWinStreak;

		public CharacterSaveData(CharacterStats stats)
		{
			userName = stats.userName;
			xp = stats.currentXP;
			level = stats.currentLevel;
			loses = stats.loses;
			wins = stats.wins;
			characterClass = stats.characterClass;
			currentWinStreak = stats.currentWinStreak;
			bestWinStreak = stats.bestWinStreak;
		}
	}
}