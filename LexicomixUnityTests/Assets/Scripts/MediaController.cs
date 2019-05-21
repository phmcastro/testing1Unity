using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Globalization;
using UnityEngine.Networking;
using LexicomixNamespace;

public class MediaController : MonoBehaviour
{
    // Reference to manager
    GameManager manager;

    // Reference to UnityConfigsLexicomix
    private UnityConfig lexicomixConfigs;

    // Default values
    public Texture2D UnknownImage;
    public AudioClip UnknownAudio;

    // MediaData
    private List<string> mediaList = new List<string>();
    private Dictionary<string, MediaElement> mediaDictionary = new Dictionary<string, MediaElement>();
    private Dictionary<string, Texture> retrievedTextures = new Dictionary<string, Texture>();
    private Dictionary<string, AudioClip> retrievedAudios = new Dictionary<string, AudioClip>();
    private Dictionary<SyllableElement, AudioClip> retrievedSyllableAudios = new Dictionary<SyllableElement, AudioClip>();
    private Dictionary<string, AudioClip> alphabetAudios = new Dictionary<string, AudioClip>();

    // The coroutines array
    private List<string> coroutines = new List<string>();

    private void StartNextCoRoutine()
    {
        // Get next coroutine to start
        if (coroutines.Count > 0)
        {
            string next = coroutines[0];
            coroutines.RemoveAt(0);
            StartCoroutine(next);
        }
    }

    public void Load(GameManager _manager)
    {
        // Store reference to manager
        manager = _manager;

        // Get alphabet audioClips
        foreach(AudioClip clip in Resources.LoadAll<AudioClip>("Sounds/Alphabet"))
        {
            alphabetAudios.Add(clip.name, clip);
            //Debug.Log("Alphabet clip loaded: " + clip.name);
        }



        // If it's running on the engine editor or the website(WebGL)
#if UNITY_EDITOR
        // Define coroutines to be executed
        coroutines.Add("LoadUnityEditorConfigs");
        coroutines.Add("LoadMediaElementsInEditor");
#else
        // Store reference to Website configs
        lexicomixConfigs = GameObject.Find("GameStarter").GetComponent<GameStarterScript>().lexicomixConf;
        Debug.Log("Configs received, user: " + lexicomixConfigs.User);

        coroutines.Add("LoadMediaElements");
#endif
        coroutines.Add("GetTextureData");
        coroutines.Add("GetAudioData");
        coroutines.Add("StartGame");

        // Start next coroutine
        StartNextCoRoutine();
    }


