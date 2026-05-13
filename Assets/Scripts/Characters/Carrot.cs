using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class Carrot : CharBase
{
    [SerializeField] SpriteRenderer sprite;

    public Sprite carrot_default;
    public Sprite tackle;

    // タックル関連
    [SerializeField] private float tackleSpeed = 20f;
    [SerializeField] private float tackleTime = 0.5f;

    private bool isTackling = false;



    override protected void Start()
    {
        base.Start();
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    override protected void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }


    override public void Skill1(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // 中断処理
            if (skill_1_cooltime != 0) return;

            // タックル開始
            StartCoroutine(Tackle());
            Debug.Log("tackle");

            // 硬直・クールタイム
            rigid += data.skill_1_rigid;
            skill_1_cooltime = data.skill_1_cooltime;
        }
    }
    private IEnumerator Tackle()
    {


        sprite.sprite = tackle;
       
        can_control = false;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 突進
        rb.linearVelocity = direction * tackleSpeed;

        // 指定時間だけ突進
        yield return new WaitForSeconds(tackleTime);

        // 停止
        rb.linearVelocity = Vector2.zero;

        can_control = true;
        sprite.sprite = carrot_default;
        transform.rotation=Quaternion.Euler(0, 0, 0);
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent<CharBase>(out var cb))
        {
            Debug.Log("tackle_damage");
            if (cb.id != id&&!can_control)
            {
                // 被弾処理
                cb.Damage(100);

                Vector2 knockbackDir = (cb.transform.position - transform.position).normalized;

                cb.KnockBack(10, knockbackDir);
            }
        }
    }
}




