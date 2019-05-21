using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

[Serializable]
public class CharPack
{
    public string charString;   // The content of the char (empty when backspace)
    public int charIndex;       // The 0-based index of the char
    public bool backspace;      // True when the backspace key was pressed
    public bool rightArrow;     // True when the right arrow key was pressed
    public bool leftArrow;      // True when the left arrow key was pressed
    public bool enter;          // True when the enter key was pressed
    public bool spaceBar;       // True when the space bar key was pressed
    public bool hyphen;         // True when the hyphen key(or minus key) was pressed

    // Override of toString for a nicer logging
    public override string ToString()
    {
        // Build log according to event
        string log = string.Empty;
        if (leftArrow)
            log = "'Left arrow'";
        else if (rightArrow)
            log = "'Right arrow'";
        else if (backspace)
            log = "'Backspace'";
        else log = string.Format("'{0}'", charString);

        // First comment

        // Return log string
        return string.Format("{0} pressed at {1}", log, charIndex);
    }
}

public class LXInputField : InputField
{
    // The custom event with some details on the key pressed
    [Serializable]
    public class OnLXKeyPressedEvent : UnityEvent<CharPack> { };

    // The vent currently being processd
    private Event m_LXProcessingEvent = new Event();

    // The accent store
    private KeyCode m_Accent = KeyCode.None;

    /// <summary>
    /// Event delegates triggered when any key is pressed
    /// </summary>
    [FormerlySerializedAs("onLXValueChange")]
    [SerializeField]
    private OnLXKeyPressedEvent m_LXKeyPressed = new OnLXKeyPressedEvent();
    public OnLXKeyPressedEvent OnLXKeyPressed { get { return m_LXKeyPressed; } set { m_LXKeyPressed = value; } }

    // Rewrite the base OnUpdateSelected to be able to generate the custom KeyPressed event
    public override void OnUpdateSelected(BaseEventData eventData)
    {
        if (!isFocused)
            return;

        while (Event.PopEvent(m_LXProcessingEvent))
        {
            if (m_LXProcessingEvent.rawType == EventType.KeyDown && IsRelevantKey(m_LXProcessingEvent.keyCode))
            {
                // The following happened:
                // - Backspace was pressed or
                // - Left or right arrows were pressed or
                // - A char key was pressed
                m_LXKeyPressed.Invoke(new CharPack()
                {
                    charIndex = 0,
                    charString = text,
                    backspace = m_LXProcessingEvent.keyCode == KeyCode.Backspace,
                    leftArrow = m_LXProcessingEvent.keyCode == KeyCode.LeftArrow,
                    rightArrow = m_LXProcessingEvent.keyCode == KeyCode.RightArrow,
                    enter = m_LXProcessingEvent.keyCode == KeyCode.Return,
                    spaceBar = m_LXProcessingEvent.keyCode == KeyCode.Space,
                    hyphen = m_LXProcessingEvent.keyCode == KeyCode.Minus,
                });

            }
            // else if(m_LXProcessingEvent.rawType == EventType.KeyDown && IsAccent(m_LXProcessingEvent.keyCode))
            //{
                // Store the accent
                //m_Accent = m_LXProcessingEvent.keyCode;
            //}
        }

        SelectAll();

        // Event properly processed
        eventData.Use();
    }

    // Check if the key pressed is an accent
    private bool IsAccent(KeyCode code)
    {
        // Process only interesting keys
        switch (code)
        {
            case KeyCode.Tilde:
            case KeyCode.Caret:
            case KeyCode.BackQuote:
            //case KeyCode.Trema:
            case KeyCode.Quote: return true;
            default: return false;
        }
    }

