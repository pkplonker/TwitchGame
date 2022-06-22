using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using TwitchIntegration;
using UnityEngine;

public class ChatOnScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI prefab;
    [SerializeField] private Transform container;

    private void OnEnable() => TwitchCore.OnMessageReceived += HandleNewTwitchMessage;
    private void OnDisable()=> TwitchCore.OnMessageReceived -= HandleNewTwitchMessage;

    private void HandleNewTwitchMessage(string user, string message)
    {
        var go = Instantiate(prefab, container);
        go.text = user + ": " + message;
    }

}
