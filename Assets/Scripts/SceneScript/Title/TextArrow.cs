using System.Collections.Generic;
using UnityEngine;

public class TextArrow : MonoBehaviour
{
    public static TextArrow instance;

    [SerializeField] public int arrow_pos;
    [SerializeField] List<GameObject> texts = new List<GameObject>();
    [SerializeField] float SPACE_X;
    [SerializeField] float SPACE_Y;

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
            transform.position = new(texts[0].transform.position.x + SPACE_X, texts[0].transform.position.y + SPACE_Y);
        }
        else if (arrow_pos == 1)
        {
            transform.position = new(texts[1].transform.position.x + SPACE_X, texts[1].transform.position.y + SPACE_Y);
        }

    }
}
