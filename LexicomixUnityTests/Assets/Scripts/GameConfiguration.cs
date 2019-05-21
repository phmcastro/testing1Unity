using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EImageMode
{
    Standard = 0,
    Exploded = 1
}

public class GameConfiguration
{
    public EImageMode ImageMode { get; set; }
    public float imageDelay;
    public float audioDelay;
    public float clueDelay;

    public void Load()
    {
        imageDelay = 0;
        audioDelay = 0;
        clueDelay = 0;
    }
}


public class DiaporamaConfig : GameConfiguration
{
    public bool onlyUppercaseTextInput;
    public bool accentCheck;
    public bool caseSensitive;


}
