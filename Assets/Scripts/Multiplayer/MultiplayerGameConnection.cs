//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using StuartHeathTools;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Multiplayer
{
	/// <summary>
	///MultiplayerController full description
	/// </summary>
	public class MultiplayerGameConnection : GenericUnitySingleton<MultiplayerGameConnection>
	{
		private string lobbyId;
		private RelayHostData relayHostData;
		private RelayJoinData relayJoinData;
		public static event Action OnFailedToFindGame;
		public static event Action OnFailedToLogin;
		public static event Action OnLoggedIn;
		public static event Action OnFailedToJoinGame;
		public static event Action OnFailedToCreateGame;
		public string lobbyCode;

		private Coroutine heartbeat;


		public async Task HandleJoinServer(string joinCode = "")
		{
			await HandleServerAuth();

			try
			{
				var options = new QuickJoinLobbyOptions();
				if (string.IsNullOrWhiteSpace(joinCode))
				{
					var lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
					Debug.Log("Joined lobby:" + lobby.Id);
					Debug.Log("Lobby players: " + lobby.Players.Count);
					joinCode = lobby.Data["joinCode"].Value;
					Debug.Log("Received code: " + joinCode);
				}

				var allocation = await Relay.Instance.JoinAllocationAsync(joinCode);

				relayJoinData = new RelayJoinData
				{
					key = allocation.Key,
					port = (ushort) allocation.RelayServer.Port,
					allocationID = allocation.AllocationId,
					allocationIDBytes = allocation.AllocationIdBytes,
					connectionData = allocation.ConnectionData,
					hostConnectionData = allocation.HostConnectionData,
					iPV4Address = allocation.RelayServer.IpV4
				};
				NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
					relayJoinData.iPV4Address,
					relayJoinData.port,
					relayJoinData.allocationIDBytes,
					relayJoinData.key,
					relayJoinData.connectionData,
					relayJoinData.hostConnectionData
				);
				NetworkManager.Singleton.StartClient();
			}
			catch (LobbyServiceException e)
			{
				Debug.Log("Unable to find lobby - " + e);
				OnFailedToFindGame?.Invoke();
				await CreateMatchmakingGame(false);
			}
		}

		private static async Task HandleServerAuth()
		{
			Debug.LogWarning("Not signed in.");
			try
			{
				await ServerSignIn.Instance.SignInAnon();
			}
			catch (Exception e)
			{
				Debug.LogWarning("Failed to sign in - " + e);
			}
		}


		public async Task CreateMatchmakingGame(bool isPrivate)
		{
			try
			{
				await HandleServerAuth();
				Debug.Log("Creating a new lobby");
				await HandleCreateMatchmakingServer(isPrivate);
			}
			catch (Exception e)
			{
				OnFailedToCreateGame?.Invoke();
			}
		}


		private async Task HandleCreateMatchmakingServer(bool isPrivate)
		{
			var maxConnections = 2;
			var lobbyName = "Default_Lobby_Name";
			try
			{
				var allocation = await Relay.Instance.CreateAllocationAsync(maxConnections);
				relayHostData = new RelayHostData
				{
					key = allocation.Key,
					port = (ushort) allocation.RelayServer.Port,
					allocationID = allocation.AllocationId,
					allocationIDBytes = allocation.AllocationIdBytes,
					connectionData = allocation.ConnectionData,
					iPV4Address = allocation.RelayServer.IpV4,
					joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId)
				};

				const int maxPLayers = 2;
				var options = new CreateLobbyOptions
				{
					IsPrivate = isPrivate,
					Data = new Dictionary<string, DataObject>()
					{
						{"joinCode", new DataObject(DataObject.VisibilityOptions.Member, relayHostData.joinCode)}
					}
				};
				var lobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxPLayers, options);
				lobbyId = lobby.Id;
				Debug.Log("Created lobby: " + lobby.Id);
				heartbeat = StartCoroutine(HeartBeatLobbyCor(lobby.Id, 15));

				NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
					relayHostData.iPV4Address,
					relayHostData.port,
					relayHostData.allocationIDBytes,
					relayHostData.key,
					relayHostData.connectionData
				);
				NetworkManager.Singleton.StartHost();
				Debug.Log("is host - " + NetworkManager.Singleton.IsHost);
				Debug.Log("is client - " + NetworkManager.Singleton.IsClient);

				if (isPrivate) lobbyCode = lobby.LobbyCode;
				Debug.LogError(lobby.LobbyCode);
			}
			catch (LobbyServiceException e)
			{
				Debug.Log(e);
				OnFailedToCreateGame?.Invoke();
				throw;
			}
		}

		private IEnumerator HeartBeatLobbyCor(string lobbyId, float waitTimeSeconds)
		{
			var delay = new WaitForSecondsRealtime(waitTimeSeconds);
			while (true)
			{
				Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
				Debug.Log("Lobby heartbeat");
				yield return delay;
			}
		}

		private void OnDestroy()
		{
			if (Lobbies.Instance != null)
			{
				Lobbies.Instance.DeleteLobbyAsync(lobbyId);
			}
		}

		private struct RelayHostData
		{
			public string joinCode;
			public string iPV4Address;
			public ushort port;
			public Guid allocationID;
			public byte[] allocationIDBytes;
			public byte[] connectionData;
			public byte[] key;
		}

		private struct RelayJoinData
		{
			public string joinCode;
			public string iPV4Address;
			public ushort port;
			public Guid allocationID;
			public byte[] allocationIDBytes;
			public byte[] connectionData;
			public byte[] hostConnectionData;
			public byte[] key;
		}
	}
}