using UnityEngine;

public class TargetBase : MonoBehaviour, IBurst
{
    public int id { get; set; } = 100;
    public int burst { get; set; } = 0;
    public int max_burst { get; set; } = 50;
    int score;
    public SoloBattleManager sbm;

    void Start()
    {
        
    }
    
    void Update()
    {
        
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
}
