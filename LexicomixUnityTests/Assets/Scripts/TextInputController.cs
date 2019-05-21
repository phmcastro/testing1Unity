using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextInputController : MonoBehaviour
{
    [HideInInspector]
    public InputField myTextInput;

    void Start()
    {
        myTextInput = GetComponent <InputField>();

        myTextInput.onValueChanged.AddListener(delegate { GetUserTextInput(myTextInput.text); });
        myTextInput.onEndEdit.AddListener(delegate { GetUserEndTextInput(myTextInput.text); });
    }

    public string GetUserEndTextInput(string inputString)
    {
        return inputString;
    }

    public string GetUserTextInput(string inputString)
    {
        return inputString;
    }

    public void ClearTextInput()
    {
        if(string.IsNullOrEmpty(myTextInput.text))
        {
            return;
        }

        myTextInput.text = "";

    }

}
