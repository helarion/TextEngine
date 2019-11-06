using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] TMP_Text textCenter;
    [SerializeField] Chapter chapitre;
    [SerializeField] Animator fadeAnimator;
    [SerializeField] Animator arrowAnimator;
    [SerializeField] TMP_InputField inputField;

    public AudioSource bipSource;
    [SerializeField] AudioClip submitSound;

    public Chapter currentChapter;

    public string name;

    private static GameManager instance = null;

    // Game Instance Singleton
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }


    void Start()
    {
        StartCoroutine(StartGame());
    }

    public void FadeIn()
    {
        fadeAnimator.SetTrigger("FadeIn");
    }

    public void FadeOut()
    {
        fadeAnimator.SetTrigger("FadeOut");
    }

    IEnumerator StartGame()
    {
        FadeIn();
        yield return new WaitForSeconds(2);
        chapitre.StartChapter();
    }

    public void ArrowBlink()
    {
        arrowAnimator.SetTrigger("Blink");
    }

    public void DisableArrow()
    {
        arrowAnimator.SetTrigger("Disable");
    }

    public void FadeOutText(TMP_Text textBox)
    {
        textBox.GetComponent<Animator>().SetTrigger("FadeOut");
    }

    public void EnableInputText()
    {
        inputField.gameObject.SetActive(true);

    }

    public void DisableInputText()
    {
        inputField.gameObject.SetActive(false);
    }

    public void ChapterGoTo(string id)
    {
        currentChapter.GoToLineId(id);
    }

    public void ResumeChapter()
    {
        currentChapter.NextLine();
    }

    public void PlaySubmitSound()
    {
        StartCoroutine(SubmitSound());
    }
    IEnumerator SubmitSound()
    {
        AudioClip save = bipSource.clip;
        bipSource.clip = submitSound;
        bipSource.Play();
        yield return new WaitForSeconds(0.5f);
        bipSource.clip = save;
    }
}
