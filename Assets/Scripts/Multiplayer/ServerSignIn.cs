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
		public static event Action OnSignedIn;
		public static event Action OnSigningIn;

		public static event Action OnSignInFailed;
		public static event Action OnSignedOut;

		private async void Start()
		{
			await UnityServices.InitializeAsync();
			SignedOut();
		}

		public bool GetSignInStatus()
		{
			if (AuthenticationService.Instance == null)
			{
				UnityServices.InitializeAsync();
				return false;
			}

			return AuthenticationService.Instance.IsSignedIn;
		}

		public void SignOut()
		{
			if (AuthenticationService.Instance == null) return;
			AuthenticationService.Instance.SignOut();
		}

		private void SetupEvents()
		{
			AuthenticationService.Instance.SignedIn += () => { SignedIn(); };

			AuthenticationService.Instance.SignInFailed += (err) => { SignInFailed(err); };

			AuthenticationService.Instance.SignedOut += () => { SignedOut(); };
		}

		private static void SignedOut()
		{
			Debug.Log("Player signed out.");
			OnSignedOut?.Invoke();
		}

		private static void SignInFailed(RequestFailedException err)
		{
			Debug.LogError(err);
			OnSignInFailed?.Invoke();
		}

		private static void SignedIn()
		{
			Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
			Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
			OnSignedIn?.Invoke();
		}

		public async Task SignInAnonymouslyAsync()
		{
			if (GetSignInStatus()) return;
			OnSigningIn?.Invoke();
			try
			{
				await AuthenticationService.Instance.SignInAnonymouslyAsync();
				Debug.Log("Sign in anonymously succeeded!");
				Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
				OnSignedIn?.Invoke();
			}
			catch (AuthenticationException ex)
			{
				Debug.LogException(ex);
				OnSignInFailed?.Invoke();
			}
			catch (RequestFailedException ex)
			{
				Debug.LogException(ex);
				OnSignInFailed?.Invoke();
			}
		}
	}
}