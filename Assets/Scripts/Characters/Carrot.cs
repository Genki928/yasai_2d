using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class Carrot : CharBase
{
    [SerializeField] SpriteRenderer sprite;

    Sprite tackle;

    // タックル関連
    private Rigidbody2D rb;
    [SerializeField] private float tackleSpeed = 15f;
    [SerializeField] private float tackleTime = 0.2f;

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
       
        if (isTackling)
        {
            return;
        }

       
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
        Debug.Log("now");
        Debug.Log(direction*tackleSpeed);
        isTackling = true;

        // 突進
        rb.linearVelocity = direction * tackleSpeed;

        // 指定時間だけ突進
        yield return new WaitForSeconds(tackleTime);

        // 停止
        rb.linearVelocity = Vector2.zero;
        rb.Sleep();
        Debug.Log("end");

        isTackling = false;
    }
}



