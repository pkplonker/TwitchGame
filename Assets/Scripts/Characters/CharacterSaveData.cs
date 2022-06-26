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

        public CharacterSaveData(string userName, int xp, int level, int wins, int loses)
        {
            this.userName = userName;
            this.xp = xp;
            this.level = level;
            this.wins = wins;
            this.loses = loses;
        }
        
        

        public CharacterSaveData(CharacterStats stats)
        {
            userName = stats.userName;
            xp = stats.currentXP;
            level = stats.currentLevel;
            loses = stats.loses;
            wins = stats.wins;
        }
    }
}
