using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Characters
{
	public class CharacterUI : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI userName;
		[SerializeField] private GameObject healthBarImage;
		[SerializeField] private Image healthBarImageFill;

		[SerializeField] private CharacterHealth character;
		private Coroutine cor;

		private void OnEnable()
		{
			character.OnHealthChanged += TakeDamage;
			CharacterStats.OnLevelUp += LevelUp;
		}

		private void LevelUp(int level, CharacterStats stats)
		{
			if (stats.userName == character.GetComponent<Character>().GetUserName()) SetName(character.GetComponent<Character>().GetUserName());
		}

		private void OnDisable()
		{
			CharacterStats.OnLevelUp -= LevelUp;
			character.OnHealthChanged -= TakeDamage;
		}

		private void Awake() => healthBarImage.SetActive(false);


		private void TakeDamage(CharacterHealth c, float max, float current)
		{
			healthBarImage.SetActive(true);
			SetFill(max, current);
			if (cor == null) cor = StartCoroutine(HealthBar());
			else
			{
				StopCoroutine(cor);
				cor = StartCoroutine(HealthBar());
			}
		}

		private void Start() => SetFill(1f, 1f);

		public void SetName(string username) =>
			userName.text = username + "(" + character.GetComponent<Character>().GetCharacterStats().currentLevel + ")";

		private void SetFill(float current, float max) => healthBarImageFill.fillAmount = current / max;

		private IEnumerator HealthBar()
		{
			yield return new WaitForSeconds(2.5f);
			healthBarImage.SetActive(false);
			cor = null;
		}
	}
}