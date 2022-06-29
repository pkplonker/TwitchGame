using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using StuartHeathTools;
using UnityEngine;

namespace UI
{
	public class Highscores : CanvasGroupBase
	{
		[SerializeField] private HighscoresEntry prefab;
		[SerializeField] private Transform entryContainer;
		private List<HighscoresEntry> currentEntries = new List<HighscoresEntry>();
		[SerializeField] private CharacterManager characterManager;
		private bool isActive = false;
		private void Awake() => Hide();
		public void Close() => Hide();

		public void Open()
		{
			Show();
			UpdateHighscores();
		}

		private void UpdateHighscores()
		{
			DeleteExisting();
			if (isActive) GetActiveStats();
			else GetAllStats();
		}


		private void GetActiveStats()
		{
			var chars = CharacterManager.characters.OrderBy(x => x.GetCharacterStats().currentLevel).ToList();
			var stats = chars.Select(character => character.GetCharacterStats()).ToList();
			stats.Reverse();

			for (var i = 0; i < chars.Count; i++)
			{
				GenerateHighscore(stats[i], i);
			}
		}

		private void GetAllStats()
		{
			var stats = characterManager.GetAllCharacterStatData().OrderBy(x => x.currentLevel).ToList();
			stats.Reverse();

			for (var i = 0; i < stats.Count(); i++)
			{
				GenerateHighscore(stats[i], i);
			}
		}

		private void GenerateHighscore(CharacterStats stats, int i)
		{
			var highscoresEntry = Instantiate(prefab, entryContainer).GetComponent<HighscoresEntry>();
			currentEntries.Add(highscoresEntry);
			highscoresEntry.Init(stats, i);
		}

		private void DeleteExisting()
		{
			for (var i = currentEntries.Count - 1; i >= 0; i--)
			{
				Destroy(currentEntries[i].gameObject);
			}

			currentEntries.Clear();
		}


		public void ShowActive()
		{
			isActive = true;
			UpdateHighscores();
		}

		public void ShowAll()
		{
			isActive = false;
			UpdateHighscores();
		}
	}
}