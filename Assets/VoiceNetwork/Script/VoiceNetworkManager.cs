using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
namespace VoiceNetwork {

    [RequireComponent(typeof(NetworkManager))]
    public class VoiceNetworkManager : MonoBehaviour
    {
        [SerializeField]
        private string targetIP;

        [SerializeField]
        private GameObject voicePlayerPrefab;

        private NetworkManager _networkManager;

        private bool isServer = false;

        private void Awake()
        {
            _networkManager = GetComponent<NetworkManager>();

            NetworkServer.RegisterHandler<CreatePlayerMessage>(OnCreatePlayer);
        }

        private class CreatePlayerMessage : MessageBase
        {
            public string name;
        }

        #region Public API
        public void StartServer() {
            _networkManager.StartHost();
            isServer = true;
        }

        public void ConnectServer(string p_ip_address = "")
        {
            if (!string.IsNullOrEmpty(p_ip_address))
                p_ip_address = targetIP;

            _networkManager.networkAddress = p_ip_address;

            _networkManager.StartClient();

            isServer = false;
        }

        public void Leave() {
            if (!_networkManager.isNetworkActive) return;

            if (isServer)
                _networkManager.StopServer();
            else
                _networkManager.StopClient();

            isServer = false;
        }
        #endregion

        #region Private API
        private void OnCreatePlayer(NetworkConnection connection, CreatePlayerMessage createPlayerMessage)
        {
            // create a gameobject using the name supplied by client
            GameObject playergo = Instantiate(voicePlayerPrefab);
            playergo.GetComponent<VoiceNetworkPlayer>().name = createPlayerMessage.name;

            // set it as the player
            NetworkServer.AddPlayerForConnection(connection, playergo);
        }
        #endregion
    }

}
