using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharBase : MonoBehaviour, IBurst
{
    // ----- プロパティ ----- //
    protected bool CanUseSkill1 => skill_1_cooltime == 0 && can_control;
    protected bool CanUseSkill2 => skill_2_cooltime == 0 && can_control;
    protected bool CanUseDash => dash_cooltime == 0 && can_control;

    // ----- 定数 ----- //
    const float DASH_POWER = 15.0f;
    const float DASHING_SECONDS = 0.3f;

    // ----- 変数 ----- //
    /// <summary> プレイヤーが死亡した際に起動するイベント </summary>
    public static event Action<int> OnPlayerDies;

    [Header("◇キャラクターデータ")]
    public CharData data;
    public int id { get; set; } = 0;
    public int max_burst { get; set; } = 100;
    public int burst { get; set; } = 0;
    public int rigid {  get; set; } = 0;
    public int skill_1_cooltime = 0;
    public int skill_2_cooltime = 0;
    public int dash_cooltime;
    public bool can_control = true;
    public int regen_burst_timer = 0;
    protected State speed = new() { generic = 0, buff = 0, debuff = 0 };
    public Effect state = new();


    [Header("◇カーソル")]
    [SerializeField] protected GameObject cursor_pf;
    protected Arrow cursor_obj;

    [Header("◇GUI")]
    [NonSerialized] public BurstBar burst_bar;
    [NonSerialized] public SkillCooltimer[] cooltimer = new SkillCooltimer[2];
    public GameObject pointer;

    [Header("◇物理")]
    protected Vector2 vec;
    [NonSerialized] public Vector2 direction;
    protected Rigidbody2D rb;

    //オーディオソース用
    public AudioSource audioSource;

    //スプライト
    [SerializeField] private SpriteRenderer sprite;

    public ShakeCamera camera;

    virtual protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        sprite = GetComponent<SpriteRenderer>();
        cursor_obj = Instantiate(cursor_pf, transform.position, Quaternion.identity).GetComponent<Arrow>();
        cursor_obj.Refresh(direction);
        cursor_obj.Set(this);
        max_burst = data.max_burst;
        speed.generic = data.speed;
    }
    //描画順番
    void LateUpdate()
    {
        sprite.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }


    virtual protected void Update()
    {
        if (rigid > 0) --rigid;
        if (skill_1_cooltime > 0) cooltimer[0].RefreshCooltimer(--skill_1_cooltime, data.skill_1_cooltime);
        if (skill_2_cooltime > 0) cooltimer[1].RefreshCooltimer(--skill_2_cooltime, data.skill_2_cooltime);
        if (dash_cooltime > 0) --dash_cooltime;

        if (regen_burst_timer < data.regen_burst_cooltime && burst < max_burst)
        {
            if (++regen_burst_timer == data.regen_burst_cooltime)
            {
                regen_burst_timer = data.restart_regen_burst_value;
                Heal(15);
            }
        }

        // スピードリセット
        speed.buff = speed.debuff = 0;

        for (int i = state.speed.Count - 1; i >= 0; i--)
        {
            // スピードのバフ処理
            if (state.speed[i].value > 0)
                // 値が強いバフがあるなら取得
                if (speed.buff < state.speed[i].value) speed.buff = state.speed[i].value;

            // スピードのデバフ処理
            if (state.speed[i].value < 0)
                // 値が強いデバフがあるなら取得
                if (speed.debuff < state.speed[i].value) speed.debuff = state.speed[i].value;

            // 時間が無くなったエフェクトを削除
            if (--state.speed[i].time <= 0) state.speed.RemoveAt(i);
        }

        //向き
        if (direction.x > 0)
        {
            sprite.flipX = false; // 右向き
        }
        else if (direction.x < 0)
        {
            sprite.flipX = true;  // 左向き
        }
    }

    virtual protected void FixedUpdate()
    {
        if (can_control)
        {
            // 硬直が無ければ移動
            if (rigid == 0)
                rb.linearVelocity = vec * SetState(speed);
            // 硬直があれば移動不可
            else
                rb.linearVelocity = Vector2.zero;
        }
        else if(burst>=max_burst)rb.linearVelocity = Vector2.zero;
    }

    /// <summary> プレイヤーにダメージを与える </summary>
    /// <param name="value"> 与えるダメージ量 </param>
    virtual public void Damage(int value, int id)
    {
        // バースト値が最大なら中断
        if (burst >= max_burst) return;
        regen_burst_timer = 0;

        // 受けるダメージが過剰ならセーブする
        burst = burst + value > max_burst ?
                     max_burst : burst + value;

        // 描画
        burst_bar.Draw(burst, max_burst);
        camera.Init(15, 10);

        // バースト値が最大なら、死亡
        if (burst == max_burst)
        {
            OnPlayerDies?.Invoke(id);
        }
    }

    /// <summary> プレイヤーを回復する </summary>
    /// <param name="value"> 回復する量 </param>
    virtual public void Heal(int value)
    {
        // バースト値が最低なら中断
        if (burst <= 0) return;
        regen_burst_timer = 0;

        // 回復が過剰ならセーブする
        burst = burst - value < 0 ?
                     0 : burst - value;

        // 描画
        burst_bar.Draw(burst, max_burst);
    }

    /// <summary>
    /// ラウンド開始時の初期化
    /// </summary>
    public virtual void ResetRound()
    {
        // バースト値リセット
        burst = 0;
        burst_bar.Draw(burst, max_burst);

        // クールタイムリセット
        skill_1_cooltime = 0;
        skill_2_cooltime = 0;

        // バースト回復タイマー
        regen_burst_timer = 0;

        // 硬直解除
        rigid = 0;

        // エフェクト解除
        state.speed.Clear();

        // 停止
        rb.linearVelocity = Vector2.zero;

        // 操作禁止（READY→GO後にtrueになる）
        can_control = false;
    }

    /// <summary> 移動関数 </summary>
    public void Move(InputAction.CallbackContext ctx)
    {
        // ベクトルの取得
        vec = ctx.ReadValue<Vector2>();

        // 向きを保存
        if (vec != new Vector2(0, 0))
        {
            direction = vec;
            cursor_obj.Refresh(vec);
        }
    }

    virtual public void Skill1(InputAction.CallbackContext ctx)
    {
        Debug.Log("Skill 1");
    }

    virtual public void Skill2(InputAction.CallbackContext ctx)
    {
        Debug.Log("Skill 2");
    }

    virtual public void Dash(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (!CanUseDash) return;

            rb.linearVelocity = direction * DASH_POWER;
            can_control = false;
            dash_cooltime = data.dash_cooltime;
            StartCoroutine(EDash());
        }
    }


    public void KnockBack(int knockbackPower, Vector2 hitDirection)
    {
        // ノックバック
        Debug.Log("knockback");
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(hitDirection.normalized * knockbackPower, ForceMode2D.Impulse);
    }

    /// <summary> リザルトに画像を渡す関数の雛型 </summary>
    /// <returns> 渡す画像 </returns>
    virtual public Sprite GetDefaultImage()
    {
        return null;
    }

    /// <summary> エフェクトの数値の整理 </summary>
    /// <param name="state"> 整理するエフェクト </param>
    /// <returns> 合算後のエフェクトの数値（0 <= n）を返す </returns>
    protected int SetState(State state)
    {
        int tmp = state.generic + state.buff - state.debuff;
        return tmp < 0 ? 0 : tmp;
    }

    IEnumerator EDash()
    {
        Debug.Log("testestse");
        yield return new WaitForSeconds(DASHING_SECONDS);
        rb.linearVelocity = Vector2.zero;
        can_control = true;
    }
}

[Serializable]
public class Effect
{
    public List<EffectState> speed = new();
}

[Serializable]
public class EffectState
{
    /// <summary> エフェクトの制限時間 </summary>
    public int time = 0;

    /// <summary> エフェクトの強度 </summary>
    public int value = 0;
}

public class State
{
    /// <summary> 元になる値 </summary>
    public int generic = 0;

    /// <summary> 増加させる値 </summary>
    public int buff = 0;

    /// <summary> 減少させる値 </summary>
    public int debuff = 0;
}