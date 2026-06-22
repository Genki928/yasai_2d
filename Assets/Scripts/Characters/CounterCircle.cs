using UnityEngine;

public class CounterCircle : MonoBehaviour
{

    public Leek owner;//ÇÀÇ¨ÇÃèÍèä
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject,1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (owner != null)
        {
            transform.position = owner.transform.position;
        }
    }
}
