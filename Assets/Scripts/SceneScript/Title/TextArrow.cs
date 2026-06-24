using System.Collections.Generic;
using UnityEngine;

public class TextArrow : MonoBehaviour
{
    public static TextArrow instance;

    [SerializeField] public int arrow_pos;
    [SerializeField] List<GameObject> texts = new List<GameObject>();
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(arrow_pos);
        if (arrow_pos == 0)
        {
            transform.position = new(texts[0].transform.position.x-4.2f, texts[0].transform.position.y + 0.2f);
        }
        else if (arrow_pos == 1)
        {
            transform.position = new(texts[1].transform.position.x-4.2f, texts[1].transform.position.y+0.2f);
        }

    }
}
