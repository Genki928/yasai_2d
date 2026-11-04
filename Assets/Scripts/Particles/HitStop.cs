using UnityEngine;
using System.Collections;

public class HitStop : MonoBehaviour
{
    [SerializeField] float stopTime = 0.08f;
    [SerializeField] float stopScale = 0.05f;

    public void Stop()
    {
        StartCoroutine(StopCoroutine());
    }

    IEnumerator StopCoroutine()
    {
        Time.timeScale = stopScale;

        yield return new WaitForSecondsRealtime(stopTime);

        Time.timeScale = 1f;
    }
}