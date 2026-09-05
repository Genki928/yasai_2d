using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharBase : MonoBehaviour, IBurst
{
    // ----- プロパティ ----- //
    protected bool CanUseSkill1 => cooltime[(int)SkillName.Skill1].Current == 0 && can_control;
    protected bool CanUseSkill2 => cooltime[(int)SkillName.Skill2].Current == 0 && can_control;
    protected bool CanUseDash => cooltime[(int)SkillName.Dash].Current == 0 && can_control;
    public List<State> State => _states;

    // ----- 定数 ----- //
    const float DASH_POWER = 15.0f;
    const float DASHING_SECONDS = 0.2f;

    // ----- 変数 ----- //
    /// <summary> プレイヤーが死亡した際に起動するイベント </summary>
    public static event Action<int> OnPlayerDies;

    [Header("◇キャラクターデータ")]
    public CharData data;
    public int id { get; set; } = 0;
    public int max_burst { get; set; } = 100;
    public int burst { get; set; } = 0;
    public float rigid {  get; set; } = 0;
    public Cooltime[] cooltime = new Cooltime[3];
    public bool can_control = true;
    public float regen_burst_timer = 0;
    protected List<State> _states = new();


    [Header("◇カーソル")]
    [SerializeField] protected GameObject cursor_pf;
    protected Arrow cursor_obj;

    [Header("◇GUI")]
    [NonSerialized] public BurstBar burst_bar;
    [NonSerialized] public SkillCooltimer[] cooltimeUI = new SkillCooltimer[3];
    public GameObject pointer;

    [Header("◇物理")]
    protected Vector2 vec;
    [NonSerialized] public Vector2 direction;
    public Rigidbody2D rb;

    [Header("◇ダメージSE")]
    [SerializeField] AudioClip se_low;   //小ダメージ
    [SerializeField] AudioClip se_high;  //大ダメージ
    [SerializeField] AudioClip CTSound;


    //オーディオソース用
    public AudioSource audioSource;

    //スプライト
    [SerializeField] protected SpriteRenderer sprite;


    virtual protected void Start()
    {
        //
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        sprite = GetComponent<SpriteRenderer>();
        
        //
        cursor_obj = Instantiate(cursor_pf, transform.position, Quaternion.identity).GetComponent<Arrow>();
        cursor_obj.Refresh(direction);
        cursor_obj.Set(this);
        
        //
        max_burst = data.max_burst;
        _states.Add(new MoveSpeed(data.speed));

        //
        foreach (var ct in cooltime)
            ct.OnCooltimeCharged += PlayCooltimeChargedSound;
    }
    //描画順番
    void LateUpdate()
    {
        sprite.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }


    virtual protected void Update()
    {
        float time = Time.deltaTime;

        // 移動速度更新
        foreach (var state in _states)
            state.UpdateAttribute(time);

        // 硬直の更新
        if (rigid > 0.0f)
            rigid = Mathf.Max(0.0f, rigid - time);

        // クールタイムの更新
        for (int i = 0; i < 3; i++)
            if (cooltime[i].Current > 0.0f) cooltime[i].RemoveCooltime(time);

        // 
        if (regen_burst_timer < data.regen_burst_cooltime && burst < max_burst)
        {
            regen_burst_timer += Time.deltaTime;
            if (regen_burst_timer >= data.regen_burst_cooltime)
            {
                regen_burst_timer = data.restart_regen_burst_value;
                Heal(15);
            }
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
                rb.linearVelocity = vec * _states[(int)StateName.Speed].CurrentState;
            // 硬直があれば移動不可
            else
                rb.linearVelocity = Vector2.zero;
        }
        else if(burst>=max_burst)rb.linearVelocity = Vector2.zero;
    }

    /// <summary> プレイヤーにダメージを与える </summary>
    /// <param name="damage"> 与えるダメージ量 </param>
    virtual public void Damage(Damage damage, int id)
    {
        // バースト値が最大なら中断
        if (burst >= max_burst) return;
        regen_burst_timer = 0;

        // 受けるダメージが過剰ならセーブする
        burst = Math.Min(max_burst, burst +  damage.Value);

        // 描画
        burst_bar.Draw(burst, max_burst);

        // ダメージ量に応じたSE再生
        if (damage.Type == DamageType.Soundable) PlayDamageSE(damage.Value);

        // バースト値が最大なら、死亡
        if (burst == max_burst)
        {
            OnPlayerDies?.Invoke(id);
        }
    }

    /// <summary> ダメージ量に応じたSEを再生 </summary>
    void PlayDamageSE(int value)
    {
        if (audioSource == null) return;

        AudioClip clip = value > 50 ? se_high : se_low;

        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
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
    //public virtual void ResetRound()
    //{
    //    // バースト値リセット
    //    burst = 0;
    //    burst_bar.Draw(burst, max_burst);

    //    // クールタイムリセット
    //    skill_1_cooltime = 0;
    //    skill_2_cooltime = 0;

    //    // バースト回復タイマー
    //    regen_burst_timer = 0;

    //    // 硬直解除
    //    rigid = 0;

    //    // 停止
    //    rb.linearVelocity = Vector2.zero;

    //    // 操作禁止（READY→GO後にtrueになる）
    //    can_control = false;
    //}

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
            cooltime[(int)SkillName.Dash].SetCooltime();
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

    IEnumerator EDash()
    {
        yield return new WaitForSeconds(DASHING_SECONDS);
        rb.linearVelocity = Vector2.zero;
        can_control = true;
    }

    public void InitCooltime()
    {
        cooltime = new Cooltime[3]
        {
            new Cooltime(cooltimeUI[0], data.skill_1_cooltime),
            new Cooltime(cooltimeUI[1], data.skill_2_cooltime),
            new Cooltime(cooltimeUI[2], data.dash_cooltime)
        };
    }

    public void PlayCooltimeChargedSound()
    {
        audioSource.PlayOneShot(CTSound);
    }

    void OnDestroy()
    {
        foreach (var ct in cooltime)
            ct.OnCooltimeCharged -= PlayCooltimeChargedSound;
    }
}

public enum SkillName
{
    Skill1 = 0,
    Skill2 = 1,
    Dash = 2
}