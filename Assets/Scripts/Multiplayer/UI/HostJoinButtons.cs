using StuartHeathTools;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer.UI
{
    public class HostJoinButtons : CanvasGroupBase
    {
        [SerializeField] private InputField inputField;

        public void Close() => Hide();
        public void InputFieldUpdated()
        {
            GetInput();
        }

        private string GetInput()=> inputField.text;
        

        public void JoinPrivate()
        {
            
        }
        public void HostPrivate()
        {
        
        }
        public void JoinMatchmaking()
        {
        
        }
    }
}
