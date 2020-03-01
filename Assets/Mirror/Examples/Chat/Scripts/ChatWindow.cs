using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror.Examples.Chat
{
    public class ChatWindow : MonoBehaviour
    {
        public InputField chatMessage;
        public Text chatHistory;
        public Scrollbar scrollbar;

        public AudioSource grabAudioSource;
        public AudioSource playAudioSource;
        Player player;

        public void Awake()
        {
            player = NetworkClient.connection.identity.GetComponent<Player>();

            Player.OnMessage += OnPlayerMessage;
            Player.OnVoice += PlayVoiceRecord;

            playAudioSource.clip = AudioClip.Create("Receiver", 1024, 1, 44100,false);
        }

        public void StartVoiceRecord() {
            float[] data = new float[1024];
            grabAudioSource.clip = Microphone.Start("Built-in Microphone", true, 1, 44100);
            grabAudioSource.clip.GetData(data, 0);

            player.CmsSendAudio(data);
        }

        private void PlayVoiceRecord(Player player, float[] data) {
            playAudioSource.clip.SetData(data, 0);
            playAudioSource.Play();
        }

        private void OnPlayerMessage(Player player, string message)
        {
            string prettyMessage = player.isLocalPlayer ?
                $"<color=red>{player.playerName}: </color> {message}" :
                $"<color=blue>{player.playerName}: </color> {message}";
            AppendMessage(prettyMessage);

            Debug.Log(message);
        }

        public void OnSend()
        {
            if (chatMessage.text.Trim() == "") return;

            // get our player
            Player player = NetworkClient.connection.identity.GetComponent<Player>();

            // send a message
            player.CmdSend(chatMessage.text.Trim());

        }

        internal void AppendMessage(string message)
        {
            StartCoroutine(AppendAndScroll(message));
        }

        IEnumerator AppendAndScroll(string message)
        {
            chatHistory.text += message + "\n";

            // it takes 2 frames for the UI to update ?!?!
            yield return null;
            yield return null;

            // slam the scrollbar down
            scrollbar.value = 0;
        }
    }
}
