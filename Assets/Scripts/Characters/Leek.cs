using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Leek : CharBase
{
    [Header("Skill 1 : Slash")]
    [SerializeField] private GameObject collision;
    [SerializeField] private int skill1Damage = 20;
    [SerializeField] private AudioClip se1;
    [Tooltip("斬撃を出す位置(自分からの前方距離)")]
    [SerializeField] private float slashDistance = 1.5f;
    [Tooltip("通常攻撃の攻撃判定の大きさ倍率(X=横に伸ばす / Y=縦)。オブジェクトの向きに沿った軸")]
    [SerializeField] private Vector2 slashAttackScale = new(2.0f, 1.0f);

    public bool isSrash = false;

    [Header("Skill 2 : Counter")]
    [SerializeField] private float counterTime = 1.0f;
    [Tooltip("カウンターのダメージの下限(受けたダメージ×倍率がこれ未満ならこの値)")]
    [SerializeField] private int counterMinDamage = 30;
    [Tooltip("受けたダメージに掛ける倍率")]
    [SerializeField] private float counterDamageMultiplier = 1.5f;
    [SerializeField] private AudioClip se2;
    [Tooltip("カウンター成立の瞬間に鳴らす音")]
    [SerializeField] private AudioClip se3;

    [Header("Other")]
    [SerializeField] private Sprite leek_default;

    [Header("Counter Attack")]
    [Tooltip("カウンター成功後、斬撃を出す位置(自分からの前方距離)")]
    [SerializeField] private float counterSlashDistance = 2.0f;

    [Header("◇生成位置")]
    [Tooltip("体の中心からの微調整(攻撃やエフェクトの出る位置をずらす)")]
    [SerializeField] private Vector2 centerOffset = Vector2.zero;

    [Header("◇演出")]
    [Tooltip("通常攻撃の斬撃エフェクト(任意)")]
    [SerializeField] private GameObject slashEffect;
    [Tooltip("カウンター構えに入った瞬間のエフェクト(任意)")]
    [SerializeField] private GameObject counterStartEffect;
    [Tooltip("カウンター成功の瞬間のエフェクト(任意)")]
    [SerializeField] private GameObject counterSuccessEffect;
    [Tooltip("カウンター反撃がヒットする位置のエフェクト(任意)")]
    [SerializeField] private GameObject counterHitEffect;
    [Tooltip("構え中に点滅する色")]
    [SerializeField] private Color counterColor = new(0.3f, 0.9f, 1f);
    [Tooltip("カウンター成功時のヒットストップ時間(秒。完全に止まる重めのストップ)")]
    [SerializeField] private float counterSuccessHitStop = 0.35f;
    [Tooltip("カウンター反撃の攻撃判定の大きさ倍率(X=横に伸ばす / Y=縦)。オブジェクトの向きに沿った軸")]
    [SerializeField] private Vector2 counterAttackScale = new(2.0f, 1.0f);

    private bool isCounter = false;
    private Coroutine counterCoroutine;

    // 点滅用(CharBase の被ダメ点滅と同じシェーダープロパティ)
    static readonly int FlashAmountID = Shader.PropertyToID("_FlashAmount");
    static readonly int FlashColorID = Shader.PropertyToID("_FlashColor");
    MaterialPropertyBlock fxMpb;

    // 構え中は体を点滅させる
    protected override void Update()
    {
        base.Update();

        if (isCounter)
        {
            float pulse = 0.35f + 0.25f * Mathf.Sin(Time.time * 20f);
            SetFlash(counterColor, pulse);
        }
    }

    // 通常攻撃
    public override void Skill1(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || !CanUseSkill1) return;

        audioSource.PlayOneShot(se1);

        Vector2 spawnPos = BodyCenter + direction.normalized * slashDistance;

        Quaternion rot = GetAttackRotation();

        GameObject slashObj = Instantiate(collision, spawnPos, rot);

        // 攻撃判定を横に伸ばす(プレハブ本来の大きさに倍率を掛ける)
        slashObj.transform.localScale = Vector3.Scale(
            slashObj.transform.localScale,
            new Vector3(slashAttackScale.x, slashAttackScale.y, 1f));

        slashObj.GetComponent<SimpleDamageArea>()
            .Init(
                id,
                new(skill1Damage, DamageType.Soundable),
                Vector2.zero,
                0.4f
            );

        // 演出:斬撃エフェクト
        SpawnFx(slashEffect, spawnPos, rot);

        rigid += data.skill_1_rigid;
        cooltime[(int)SkillName.Skill1].SetCooltime();
    }

    // カウンター構え
    public override void Skill2(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || !CanUseSkill2) return;

        isCounter = true;

        moveSpeed.AddEffect(new(-3.0f, 0.0f, 0, false));

        audioSource.PlayOneShot(se2);

        // 演出:構え開始のエフェクト(画面は動かさない)
        SpawnFx(counterStartEffect, BodyCenter, Quaternion.identity);

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

        moveSpeed.RemoveEffect(0);

        counterCoroutine = null;

        // 点滅を止める
        SetFlash(counterColor, 0f);
    }

    // カウンター成功
    private void CounterAttack(int counterDamage)
    {
        // カウンター成立の音(未設定ならログで知らせる)
        if (se3 != null) audioSource.PlayOneShot(se3);
        else Debug.LogWarning("Leek: se3(カウンター成立の音)が未設定です");

        // 演出:ヒットストップ + 強い画面揺れ + ズームイン + 白フラッシュ + エフェクト
        ScreenFx.HitStop(counterSuccessHitStop, 0f);   // 時間を完全に止める重いストップ
        ScreenFx.Shake(0.35f, 0.3f);            // 画面揺れはここ(カウンター成功時)だけ
        ScreenFx.ZoomPunch(-1.0f, 0.35f);
        SpawnFx(counterSuccessEffect, BodyCenter, Quaternion.identity);
        StartCoroutine(SuccessFlash());

        StartCoroutine(CounterSlash(counterDamage));
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

            // 受けたダメージを倍率で増やして返す(下限あり)
            int counterDamage = Mathf.Max(
                counterMinDamage,
                Mathf.RoundToInt(damage.Value * counterDamageMultiplier));
            Debug.Log($"Counter damage: 受けた{damage.Value} → 反撃{counterDamage}");

            CounterAttack(counterDamage);

            return;
        }

        base.Damage(damage, attackerId);
    }
    // カウンター成功時の反撃(その場で斬撃を出す。ダッシュ・テレポートはしない)
    private IEnumerator CounterSlash(int counterDamage)
    {
        // 操作禁止
        can_control = false;
        rb.linearVelocity = Vector2.zero;

        // ヒットストップが明けるまで待つ(止まっている間は実時間で待つ)
        yield return new WaitForSecondsRealtime(counterSuccessHitStop);

        // 攻撃オブジェクト生成(伸ばした分、少し遠くに出す)
        Vector2 spawnPos = BodyCenter + direction.normalized * counterSlashDistance;

        GameObject counterObj = Instantiate(collision, spawnPos, GetAttackRotation());

        // カウンター反撃の攻撃判定を伸ばす(プレハブ本来の大きさに倍率を掛ける)
        counterObj.transform.localScale = Vector3.Scale(
            counterObj.transform.localScale,
            new Vector3(counterAttackScale.x, counterAttackScale.y, 1f));

        // 構え中の点滅と同じ色にする
        TintObject(counterObj, counterColor);

        counterObj.GetComponent<SimpleDamageArea>()
            .Init(
                id,
                new(counterDamage, DamageType.Soundable),
                Vector2.zero,
                0.4f
            );

        // 演出:反撃ヒットのエフェクト
        SpawnFx(counterHitEffect, spawnPos, GetAttackRotation());

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

    // ---------------------------------------------------------
    // 演出ヘルパー
    // ---------------------------------------------------------

    // 体の中心(画像の中心がボトムセンターなので、スプライトの見た目の中心を使う)
    private Vector2 BodyCenter =>
        (sprite != null ? (Vector2)sprite.bounds.center : (Vector2)transform.position) + centerOffset;

    // エフェクトを生成(未設定なら何もしない。念のため3秒で消す)
    private void SpawnFx(GameObject prefab, Vector2 pos, Quaternion rot)
    {
        if (prefab == null) return;
        Destroy(Instantiate(prefab, pos, rot), 3f);
    }

    // オブジェクト(子も含む)のスプライトを color に変える(透明度は元のまま)
    private void TintObject(GameObject obj, Color color)
    {
        foreach (var sr in obj.GetComponentsInChildren<SpriteRenderer>())
            sr.color = new Color(color.r, color.g, color.b, sr.color.a);
    }

    // 体を color で amount だけ染める(0で元に戻る)
    private void SetFlash(Color color, float amount)
    {
        if (sprite == null) return;
        fxMpb ??= new MaterialPropertyBlock();
        sprite.GetPropertyBlock(fxMpb);
        fxMpb.SetColor(FlashColorID, color);
        fxMpb.SetFloat(FlashAmountID, amount);
        sprite.SetPropertyBlock(fxMpb);
    }

    // カウンター成功時に白く光って戻る
    private IEnumerator SuccessFlash()
    {
        const float time = 0.35f;
        float t = 0f;

        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            SetFlash(Color.white, 1f - t / time);
            yield return null;
        }

        SetFlash(Color.white, 0f);
    }
}