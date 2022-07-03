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

	
		public async Task<RelayHostData> SetupRelay()
		{
			try
			{
				OnCreatingGame?.Invoke();
				InitializationOptions options = new InitializationOptions().SetEnvironmentName(environment);
				await UnityServices.InitializeAsync(options);
				await ServerSignIn.Instance.SignInAnonymouslyAsync();
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
				try
				{
					NetworkManager.Singleton.StartHost();
				}
				catch (Exception e)
				{
					Logger.Instance.LogError("Failed to start host " + e);
				}
				
				OnGameCreated?.Invoke();
				Logger.Instance.Log("Code is: " + relayHostData.JoinCode);

				Logger.Instance.Log("Created game");
				return relayHostData;
			}
			catch (Exception e)
			{
				Logger.Instance.LogError(e.ToString());
				OnFailedToCreateGame?.Invoke();
				throw;
			}
		}


		public async Task<RelayJoinData> JoinRelay(string joinCode)
		{
			try
			{
				InitializationOptions options = new InitializationOptions().SetEnvironmentName(environment);
				await UnityServices.InitializeAsync(options);
				await ServerSignIn.Instance.SignInAnonymouslyAsync();
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
					joinData.AllocationIDBytes, joinData.Key,
					joinData.ConnectionData, joinData.HostConnectionData);
				OnJoinedGame?.Invoke();
				Logger.Instance.Log("Joined Game");

				return joinData;
			}
			catch (Exception e)
			{
				Logger.Instance.LogError(e.ToString());
				OnFailedToJoinGame?.Invoke();
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