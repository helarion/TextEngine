using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextInputHandler : MonoBehaviour
{
    public TextAsset jsonFile;
    private TextInputJSON textInputJson;
    private string currentInputID;
    private TextInput currentTextInput;

    [SerializeField] TMP_InputField inputField;

    [SerializeField] AudioClip clip;

    [SerializeField] float initialPitch = 1;
    [SerializeField] float minPitchVariation = 0.05f;
    [SerializeField] float maxPitchVariation = 0.05f;

    bool isWaitingTextInput = false;
    bool hasFound = false;
    bool hasChangedClip = false;

    int previousLength=0;

    private void Start()
    {
        textInputJson = JsonUtility.FromJson<TextInputJSON>(jsonFile.text);
    }

    private void Update()
    {
        if(isWaitingTextInput)
        {
            if(!hasChangedClip)
            {
                hasChangedClip = true;
                GameManager.Instance.bipSource.clip = clip;
            }
            if(inputField.text.Length>previousLength)
            {
                GameManager.Instance.bipSource.pitch = initialPitch + Random.Range(-minPitchVariation, maxPitchVariation);
                GameManager.Instance.bipSource.Play();
            }
            previousLength = inputField.text.Length;

            if (Input.GetButtonDown("Submit") && inputField.text.Length>0)
            {
                isWaitingTextInput = false;
                GameManager.Instance.PlaySubmitSound();
                ProcessAnswer();
            }
        }
    }

    public void StartWaitingInput(string id)
    {
        GameManager.Instance.EnableInputText();
        currentInputID = id;
        isWaitingTextInput = true;
    }

    public void ProcessAnswer()
    {
        string answer = inputField.text;
        isWaitingTextInput = false;
        inputField.text = "";
        GameManager.Instance.DisableInputText();

        if (answer.Length > GameManager.Instance.currentChapter.maxInputLength)
        {
            GameManager.Instance.ChapterGoTo("TooLong");
        }
        else
        {
            List<int> idList = LookForID(currentInputID);
            foreach (int currentID in idList)
            {
                if (textInputJson.textInput[currentID].needsRecord)
                {
                    RecordAnswer(textInputJson.textInput[currentID].recordVariable, answer);
                    inputField.text = "";
                    GameManager.Instance.DisableInputText();
                    GameManager.Instance.ResumeChapter();
                    hasFound = true;
                }
                else
                {
                    Answer an = textInputJson.textInput[currentID].answers;
                    foreach (string possibleAnswer in an.acceptableWords)
                    {
                        print("Input:" + answer + " possible answer:" + possibleAnswer);
                        if (answer.Equals(possibleAnswer))
                        {
                            print("match found");
                            GameManager.Instance.ChapterGoTo(an.leadsTo);
                            hasFound = true;
                            break;
                        }
                    }
                }
                if (hasFound)
                {
                    break;
                }
            }
            if (!hasFound)
            {
                GameManager.Instance.ChapterGoTo("Error");
            }
            hasFound = false;
        }
    }

    private void RecordAnswer(string variable, string answer)
    {
        switch(variable)
        {
            case "name":
                GameManager.Instance.name = answer;
                break;
            default:

                break;
        }
    }

    private List<int> LookForID(string id)
    {
        int i = 0;
        List<int> list = new List<int>();
        foreach(TextInput ti in textInputJson.textInput)
        {
            //print("Input:" + ti.id + " id:" + id);
            if(ti.id.Equals(id))
            {
                list.Add(i);
                //print("match found");
            }
            i++;
        }
        print("taille de la liste:" + list.Count);
        return list;
    }
}
