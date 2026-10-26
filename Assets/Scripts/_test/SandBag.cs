using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// DPSチェック・最大一撃ダメージ計測用のサンドバッグ。
/// CharBaseを継承しているので、既存の攻撃判定から Damage() が呼ばれる想定。
/// 死なない・動かない・スキルも使わない。
/// </summary>
public class SandBag : CharBase
{
    [Header("◇サンドバッグ設定")]
    [Tooltip("直近何秒間のダメージでDPSを出すか")]
    [SerializeField] float dps_window = 5f;
    [Tooltip("最後の被弾から何秒で戦闘終了(計測停止)とするか。次の被弾で自動リセット")]
    [SerializeField] float combat_timeout = 3f;
    [Tooltip("結果表示用テキスト(未設定ならConsoleに出力のみ)")]
    [SerializeField] TMP_Text info_text;

    // ----- 計測データ ----- //
    readonly Queue<(float time, int value)> hit_log = new();
    int total_damage;
    int hit_count;
    int max_hit;
    float first_hit_time;
    float last_hit_time;
    bool in_combat;

    // ----- 公開（他クラスから参照したい場合用） ----- //
    public int TotalDamage => total_damage;
    public int MaxHit => max_hit;
    public int HitCount => hit_count;

    /// <summary> 戦闘開始〜現在(or最後の被弾)までの平均DPS </summary>
    public float AverageDps
    {
        get
        {
            float end = in_combat ? Time.time : last_hit_time;
            float duration = end - first_hit_time;
            return duration > 0f ? total_damage / duration : total_damage;
        }
    }

    /// <summary> 直近 dps_window 秒間のDPS </summary>
    public float WindowDps
    {
        get
        {
            if (!in_combat) return 0f;
            float elapsed = Time.time - first_hit_time;
            float span = Mathf.Min(dps_window, Mathf.Max(elapsed, 0.1f));
            int sum = 0;
            foreach (var h in hit_log) sum += h.value;
            return sum / span;
        }
    }

    // CharBaseのStartは cursor_pf / data / cooltime を前提にしているので呼ばない
    protected override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        sprite = GetComponent<SpriteRenderer>();

        // プレイヤーのidと被らない値にする(攻撃側が id で自分を除外しているため)
        id = 99;
        //シイタケの毒沼用
        _states.Add(new MoveSpeed(0f)); // State[Speed]を参照する攻撃対策

        // ノックバックで動かないようにする
        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;

        RefreshUI();
    }

    protected override void Update()
    {
        if (in_combat)
        {
            float now = Time.time;

            // ウィンドウ外の古い記録を捨てる
            while (hit_log.Count > 0 && now - hit_log.Peek().time > dps_window)
                hit_log.Dequeue();

            // 一定時間被弾が無ければ戦闘終了(結果は表示したまま)
            if (now - last_hit_time >= combat_timeout)
            {
                in_combat = false;
                hit_log.Clear();
                Debug.Log($"[SandBag] 計測終了 合計:{total_damage} 最大一撃:{max_hit} 平均DPS:{AverageDps:F1} ({hit_count}hit)");
            }
        }

        RefreshUI();
    }

    // 何もしない(移動・クールタイム処理は不要)
    protected override void FixedUpdate() { }

    /// <summary> ダメージ計測。バーストは増やさず、死亡もしない </summary>
    public override void Damage(Damage damage, int id)
    {
        float now = Time.time;

        // 新しい戦闘の最初の一撃なら計測をリセット
        if (!in_combat)
        {
            ResetStats();
            in_combat = true;
            first_hit_time = now;
        }

        int value = damage.Value;
        total_damage += value;
        hit_count++;
        max_hit = Mathf.Max(max_hit, value);
        last_hit_time = now;
        hit_log.Enqueue((now, value));

        // 被弾フラッシュ演出(CharBaseのもの)
        StartCoroutine(FlashDamage());
        if (damage.Type == DamageType.Soundable) PlayDamageSE(value);

        Debug.Log($"[SandBag] {value}ダメージ (最大一撃:{max_hit})");
    }

    /// <summary> 回復は無効 </summary>
    public override void Heal(int value) { }

    /// <summary> 計測値を手動リセット </summary>
    [ContextMenu("Reset Stats")]
    public void ResetStats()
    {
        hit_log.Clear();
        total_damage = 0;
        hit_count = 0;
        max_hit = 0;
        first_hit_time = 0f;
        last_hit_time = 0f;
        in_combat = false;
        RefreshUI();
    }

    void RefreshUI()
    {
        if (info_text == null) return;

        info_text.text =
            $"MAX_DAMAGE: {max_hit}\n" +
            $"TOTAL: {total_damage} ({hit_count}hit)\n" +
            $"NAW_DPS({dps_window:F0}seconds): {WindowDps:F1}\n" +
            $"AV_DPS: {AverageDps:F1}" +
            (in_combat ? "" : "\n[stop]");
    }
}