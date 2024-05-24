using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectionZone : MonoBehaviour
{
    Collider2D col;
    public UnityEvent noCollidersRemain;
    public List<Collider2D> detectedColliders = new List<Collider2D>();

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        detectedColliders.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        detectedColliders.Remove(collision);

        if(detectedColliders.Count <= 0)
        {
            noCollidersRemain.Invoke();
        }
    }
}
