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
            

        }
        private void JoinedGame()=> gametext.text = "Joined Game";

        private void GameCreated()=> gametext.text = "Game Created";
        private void CreatingGame()=> gametext.text = "Creating game ";

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
