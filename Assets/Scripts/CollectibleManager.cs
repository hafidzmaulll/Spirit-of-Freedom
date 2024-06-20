using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleManager : MonoBehaviour
{
    public int gemCount;
    public TextMeshProUGUI gemText;
    public GameObject Doors;
    private bool doorsDestroyed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gemText.text = "= " + gemCount.ToString();

        if(gemCount == 1 && !doorsDestroyed)
        {
            doorsDestroyed = true;
            Destroy(Doors);
        }
    }
}
