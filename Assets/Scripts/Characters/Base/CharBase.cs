using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharBase : MonoBehaviour
{
    /// <summary> プレイヤーが死亡した際に起動するイベント </summary>
    public static event Action<int> OnPlayerDies;

    [Header("◇キャラクターデータ")]
    public CharData data;
    public int burst = 0;
    public int id;
    public int rigid;
    protected int skill_1_cooltime = 0;
    protected int skill_2_cooltime = 0;
    protected int skill_3_cooltime = 0;
    protected bool can_control = true;
    [SerializeField] protected GameObject cursor_pf;
    protected GameObject cursor_obj;


    [Header("◇物理")]
    protected Vector2 vec;
    protected Vector2 direction;
    protected Rigidbody2D rb;


    virtual protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cursor_obj = Instantiate(cursor_pf, transform.position, Quaternion.identity);
    }

    virtual protected void Update()
    {
        if (rigid > 0) --rigid;
        if (skill_1_cooltime > 0) --skill_1_cooltime;
        if (skill_2_cooltime > 0) --skill_2_cooltime;
        if (skill_3_cooltime > 0) --skill_3_cooltime;
        cursor_obj.transform.position = new(transform.position.x, transform.position.y - 0.5f);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        cursor_obj.transform.localRotation = Quaternion.Euler(0, 0, angle - 90);
    }

    virtual protected void FixedUpdate()
    {
        if (can_control)
        {
            // 硬直が無ければ移動
            if (rigid == 0)
                rb.linearVelocity = vec * data.speed;
            // 硬直があれば移動不可
            else
                rb.linearVelocity = Vector2.zero;
        }
    }

    /// <summary> プレイヤーにダメージを与える </summary>
    /// <param name="value"> 与えるダメージ量 </param>
    public void Damage(int value)
    {
        // バースト値が最大なら中断
        if (burst >= data.max_burst) return;

        // 受けるダメージが過剰ならセーブする
        burst = burst + value > data.max_burst ?
                     data.max_burst : burst + value;

        // バースト値が最大なら、死亡
        if (burst == data.max_burst)
        {
            OnPlayerDies?.Invoke(id);
        }
    }

    /// <summary> 移動関数 </summary>
    public void Move(InputAction.CallbackContext ctx)
    {
        // ベクトルの取得
        vec = ctx.ReadValue<Vector2>();
        Debug.Log(vec + " , " + direction);

        // 向きを保存
        if (vec != new Vector2(0, 0))
        {
            direction = vec;
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
}
