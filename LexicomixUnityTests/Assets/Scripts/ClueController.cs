using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClueController : MonoBehaviour
{
    private TextMeshProUGUI text;


    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void SetClues(string clue, float delay)
    {

        StartCoroutine(ShowClue(clue, delay));
    }

    IEnumerator ShowClue(string clue, float delay)
    {
        yield return new WaitForSeconds(delay);

        text.text = clue.ToUpper();

    }

}
