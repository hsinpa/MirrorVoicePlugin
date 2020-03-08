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

        public static string Getname(uint netID)
        {
            return "VoicePlayer " + netID;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            this.gameObject.name = Getname(this.netId);

            playoutAudioSource.clip = AudioClip.Create("Receiver", 2048, 1, 22050, false);

            string[] devices = Microphone.devices;

            if (devices != null && devices.Length > 0)
            {
                microphoneName = devices[0];
                micAudioSource.clip = Microphone.Start(microphoneName, true, 1, 22050);
                micAudioSource.loop = true;
                micAudioSource.mute = true;
                micAudioSource.Play();
            }
        }

        [Command]
        public void CmdSendAudio(byte[] byteArray)
        {

            if (byteArray != null && byteArray.Length > 0) {
                VoiceNetworkManager.VoiceMessage voiceMsg = new VoiceNetworkManager.VoiceMessage(byteArray, this.netId);

                //NetworkClient.Send<VoiceNetworkManager.VoiceMessage>(voiceMsg);
                RpcReceiveAudio(byteArray);
            }
        }

        [ClientRpc]
        public void RpcReceiveAudio(byte[] raw_data)
        {
            if (!isLocalPlayer) {
                var floatArray2 = new float[raw_data.Length / 4];
                System.Buffer.BlockCopy(raw_data, 0, floatArray2, 0, raw_data.Length);

                for (int i = 0; i < 2000; i += 100) {
                    Debug.Log(floatArray2[i]);
                }

                playoutAudioSource.clip.SetData(floatArray2, 0);
                playoutAudioSource.Play();
                playoutAudioSource.loop = true;
            }
        }

        private void StartVoiceRecord()
        {
            if (micAudioSource.clip != null) {
                float[] data = new float[2048];
                int index = Microphone.GetPosition(microphoneName);
                micAudioSource.clip.GetData(data, index);

                var byteArray = new byte[data.Length * 4];
                System.Buffer.BlockCopy(data, 0, byteArray, 0, byteArray.Length);

                CmdSendAudio(byteArray);
            }
        }

        private void Update()
        {
            if (isLocalPlayer)
            {
                StartVoiceRecord();
            }
        }
    }
}