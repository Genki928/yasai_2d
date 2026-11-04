
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Leek : CharBase
{
    [Header("Skill 1 : Slash")]
    [SerializeField] private GameObject collision;
    [SerializeField] private int skill1Damage = 20;
    [SerializeField] private AudioClip se1;

    public bool isSrash = false;

    [Header("Skill 2 : Counter")]
    [SerializeField] private GameObject countercircle;
    [SerializeField] private float counterTime = 1.0f;
    [SerializeField] private int counterDamage = 50;
    [SerializeField] private AudioClip se2;
    [SerializeField] private AudioClip se3;

    [Header("Other")]
    [SerializeField] private Sprite leek_default;

    [Header("Counter Dash")]
    [SerializeField] private float counterDashPower = 40f;
    [SerializeField] private float counterDashDuration = 0.05f;
    [SerializeField] AfterImage afterImage;

    private bool isCounter = false;
    private Coroutine counterCoroutine;

    // 通常攻撃
    public override void Skill1(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || !CanUseSkill1) return;

        audioSource.PlayOneShot(se1);

        Vector2 spawnPos =
            (Vector2)transform.position +
            direction.normalized * 1.5f;

        Quaternion rot = GetAttackRotation();

        Instantiate(collision, spawnPos, rot)
            .GetComponent<SimpleDamageArea>()
            .Init(
                id,
                new(skill1Damage, DamageType.Soundable),
                Vector2.zero,
                0.4f
            );

        rigid += data.skill_1_rigid;
        cooltime[(int)SkillName.Skill1].SetCooltime();
    }

    // カウンター構え
    public override void Skill2(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || !CanUseSkill2) return;

        isCounter = true;

        _states[(int)StateName.Speed]
            .AddSpecialContent(new SpecialState(0, -3));

        audioSource.PlayOneShot(se2);

        GameObject obj = Instantiate(countercircle);

        CounterCircle circle = obj.GetComponent<CounterCircle>();
        circle.owner = this;

        counterCoroutine = StartCoroutine(Counter());

        rigid += data.skill_2_rigid;
        cooltime[(int)SkillName.Skill2].SetCooltime();
    }

    // カウンター受付時間
    private IEnumerator Counter()
    {
        yield return new WaitForSeconds(counterTime);

        EndCounter();
    }

    // カウンター終了処理
    private void EndCounter()
    {
        if (!isCounter) return;

        isCounter = false;

        _states[(int)StateName.Speed]
            .RemoveSpecialContent(0);

        counterCoroutine = null;
    }

    // カウンター成功
    private void CounterAttack()
    {
        audioSource.PlayOneShot(se3);

        StartCoroutine(CounterDash());
    }

    // ダメージ処理
    public override void Damage(Damage damage, int attackerId)
    {
        if (isCounter)
        {
            Debug.Log("Counter!");

            if (counterCoroutine != null)
            {
                StopCoroutine(counterCoroutine);
                counterCoroutine = null;
            }

            EndCounter();

            CounterAttack();

            return;
        }

        base.Damage(damage, attackerId);
    }
    // カウンター成功時の前方移動
    private IEnumerator CounterDash()
    {
        // 操作禁止
        can_control = false;

        afterImage.isActive = true;

        // ダッシュ方向を固定
        Vector2 dashDirection = direction.normalized;

        // ダッシュ開始
        rb.linearVelocity = dashDirection * counterDashPower;

        yield return new WaitForSeconds(counterDashDuration);

        // ダッシュ終了
        rb.linearVelocity = Vector2.zero;

        afterImage.isActive = false;
        // 攻撃オブジェクト生成
        Vector2 spawnPos =
            (Vector2)transform.position +
            dashDirection * 1.0f;

        Instantiate(collision, spawnPos, GetAttackRotation())
            .GetComponent<SimpleDamageArea>()
            .Init(
                id,
                new(counterDamage, DamageType.Soundable),
                Vector2.zero,
                0.4f
            );

        // 操作可能に戻す
        can_control = true;
    }

    // 攻撃方向の回転を取得
    private Quaternion GetAttackRotation()
    {
        float angle =
            Mathf.Atan2(direction.y, direction.x) *
            Mathf.Rad2Deg;

        return Quaternion.Euler(0, 0, angle + 180f);
    }

    // デフォルト画像
    public override Sprite GetDefaultImage()
    {
        return leek_default;
    }
}
