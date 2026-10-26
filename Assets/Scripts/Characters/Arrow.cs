using UnityEngine;

public class Arrow : MonoBehaviour
{
    CharBase player;
    [SerializeField] GameObject parent;

    void Update()
    {
        transform.position = player.transform.position;
    }

    /// <summary> Šp“x‚ÌÄŒˆ’è </summary>
    /// <param name="direction"> Œü‚« </param>
    public void Refresh(Vector2 direction)
    { 
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        parent.transform.localRotation = Quaternion.Euler(0, 0, angle - 90);
    }

    /// <summary> ’Ç]æ‚Ì•Û‘¶ </summary>
    /// <param name="player"> ’Ç]æ </param>
    public void Set(CharBase player)
    {
        this.player = player;
    }
}
