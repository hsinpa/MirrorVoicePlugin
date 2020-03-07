using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
namespace VoiceNetwork {
    public class VoiceNetworkManager : NetworkManager
    {
        [SerializeField]
        private string targetIP;

        [SerializeField]
        private GameObject voicePlayerPrefab;

        private Dictionary<string, VoiceNetworkPlayer> voicePlayerDict = new Dictionary<string, VoiceNetworkPlayer>();

        private bool isServer = false;

        public override void OnStartServer()
        {
            base.OnStartServer();
            //NetworkServer.RegisterHandler<CreatePlayerMessage>(OnCreatePlayer);
        }

        #region Public API
        public void StartTheServer() {
            StartHost();
            isServer = true;
        }

        public void ConnectServer(string p_ip_address = "")
        {
            if (string.IsNullOrEmpty(p_ip_address))
                p_ip_address = targetIP;

            networkAddress = p_ip_address;

            StartClient();

            isServer = false;
        }

        public void Leave() {
            if (!isNetworkActive) return;

            if (isServer)
                StopServer();
            else
                StopClient();

            isServer = false;
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            Debug.Log("Player is connect : " + conn.identity.netId.ToString());
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

        public class VoiceByteMsg : MessageBase
        {
            public byte[] voiceBytes;
            public string player_id;
        }

        private class CreatePlayerMessage : MessageBase
        {
            public string name;
        }
    }

}
