using System;
using System.Collections.Generic;
using UnityEngine;
using LexicomixNamespace;
using UnityEngine.UI;
using UnityEditor;

public class InputZoneController : MonoBehaviour
{
    public GameObject syllableSlot;
    public GameObject charSlot;
    private float canvasWidth;
    private List<GameObject> syllablesSlots = new List<GameObject>();
    private List<GameObject> charSlots = new List<GameObject>();
    private List<LXInputField> charInputs = new List<LXInputField>();
    private List<SyllableElement> currentSyllables = new List<SyllableElement>();
    private string currentWord;

    private GameManager gM;
    public static float inicialScreenWidth;
    public static float inicialScreenHeight;
    private Resolution changedResolution;


    private void Start()
    {
        canvasWidth = GetComponent<RectTransform>().rect.size.x;
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();

        inicialScreenWidth = Screen.width;
        inicialScreenHeight = Screen.height;

        changedResolution = Screen.currentResolution;
    }

    private void Update()
    {
        if(Screen.currentResolution.width != changedResolution.width || Screen.currentResolution.height != changedResolution.height)
        {
            Debug.Log("Resolution changed to: " + Screen.currentResolution.width + " x " + Screen.currentResolution.height);

            ResizeElements();

            changedResolution = Screen.currentResolution;
        }
    }

    public void SelectNextCharInput(int charChanged)
    {
        if (charChanged + 1 >= charInputs.Count)
            return;

        charInputs[charChanged + 1].Select();
        charInputs[charChanged + 1].ActivateInputField();

    }

    public void SelectPreviousCharInput(int charChanged)
    {
        if (charChanged - 1 < 0)
            return;
            
        charInputs[charChanged - 1].Select();
        charInputs[charChanged - 1].ActivateInputField();

    }

    public void BuildLayout(List<SyllableElement> syllables, string word)
    {
        currentSyllables = syllables;
        currentWord = word;

        float scaleX = Screen.width / inicialScreenWidth;
        float scaleY = Screen.height / inicialScreenHeight;

        Debug.Log("Nb syllables : " + syllables.Count);
        Debug.Log("Word : " + word);

        Debug.Log("Screen width: " + Screen.width);
        Debug.Log("Screen height: " + Screen.height);

        float charWidth = charSlot.GetComponent<RectTransform>().sizeDelta.x * scaleX;
        float charHeight = charSlot.GetComponent<RectTransform>().sizeDelta.y * scaleY;
        float charSpacing = 5f * scaleX;

        float syllableHeight = charHeight + 10f * scaleY;
        //float syllableHeight = syllableSlot.GetComponent<RectTransform>().sizeDelta.y;
        float syllableWidth = 0;

        float wordWidth = word.Length * (charWidth + charSpacing);


        // Instanciate the missing syllables if any
        Debug.Log("SyllableObjs count: " + syllablesSlots.Count);
        int syllCount = syllables.Count - syllablesSlots.Count;
        for (int i = 0; i < syllCount; i++)
        {
            var newSyllObj = Instantiate(syllableSlot);
            newSyllObj.transform.SetParent(transform);

            syllablesSlots.Add(newSyllObj);

            Debug.Log("Creating syllable no " + i);
        }

        // Disable all syllables first
        foreach (GameObject obj in syllablesSlots)
        {
            obj.SetActive(false);
        }

        // Only show and set position of the necessary syllables
        float posX = (Screen.width - wordWidth) / 2;
        float posY = 55 * scaleY;
        Debug.Log("syllablesObjs contains " + syllablesSlots.Count);
        for (int i = 0; i < syllables.Count; i++)
        {
            syllableWidth = syllables[i].Text.Length * (charWidth + charSpacing);

            syllablesSlots[i].GetComponent<RectTransform>().sizeDelta = new Vector2(syllableWidth, syllableHeight);
            
            posX += syllableWidth / 2;
            syllablesSlots[i].transform.position = new Vector2(posX, posY);
            posX += syllableWidth / 2;
        }

        // Instanciate the missing chars if any
        Debug.Log("charSlots count: " + charSlots.Count);
        int charCount = word.Length - charSlots.Count;
        for (int i = 0; i < charCount; i++)
        {
            var newCharObj = Instantiate(charSlot);

            charSlots.Add(newCharObj);

            Debug.Log("Creating char no " + i);

        }

        // Disable all char first
        foreach (GameObject obj in charSlots)
        {
            obj.SetActive(false);
        }

        // Only show and set position of the necessary chars
        int j = 0;
        charInputs.Clear();
        LXInputField nextField;
        CharInputController charController;
        SyllableSlotController syllableController;
        for (int i = 0; i < syllables.Count; i++)
        {
            // Retrieve controller for that syllable and associate the 
            syllableController = syllablesSlots[i].GetComponent<SyllableSlotController>();
            syllableController.ReceiveSyllable(syllables[i]);

            // Associate all chars to this syllable
            char[] syllArray = syllables[i].Text.ToCharArray();
            posX = syllablesSlots[i].transform.position.x - syllables[i].Text.Length * (charWidth + charSpacing) / 2;
            foreach (char nextChar in syllArray)
            {
                // Retrieve controller for that char and show it
                charController = charSlots[j].GetComponent<CharInputController>();
                charController.myIndex = j;
                charController.mySyllableParent = syllableController;

                // Set parent/child relation
                charSlots[j].transform.SetParent(syllablesSlots[i].transform);

                // Initiate and store the text input
                nextField = charSlots[j].GetComponentInChildren<LXInputField>();
                nextField.text = string.Empty;
                charInputs.Add(nextField);
                syllableController.myCharInputs.Add(nextField);

                charSlots[j].GetComponent<RectTransform>().sizeDelta = new Vector2(charWidth, charHeight);

                posX += (charWidth + charSpacing) / 2;
                charSlots[j].transform.position = new Vector2(posX, posY);
                posX += (charWidth + charSpacing) / 2;

                // Show char slot
                charSlots[j].SetActive(true);

                j++;
            }

            // Show syllable
            syllablesSlots[i].SetActive(true);
        }

        foreach(GameObject g in charSlots)
        {
            g.GetComponent<Image>().color = GetComponentInChildren<SyllableSlotController>().originalColor;
        }

        SelectNextCharInput(-1);

    }


