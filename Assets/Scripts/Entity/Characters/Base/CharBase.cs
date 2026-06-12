using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharBase : MonoBehaviour, IBurst
{
    /// <summary> プレイヤーが死亡した際に起動するイベント </summary>
    public static event Action<int> OnPlayerDies;

    [Header("◇キャラクターデータ")]
    public CharData data;
    public int burst { get; set; } = 0;
    public int id { get; set; } = 0;
    public int max_burst { get; set; } = 100;
    public int rigid;
    public int skill_1_cooltime = 0;
    public int skill_2_cooltime = 0;
    protected bool can_control = true;
    public int regen_burst_timer = 0;

    //速度
    protected int speed;
    Effect effect = new();


    [Header("◇カーソル")]
    [SerializeField] protected GameObject cursor_pf;
    protected Arrow cursor_obj;

    [Header("◇GUI")]
    [NonSerialized] public BurstBar burst_bar;
    [NonSerialized] public SkillCooltimer[] cooltimer = new SkillCooltimer[2];

    [Header("◇物理")]
    protected Vector2 vec;
    [NonSerialized] public Vector2 direction;
    protected Rigidbody2D rb;

    //オーディオソース用
    public AudioSource audioSource;

    virtual protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        cursor_obj = Instantiate(cursor_pf, transform.position, Quaternion.identity).GetComponent<Arrow>();
        cursor_obj.Refresh(direction);
        cursor_obj.Set(this);
        max_burst = data.max_burst;
        speed = data.speed;
    }

    virtual protected void Update()
    {
        if (rigid > 0) --rigid;
        if (skill_1_cooltime > 0) cooltimer[0].RefreshCooltimer(--skill_1_cooltime, data.skill_1_cooltime);
        if (skill_2_cooltime > 0) cooltimer[1].RefreshCooltimer(--skill_2_cooltime, data.skill_2_cooltime);

        if (regen_burst_timer < data.regen_burst_cooltime && burst < max_burst)
        {
            if (++regen_burst_timer == data.regen_burst_cooltime)
            {
                regen_burst_timer = data.restart_regen_burst_value;
                Heal(5);
            }
        }
        //if (effect.type == EffectType.Speed)
        //{
        //    if (effect.time > speed_.time)
        //    {
        //        speed_.time = effect.time;
        //    }
        //}
    }

    virtual protected void FixedUpdate()
    {
        if (can_control)
        {
            // 硬直が無ければ移動
            if (rigid == 0)
                rb.linearVelocity = vec * speed;
            // 硬直があれば移動不可
            else
                rb.linearVelocity = Vector2.zero;
        }
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

        // バースト値が最大なら、死亡
        if (burst == max_burst)
        {
            OnPlayerDies?.Invoke(id);
        }
    }

    /// <summary> プレイヤーにダメージを与える </summary>
    /// <param name="value"> 与えるダメージ量 </param>
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

    virtual public void Skill3(InputAction.CallbackContext ctx)
    {
        Debug.Log("Skill 3");
    }


    public void KnockBack(int knockbackPower, Vector2 hitDirection)
    {
        // ノックバック
        Debug.Log("knockback");
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(hitDirection.normalized * knockbackPower, ForceMode2D.Impulse);
    }

    virtual public Sprite GetDefaultImage()
    {
        return null;
    }

    public void GiveEffect(EffectState effect)
    {
        this.effect.speed.Add(effect);
    }
}

public class Effect
{
    public List<EffectState> speed = new();
}

public class EffectState
{
    //public EffectType type;
    public int time = 0;
    public int value = 0;
}

public enum EffectType
{
    Speed
}