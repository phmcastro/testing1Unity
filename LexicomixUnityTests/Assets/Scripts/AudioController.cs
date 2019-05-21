using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [HideInInspector]
    public AudioSource src;
    public bool canClick;
    private List<AudioClip> audioQueue = new List<AudioClip>();

    void Start()
    {
        src = GetComponent<AudioSource>();
        canClick = true;
    }

    public void SetSound(AudioClip clip, float delay)
    {
        if(canClick)
        {
            canClick = false;

            //if (src.isPlaying)
            //{
            //    audioQueue.Add(clip);
            //    Debug.Log(audioQueue.Count);
            //    return;
            //}
            //audioQueue.Add(clip);


            StartCoroutine(PlaySound(clip, delay));

        }


    }

    //public void SetSound(AudioClip clip, float delay, bool queue)
    //{
    //    if (canClick)
    //    {
    //        canClick = false;

    //        if (queue)
    //            audioQueue.Add(clip);
    //    }


    //}

    public void PlayLetterAudio(AudioClip letterAudioClip)
    {
        src.clip = letterAudioClip;
        src.Play();
    }

    public void PlaySyllableAudio(AudioClip syllableAudioClip)
    {
        StartCoroutine(WaitSrcAndPlay(syllableAudioClip));
    }


    IEnumerator WaitSrcAndPlay(AudioClip clip)
    {
        while (src.isPlaying)
            yield return null;

        src.clip = clip;
        src.Play();
    }


    IEnumerator PlaySound(AudioClip clip, float delay)
    {


        yield return new WaitForSeconds(delay);

        src.clip = clip;
        src.Play();

        while (src.isPlaying)
        yield return null;


        //foreach (AudioClip a in audioQueue)
        //{
        //    src.clip = a;
        //    src.Play();
        //    Debug.Log("Played on the queue!!!");

        //    while (src.isPlaying)
        //        yield return null;

        //}

        //audioQueue.Clear();


        canClick = true;

    }
}