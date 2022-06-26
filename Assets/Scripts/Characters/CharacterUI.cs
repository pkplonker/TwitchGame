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

		[SerializeField] private Character character;
		private Coroutine cor;
		private void OnEnable() => character.OnHealthChanged += TakeDamage;
		private void OnDisable() => character.OnHealthChanged -= TakeDamage;

		private void Awake()
		{
			healthBarImage.SetActive(false);	
		}

		private void TakeDamage(Character c, float max, float current)
		{
			healthBarImage.SetActive(true);
			SetFill(max, current);
			if (cor == null)
			{
				cor = StartCoroutine(HealthBar());
			}
			else
			{
				StopCoroutine(cor);
				cor = StartCoroutine(HealthBar());
			}
		}

		private void Start() => SetFill(1f, 1f);
		public void SetName(string username) => userName.text = username;
		private void SetFill(float current, float max) => healthBarImageFill.fillAmount = current / max;

		private IEnumerator HealthBar()
		{
			yield return new WaitForSeconds(2.5f);
			healthBarImage.SetActive(false);
			cor = null;
		}
	}
}