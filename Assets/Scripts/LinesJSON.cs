using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LinesJSON
{
    public LineType[] lineType;
}

[Serializable]
public class LineType
{
    public string id;
    public int size;
    public float waitLetter;
    public float waitSentence;
    public float waitDramatic;
    public bool waitForInput;
    public bool waitForTextInput;
    public int maxInputLength;
    public string[] sentence;
    public bool changesLine;
    public string idNextLine;
}
