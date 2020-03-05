using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace VoiceNetwork
{
    public class VoiceNetworkDemo : MonoBehaviour
    {
        [SerializeField]
        VoiceNetworkManager manager;

        [SerializeField]
        InputField textInput;

        [SerializeField]
        Button StartServerBtn;

        [SerializeField]
        Button ConnectServerBtn;

        #region UI Functions
        private void Start()
        {
            StartServerBtn.onClick.RemoveAllListeners();
            StartServerBtn.onClick.AddListener(() => HostRoom());

            ConnectServerBtn.onClick.RemoveAllListeners();
            ConnectServerBtn.onClick.AddListener(() => JoinRoom());
        }

        public void JoinRoom() {
            manager.ConnectServer( textInput.text );
            RemoveFromSight(false);
        }

        public void HostRoom()
        {
            manager.StartServer();
            RemoveFromSight(false);
        }

        public void RemoveFromSight(bool isVisible) {
            this.gameObject.SetActive(isVisible);
        }
        #endregion
    }
}