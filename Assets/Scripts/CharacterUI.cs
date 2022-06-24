using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI userName;
	[SerializeField] private Image healthBarImage;
	[SerializeField] private Character character;

	private void OnEnable()
	{
		character.OnTakeDamage += TakeDamage;
	}

	private void TakeDamage(float max, float current)
	{
		SetFill(max, current);
	}

	private void OnDisable()
	{
		character.OnTakeDamage -= TakeDamage;
	}

	private void Start() => SetFill(1f, 1f);


	public void SetName(string username) => userName.text = username;


	private void SetFill(float max, float current)
	{
		healthBarImage.fillAmount = math.remap(0, max, 0, 1, current);
	}
}