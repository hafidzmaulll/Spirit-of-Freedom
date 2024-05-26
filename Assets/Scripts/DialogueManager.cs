using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Image actorImage;
    public TextMeshProUGUI actorName;
    public TextMeshProUGUI messageText;
    public RectTransform backgroundBox;
    public Button continueButton; // Reference to the Continue button

    Message[] currentMessages;
    actor[] currentActors;
    int activeMessage = 0;
    public static bool isActive = false;

    public void OpenDialogue(Message[] messages, actor[] actors)
    {
        currentMessages = messages;
        currentActors = actors;
        activeMessage = 0;
        isActive = true;

        backgroundBox.gameObject.SetActive(true); // Show the dialogue box
        continueButton.gameObject.SetActive(true); // Show the continue button

        Debug.Log("Started Conversation, Loading Messages: " + messages.Length);
        DisplayMessage();
    }

    void DisplayMessage()
    {
        if (activeMessage < currentMessages.Length)
        {
            Message messageToDisplay = currentMessages[activeMessage];
            messageText.text = messageToDisplay.message;

            actor actorToDisplay = currentActors[messageToDisplay.actorId];
            actorName.text = actorToDisplay.name;
            actorImage.sprite = actorToDisplay.sprite;
        }
        else
        {
            EndDialogue();
        }
    }

    public void OnContinueButtonClick()
    {
        activeMessage++;
        DisplayMessage();
    }

    void EndDialogue()
    {
        isActive = false;
        backgroundBox.gameObject.SetActive(false); // Hide the dialogue box
        continueButton.gameObject.SetActive(false); // Hide the continue button
    }

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the dialogue box and continue button are hidden at the start
        backgroundBox.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        continueButton.onClick.AddListener(OnContinueButtonClick); // Add listener for button click
    }

    // Update is called once per frame
    void Update()
    {

    }
}
