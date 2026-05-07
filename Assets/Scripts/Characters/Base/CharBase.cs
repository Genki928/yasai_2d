using System;
using UnityEngine;

public class CharBase : MonoBehaviour
{
    /// <summary> プレイヤーが死亡した際に起動するイベント </summary>
    public static event Action<int> OnPlayerDies;

    [Header("◇キャラクターデータ")]
    public CharData data;
    protected int id;
    Rigidbody2D rb;

    virtual protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    virtual protected void Update()
    {
        ;
    }

    /// <summary> プレイヤーにダメージを与える </summary>
    /// <param name="value"> 与えるダメージ量 </param>
    public void Damage(int value)
    {
        // バースト値が最大なら中断
        if (data.burst >= data.max_burst) return;

        // 受けるダメージが過剰ならセーブする
        data.burst = data.burst + value > data.max_burst ?
                     data.max_burst : data.burst + value;

        // バースト値が最大なら、死亡
        if (data.burst == data.max_burst)
        {
            OnPlayerDies?.Invoke(id);
        }
    }
}
