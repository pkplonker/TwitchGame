//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using System.Collections.Generic;
using StuartHeathTools;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace Multiplayer
{
	/// <summary>
	///MultiplayerPlayerManager full description
	/// </summary>
	public class MultiplayerPlayerManager : NetworkSingleton<MultiplayerPlayerManager>
	{
		public NetworkVariable<int> connectedClients { get; private set; } = new NetworkVariable<int>();

		private void OnEnable()
		{
			NetworkManager.Singleton.OnClientConnectedCallback += ClientAdded;
			NetworkManager.Singleton.OnClientDisconnectCallback += ClientRemoved;

			NetworkManager.Singleton.OnServerStarted += () => Logger.Instance.Log("server started");
		}

		private void OnDisable()
		{
			if (NetworkManager.Singleton!=null)
			{
				NetworkManager.Singleton.OnClientConnectedCallback -= ClientAdded;
				NetworkManager.Singleton.OnClientDisconnectCallback -= ClientRemoved;
			}
		}

		private void ClientAdded(ulong id)
		{
			if (IsServer)
			{
				connectedClients.Value++;
				Logger.Instance.LogWithColor("Current players = " + connectedClients.Value, Color.blue);
			}
		}


		private void ClientRemoved(ulong id)
		{
			if (IsServer)
			{
				connectedClients.Value--;
			}
			Logger.Instance.Log("---disconnecting---");
			
		}
	}
}