    // Check if the key pressed is relevant
    private bool IsRelevantKey(KeyCode code)
    {
        // Process only interesting keys
        switch (code)
        {
            case KeyCode.Tilde: text = "~"; m_Accent = KeyCode.Tilde; SelectAll(); return false;
            case KeyCode.Caret: text = "^"; m_Accent = KeyCode.Caret; SelectAll(); return false;
            case KeyCode.BackQuote: text = "`"; m_Accent = KeyCode.BackQuote; SelectAll(); return false;
            //case KeyCode.Trema: text = "`"; m_Accent = KeyCode.Tilde; SelectAll(); return false;
            case KeyCode.Quote: text = "'"; m_Accent = KeyCode.Quote; SelectAll(); return false;

            case KeyCode.A:
                {
                    switch (m_Accent)
                    {
                        case KeyCode.Tilde: text = "Ã"; SelectAll(); return true;
                        case KeyCode.Caret: text = "Â"; SelectAll(); return true;
                        case KeyCode.BackQuote: text = "À"; SelectAll(); return true;
                        case KeyCode.Quote: text = "Á"; SelectAll(); return true;
                        //case KeyCode.Trema: text = "Ä"; SelectAll(); return true;
                        default: text = "A"; SelectAll(); return true;
                    }
                 }
            case KeyCode.B: text = "B"; SelectAll(); return true;
            case KeyCode.C:
                {
                    switch (m_Accent)
                    {
                        //case KeyCode.Cedilla: text = "Ç"; SelectAll(); return true;
                        default: text = "C"; SelectAll(); return true;
                    }
                }
            case KeyCode.D: text = "D"; SelectAll(); return true;
            case KeyCode.E:
                {
                    switch (m_Accent)
                    {
                        case KeyCode.Caret: text = "Ê"; SelectAll(); return true;
                        case KeyCode.BackQuote: text = "È"; SelectAll(); return true;
                        case KeyCode.Quote: text = "É"; SelectAll(); return true;
                        //case KeyCode.Trema: text = "Ë"; SelectAll(); return true;
                        default: text = "E"; SelectAll(); return true;
                    }
                }
            case KeyCode.F: text = "F"; SelectAll(); return true;
            case KeyCode.G: text = "G"; SelectAll(); return true;
            case KeyCode.H: text = "H"; SelectAll(); return true;
            case KeyCode.I:
                {
                    switch (m_Accent)
                    {
                        case KeyCode.Caret: text = "Î"; SelectAll(); return true;
                        case KeyCode.BackQuote: text = "Ì"; SelectAll(); return true;
                        case KeyCode.Quote: text = "Í"; SelectAll(); return true;
                        //case KeyCode.Trema: text = "Ï"; SelectAll(); return true;
                        default: text = "I"; SelectAll(); return true;
                    }
                }
            case KeyCode.J: text = "J"; SelectAll(); return true;
            case KeyCode.K: text = "K"; SelectAll(); return true;
            case KeyCode.L: text = "L"; SelectAll(); return true;
            case KeyCode.M: text = "M"; SelectAll(); return true;
            case KeyCode.N:
                {
                    switch (m_Accent)
                    {
                        case KeyCode.Tilde: text = "Ñ"; SelectAll(); return true;
                        default: text = "N"; SelectAll(); return true;
                    }
                }
            case KeyCode.O:
                {
                    switch (m_Accent)
                    {
                        case KeyCode.Tilde: text = "Õ"; SelectAll(); return true;
                        case KeyCode.Caret: text = "Ô"; SelectAll(); return true;
                        case KeyCode.BackQuote: text = "Ò"; SelectAll(); return true;
                        case KeyCode.Quote: text = "Ó"; SelectAll(); return true;
                        //case KeyCode.Trema: text = "Ö"; SelectAll(); return true;
                        default: text = "O"; SelectAll(); return true;
                    }
                }
            case KeyCode.P: text = "P"; SelectAll(); return true;
            case KeyCode.Q: text = "Q"; SelectAll(); return true;
            case KeyCode.R: text = "R"; SelectAll(); return true;
            case KeyCode.S: text = "S"; SelectAll(); return true;
            case KeyCode.T: text = "T"; SelectAll(); return true;
            case KeyCode.U:
                {
                    switch (m_Accent)
                    {
                        case KeyCode.Caret: text = "Û"; SelectAll(); return true;
                        case KeyCode.BackQuote: text = "Ù"; SelectAll(); return true;
                        case KeyCode.Quote: text = "Ú"; SelectAll(); return true;
                        //case KeyCode.Trema: text = "Ü"; SelectAll(); return true;
                        default: text = "U"; SelectAll(); return true;
                    }
                }
            case KeyCode.V: text = "V"; SelectAll(); return true;
            case KeyCode.W: text = "W"; SelectAll(); return true;
            case KeyCode.X: text = "X"; SelectAll(); return true;
            case KeyCode.Y: text = "Y"; SelectAll(); return true;
            case KeyCode.Z: text = "Z"; SelectAll(); return true;
            case KeyCode.Backspace: text = ""; SelectAll(); return true;
            case KeyCode.Space: text = " "; SelectAll(); return true;
            case KeyCode.Minus: text = "-"; SelectAll(); return true;
            case KeyCode.Return: SelectAll(); return true;
            case KeyCode.LeftArrow: SelectAll(); return true;
            case KeyCode.RightArrow: SelectAll(); return true;
            default: return false;
        }
    }
}
