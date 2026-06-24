using UnityEngine;

public class TargetBase : MonoBehaviour, IBurst
{
    public int id { get; set; } = 100;
    public int burst { get; set; } = 0;
    public int max_burst { get; set; } = 10;
    public int rigid { get; set; } = 0;
    Rigidbody2D rb;
    int score;
    public SoloBattleManager sbm;
    public CharBase player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        ;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = (player.transform.position - transform.position).normalized * 0.5f;
    }

    public void Damage(int value, int id)
    {
        // バースト値が最大なら中断
        if (burst >= max_burst) return;

        // 受けるダメージが過剰ならセーブする
        burst = burst + value > max_burst ?
                     max_burst : burst + value;

        // バースト値が最大なら、死亡
        if (burst == max_burst)
        {
            sbm.CalculateScore(score);
            Destroy(gameObject);
        }
    }

    public void Init(SoloBattleManager sbm, CharBase player)
    {
        this.sbm = sbm;
        this.player = player;
    }
}
