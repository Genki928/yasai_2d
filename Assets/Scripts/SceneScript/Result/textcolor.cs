using UnityEngine;
using UnityEngine.UI;

public class TextColorChanger : MonoBehaviour
{
    [SerializeField] private Color redColor = Color.red;
    [SerializeField] private Color blueColor = Color.blue;

    private Text text;

    void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        if (Winner.w_id == 0)
        {
            text.color = redColor;
        }
        else
        {
            text.color = blueColor;
        }
    }
}
