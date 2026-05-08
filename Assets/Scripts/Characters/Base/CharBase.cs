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
    protected int id;

    [Header("◇物理")]
    Vector2 vec;
    protected Vector2 direction;

    Rigidbody2D rb;

    virtual protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    virtual protected void Update()
    {
        var current = Keyboard.current;
    }

    virtual protected void FixedUpdate()
    {
        rb.linearVelocity = vec * data.speed;
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

    public void Move(InputAction.CallbackContext ctx)
    {
        vec = ctx.ReadValue<Vector2>();
        Debug.Log(vec + " , " + direction);

        if(vec!=new Vector2(0,0))
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
}
