using UnityEngine;

public class BossBase : MonoBehaviour, IBurst
{

    [Header("◇キャラクターデータ")]
    public int id { get; set; } = 100;
    public int burst { get; set; } = 0;
    public int max_burst { get; set; } = 100;
    Rigidbody2D rb;
    public Collider2D hit_box;
    BossState state;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Freeze(true);
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = Vector2.zero;
    }

    public void Damage(int value, int id)
    {
        Debug.Log("mi");
    }

    /// <summary> 硬直させる </summary>
    /// <param name="freeze"> 有効なら当たり判定を有効化、座標を固定 </param>
    public void Freeze(bool freeze)
    {
        if(freeze)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            hit_box.enabled = true;
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.None;
            hit_box.enabled = false;
        }
    }
}