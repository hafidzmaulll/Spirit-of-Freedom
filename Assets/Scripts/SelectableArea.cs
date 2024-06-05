using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectableArea : MonoBehaviour
{
    public GameObject selectionCanvas; // Reference to the selection canvas

    private void Start()
    {
        // Ensure the canvas is hidden at the start
        if (selectionCanvas != null)
        {
            selectionCanvas.SetActive(false);
        }
        else
        {
            Debug.LogError("Selection Canvas is not assigned in the Inspector");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger");
            // Show the selection canvas when player enters trigger
            if (selectionCanvas != null)
            {
                selectionCanvas.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited trigger");
            // Hide the selection canvas when player exits trigger
            if (selectionCanvas != null)
            {
                selectionCanvas.SetActive(false);
            }
        }
    }

    // Methods to load scenes based on button clicks
    public void SelectArea1()
    {
        Debug.Log("Loading Scene 3");
        SceneManager.LoadScene(3); // Replace with the correct build index or scene name
    }

    public void SelectArea2()
    {
        Debug.Log("Loading Scene 4");
        SceneManager.LoadScene(4); // Replace with the correct build index or scene name
    }
}
