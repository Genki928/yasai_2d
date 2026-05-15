using UnityEngine;
using UnityEngine.UI;

public class BurstBar : MonoBehaviour
{
    [SerializeField] private CharBase targetPlayer;

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();

        Debug.Log(image);
    }

    void Update()
    {
        if (targetPlayer != null)
        {
            //Debug.Log(targetPlayer.burst+"damage");
            //image.fillAmount = targetPlayer.burst / 100f;
        }
    }

    public void Init(GameObject player_obj)
    {
        Debug.Log("test");
        CharBase player = player_obj.GetComponent<CharBase>();
        targetPlayer = player;
    }

    public void Draw(int burst, int max)
    {
        image.fillAmount = targetPlayer.burst / 100f;
    }
}