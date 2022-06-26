using System;
using Characters;
using Control;
using StuartHeathTools;
using TMPro;
using UnityEngine;

namespace UI
{
	public class FightOverScreen : CanvasGroupBase
	{
		[SerializeField] private TextMeshProUGUI text;

		private void Awake() => Hide();
		private void OnEnable() => FightController.OnFightOver += FightOver;
		private void OnDisable() => FightController.OnFightOver -= FightOver;
		private void FightOver(Character winner, Character loser)
		{
			Show();
			text.text = winner.GetUserName() + " Wins!";
			Invoke(nameof(Hide), 5f);
		}
	}
}