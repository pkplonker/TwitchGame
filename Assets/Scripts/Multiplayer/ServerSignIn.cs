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
		public static event Action OnSignedIn;
		public static event Action OnSigningIn;

		public static event Action OnSignInFailed;
		public static event Action OnSignedOut;

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
				OnSignedIn?.Invoke();
			};
			AuthenticationService.Instance.SignInFailed += (err) =>
			{
				Debug.LogError(err);
				OnSignInFailed?.Invoke();
			};
			AuthenticationService.Instance.SignedOut += () =>
			{
				Debug.Log("Player signed out");
				OnSignedOut?.Invoke();
			};
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
				OnSigningIn?.Invoke();

				await AuthenticationService.Instance.SignInAnonymouslyAsync();
				Debug.Log("Signed in anonymously!");
				signInTask = null;
				OnSignedIn?.Invoke();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				throw;
			}
		}
	}
}