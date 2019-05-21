using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LexicomixNamespace;

public class WordSlotManager : MonoBehaviour
{
    private List<GameObject> mySlots;


    void Start()
    {
        mySlots = new List<GameObject>();

        foreach(Transform child in transform)
        {
            mySlots.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }

    }

    public void GetSyllablesElements(List<SyllableElement> syllableElements)
    {
        foreach(GameObject obj in mySlots)
        {
            obj.SetActive(false);
        }

        int i = 0;
        foreach(SyllableElement s in syllableElements)
        {
            mySlots[i].SetActive(true);
            mySlots[i].GetComponentInChildren<TextMeshProUGUI>().text = s.Text.ToUpper();
            mySlots[i].GetComponent<SlotController>().SetSyllableElementSlot(s);

            i++;
        }

    }



}
