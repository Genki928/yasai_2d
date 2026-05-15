using UnityEngine;
using UnityEngine.UI;

public class BurstBar : MonoBehaviour
{
    [SerializeField] private CharBase targetPlayer;

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        ;
    }

    public void Init(GameObject player_obj)
    {
        CharBase player = player_obj.GetComponent<CharBase>();
        targetPlayer = player;
    }

    public void Draw(int burst, int max)
    {
        image.fillAmount = targetPlayer.burst / 100f;
    }
}