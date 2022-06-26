using Control;
using StuartHeathTools;
using UI;
using UnityEngine;

namespace Characters
{
	public class ExpHandler : MonoBehaviour
	{
		[SerializeField] private int winnerXp = 150;
		[SerializeField] private int loserXP = 50;
		[SerializeField] private XPPopup popup;
		private void OnEnable() => FightController.OnFightOver += EarnExp;


		private void OnDisable() => FightController.OnFightOver -= EarnExp;


		private void EarnExp(Character winner, Character loser)
		{
			AllocateXP(winner, winnerXp);
			AllocateXP(loser, loserXP);
		}

		private void AllocateXP(Character character, long amount)
		{
			character.GetCharacterStats().EarnXP(amount);
			var p = Instantiate(popup, character.transform);
			p.SetXpText(amount);
		}

		public void SetLevel(Character character, long level)
		{
			if (character == null) return;
			if (level > character.GetCharacterStats().levelData.maxLevel)
				level = character.GetCharacterStats().levelData.maxLevel;
			character.GetCharacterStats().currentLevel = 1;
			character.GetCharacterStats().currentXP = 0;


			var x = character.GetCharacterStats().levelData.levels[level] - character.GetCharacterStats().currentXP;
			Debug.Log("adding " + x + " exp");
			character.GetCharacterStats().EarnXP((character.GetCharacterStats().levelData.levels[level]));
		}
	}
}