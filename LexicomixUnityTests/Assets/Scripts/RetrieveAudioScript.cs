using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;


namespace LexicomixNamespace
{
    public class RetrieveAudioScript : MonoBehaviour
    {
        public Text text;
        public Text buttonText;
        public AudioSource[] audioSrc = new AudioSource[5];


        // Everytime this application STARTS it will load the file
        void Start()
        {
            StartCoroutine(GetData());
        }


        //Everytime this GameObject is ENABLED it will reload the file from the website
        private void OnEnable()
        {
            StartCoroutine(GetData());
        }


        public void PlayAudiosWithInterval(float seconds)
        {

            StartCoroutine(PlayAudios(seconds));

        }

        IEnumerator PlayAudios(float interval)
        {

            foreach (AudioSource ads in audioSrc)
            {
                ads.Play();

                while (ads.isPlaying)
                    yield return null;

                yield return new WaitForSeconds(interval);

            }

            //audioSrc[0].Play();

            //yield return new WaitForSeconds(interval + audioSrc[0].clip.length);

        }


        IEnumerator GetData()
        {
            text.text = "Loading audios...";
            UnityWebRequest www = UnityWebRequest.Get("https://www.lexicomix.com/Game/GetRandomAudios?nb=5");
            www.certificateHandler = new AcceptAllCertificates();  // To remove before going to production because certificate is valid
            yield return www.SendWebRequest();
            if (www.isNetworkError)
            {
                text.text = "Network error while loading";
                Debug.Log(www.error);
            }
            else if (www.isHttpError)
            {
                text.text = "Http error while loading";
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log("Audios received from webservice");
                Debug.Log(www.downloadHandler.text);
                // Deserialize result
                //string jsonString = JsonHelper.fixJson(www.downloadHandler.data);   // byte[] is received to get images for instance
                string jsonString = JsonHelper.fixJson(www.downloadHandler.text);
                Debug.Log(jsonString);
                Audio[] audios = JsonHelper.FromJson<Audio>(jsonString);
                Debug.Log(audios.Length + " audios received");
                // Display first picture
                int index = 0;
                foreach (Audio nextAudio in audios)
                {
                    text.text = nextAudio.URL;
                    using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(nextAudio.URL, UnityEngine.AudioType.WAV))
                    {
                        Debug.Log("Loading the Audio as a audioCLip");
                        uwr.certificateHandler = new AcceptAllCertificates();  // To remove before going to production because certificate is valid
                        yield return uwr.SendWebRequest();
                        while (!uwr.isDone)
                        {
                            yield return uwr;
                            buttonText.text = "Loading...";
                        }
                        if (uwr.isNetworkError)
                        {
                            text.text = "Network error while loading audioClip";
                            Debug.Log(uwr.error);
                        }
                        else if (www.isHttpError)
                        {
                            text.text = "Http error while loading audioClip";
                            Debug.Log(uwr.error);
                        }
                        else
                        {
                            buttonText.text = "Play";
                            // Get downloaded asset bundle
                            Debug.Log("AudioClip loaded");
                            var audioC = DownloadHandlerAudioClip.GetContent(uwr);
                            audioSrc[index++].clip = audioC;
                            //image.texture = texture;
                        }
                    }
                }
            }
        }
    }


}

