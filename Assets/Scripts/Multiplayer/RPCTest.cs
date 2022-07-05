 //
 // Copyright (C) 2022 Stuart Heath. All rights reserved.
 //

 using System;
 using Characters;
 using Unity.Netcode;
 using UnityEngine;

 namespace Multiplayer
 {
	 /// <summary>
	 ///RPCTest full description
	 /// </summary>
    
	 public class RPCTest : NetworkBehaviour
	 {
		 [SerializeField] private GameObject character;

		 private void Update()
		 {
			 if (!Input.GetKeyDown(KeyCode.Space)) return;
			 Logger.Instance.Log("requesting spawn");
			 SpawnServerRPC();
		 }

		 [ServerRpc]
		 private void SpawnServerRPC()
		 {
			 Logger.Instance.Log("rpc");

			 NetworkObject no = Instantiate(character).GetComponent<NetworkObject>();
			 no.SpawnWithOwnership(OwnerClientId);
			 Logger.Instance.Log("created");
		 }
	 }
 }
