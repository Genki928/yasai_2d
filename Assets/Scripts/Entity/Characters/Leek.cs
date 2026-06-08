using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;


public class Leek : CharBase, IBurst
{
    //斬撃用
    [SerializeField] GameObject collision;
    [SerializeField] int skill1Damage = 20;

    //カウンター用
    [SerializeField] float counterTime = 1.0f;
    [SerializeField] int counterDamage = 30;

    private bool isCounter = false;



    [SerializeField] SpriteRenderer sprite;
    public int id { get; set; } = 0;


    private Vector2 defaultSize;
    private bool isHeadBanging = false;

    override protected void Start()
    {
        base.Start();
        sprite = GetComponent<SpriteRenderer>();
    }

    override protected void Update()
    {
    }

    protected override void FixedUpdate()
    {
    }

    public override void Skill1(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        Vector2 spawnPos =
            (Vector2)transform.position +
            direction.normalized * 1.5f;

        GameObject obj =
            Instantiate(collision, spawnPos, Quaternion.identity);

        HitDamageArea hit =
            obj.GetComponent<HitDamageArea>();

        hit.Init(id, skill1Damage, Vector2.zero);
    }

    override public void Skill2(InputAction.CallbackContext ctx)
    {

    }

    void OnTriggerEnter2D(Collider2D col)
    {

    }
}


