using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TextInputJSON
{
    public TextInput[] textInput;
}

[Serializable]
public class TextInput
{
    public string id;
    public bool needsRecord;
    public string recordVariable;
    public Answer answers;
}

[Serializable]
public class Answer
{
    public string[] acceptableWords;
    public string leadsTo;
}

