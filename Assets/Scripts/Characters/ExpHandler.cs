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

		private void AllocateXP(Character character, int amount)
		{
			character.GetCharacterStats().EarnXP(amount);
			var p = Instantiate(popup, character.transform);
			p.SetXpText(amount);
			Debug.Log((character.name + " earned " + amount).WithColor(Color.green));
		}
	}
}