using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class DataGetterScript : MonoBehaviour
{
    public string diaporamaID;
    public int howManyImages;


    [HideInInspector]
    //public Diaporama Diaporama;
    public Dictionary<string, string> PictureDictionary = new Dictionary<string, string>();
    public Dictionary<string, string> AudioDictionary = new Dictionary<string, string>();
    public Dictionary<string, string> TextDictionary = new Dictionary<string, string>();

    [HideInInspector]
    public List<string> requestedIDs;


    void Start()
    {
       
        //StartCoroutine(GetData());
    }



    //IEnumerator GetData()
    //{

    //    if (waitALittleBit)
    //        yield return new WaitForSeconds(0.1f);

    //    Debug.Log("Loading diaporama...!!!");
    //    UnityWebRequest www = UnityWebRequest.Get(string.Format("https://lexicomix.com/Game/GetDiaporama?id={0}&nb={1}", diaporamaID, howManyImages));
    //    www.certificateHandler = new AcceptAllCertificates();  // To remove before going to production because certificate is valid
    //    yield return www.SendWebRequest();

    //    if (www.isNetworkError)
    //    {
    //        Debug.Log("Network error while loading");
    //        Debug.Log(www.error);
    //    }
    //    else if (www.isHttpError)
    //    {
    //        Debug.Log("Http error while loading");
    //        Debug.Log(www.error);
    //    }
    //    else
    //    {
    //        // Show results as text
    //        Debug.Log("Diaporama Data received from webservice");
    //        Debug.Log(www.downloadHandler.text);
    //        // Deserialize result
    //        //string jsonString = JsonHelper.fixJson(www.downloadHandler.data);   // byte[] is received to get images for instance
    //        string jsonString = JsonHelper.fixJson(www.downloadHandler.text);
    //        Debug.Log(jsonString);
    //        Diaporama[] diaporama = JsonHelper.FromJson<Diaporama>(jsonString);
    //        Debug.Log(diaporama.Length + " Diaporama received with " + diaporama[0].MediaList.Count + " MediaElements");



    //        int index = 0;
    //        //Fill in the dictionaries
    //        foreach (MediaElement nextElement in diaporama[0].MediaList)
    //        {

    //            // Add next picture
    //            if (!PictureDictionary.ContainsKey(nextElement.WordID))
    //                PictureDictionary.Add(nextElement.WordID, nextElement.PictureURL);

    //            // Add next audio
    //            if (!AudioDictionary.ContainsKey(nextElement.WordID))
    //                AudioDictionary.Add(nextElement.WordID, nextElement.AudioURL);


    //            // Fill in the dictionaries

    //            foreach (SyllableElement nextSyllable in nextElement.Syllables)
    //            {
    //                // Add next syllable audio
    //                //if (!AudioDictionary.ContainsKey(nextSyllable.WordID))
    //                    //AudioDictionary.Add(nextSyllable.WordID, nextSyllable.AudioURL);
    //            }

    //            foreach(TextClue nextText in nextElement.TextClues)
    //            {
    //                // Add next text
    //                if (!TextDictionary.ContainsKey(nextElement.WordID))
    //                    TextDictionary.Add(nextElement.WordID, nextText.Text.ToUpper());
    //                //Debug.Log("@#@ " + TextDictionary[nextElement.WordID] + " @#@");

    //            }

    //            // Fill in WordID List
    //            requestedIDs.Add(nextElement.WordID);
    //            Debug.Log("*** " + nextElement.WordID + ": " + TextDictionary[nextElement.WordID] + " added ***");



    //            index++;
    //            if (index >= diaporama[0].MediaList.Count)
    //            {
    //                Diaporama = diaporama[0];
    //                Debug.Log("!!!Diaporama Data Acquired!!!");


    //                // Call all loading methods in every Media Retriever Script inside the GameObjects with media
    //                // and send the Dictionaries with IDs and URLs
    //                foreach (GameObject obj in mediaObjects)
    //                {
    //                    obj.SendMessage("StartLoadingAudioData", AudioDictionary, SendMessageOptions.DontRequireReceiver);
    //                    obj.SendMessage("StartLoadingTextureData", PictureDictionary, SendMessageOptions.DontRequireReceiver);

    //                }

    //                GameObject.FindWithTag("MediaController").GetComponent<MediaController>().GetTexts(TextDictionary);
    //                //SendMessage("StartLoadingAudioData", AudioDictionary, SendMessageOptions.RequireReceiver);
    //                //SendMessage("GetTexts", TextDictionary, SendMessageOptions.RequireReceiver);

    //            }
    //        }


    //    }
    //}
}