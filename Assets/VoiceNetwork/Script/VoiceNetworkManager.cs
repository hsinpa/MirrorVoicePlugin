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

        private Dictionary<string, VoiceMessage> voiceDict = new Dictionary<string, VoiceMessage>();

        private bool isServer = false;

        private void Awake()
        {
            _networkManager = GetComponent<NetworkManager>();

            NetworkServer.RegisterHandler<VoiceMessage>(OnServerVoiceMessage);
            NetworkClient.RegisterHandler<VoiceMessage>(OnClientVoiceMessage);
        }

        private class CreatePlayerMessage : MessageBase
        {
            public string name;
        }

        public class VoiceMessage : MessageBase
        {
            public override void Deserialize(NetworkReader reader)
            {
                id = reader.ReadUInt32();
                voiceData = reader.ReadBytes(voiceData, 2048 * 4);
            }

            public override void Serialize(NetworkWriter writer)
            {
                writer.WriteUInt32(id);
                writer.WriteBytes(voiceData, 0, 2048 * 4);
            }

            public byte[] voiceData;
            public uint id;
        }

        #region Public API
        public void StartServer() {
            _networkManager.StartHost();
            isServer = true;
        }

        public void ConnectServer(string p_ip_address = "")
        {
            if (string.IsNullOrEmpty(p_ip_address))
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
        private void OnServerVoiceMessage(NetworkConnection connection, VoiceMessage voiceMessage)
        {
            Debug.Log("voiceMsg.id " + voiceMessage.id);

            NetworkServer.SendToAll<VoiceMessage>(voiceMessage);
        }

        private void OnClientVoiceMessage(NetworkConnection connection, VoiceMessage voiceMessage)
        {
            if (voiceMessage.voiceData == null) return;
            Debug.Log("voiceMessage.id " + voiceMessage.voiceData.Length);
            //VoiceNetworkPlayer voicePlayer = connection.identity.gameObject.GetComponent<VoiceNetworkPlayer>();
            //voicePlayer.RpcReceiveAudio(voiceMessage.voiceData);
        }
        #endregion
    }

}
