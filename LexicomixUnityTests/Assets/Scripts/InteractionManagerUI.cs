using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class InteractionManagerUI : MonoBehaviour
{
    public AudioClip startSound;
    private AudioSource audioSrc;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {   
        audioSrc = gameObject.AddComponent<AudioSource>();
        audioSrc.playOnAwake = false;
        audioSrc.loop = false;
        audioSrc.clip = startSound;

        audioSrc.Play();
    }
        
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
        gameObject.GetComponent<Canvas>().enabled = false;
    }

}
