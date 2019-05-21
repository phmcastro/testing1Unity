using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using LexicomixNamespace;

public class SyllableSlotController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [HideInInspector]
    public Color originalColor;
    private Color changedColor;
    private Vector2 originalSize;
    private Image syllablePanelGraphic;
    private RectTransform rectTransform;
    private SyllableElement thisSyllable;
    private InputZoneController myInputZoneParent;
    public GameObject mySyllableHandle;
    private GameObject handle;
    [HideInInspector]
    public List<LXInputField> myCharInputs = new List<LXInputField>();

    private bool staySelected;
    private bool mouseInside;



    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        syllablePanelGraphic = GetComponent<Image>();
        originalColor = syllablePanelGraphic.color;
        changedColor = originalColor;

        handle = Instantiate(mySyllableHandle, transform);
        handle.transform.position = new Vector2(transform.position.x, transform.position.y - 35f);
        handle.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(rectTransform.sizeDelta.x, 15f);
        handle.transform.SetAsFirstSibling();
    }

    private void Start()
    {
        originalSize = rectTransform.sizeDelta;

        myInputZoneParent = GetComponentInParent<InputZoneController>();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && !mouseInside && changedColor != Color.green)
        {
            ResetAtributes();
            staySelected = false;
        }
    }

    public void HandleClicked()
    {
        if (mouseInside)
        {
            myInputZoneParent.OnSyllableHit(thisSyllable);
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (mouseInside)
        {
            myInputZoneParent.OnSyllableHit(thisSyllable);

            staySelected = !staySelected;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseInside = true;

        if (staySelected)
            return;

        syllablePanelGraphic.color = Color.cyan;

        rectTransform.SetAsLastSibling();

        //Debug.Log("Mouse Over!!");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseInside = false;

        if (staySelected)
            return;

        syllablePanelGraphic.color = changedColor;
        //rectTransform.sizeDelta = originalSize;
        //transform.localScale = Vector3.one;
        rectTransform.SetAsFirstSibling();

        //Debug.Log("Mouse Exit!!");
    }

    public void ResetAtributes()
    {
        syllablePanelGraphic.color = originalColor;
        changedColor = originalColor;
        transform.localScale = Vector3.one;
        rectTransform.SetAsFirstSibling();

        //Debug.Log("Atributes reset!!");

    }

    void SyllableHit(int charIndex)
    {
        Debug.Log("Syllable Hit!!!");

        syllablePanelGraphic.color = Color.green;
        changedColor = Color.green;

        myInputZoneParent.OnSyllableHit(thisSyllable);

        myInputZoneParent.SelectNextCharInput(charIndex);

    }


    public void ReceiveSyllable(SyllableElement syllable)
    {
        //foreach (LXInputField g in myCharInputs)
        //{
        //    g.transform.parent.GetComponent<Image>().color = originalColor;
        //}


        UnlockAllInputs();

        myCharInputs.Clear();

        thisSyllable = syllable;
        Debug.Log("Syllable received!!!");

        ResetAtributes();

        float scaleY = Screen.height / InputZoneController.inicialScreenHeight;

        handle.transform.position = new Vector2(transform.position.x, transform.position.y - 35f * scaleY);
        handle.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(rectTransform.sizeDelta.x, 15f * scaleY);
        handle.transform.SetAsFirstSibling();

    }

    public void ResizeHandle()
    {
        float scaleY = Screen.height / InputZoneController.inicialScreenHeight;

        handle.transform.position = new Vector2(transform.position.x, transform.position.y - 35f * scaleY);
        handle.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(rectTransform.sizeDelta.x, 15f * scaleY);
        handle.transform.SetAsFirstSibling();

    }

    void LockInput(int inputIndex)
    {

        myCharInputs[inputIndex].interactable = false;

    }

    void LockAllInputs()
    {
        foreach (LXInputField lx in myCharInputs)
        {
            lx.interactable = false;
        }
    }

    void UnlockAllInputs()
    {
        foreach (LXInputField lx in myCharInputs)
        {
            lx.interactable = true;
        }
    }

    public void BuildingSyllable(CharPack pack)
    {

        Debug.Log("Input Syllable:  " + thisSyllable.Text);
        Debug.Log("Pack: " + pack);

        bool hit = true;
        char[] syllArray = thisSyllable.Text.ToCharArray();
        for (int i = 0; i < syllArray.Length; i++)
        {
            // String treatments (Remove Diacritics locked by default, for now)
            string input = myCharInputs[i].text.ToLower().Trim();
            string reference = StringTreatment.RemoveDiacritics(syllArray[i].ToString().ToLower().Trim());

            Debug.Log(input + "=" + reference);

            if (!input.Equals(reference))
            {
                Debug.Log(myCharInputs[i].text + "=" + syllArray[i]);
                hit = false;
                break;
            }

            myCharInputs[i].transform.parent.GetComponent<Image>().color = Color.green;

        }

        if (pack.enter)
        {
            myInputZoneParent.SendWholeInputText();
            return;

        }


        if (hit)
        {
            SyllableHit(pack.charIndex);

            //LockInputs();

            return;
        }

        // Move forward/backward according to key pressed
        if (pack.backspace || pack.leftArrow)
        {
            if (pack.charIndex > 0)
                myInputZoneParent.SelectPreviousCharInput(pack.charIndex);
        }
        else
        {
            myInputZoneParent.SelectNextCharInput(pack.charIndex);
        }

    }


    public void BuildingSyllable2(CharPack pack)
    {

        Debug.Log("Full Syllable:  " + thisSyllable.Text);
        Debug.Log("Pack: " + pack);

        if (pack.enter)
        {
            myInputZoneParent.SendWholeInputText();
            return;
        }

        int hits = thisSyllable.Text.Length;
        char[] syllArray = thisSyllable.Text.ToCharArray();
        for (int i = 0; i < syllArray.Length; i++)
        {
            // String treatments (Remove Diacritics locked by default, for now)
            string input = myCharInputs[i].text.ToLower().Trim();
            string reference = StringTreatment.RemoveDiacritics(char.ToLower(syllArray[i]).ToString());

            Debug.Log(input + " should be equal to " + reference);

            if (!input.Equals(reference))
            {
                Debug.Log(myCharInputs[i].text + "!=" + syllArray[i]);

                if (input.Length == 0 && !thisSyllable.Text.Contains(input))
                    myCharInputs[i].transform.parent.GetComponent<Image>().color = Color.red;

                if (input.Length == 0 && thisSyllable.Text.Contains(input))
                    myCharInputs[i].transform.parent.GetComponent<Image>().color = Color.yellow;

            }
            else
            {
                hits--;
                myCharInputs[i].transform.parent.GetComponent<Image>().color = Color.green;
                //LockInput(i);
            }

            if (hits <= 0)
            {
                SyllableHit(pack.charIndex);
                return;
            }

        }

        foreach (LXInputField g in myCharInputs)
        {
            if (g.text.Length == 0)
                g.transform.parent.GetComponent<Image>().color = originalColor;
        }

        if (pack.backspace && pack.charString.Length != 0)
        {
            myCharInputs[pack.charIndex].text = string.Empty;
            return;
        }
                    

        // Move forward/backward according to key pressed
        if (pack.backspace || pack.leftArrow)
        {
            //myCharInputs[pack.charIndex].text = string.Empty;

            if (pack.charIndex > 0)
                myInputZoneParent.SelectPreviousCharInput(pack.charIndex);

        }
        else
        {
            myInputZoneParent.SelectNextCharInput(pack.charIndex);
        }

    }

}