    IEnumerator LoadUnityEditorConfigs()
    {
        WWWForm form = new WWWForm();

        Debug.Log("Loading configs...!!!");
        UnityWebRequest www = UnityWebRequest.Post("https://lexicomix.com/Dev/GetUnityConfig", form);


        www.certificateHandler = new AcceptAllCertificates();  // To remove before going to production because certificate is valid
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("Network error while loading");
            Debug.Log(www.error);
        }
        else if (www.isHttpError)
        {
            Debug.Log("Http error while loading");
            Debug.Log(www.error);
        }
        else
        {
            manager.UpdateLoadingBar(0.05f);

            // Show results as text
            Debug.Log("Config Data received from webservice");
            Debug.Log(www.downloadHandler.text);
            // Deserialize result
            //string jsonString = JsonHelper.fixJson(www.downloadHandler.data);   // byte[] is received to get images for instance
            string jsonString = JsonHelper.fixJson(www.downloadHandler.text);
            Debug.Log(jsonString);
            UnityConfig[] unityConfigs = JsonHelper.FromJson<UnityConfig>(jsonString);
            Debug.Log("Configs received with " + unityConfigs.Length + " element(s)");

            Debug.Log("GameID: " + unityConfigs[0].GameID);
            Debug.Log("GameType: " + unityConfigs[0].GameType);
            Debug.Log("Professional: " + unityConfigs[0].Professional);
            Debug.Log("User: " + unityConfigs[0].User);
            Debug.Log("GetMediaURL: " + unityConfigs[0].GetMediaService);

            lexicomixConfigs = unityConfigs[0];
            Debug.Log("Configs received, user: " + lexicomixConfigs.User);

            // Start next coroutine
            StartNextCoRoutine();
        }
    }




    IEnumerator LoadMediaElementsInEditor()
    {
        WWWForm form = new WWWForm();

        form.AddField("GameID", lexicomixConfigs.GameID);
        form.AddField("GameType", (int)lexicomixConfigs.GameType);
        form.AddField("User", lexicomixConfigs.User);
        form.AddField("Professional", lexicomixConfigs.Professional);


        Debug.Log("Loading medias...!!!");
        UnityWebRequest www = UnityWebRequest.Post(lexicomixConfigs.SiteURLBase + lexicomixConfigs.GetMediaService, form);


        www.certificateHandler = new AcceptAllCertificates();  // To remove before going to production because certificate is valid
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("Network error while loading");
            Debug.Log(www.error);
        }
        else if (www.isHttpError)
        {
            Debug.Log("Http error while loading");
            Debug.Log(www.error);
        }
        else
        {
            manager.UpdateLoadingBar(0.05f);

            // Show results as text
            Debug.Log("Media Data received from webservice");
            Debug.Log(www.downloadHandler.text);
            // Deserialize result
            //string jsonString = JsonHelper.fixJson(www.downloadHandler.data);   // byte[] is received to get images for instance
            string jsonString = JsonHelper.fixJson(www.downloadHandler.text);
            Debug.Log(jsonString);
            MediaElement[] mediaElements = JsonHelper.FromJson<MediaElement>(jsonString);
            Debug.Log(" Medias received with " + mediaElements.Length + " MediaElements");

            string prefixPictures = lexicomixConfigs.BlobURLBase + lexicomixConfigs.BlobPicture;
            string prefixAudios = lexicomixConfigs.BlobURLBase + lexicomixConfigs.BlobAudio;

            foreach (MediaElement element in mediaElements)
            {
                manager.UpdateLoadingBar(0.23f / mediaElements.Length);
                mediaList.Add(element.WordID);

                if (!mediaDictionary.ContainsKey(element.WordID))
                {

                    element.AudioURL = string.Concat(prefixAudios, element.AudioURL);
                    element.PictureURL = string.Concat(prefixPictures, element.PictureURL);

                    mediaDictionary.Add(element.WordID, element);
                }
                Debug.Log("*** " + element.WordID + ": " + element.Text.ToUpper() + " added ***");

                Debug.Log(element.Syllables.Count + " Syllables");
                foreach (SyllableElement syllable in element.Syllables)
                {
                    syllable.AudioURL = string.Concat(prefixAudios, syllable.AudioURL);
                    Debug.Log("### Syllables: " + syllable.Text);
                }

                Debug.Log(element.PhonoSyllables.Count + " SyllablesPhono");
                foreach (SyllableElement syllableP in element.PhonoSyllables)
                {
                    syllableP.AudioURL = string.Concat(prefixAudios, syllableP.AudioURL);
                    Debug.Log("### SyllablesPhono: " + syllableP.Text);
                }


            }

            // Start next coroutine
            StartNextCoRoutine();
        }
    }






    IEnumerator LoadMediaElements()
    {
        WWWForm form = new WWWForm();

        form.AddField("GameID", lexicomixConfigs.GameID);
        form.AddField("GameType", (int)lexicomixConfigs.GameType);
        form.AddField("User", lexicomixConfigs.User);
        form.AddField("Professional", lexicomixConfigs.Professional);


        Debug.Log("Loading medias...!!!");
        UnityWebRequest www = UnityWebRequest.Post(lexicomixConfigs.SiteURLBase + lexicomixConfigs.GetMediaService, form);


        www.certificateHandler = new AcceptAllCertificates();  // To remove before going to production because certificate is valid
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("Network error while loading");
            Debug.Log(www.error);
        }
        else if (www.isHttpError)
        {
            Debug.Log("Http error while loading");
            Debug.Log(www.error);
        }
        else
        {
            manager.UpdateLoadingBar(0.1f);

            // Show results as text
            Debug.Log("Media Data received from webservice");
            Debug.Log(www.downloadHandler.text);
            // Deserialize result
            //string jsonString = JsonHelper.fixJson(www.downloadHandler.data);   // byte[] is received to get images for instance
            string jsonString = JsonHelper.fixJson(www.downloadHandler.text);
            Debug.Log(jsonString);
            MediaElement[] mediaElements = JsonHelper.FromJson<MediaElement>(jsonString);
            Debug.Log(" Medias received with " + mediaElements.Length + " MediaElements");

            string prefixPictures = lexicomixConfigs.BlobURLBase + lexicomixConfigs.BlobPicture;
            string prefixAudios = lexicomixConfigs.BlobURLBase + lexicomixConfigs.BlobAudio;

            foreach (MediaElement element in mediaElements)
            {
                manager.UpdateLoadingBar(0.23f / mediaElements.Length);
                mediaList.Add(element.WordID);

                if (!mediaDictionary.ContainsKey(element.WordID))
                {

                    element.AudioURL = string.Concat(prefixAudios, element.AudioURL);
                    element.PictureURL = string.Concat(prefixPictures, element.PictureURL);

                    mediaDictionary.Add(element.WordID, element);
                }
                Debug.Log("*** " + element.WordID + ": " + element.Text.ToUpper() + " added ***");

                Debug.Log(element.Syllables.Count + " Syllables");
                foreach (SyllableElement syllable in element.Syllables)
                {
                    syllable.AudioURL = string.Concat(prefixAudios, syllable.AudioURL);
                    Debug.Log("### Syllables: " + syllable.Text);
                }

                Debug.Log(element.PhonoSyllables.Count + " SyllablesPhono");
                foreach (SyllableElement syllableP in element.PhonoSyllables)
                {
                    syllableP.AudioURL = string.Concat(prefixAudios, syllableP.AudioURL);
                    Debug.Log("### SyllablesPhono: " + syllableP.Text);
                }


            }

            // Start next coroutine
            StartNextCoRoutine();
        }
    }




    //IEnumerator LoadMediaElements()
    //{
    //    Debug.Log("Loading medias...!!!");
    //    UnityWebRequest www = UnityWebRequest.Get("https://lexicomix.com/Slideshow/GetMedias?start=0&nb=25");

    //    yield return new WaitForSeconds(2f);

    //    //foreach(KeyValuePair<string,string> pair in sessionCookies)
    //    //{
    //    //    www.SetRequestHeader(pair.Key, pair.Value);

    //    //}

    //    yield return null;

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
    //        manager.UpdateLoadingBar(0.1f);

    //        // Show results as text
    //        Debug.Log("Media Data received from webservice");
    //        Debug.Log(www.downloadHandler.text);
    //        // Deserialize result
    //        //string jsonString = JsonHelper.fixJson(www.downloadHandler.data);   // byte[] is received to get images for instance
    //        string jsonString = JsonHelper.fixJson(www.downloadHandler.text);
    //        Debug.Log(jsonString);
    //        MediaElement[] mediaElements = JsonHelper.FromJson<MediaElement>(jsonString);
    //        Debug.Log(" Medias received with " + mediaElements.Length + " MediaElements");

    //        string prefixPictures = "https://dicoadmin20181018034609.blob.core.windows.net/images/";
    //        string prefixAudios = "https://dicoadmin20181018034609.blob.core.windows.net/sounds/";

    //        foreach (MediaElement element in mediaElements)
    //        {
    //            manager.UpdateLoadingBar(0.23f / mediaElements.Length);
    //            mediaList.Add(element.WordID);

    //            if (!mediaDictionary.ContainsKey(element.WordID))
    //            {

    //                element.AudioURL = string.Concat(prefixAudios, element.AudioURL);
    //                element.PictureURL = string.Concat(prefixPictures, element.PictureURL);

    //                mediaDictionary.Add(element.WordID, element);
    //            }
    //            Debug.Log("*** " + element.WordID + ": " + element.Text.ToUpper() + " added ***");

    //            Debug.Log(element.Syllables.Count + " Syllables");
    //            foreach(SyllableElement syllable in element.Syllables)
    //            {
    //                syllable.AudioURL = string.Concat(prefixAudios, syllable.AudioURL);
    //                Debug.Log("### Syllables: " + syllable.Text);
    //            }

    //            Debug.Log(element.PhonoSyllables.Count + " SyllablesPhono");
    //            foreach (SyllableElement syllableP in element.PhonoSyllables)
    //            {
    //                syllableP.AudioURL = string.Concat(prefixAudios, syllableP.AudioURL);
    //                Debug.Log("### SyllablesPhono: " + syllableP.Text);
    //            }


    //        }

    //        // Start next coroutine
    //        StartNextCoRoutine();
    //    }
    //}







    #region Providing medias to GameManager
    public Texture GetImage(string wordID, EImageMode imageMode)
    {
        // Get texture for this word
        if (retrievedTextures.ContainsKey(wordID))
        {
            //Debug.Log(mediaDictionary[wordID].PictureURL);
            return retrievedTextures[wordID];
        }

        Debug.Log(UnknownImage);
        return UnknownImage;
    }

    public AudioClip GetAudio(string wordID)
    {
        if (retrievedAudios.ContainsKey(wordID))
        {
            //Debug.Log(mediaDictionary[wordID].AudioURL);
            return retrievedAudios[wordID];
        }

        return UnknownAudio;
    }

    public AudioClip GetAlphabetAudio(string letter)
    {
        if (alphabetAudios.ContainsKey(letter))
            return alphabetAudios[letter];

        return null;

    }

    public string GetClues(string wordID)
    {
        if (mediaDictionary.ContainsKey(wordID))
            return mediaDictionary[wordID].Text;

        return "-";

    }


    public List<SyllableElement> GetSyllablesPhono(string wordID)
    {
        List<SyllableElement> syllablesP = new List<SyllableElement>();

        foreach(SyllableElement s in mediaDictionary[wordID].PhonoSyllables)
        {
            syllablesP.Add(s);
        }
        return syllablesP;
    }

    public List<SyllableElement> GetSyllables(string wordID)
    {
        List<SyllableElement> syllables = new List<SyllableElement>();

        foreach (SyllableElement s in mediaDictionary[wordID].Syllables)
        {
            syllables.Add(s);
        }
        return syllables;
    }

    public AudioClip GetSyllableAudio(SyllableElement s)
    {
        if(retrievedSyllableAudios.ContainsKey(s))
        {
            return retrievedSyllableAudios[s];
        }

        return UnknownAudio;
    }
    #endregion






    public string RemoveDiacritics(string text)
    {

        var normalizedString = text.Normalize(NormalizationForm.FormD);

        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);

            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }

        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);

    }






    #region Download Media Data

    IEnumerator GetTextureData()
    {
        //text.text = "Loading images...";
        Debug.Log("Loading images...");
        foreach (KeyValuePair<string, MediaElement> entry in mediaDictionary)
        {

            manager.UpdateLoadingBar(0.2f / mediaDictionary.Count);


            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(entry.Value.PictureURL))
            {
                Debug.Log("Loading the image as a texture");
                uwr.certificateHandler = new AcceptAllCertificates();  // To remove before going to production because certificate is valid
                yield return uwr.SendWebRequest();
                while (!uwr.isDone)
                {
                    yield return uwr;
                }
                if (uwr.isNetworkError)
                {
                    //text.text = "Network error while loading texture";
                    Debug.Log("Network error while loading texture");
                    Debug.Log(uwr.error);
                }
                else if (uwr.isHttpError)
                {
                    //text.text = "Http error while loading texture";
                    Debug.Log("Http error while loading texture");
                    Debug.Log(uwr.error);
                }
                else
                {
                    // Get downloaded asset bundle
                    Debug.Log("Texture " + entry.Key + " loaded");

                    if (!retrievedTextures.ContainsKey(entry.Key))
                    {
                        retrievedTextures.Add(entry.Key, DownloadHandlerTexture.GetContent(uwr));
                    }

                }
            }
        }

        Debug.Log("All " + retrievedTextures.Count + " textures loaded!!!");

        // Start next coroutine
        StartNextCoRoutine();
    }


    IEnumerator GetAudioData()
    {

        Debug.Log("Loading word audios..");
        foreach (KeyValuePair<string, MediaElement> entry in mediaDictionary)
        {
            manager.UpdateLoadingBar(0.2f / mediaDictionary.Count);

            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(entry.Value.AudioURL, AudioType.WAV))
            {
                Debug.Log("Loading the audio as a audioClip");
                uwr.certificateHandler = new AcceptAllCertificates();  // To remove before going to production because certificate is valid
                yield return uwr.SendWebRequest();
                while (!uwr.isDone)
                {
                    yield return uwr;
                }
                if (uwr.isNetworkError)
                {
                    //text.text = "Network error while loading audioClip";
                    Debug.Log("Network error while loading audioClip");
                    Debug.Log(uwr.error);
                }
                else if (uwr.isHttpError)
                {
                    //text.text = "Http error while loading audioClip";
                    Debug.Log("Http error while loading audioClip");
                    Debug.Log(uwr.error);
                }
                else
                {
                    // Get downloaded asset bundle
                    Debug.Log("audioClip " + entry.Key + " loaded");

                    if (!retrievedAudios.ContainsKey(entry.Key))
                        retrievedAudios.Add(entry.Key, DownloadHandlerAudioClip.GetContent(uwr));

                }
            }
        }


        Debug.Log("Loading syllables audios..");
        foreach (KeyValuePair<string, MediaElement> entry in mediaDictionary)
        {
            manager.UpdateLoadingBar(0.2f / mediaDictionary.Count);

            foreach (SyllableElement s in entry.Value.Syllables)
            {
                using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(s.AudioURL, AudioType.WAV))
                {
                    Debug.Log("Loading the syllable audio as a audioClip");
                    uwr.certificateHandler = new AcceptAllCertificates();  // To remove before going to production because certificate is valid
                    yield return uwr.SendWebRequest();
                    while (!uwr.isDone)
                    {
                        yield return uwr;
                    }
                    if (uwr.isNetworkError)
                    {
                        //text.text = "Network error while loading audioClip";
                        Debug.Log("Network error while loading audioClip");
                        Debug.Log(uwr.error);
                    }
                    else if (uwr.isHttpError)
                    {
                        //text.text = "Http error while loading audioClip";
                        Debug.Log("Http error while loading audioClip");
                        Debug.Log(uwr.error);
                    }
                    else
                    {
                        // Get downloaded asset bundle
                        Debug.Log("Syllable audioClip " + s.WordID + ", position: " + s.Position  + " loaded");

                        if (!retrievedSyllableAudios.ContainsKey(s))
                            retrievedSyllableAudios.Add(s, DownloadHandlerAudioClip.GetContent(uwr));

                    }
                }

            }

        }


        Debug.Log("Loading syllablesPhono audios..");
        foreach (KeyValuePair<string, MediaElement> entry in mediaDictionary)
        {
            manager.UpdateLoadingBar(0.2f / mediaDictionary.Count);

            foreach (SyllableElement sP in entry.Value.PhonoSyllables)
            {
                using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(sP.AudioURL, AudioType.WAV))
                {
                    Debug.Log("Loading the syllable audio as a audioClip");
                    uwr.certificateHandler = new AcceptAllCertificates();  // To remove before going to production because certificate is valid
                    yield return uwr.SendWebRequest();
                    while (!uwr.isDone)
                    {
                        yield return uwr;
                    }
                    if (uwr.isNetworkError)
                    {
                        //text.text = "Network error while loading audioClip";
                        Debug.Log("Network error while loading audioClip");
                        Debug.Log(uwr.error);
                    }
                    else if (uwr.isHttpError)
                    {
                        //text.text = "Http error while loading audioClip";
                        Debug.Log("Http error while loading audioClip");
                        Debug.Log(uwr.error);
                    }
                    else
                    {
                        // Get downloaded asset bundle
                        Debug.Log("SyllablePhono audioClip " + sP.WordID + ", position: " + sP.Position + " loaded");

                        if (!retrievedSyllableAudios.ContainsKey(sP))
                            retrievedSyllableAudios.Add(sP, DownloadHandlerAudioClip.GetContent(uwr));

                    }
                }

            }

        }

        Debug.Log("All " + (retrievedAudios.Count + retrievedSyllableAudios.Count) + " audioClips loaded!!!");

        // Start next coroutine
        StartNextCoRoutine();

    }

    #endregion



    IEnumerator StartGame()
    {
        // Call the manager startgame
        manager.StartGame(mediaList);
        return null;
    }
}