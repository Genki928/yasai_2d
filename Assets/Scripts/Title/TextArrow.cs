using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        if (arrow_pos == 0)
        {
            transform.position = new(texts[0].transform.position.x-4.0f, texts[0].transform.position.y+0.5f);
        }
        else if (arrow_pos == 1)
        {
            transform.position = new(texts[1].transform.position.x-4.0f, texts[1].transform.position.y+0.5f);
        }

    }
}
