using System;
using System.Threading.Tasks;
using StuartHeathTools;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Multiplayer
{
	public class ServerSignIn : GenericUnitySingleton<ServerSignIn>
	{
		private Task signInTask;

		private async void Start()
		{
			await UnityServices.InitializeAsync();
			SetupAuthEvents();
		}

		private void SetupAuthEvents()
		{
			AuthenticationService.Instance.SignedIn += () =>
			{
				Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
				Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
			};
			AuthenticationService.Instance.SignInFailed += (err) => { Debug.LogError(err); };
			AuthenticationService.Instance.SignedOut += () => { Debug.Log("Player signed out"); };
		}

		public async Task SignInAnon()
		{
			Debug.Log("Attempting sign in");
			if (signInTask == null)
			{
				signInTask = UnityServices.InitializeAsync();
				await signInTask;
			}

			if (AuthenticationService.Instance.IsAuthorized) return;
			try
			{
				await AuthenticationService.Instance.SignInAnonymouslyAsync();
				Debug.Log("Signed in anonymously!");
				signInTask = null;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				throw;
			}
		}
	}
}