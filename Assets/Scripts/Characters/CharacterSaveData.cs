using System;

namespace Characters
{
    [Serializable]
    public class CharacterSaveData
    {
        public string userName;
        public long xp;
        public int level;

        public CharacterSaveData(string userName, int xp, int level)
        {
            this.userName = userName;
            this.xp = xp;
            this.level = level;
        }
        
        

        public CharacterSaveData(CharacterStats stats)
        {
            userName = stats.userName;
            xp = stats.currentXP;
            level = stats.currentLevel;
        }
    }
}