    void ResizeElements()
    {
        float scaleX = Screen.width / inicialScreenWidth;
        float scaleY = Screen.height / inicialScreenHeight;

        float charWidth = charSlot.GetComponent<RectTransform>().sizeDelta.x * scaleX;
        float charHeight = charSlot.GetComponent<RectTransform>().sizeDelta.y * scaleY;
        float charSpacing = 5f * scaleX;

        float syllableHeight = charHeight + 10f * scaleY;
        //float syllableHeight = syllableSlot.GetComponent<RectTransform>().sizeDelta.y;
        float syllableWidth = 0;

        float wordWidth = currentWord.Length * (charWidth + charSpacing);



        float posX = (Screen.width - wordWidth) / 2;
        float posY = 55 * scaleY;
        for (int i = 0; i < currentSyllables.Count; i++)
        {
            syllableWidth = currentSyllables[i].Text.Length * (charWidth + charSpacing);

            syllablesSlots[i].GetComponent<RectTransform>().sizeDelta = new Vector2(syllableWidth, syllableHeight);

            posX += syllableWidth / 2;
            syllablesSlots[i].transform.position = new Vector2(posX, posY);
            posX += syllableWidth / 2;

            syllablesSlots[i].GetComponent<SyllableSlotController>().ResizeHandle();

        }


        int j = 0;
        for (int i = 0; i < currentSyllables.Count; i++)
        {
            // Associate all chars to this syllable
            char[] syllArray = currentSyllables[i].Text.ToCharArray();
            posX = syllablesSlots[i].transform.position.x - currentSyllables[i].Text.Length * (charWidth + charSpacing) / 2;
            foreach (char nextChar in syllArray)
            {
                charSlots[j].GetComponent<RectTransform>().sizeDelta = new Vector2(charWidth, charHeight);

                posX += (charWidth + charSpacing) / 2;
                charSlots[j].transform.position = new Vector2(posX, posY);
                posX += (charWidth + charSpacing) / 2;

                // Show char slot
                charSlots[j].SetActive(true);

                j++;


            }
        }
    }


    public void SendWholeInputText()
    {
        string wholeText = "";

        foreach(LXInputField input in charInputs)
        {
            wholeText += input.text;
        }

        Debug.Log(wholeText);

        gM.OnCheckInput(wholeText);
    }



    public void OnSyllableHit(SyllableElement hitSyllable)
    {
        gM.OnSyllableClickPlay(hitSyllable);
    }

}