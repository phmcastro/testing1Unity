using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using LexicomixNamespace;




public class RetrieveMyAudioScript : MonoBehaviour
{
    public Text text;
    public int audioID;
    private int lastAudioID;
    public int voiceID = 1;
    public int accentID = 9;
    private AudioSource audioSrc;
    private string myGetAudioString;
    public Texture textureTest;

    //void OnGUI()
    //{
    //    GUI.skin.button.stretchWidth = true;
    //    GUI.skin.button.stretchHeight = true;

    //    if(GUI.Button(new Rect(10, 10, textureTest.width, textureTest.height), textureTest,GUIStyle.none))
    //    {
    //        GetAnotherAudioAndPlayAudio();

    //    }
    //}

    // Everytime this application STARTS it will load the file
    void Start()
    {
        //StartCoroutine(GetData());
        audioSrc = GetComponent<AudioSource>();
        //myGetAudioString = "https://www.lexicomix.com/Game/GetAudios?Parent=" + audioID + "&VoiceID=1&AccentID=9";
        lastAudioID = 0;
        myGetAudioString = String.Format("https://www.lexicomix.com/Game/GetAudios?Parent={0}&VoiceID={1}&AccentID={2}", audioID, voiceID, accentID);

    }


    //Everytime this GameObject is ENABLED it will reload the file from the website
    private void OnEnable()
    {
        StartCoroutine(GetData());
    }

    //To be called by the button component
    public void GetAnotherAudioAndPlayAudio()
    {
        if(audioID == lastAudioID)
        {
            audioSrc.Play();
            return;
        }

        myGetAudioString = String.Format("https://www.lexicomix.com/Game/GetAudios?Parent={0}&VoiceID={1}&AccentID={2}", audioID, voiceID, accentID);

        StartCoroutine(GetData());


        lastAudioID = audioID;
    }




    IEnumerator GetData()
    {
        text.text = "Loading audios...";
        UnityWebRequest www = UnityWebRequest.Get(myGetAudioString);
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
            if (audios.Length > 0)
            {
                text.text = audios[0].URL;
                using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(audios[0].URL, UnityEngine.AudioType.WAV))
                {
                    Debug.Log("Loading the Audio as a audioCLip");
                    uwr.certificateHandler = new AcceptAllCertificates();  // To remove before going to production because certificate is valid
                    yield return uwr.SendWebRequest();
                    while (!uwr.isDone)
                    {

                        yield return uwr;
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
                        // Get downloaded asset bundle
                        Debug.Log("AudioClip loaded");

                        var audioC = DownloadHandlerAudioClip.GetContent(uwr);
                        audioSrc.clip = audioC;
                        audioSrc.Play();
                        //image.texture = texture;
                    }
                }
            }
        }
    }
}