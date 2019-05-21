using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayAudioController : MonoBehaviour
{
    private Text thisText;
    private AudioSource audioSrc;
    public GameObject audioRetrieverObj;
    public bool playDifferentAudioEachTime;

    // Start is called before the first frame update
    void Start()
    {
        thisText = GetComponentInChildren<Text>();
        audioSrc = GetComponent<AudioSource>();
        thisText.text = "Waiting for audio...";
    }


    public void Click()
    {
        if (thisText.text == "Play" && !audioSrc.isPlaying)
            audioSrc.Play();
        else if (thisText.text != "Play")
            Debug.Log("AudioClip File Not Found");

        if (playDifferentAudioEachTime)
            StartCoroutine(LoadNewAudio());

    }

    IEnumerator LoadNewAudio()
    {
        audioRetrieverObj.SetActive(false);


        yield return new WaitForSeconds(1);

        audioRetrieverObj.SetActive(true);


    }

}
