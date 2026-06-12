using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextBlink : MonoBehaviour
{
    Text text;

    [SerializeField] float interval = 0.5f;

    void Start()
    {
        text = GetComponent<Text>();

        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        while (true)
        {
            Color color = text.color;

            // ”ñ•\Ž¦
            color.a = 0f;
            text.color = color;

            yield return new WaitForSeconds(interval);

            // •\Ž¦
            color.a = 1f;
            text.color = color;

            yield return new WaitForSeconds(interval);
        }
    }
}