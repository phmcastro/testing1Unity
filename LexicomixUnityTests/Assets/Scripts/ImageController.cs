using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using LexicomixNamespace;


public class ImageController : MonoBehaviour

{
    private RawImage img;

    void Start()
    {
        img = GetComponent<RawImage>();
    }

    public void SetImage(Texture texture, float delay)
    {
        //img.texture = texture;

        // Update Image Texture
        StartCoroutine(ShowImage(texture, delay));
    }

    IEnumerator ShowImage(Texture texture, float delay)
    {
        yield return new WaitForSeconds(delay);


        img.texture = texture;
        img.SizeToParent();
    }
}
