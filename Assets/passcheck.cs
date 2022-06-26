using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class passcheck : MonoBehaviour
{
	private TextMeshProUGUI textMeshProUGUI;

	private void Awake()
	{
		textMeshProUGUI = GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		var p = TwitchPasswordHandler.GetPassword();
		textMeshProUGUI.text = string.IsNullOrWhiteSpace(p) ? "No pass located" : p;
	}
}