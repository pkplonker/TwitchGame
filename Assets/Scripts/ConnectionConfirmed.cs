using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TwitchIntegration;
using UnityEngine;

public class ConnectionConfirmed : MonoBehaviour
{
	private TextMeshProUGUI text;
	private void Start() => text = GetComponent<TextMeshProUGUI>();
	private void OnEnable() => TwitchCore.OnConnectionConfirmed += UpdateText;
	private void UpdateText() => text.text = "Connection confirmed";
	private void OnDisable() => TwitchCore.OnConnectionConfirmed -= UpdateText;
}