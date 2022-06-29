//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using Characters;
using TMPro;
using UnityEngine;

namespace UI
{
	/// <summary>
	///HighscoresEntry full description
	/// </summary>
	public class HighscoresEntry : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI rankText;
		[SerializeField] private TextMeshProUGUI usernameText;
		[SerializeField] private TextMeshProUGUI levelText;
		[SerializeField] private TextMeshProUGUI winLossText;
		[SerializeField] private TextMeshProUGUI streakText;


		public void Init(CharacterStats stats, int rank)
		{
			if (stats == null) return;
			rank++;
			rankText.text = rank.ToString();
			usernameText.text = stats.userName;
			levelText.text = stats.currentLevel.ToString();
			winLossText.text = stats.wins + "/" + stats.loses;
			streakText.text = stats.bestWinStreak.ToString();
		}
	}
}