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

        private string microphoneName = "";

        public override void OnStartClient()
        {
            base.OnStartClient();
            playoutAudioSource.clip = AudioClip.Create("Receiver", 1024, 1, 44100, false);

            string[] devices = Microphone.devices;

            if (devices != null && devices.Length > 0) {
                microphoneName = devices[0];
                micAudioSource.clip = Microphone.Start(microphoneName, true, 1, 44100);
            }
        }

        [Command]
        public void CmsSendAudio(float[] raw_data)
        {

            var byteArray = new byte[raw_data.Length * 4];
            System.Buffer.BlockCopy(raw_data, 0, byteArray, 0, byteArray.Length);

            if (raw_data != null && raw_data.Length > 0)
                RpcReceiveAudio(byteArray);
        }

        [ClientRpc]
        public void RpcReceiveAudio(byte[] raw_data)
        {
            if (!isLocalPlayer) {
                var floatArray2 = new float[raw_data.Length / 4];
                System.Buffer.BlockCopy(raw_data, 0, floatArray2, 0, raw_data.Length);

                playoutAudioSource.clip.SetData(floatArray2, 0);
                playoutAudioSource.Play();
            }
        }

        [Command]
        public void CmsSendSimpleComment()
        {
            RpcReceiveHello("Hello from" + ((netIdentity.netId)));
        }

        [ClientRpc]
        public void RpcReceiveHello(string SayHello)
        {
            if (!isLocalPlayer)
            {
                Debug.Log(SayHello);
            }
        }

        private void StartVoiceRecord()
        {
            if (micAudioSource.clip != null) {
                float[] data = new float[1024];
                micAudioSource.clip.GetData(data, 0);

                CmsSendAudio(data);
            }
        }

        private void Update()
        {
            StartVoiceRecord();
            CmsSendSimpleComment();
        }
    }
}