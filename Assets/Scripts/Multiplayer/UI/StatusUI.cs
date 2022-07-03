using TMPro;
using UnityEngine;

namespace Multiplayer.UI
{
    public class StatusUI : MonoBehaviour
    {
        [SerializeField]    private TextMeshProUGUI loginText;
        [SerializeField]   private TextMeshProUGUI gametext;

        private void OnEnable()
        {
            ServerSignIn.OnSignedOut += SignedOut;
            ServerSignIn.OnSignInFailed += SignInFailed;
            ServerSignIn.OnSignedIn += SignedIn;
       
            MultiplayerGameConnection.OnFailedToFindGame += FailedToFindGame;
            MultiplayerGameConnection.OnFailedToJoinGame += FailedToJoinGame;
            MultiplayerGameConnection.OnFailedToCreateGame += FailedToCreateGame;

        }

        private void FailedToFindGame() => gametext.text = "Unable to locate game";
        private void FailedToJoinGame() => gametext.text = "Unable to join game";
        private void FailedToCreateGame() => gametext.text = "Unable to create  game";

        private void SignedOut() => loginText.text = "Signed Out";
        private void SignInFailed() => loginText.text = "Sign In Failed";
        private void SignedIn() => loginText.text = "Signed In";


        private void OnDisable()
        {
        }
    }
}
