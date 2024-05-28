using UnityEngine;
using TMPro;
using System.Collections;

public class TypingEffect : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float typingSpeed = 0.1f;
    public float pauseBetweenSections = 1.0f;
    public float initialDelay = 1.0f;
    public string[] sections;
    public AudioClip typingSound;
    public GameObject letsDoItButton; // Reference to the LetsDoItButton gameObject

    private int currentSectionIndex = 0;
    private string currentText = "";
    private Coroutine typingCoroutine;
    private AudioSource[] audioSources;

    void Start()
    {
        audioSources = new AudioSource[sections.Length];
        for (int i = 0; i < sections.Length; i++)
        {
            GameObject audioSourceGO = new GameObject("TypingAudioSource_" + i);
            audioSourceGO.transform.SetParent(transform);
            audioSources[i] = audioSourceGO.AddComponent<AudioSource>();
            audioSources[i].volume = 0.2f; // Set volume to 20%
        }

        StartCoroutine(StartTypingWithDelay());
    }

    IEnumerator StartTypingWithDelay()
    {
        yield return new WaitForSeconds(initialDelay);
        StartTyping();
    }

    void StartTyping()
    {
        currentSectionIndex = 0;
        currentText = "";
        textComponent.text = "";
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText(sections[currentSectionIndex]));
    }

    IEnumerator TypeText(string text)
    {
        AudioSource currentAudioSource = audioSources[currentSectionIndex];
        for (int i = 0; i < text.Length; i++)
        {
            currentText += text[i];
            textComponent.text = currentText;
            if (typingSound != null && currentAudioSource != null)
            {
                currentAudioSource.PlayOneShot(typingSound);
            }
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(pauseBetweenSections);

        currentText = "";
        textComponent.text = currentText;

        currentSectionIndex++;
        if (currentSectionIndex < sections.Length)
        {
            typingCoroutine = StartCoroutine(TypeText(sections[currentSectionIndex]));
        }
        else
        {
            // All text displayed, enable the LetsDoItButton
            letsDoItButton.SetActive(true);
        }
    }
}