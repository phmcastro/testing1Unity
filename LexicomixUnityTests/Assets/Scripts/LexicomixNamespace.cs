using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace LexicomixNamespace
{
    [Serializable]
    public class SyllableElement
    {
        public string WordID;
        public int Stress;
        public int Position;
        public string Text;
        public string AudioURL;
    }
    [Serializable]
    public class TextClue
    {
        public int Level;
        public string Title;
        public string Text;
        public List<int> ChallengePosition;
    }
    [Serializable]
    public class MediaElement
    {
        public string WordID;
        public string PictureURL;
        public string AudioURL;
        public string Text;
        public SyllableElement FirstSyllable;
        public List<SyllableElement> Syllables;
        public List<SyllableElement> PhonoSyllables;
        public List<TextClue> TextClues;
    }

    [Serializable]
    public class LocalizationElement
    {
        public string Key;
        public string Text;
    }


    public enum PictureType
    {
        Unknown = 0,
        DrawingBlackAndWhite = 1,
        DrawingColor = 2,
        PhotoBlackAndWhite = 3,
        PhotoColor = 4,
        ClipartBlackAndWhite = 5,
        ClipartColor = 6
    }


    [Serializable]
    public class Picture
    {
        public int ID;
        public PictureType PictureTypeID;
        public int ParentID;
        public string OwnerID;
        public string URL;
    }

    [Serializable]
    public class Audio
    {
        public int ID;
        public int TranscriptionID;
        public int VoiceID;
        public string URL;
        public string AudioType;
    }

    public enum GameTypeEnum
    {
        Unknown = 0,
        Diaporama = 1,
        Memory = 2
    }

    [Serializable]
    public class UnityConfig
    {
        public string SiteURLBase;  // For instance "https://www.lexicomix.com/" or "https://localhost:44302/"
        public string BlobURLBase;  // For instance "https://dicoadmin20181018034609.blob.core.windows.net:443/"
        public string BlobPicture;
        public string BlobAudio;
        public string GetMediaService;
        public string GetGameConfigService;
        public string SetGameStatService;
        public GameTypeEnum GameType;
        public int Professional;
        public int User;
        public int GameID;
    }



    public static class AnchoringUI
    {
        //public static void AnchorsToCorners(List<Transform> elementsAnchors)
        //{
        //    foreach (Transform transform in elementsAnchors)
        //    {
        //        RectTransform t = transform as RectTransform;
        //        RectTransform pt = transform.parent as RectTransform;

        //        if (t == null || pt == null) return;

        //        Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
        //                                            t.anchorMin.y + t.offsetMin.y / pt.rect.height);
        //        Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
        //                                            t.anchorMax.y + t.offsetMax.y / pt.rect.height);

        //        t.anchorMin = newAnchorsMin;
        //        t.anchorMax = newAnchorsMax;
        //        t.offsetMin = t.offsetMax = new Vector2(0, 0);
        //    }
        //}

        //public static void AnchorsToCorners(Transform element)
        //{

        //    RectTransform t = element as RectTransform;
        //    RectTransform pt = element.parent as RectTransform;

        //    if (t == null || pt == null) return;

        //    //t.anchorMin = Vector2.zero;
        //    //t.anchorMax = Vector2.zero;
        //    //t.offsetMax = Vector2.zero;
        //    //t.offsetMin = Vector2.zero;

        //    Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
        //                                        t.anchorMin.y + t.offsetMin.y / pt.rect.height);
        //    Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
        //                                        t.anchorMax.y + t.offsetMax.y / pt.rect.height);

        //    t.anchorMin = newAnchorsMin;
        //    t.anchorMax = newAnchorsMax;
        //    t.offsetMin = t.offsetMax = new Vector2(0, 0);

        //    Debug.Log("Anchors set in element:  " + element.name);
        //}


    }



    public static class CanvasExtensions
    {
        public static Vector2 SizeToParent(this RawImage image, float padding = 0)
        {
            var parent = image.transform.parent.GetComponentInParent<RectTransform>();
            var imageTransform = image.GetComponent<RectTransform>();
            if (!parent) { return imageTransform.sizeDelta; } //if we don't have a parent, just return our current width;
            padding = 1 - padding;
            float w = 0, h = 0;
            float ratio = image.texture.width / (float)image.texture.height;
            var bounds = new Rect(0, 0, parent.rect.width, parent.rect.height);
            if (Mathf.RoundToInt(imageTransform.eulerAngles.z) % 180 == 90)
            {
                //Invert the bounds if the image is rotated
                bounds.size = new Vector2(bounds.height, bounds.width);
            }
            //Size by height first
            h = bounds.height * padding;
            w = h * ratio;
            if (w > bounds.width * padding)
            { //If it doesn't fit, fallback to width;
                w = bounds.width * padding;
                h = w / ratio;
            }
            imageTransform.sizeDelta = new Vector2(w, h);
            return imageTransform.sizeDelta;
        }



        public static Vector2 SizeToParent(this Image image, float padding = 0)
        {
            var parent = image.transform.parent.GetComponentInParent<RectTransform>();
            var imageTransform = image.GetComponent<RectTransform>();
            if (!parent) { return imageTransform.sizeDelta; } //if we don't have a parent, just return our current width;
            padding = 1 - padding;
            float w = 0, h = 0;
            float ratio = image.mainTexture.width / (float)image.mainTexture.height;
            var bounds = new Rect(0, 0, parent.rect.width, parent.rect.height);
            if (Mathf.RoundToInt(imageTransform.eulerAngles.z) % 180 == 90)
            {
                //Invert the bounds if the image is rotated
                bounds.size = new Vector2(bounds.height, bounds.width);
            }
            //Size by height first
            h = bounds.height * padding;
            w = h * ratio;
            if (w > bounds.width * padding)
            { //If it doesn't fit, fallback to width;
                w = bounds.width * padding;
                h = w / ratio;
            }
            imageTransform.sizeDelta = new Vector2(w, h);
            return imageTransform.sizeDelta;
        }


    }


    public static class StringTreatment
    {
        public static string RemoveDiacritics(string text)
        {

            var normalizedString = text.Normalize(NormalizationForm.FormD);

            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);

                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }

            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);

        }


    }


    public static class JsonHelper
    {
        public static string fixJson(string value)
        {
            value = "{\"Items\":" + value + "}";
            return value;
        }
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }
        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }
        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
    class AcceptAllCertificates : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            // Good dog
            return true;
        }
    }




}


