using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class RainbowText : MonoBehaviour
{
    public float speed = 1.0f;

    private Text text;

    void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        float h = Mathf.Repeat(Time.time * speed, 1f);
        text.color = Color.HSVToRGB(h, 1f, 1f);
    }
}