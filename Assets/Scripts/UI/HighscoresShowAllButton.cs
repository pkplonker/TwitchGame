//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using TMPro;
using UnityEngine;

namespace UI
{
	/// <summary>
	///HighscoresShowAllButton full description
	/// </summary>
	public class HighscoresShowAllButton : MonoBehaviour
	{
		private TextMeshProUGUI buttonText;
		[SerializeField] private string showAllText = "Show All";
		[SerializeField] private string showActiveText = "Show Active";
		private Highscores highscores;

		private void Awake()
		{
			highscores = GetComponentInParent<Highscores>();
			buttonText = GetComponentInChildren<TextMeshProUGUI>();
		}

		private void Start() => buttonText.text = showActiveText;

		public void Toggle()
		{
			if (buttonText.text == showActiveText)
			{
				highscores.ShowActive();
				buttonText.text = showAllText;
			}
			else if (buttonText.text == showAllText)
			{
				highscores.ShowAll();
				buttonText.text = showActiveText;
			}
		}
	}
}