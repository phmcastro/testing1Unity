using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LexicomixNamespace;

public class GameStarterScript : MonoBehaviour
{
    [HideInInspector]
    public UnityConfig lexicomixConf;

    public GameObject startButton;

    private void Start()
    {
        startButton.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }

    public void SetCookie(string configs)
    {
        Debug.Log("InsideSetCookie");

        Debug.Log(configs);

        string jsonString = JsonHelper.fixJson(configs);
        Debug.Log(jsonString);
        UnityConfig[] unityConfigs = JsonHelper.FromJson<UnityConfig>(jsonString);
        Debug.Log("UnityConfig lenght: " + unityConfigs.Length);


        Debug.Log("GameID: " + unityConfigs[0].GameID);
        Debug.Log("GameType: " + unityConfigs[0].GameType);
        Debug.Log("Professional: " + unityConfigs[0].Professional);
        Debug.Log("User: " + unityConfigs[0].User);
        Debug.Log("GetMediaURL: " + unityConfigs[0].GetMediaService);

        lexicomixConf = unityConfigs[0];

        Invoke("ActivateStartButton", 3f);
    }

    void ActivateStartButton()
    {
        startButton.SetActive(true);
    }

}