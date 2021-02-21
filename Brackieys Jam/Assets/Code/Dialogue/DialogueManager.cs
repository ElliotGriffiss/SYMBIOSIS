using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Animator animator;
    [SerializeField] private Image image;
    private string sceneName;
    private Queue<string> sentences;

    void Start()
    {
        sentences = new Queue<string>();
        Scene currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
    }

    public void StartDialogue (Dialogue dialogue)
    {
        Debug.Log("starting dialogue");

        sentences = new Queue<string>();

        foreach (string sentence in dialogue.sentances)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentenceCo(sentence));
    }

    IEnumerator TypeSentenceCo(string sentence)
    {
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.07f);
        }
    }

    void EndDialogue()
    {
        StartCoroutine(FadeCo(sceneName));
        Debug.Log("end");
    }
    IEnumerator FadeCo(string sceneName)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
        animator.SetBool("FadeIn", true);
        yield return new WaitUntil(() => image.color.a == 1);
        if(sceneName == "Cutscene 1")
        {
            SceneManager.LoadScene("Cutscene 2");
        }
        else if (sceneName == "Cutscene 2")
        {
            SceneManager.LoadScene("Test Scene");
        }
    }
}
