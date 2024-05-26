using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Message[] messages;
    public actor[] actors;

    private DialogueManager dialogueManager;

    private void Awake()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    public void StartDialogue()
    {
        if (dialogueManager != null)
        {
            dialogueManager.OpenDialogue(messages, actors);
        }
        else
        {
            Debug.LogError("DialogueManager not found in the scene.");
        }
    }
}

[System.Serializable]
public class Message
{
    public int actorId;
    public string message;
}

[System.Serializable]
public class actor
{
    public string name;
    public Sprite sprite;
}
