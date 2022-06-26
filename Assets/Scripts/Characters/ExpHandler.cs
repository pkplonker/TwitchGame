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
			winner.GetCharacterStats().EarnXP(winnerXp);
			Debug.Log(("winner earned " + winnerXp).WithColor(Color.green));
			var p = Instantiate(popup, winner.transform);
			p.SetXpText(winnerXp);
		}
	}
}