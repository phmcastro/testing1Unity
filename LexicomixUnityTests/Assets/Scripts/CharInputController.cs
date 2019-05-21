using UnityEngine;

public class CharInputController : MonoBehaviour
{
    // The field for this char and the associated zero-based index in the world
    private LXInputField thisInputField;
    [HideInInspector]
    public int myIndex { get; set; }
    [HideInInspector]
    public SyllableSlotController mySyllableParent { get; set; }

    private void Start()
    {
        // Subscribe to key pressed event of the LX Input field
        thisInputField = GetComponentInChildren<LXInputField>();
        thisInputField.OnLXKeyPressed.AddListener(OnKeyPressed);
    }

    private void OnKeyPressed(CharPack pack)
    {
        // Enrich the key pressed details with the position of this char in the word
        pack.charIndex = myIndex;

        // A key pressed event occured, forward it to the parent
        Debug.Log("Key pressed : " + pack);
        mySyllableParent.BuildingSyllable2(pack);
    }
}
