
using System.Collections.Generic;
using UnityEngine;

public class MushroomDamageArea : MonoBehaviour
{
    [Header("攻撃設定")]
    [SerializeField] private float lifeTime = 3.0f;
    [SerializeField] private float hitInterval = 0.5f;

    [Header("デバフ設定")]
    [SerializeField] private float slowValue = -2.0f;
    [SerializeField] private float slowDuration = 1.0f;

    private int ownerId;
    private int damage;

    // 敵ごとの次回ダメージ時刻
    private Dictionary<int, double> nextHitTime = new();

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Init(int id, int damage)
    {
        ownerId = id;
        this.damage = damage;
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        // 対象を取得
        IBurst target = col.GetComponent<IBurst>();

        if (target == null)
            target = col.GetComponentInParent<IBurst>();

        if (target == null)
            return;

        // 発射主には当たらない
        if (target.id == ownerId)
            return;

        double now = Time.timeAsDouble;

        // 初回接触なら即座に攻撃
        if (!nextHitTime.ContainsKey(target.id))
        {
            nextHitTime.Add(target.id, 0.0);
        }

        // ダメージ間隔の確認
        if (now < nextHitTime[target.id])
            return;

        // 次回ダメージ時刻を設定
        nextHitTime[target.id] = now + hitInterval;

        // 移動速度低下
        CharBase character = col.GetComponent<CharBase>();

        if (character == null)
            character = col.GetComponentInParent<CharBase>();

        if (character != null)
        {
            character.State[(int)StateName.Speed].AddState(
                new Attribute(slowValue, slowDuration)
            );
        }

        // ダメージ
        target.Damage(
            new Damage(damage, DamageType.Soundable),
            ownerId
        );

        Debug.Log($"キノコ攻撃：対象ID={target.id}");
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        IBurst target = col.GetComponent<IBurst>();

        if (target == null)
            target = col.GetComponentInParent<IBurst>();

        if (target == null)
            return;

        nextHitTime.Remove(target.id);
    }
}
