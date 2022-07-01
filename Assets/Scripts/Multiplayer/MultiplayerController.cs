//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
	public class MultiplayerController : MonoBehaviour
	{
		private string lobbyId;
		private bool isSignedIn = false;
		private RelayHostData relayHostData;
		private RelayJoinData relayJoinData;

		private async void Start()
		{
			await UnityServices.InitializeAsync();
			SetupAuthEvents();
			await SignInAnon();
		}

		private void SetupAuthEvents()
		{
			AuthenticationService.Instance.SignedIn += () =>
			{
				Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
				Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
				isSignedIn = true;
			};
			AuthenticationService.Instance.SignInFailed += (err) =>
			{
				Debug.LogError(err);
				isSignedIn = false;
			};
			AuthenticationService.Instance.SignedOut += () =>
			{
				Debug.Log("Player signed out");
				isSignedIn = false;
			};
		}

		async Task SignInAnon()
		{
			try
			{
				await AuthenticationService.Instance.SignInAnonymouslyAsync();
				Debug.Log("Signed in anonymously!");
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				throw;
			}
		}

		public async void FindMatch()
		{
			Debug.Log("Looking for lobby...");
			if (!isSignedIn)
			{
				Debug.LogWarning("Not signed in.");
				return;
			}

			try
			{
				var options = new QuickJoinLobbyOptions();
				var lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
				Debug.Log("Joined lobby:" + lobby.Id);
				Debug.Log("Lobby players: " + lobby.Players.Count);
				var joinCode = lobby.Data["joinCode"].Value;
				Debug.Log("Received code: " + joinCode);
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
				CreateMatch();
			}
		}

		private async void CreateMatch()
		{
			var maxConnections = 1;
			Debug.Log("Creating a new lobby");


			try
			{
				var allocation = await Relay.Instance.CreateAllocationAsync(maxConnections);
				relayHostData = new RelayHostData()
				{
					key = allocation.Key,
					port = (ushort) allocation.RelayServer.Port,
					allocationID = allocation.AllocationId,
					allocationIDBytes = allocation.AllocationIdBytes,
					connectionData = allocation.ConnectionData,
					iPV4Address = allocation.RelayServer.IpV4
				};

				relayHostData.joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
				const string lobbyName = "game_lobby";
				const int maxPLayers = 2;
				var options = new CreateLobbyOptions
				{
					IsPrivate = false
				};
				options.Data = new Dictionary<string, DataObject>()
				{
					{"joinCode", new DataObject(DataObject.VisibilityOptions.Member, relayHostData.joinCode)}
				};
				var lobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxPLayers, options);
				lobbyId = lobby.Id;
				Debug.Log("Created lobby: " + lobby.Id);
				StartCoroutine(HeartBeatLobbyCor(lobby.Id, 15));

				NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
					relayHostData.iPV4Address,
					relayHostData.port,
					relayHostData.allocationIDBytes,
					relayHostData.key,
					relayHostData.connectionData
				);
				NetworkManager.Singleton.StartHost();
			}
			catch (LobbyServiceException e)
			{
				Debug.Log(e);
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

		private void OnDestroy() => Lobbies.Instance.DeleteLobbyAsync(lobbyId);

		public struct RelayHostData
		{
			public string joinCode;
			public string iPV4Address;
			public ushort port;
			public Guid allocationID;
			public byte[] allocationIDBytes;
			public byte[] connectionData;
			public byte[] key;
		}

		public struct RelayJoinData
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