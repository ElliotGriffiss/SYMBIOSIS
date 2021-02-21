using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text dialogueText;
    [SerializeField] private Animator animator;
    [SerializeField] private Image image;
    private Queue<string> sentences;

    void Start()
    {
        sentences = new Queue<string>();
        
    }

    public void StartDialogue (Dialogue dialogue)
    {
        Debug.Log("starting dialogue");

        sentences.Clear();

        foreach(string sentence in dialogue.sentances)
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
        StartCoroutine(FadeCo());
        Debug.Log("end");
    }
    IEnumerator FadeCo()
    {
        animator.SetBool("FadeIn", true);
        yield return new WaitUntil(() => image.color.a == 1);
        SceneManager.LoadScene("Test Scene");
    }
}
