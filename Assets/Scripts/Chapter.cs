using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Chapter : MonoBehaviour
{
    public TextAsset jsonFile;
    private LinesJSON linesJson;
    private TextInputHandler textInputHandler;
    private int currentLineTypeID=0;
    private int currentSentenceID=0;
    private float waitBetweenLetters;
    private float waitBetweenSentences;
    private float waitDramaticRythm;
    private bool waitForInput;
    private bool waitForTextInput;
    private string currentTextId;

    private LineType currentLineType;
    private string currentSentence;

    [SerializeField] TMP_Text textBox;
    [SerializeField] TMP_FontAsset font;
    [SerializeField] TextInputHandler chapterInputHandler;

    [SerializeField] AudioClip clip;

    [SerializeField] float initialPitch=1;
    [SerializeField] float minPitchVariation=0.05f;
    [SerializeField] float maxPitchVariation=0.05f;

    bool isReadingBalise = false;
    bool isWaitingInput = false;

    private void Start()
    {
        linesJson = JsonUtility.FromJson<LinesJSON>(jsonFile.text);
        textBox.font = font;
    }

    private void Update()
    {
        if(isWaitingInput)
        {
            if(Input.GetAxisRaw("Submit")!=0)
            {
                isWaitingInput = false;
                GameManager.Instance.DisableArrow();
                StartCoroutine(NextSentence());
            }
        }
    }

    IEnumerator NextSentence()
    {
        GameManager.Instance.FadeOutText(textBox);
        yield return new WaitForSeconds(1.5f);
        textBox.text = "";
        yield return new WaitForSeconds(0.5f);
        //Color c =textBox.color;
        //c.a = 1;
        //textBox.color = c;
        currentSentenceID++;
        ReadSentence();
    }

    public void StartChapter()
    {
        GameManager.Instance.bipSource.clip = clip;
        ReadLineType();
    }

    void ReadLineType()
    {
        if(currentLineTypeID < linesJson.lineType.Length)
        {
            currentSentenceID = 0;
            currentLineType = linesJson.lineType[currentLineTypeID];
            currentTextId = currentLineType.id;
            textBox.fontSize = currentLineType.size;
            waitBetweenLetters = currentLineType.waitLetter;
            waitBetweenSentences = currentLineType.waitSentence;
            waitDramaticRythm = currentLineType.waitDramatic;
            waitForInput = currentLineType.waitForInput;
            waitForTextInput = currentLineType.waitForTextInput;
            ReadSentence();
        }
        else
        {
            EndChapter();
        }
    }

    void EndChapter()
    {
        print("the end");
        GameManager.Instance.FadeOut();
    }

    void ReadSentence()
    {
        if(currentSentenceID<currentLineType.sentence.Length)
        {
            currentSentence = currentLineType.sentence[currentSentenceID];
            StopAllCoroutines();
            StartCoroutine(TypeSentence(currentSentence));
        }
        else
        {
            if(!currentLineType.changesLine)
            {
                currentLineTypeID++;
                ReadLineType();
            }
            else
            {
                GoToLineId(currentLineType.idNextLine);
            }
        }
    }

    IEnumerator TypeSentence(string sentance)
    {
        textBox.text = "";
        foreach (char letter in sentance.ToCharArray())
        {
            if (letter == '*')
                yield return new WaitForSeconds(waitDramaticRythm);
            else if(letter =='<')
            {
                isReadingBalise = true;
                textBox.text += letter;
            }
            else if(letter =='>')
            {
                isReadingBalise = false;
                textBox.text += letter;
            }
            else
            {
                textBox.text += letter;
                if(!isReadingBalise)
                {
                    GameManager.Instance.bipSource.pitch = Random.Range(initialPitch - minPitchVariation, initialPitch + maxPitchVariation);
                    GameManager.Instance.bipSource.Play();
                }
            }
                
            if (letter != ' ' && !isReadingBalise)
                yield return new WaitForSeconds(waitBetweenLetters);
            GameManager.Instance.bipSource.Stop();
        }
        if (waitForInput)
        {
            GameManager.Instance.ArrowBlink();
            isWaitingInput = true;
        }
        else if(waitForTextInput)
        {
            chapterInputHandler.StartWaitingInput(currentTextId);
        }
        else
        {
            yield return new WaitForSeconds(waitBetweenSentences);
            NextSentence();
        }
    }

    public void GoToLineId(string id)
    {
        int i = 0;
        foreach(LineType line in linesJson.lineType)
        {
            if (line.id.Equals(id))
            {
                currentLineTypeID = i;
                currentSentenceID = 0;
                ReadSentence();
                break;
            }
            else
                i++;
        }
    }

    public void NextLine()
    {
        currentSentenceID++;
        ReadSentence();
    }
}
