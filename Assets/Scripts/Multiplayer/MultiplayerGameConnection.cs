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
using Unity.Services.Core.Environments;
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
		[SerializeField] private string environment = "development";
		public static event Action OnFailedToJoinGame;
		public static event Action OnFailedToCreateGame;
		public static event Action OnGameCreated;

		public static event Action OnCreatingGame;
		public static event Action OnJoinedGame;



		private Coroutine heartbeat;

		public UnityTransport Transport => NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();

		public bool IsRelayEnabled =>
			Transport != null && Transport.Protocol == UnityTransport.ProtocolType.RelayUnityTransport;

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

		public async Task<RelayHostData> SetupRelay()
		{
			try
			{
				OnCreatingGame?.Invoke();
				var options = SetupEnvironment();
				await Login(options);
				Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
				RelayHostData relayHostData = new RelayHostData()
				{
					Key = allocation.Key,
					Port = (ushort) allocation.RelayServer.Port,
					AllocationID = allocation.AllocationId,
					AllocationIDBytes = allocation.AllocationIdBytes,
					IPv4Address = allocation.RelayServer.IpV4,
					ConnectionData = allocation.ConnectionData
				};
				relayHostData.JoinCode = await RelayService.Instance.GetJoinCodeAsync(relayHostData.AllocationID);
				Transport.SetRelayServerData(relayHostData.IPv4Address, relayHostData.Port,
					relayHostData.AllocationIDBytes,
					relayHostData.Key, relayHostData.ConnectionData);
				OnGameCreated?.Invoke();
				return relayHostData;
			}
			catch (Exception e)
			{
				Debug.Log(e);
				OnFailedToCreateGame?.Invoke();
				throw;
			}
		}

		private static async Task Login(InitializationOptions options)
		{
			await UnityServices.InitializeAsync(options);
			await ServerSignIn.Instance.SignInAnonymouslyAsync();
		}

		private InitializationOptions SetupEnvironment()
		{
			InitializationOptions options = new InitializationOptions().SetEnvironmentName(environment);
			return options;
		}

		public async Task<RelayJoinData> JoinRelay(string joinCode)
		{
			try
			{
				var options = SetupEnvironment();
				await Login(options);

				JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
				RelayJoinData joinData = new RelayJoinData()
				{
					Key = allocation.Key,
					Port = (ushort) allocation.RelayServer.Port,
					AllocationID = allocation.AllocationId,
					AllocationIDBytes = allocation.AllocationIdBytes,
					IPv4Address = allocation.RelayServer.IpV4,
					ConnectionData = allocation.HostConnectionData,
					JoinCode = joinCode
				};
				Transport.SetRelayServerData(joinData.IPv4Address, joinData.Port,
					joinData.AllocationIDBytes,
					joinData.ConnectionData, joinData.HostConnectionData);
				OnJoinedGame?.Invoke();
				return joinData;
			}
			catch (Exception e)
			{
				Debug.Log(e);
				OnFailedToCreateGame?.Invoke();
				throw;
			}
		}


		public struct RelayHostData
		{
			public string JoinCode;
			public string IPv4Address;
			public ushort Port;
			public Guid AllocationID;
			public byte[] AllocationIDBytes;
			public byte[] ConnectionData;
			public byte[] Key;
		}

		public struct RelayJoinData
		{
			public string IPv4Address;
			public ushort Port;
			public Guid AllocationID;
			public byte[] AllocationIDBytes;
			public byte[] ConnectionData;
			public byte[] HostConnectionData;
			public byte[] Key;
			public string JoinCode;
		}
	}
}