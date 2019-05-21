using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using LexicomixNamespace;

public class SlotController : MonoBehaviour, IDropHandler, IPointerDownHandler
{
    private SyllableElement syllable;
    private GameManager gM;

    private void Start()
    {
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public GameObject Item
    {
        get
        {
            if(transform.childCount > 0)
            {
                return transform.GetChild(0).gameObject;

            }
            return null;

        }


    }


    public void SetSyllableElementSlot(SyllableElement s)
    {
        syllable = s;
    }
    

    public void OnDrop(PointerEventData eventData)
    {
        if(!Item)
        {
            DragHandler.objectBeingDragged.transform.SetParent(transform);

        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        gM.OnSyllableClickPlay(syllable);
    }


}
