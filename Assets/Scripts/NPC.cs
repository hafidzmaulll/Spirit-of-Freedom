using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public DialogueTrigger trigger;

    private void Awake()
    {
        // Ensure the trigger is set to the DialogueTrigger component on this GameObject
        trigger = GetComponent<DialogueTrigger>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            trigger.StartDialogue();
        }
    }
}
