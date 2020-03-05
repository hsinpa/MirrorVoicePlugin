using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace VoiceNetwork
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class VoiceNetworkPlayer : NetworkBehaviour
    {
        [SerializeField]
        private AudioSource micAudioSource;

        [SerializeField]
        public AudioSource playoutAudioSource;

        private bool isMicAvailable;

        public override void OnStartClient()
        {
            base.OnStartClient();
            playoutAudioSource.clip = AudioClip.Create("Receiver", 1024, 1, 44100, false);

            string[] devices = Microphone.devices;
            if (devices == null || devices.Length <= 0) return;

            isMicAvailable = (devices != null && devices.Length > 0);
        }

        [Command]
        public void CmsSendAudio(float[] raw_data)
        {
            if (raw_data != null && raw_data.Length > 0)
                RpcReceiveAudio(raw_data);
        }

        [ClientRpc]
        public void RpcReceiveAudio(float[] raw_data)
        {
            playoutAudioSource.clip.SetData(raw_data, 0);
            playoutAudioSource.Play();
        }

        private void StartVoiceRecord()
        {
            if (!isMicAvailable) return;

            float[] data = new float[1024];
            micAudioSource.clip = Microphone.Start("Built-in Microphone", true, 1, 44100);
            micAudioSource.clip.GetData(data, 0);

            CmsSendAudio(data);
        }

        private void Update()
        {
            StartVoiceRecord();
        }
    }
}