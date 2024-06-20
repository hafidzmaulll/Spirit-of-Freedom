using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiddleCutscene : MonoBehaviour
{
    void OnEnable()
    {
        // Only specifying the sceneName or sceneBuildIndex will load the Scene with the single mode
        SceneManager.LoadScene("Stage2-Area1", LoadSceneMode.Single);
    }
}
