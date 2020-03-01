using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror.Examples.Chat
{
    public class Player : NetworkBehaviour
    {
        [SyncVar]
        public string playerName;

        public static event Action<Player, string> OnMessage;
        public static event Action<Player, float[]> OnVoice;

        [Command]
        public void CmdSend(string message)
        {
            if (message.Trim() != "")
                RpcReceive(message.Trim());
        }

        [Command]
        public void CmsSendAudio(float[] raw_data) {
            if (raw_data != null && raw_data.Length > 0)
                RpcReceiveAudio(raw_data);
        }

        [ClientRpc]
        public void RpcReceiveAudio(float[] raw_data)
        {
            OnVoice?.Invoke(this, raw_data);
        }

        [ClientRpc]
        public void RpcReceive(string message)
        {
            OnMessage?.Invoke(this, message);
        }
    }
}
