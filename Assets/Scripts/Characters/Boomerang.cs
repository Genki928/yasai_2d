using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャベツのブーメラン
/// ・一直線に飛んだあと、持ち主に向かって戻ってくる(持ち主に触れたら消える)
/// ・多段ヒット(同じ相手には hitInterval 秒おきにヒット)
/// プレハブには Collider2D(Is Trigger ON)を付けてください。
/// </summary>
public class Boomerang : MonoBehaviour
{
    [SerializeField] float spinSpeed = 720f;        // 回転速度(度/秒)
    [SerializeField] float catchRadius = 0.6f;      // 持ち主にこの距離まで近づいたら回収
    [SerializeField] float returnSpeedMultiplier = 1.2f;

    CharBase owner;
    int ownerId;
    int damage;
    float hitInterval;
    float speed;
    float outDistance;
    Vector2 dir;

    float travelled = 0f;
    bool returning = false;

    // 相手ごとの次にヒットできる時刻(多段ヒット用)
    readonly Dictionary<CharBase, float> nextHitTime = new();

    /// <param name="owner"> 投げたキャラ </param>
    /// <param name="dir"> 飛ばす向き </param>
    /// <param name="damage"> 1ヒットあたりのダメージ </param>
    /// <param name="speed"> 飛ぶ速さ </param>
    /// <param name="outDistance"> 行きの飛距離 </param>
    /// <param name="hitInterval"> 同じ相手への多段ヒット間隔(秒) </param>
    public void Init(CharBase owner, Vector2 dir, int damage, float speed, float outDistance, float hitInterval)
    {
        this.owner = owner;
        ownerId = owner.id;
        this.dir = dir.normalized;
        this.damage = damage;
        this.speed = speed;
        this.outDistance = outDistance;
        this.hitInterval = hitInterval;
    }

    void Update()
    {
        // 持ち主がいなくなったら消える(キャラ切り替え・リセット対策)
        if (owner == null)
        {
            Destroy(gameObject);
            return;
        }

        float step = speed * Time.deltaTime;

        if (!returning)
        {
            // 行き:一直線
            transform.position += (Vector3)(dir * step);
            travelled += step;
            if (travelled >= outDistance) returning = true;
        }
        else
        {
            // 帰り:持ち主の現在位置に向かう
            Vector3 target = owner.transform.position;
            transform.position = Vector3.MoveTowards(transform.position, target, step * returnSpeedMultiplier);

            if (Vector2.Distance(transform.position, target) <= catchRadius)
            {
                Destroy(gameObject);
                return;
            }
        }

        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        var target = other.GetComponentInParent<CharBase>();
        if (target == null || target == owner || target.id == ownerId) return;

        // 多段ヒット:間隔が空くまで同じ相手には当てない
        if (nextHitTime.TryGetValue(target, out float next) && Time.time < next) return;
        nextHitTime[target] = Time.time + hitInterval;

        target.Damage(new(damage, DamageType.Soundable), ownerId);
    }
}