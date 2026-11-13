
using UnityEngine;

public class RandomTruckShake : MonoBehaviour
{
    [Header("—h‚ê‚Ìİ’è")]
    [SerializeField] private float shakeAmount = 0.08f; // —h‚ê•
    [SerializeField] private float minInterval = 0.05f; // —h‚ê‚ÌÅ’ZŠÔŠu
    [SerializeField] private float maxInterval = 0.2f;  // —h‚ê‚ÌÅ’·ŠÔŠu

    private Vector3 initialPosition;
    private float timer;
    private Vector3 targetOffset;

    void Start()
    {
        initialPosition = transform.localPosition;
        timer = Random.Range(minInterval, maxInterval);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        // ƒ‰ƒ“ƒ_ƒ€‚Èƒ^ƒCƒ~ƒ“ƒO‚Å—h‚ê‚é•ûŒü‚ğ•ÏX
        if (timer <= 0)
        {
            targetOffset = new Vector3(
                                        0f,
                                        Random.Range(-shakeAmount, shakeAmount),
                                        0f
                                        );

            timer = Random.Range(minInterval, maxInterval);
        }

        // Œ³‚ÌˆÊ’u‚ğŠî€‚É—h‚ç‚·
        transform.localPosition = initialPosition + targetOffset;
    }

    void OnDisable()
    {
        transform.localPosition = initialPosition;
    }
}
