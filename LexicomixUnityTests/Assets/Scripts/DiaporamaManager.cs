using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiaporamaManager : GameManager
{
    public InputField userInput;
    public Text sessionTime;
    public TextMeshProUGUI myTextMesh;

    public void Init()
    {
        gameConfig = new DiaporamaConfig();

        if(((DiaporamaConfig)gameConfig).onlyUppercaseTextInput)
            userInput.onValueChanged.AddListener(delegate { userInput.text = userInput.text.ToUpper(); });

        userInput.onEndEdit.AddListener(delegate { CheckInput(); });


    }


    void CheckInput()
    {
        //if (Input.GetKeyDown(KeyCode.Return) && canClick)
        //{
        //    string userInput = GetTextInput();
        //    string lexicomixWord = textStrings[diaporamaData.requestedIDs[index]];

        //    //string userInput = RemoveDiacritics(GetTextInput()).Replace('-', ' ');
        //    //string lexicomixWord = RemoveDiacritics(textStrings[diaporamaData.requestedIDs[index]]).Replace('-', ' ');


        //    // Remove accents and dash for comparison
        //    if (!accentCheck)
        //    {
        //        userInput = RemoveDiacritics(userInput).Replace('-', ' ');
        //        lexicomixWord = RemoveDiacritics(lexicomixWord).Replace('-', ' ');
        //    }


        //    // Depending on case sensitivity
        //    if (!caseSensitive)
        //    {
        //        userInput = userInput.ToLower();
        //        lexicomixWord = lexicomixWord.ToLower();
        //    }

        //    if (userInput == lexicomixWord)
        //    {
        //        //StartCoroutine(PlayAudioWaitAndDisplayNextImage(changeImageInEveryRound));

        //        Debug.Log(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ff") + "Right word typed!!!");

        //    }


        //}


    }

}
