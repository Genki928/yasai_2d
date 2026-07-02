using Const;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoloBattleManager : BattleManagerBase
{
    // ----- 定数 -----
    const int SPAWN_COOLTIME = 40;
    const int BONUS_TIMELIMIT_FRAMERATE = 400;

    // ----- 変数 -----
    [Header("◇的生成")]
    [SerializeField] List<TargetBase> targets = new();
    [SerializeField] List<Spawner> target_spawn_point = new();
    [SerializeField] List<Sprite> target_sprites = new();
    public int spawn_cooltime = 0;

    // -----
    int score;

    [Header("◇GUI")]
    [SerializeField] GUI gui;
    [SerializeField] Text score_text;
    [SerializeField] Text score_bonus;
    [SerializeField] Image score_circle;
    float default_score_bonus = 1.0f;
    float now_score_bonus = 1.0f;
    int bonus_timer = 0;

    [SerializeField] AudioClip se;
    [SerializeField] AudioClip se1;
    [SerializeField] GameObject deathEffect;
    [SerializeField] Text koText;

    void Awake()
    {
        Application.targetFrameRate = 60;
        timer.OnFinish += Finish;
        CharBase.OnPlayerDies += Death;
    }

    protected override void Start()
    {
        base.Start();

        // プレイヤー生成
        Debug.Log(CharPickData.id);
        player[0] = Instantiate(characters[CharPickData.id].chars, spawn_point[0].point.transform.position, Quaternion.identity);
        datas[0] = player[0].GetComponent<CharBase>();

        // バーストバーとの紐づけ
        gui.bar.Init(player[0]);

        // 各種UIとの紐づけ
        if (player[0].TryGetComponent<CharBase>(out var p))
        {
            datas[0].id = 0;   // id
            datas[0].direction = SetDirect(spawn_point[0].direct);    // 方向
            datas[0].burst_bar = gui.bar;   // バースト
            gui.name.text = datas[0].data.char_name;    // キャラ名
            datas[0].cooltimer[0] = gui.skill1_cooltimer;   // スキル1のクールタイムを表示
            datas[0].cooltimer[1] = gui.skill2_cooltimer;   // スキル2のクールタイムを表示
            gui.icon.sprite = characters[CharPickData.id].icon; // アイコン
        }

        //boss = Instantiate(bosses[0], new(-10.0f, -5.0f), Quaternion.identity);
        //boss.player = player;
        StartCoroutine(StartBattleEffect());
    }
    

    void Update()
    {
        // スコアボーナスが初期状態じゃない（ボーナスが付与されている）なら、
        if (now_score_bonus != 1.0f)
        {
            // 時間制限の更新
            score_circle.fillAmount = 1 - (float)++bonus_timer / BONUS_TIMELIMIT_FRAMERATE;
            
            // 時間制限が終わったなら、
            if (score_circle.fillAmount == 0)
            {
                // タイマー変数や時間制限の初期化
                score_circle.fillAmount = 1;
                now_score_bonus = default_score_bonus;

                // UIの更新
                score_bonus.text = "x " + now_score_bonus.ToString("N1");
            }
        }
        if (++spawn_cooltime > SPAWN_COOLTIME)
        {
            // ランダムな場所からスポーン
            int spawn = Random.Range(0, target_spawn_point.Count);
            TargetBase tb = Instantiate(targets[0], target_spawn_point[spawn].point.transform.position, Quaternion.identity);

            // Spriteを調整
            tb.Init(this, player[0].GetComponent<CharBase>(), transform.position.x > tb.transform.position.x ? true : false);

            // 操作キャラクターと画像が被らないよう調整
            int img = CharPickData.id;
            do
            {
                img = Random.Range(0, target_sprites.Count);
            } while (img == CharPickData.id);
            tb.GetComponent<SpriteRenderer>().sprite = target_sprites[img];

            // 見やすく（黒く）する
            tb.GetComponent<SpriteRenderer>().color = new Color32(128, 128, 128, 255);

            // タイマーをリセット
            spawn_cooltime = 0;
        }
    }

    void Death(int i = 0)
    {
        timer.Stop();
        StartCoroutine(DeathC());
    }


    IEnumerator DeathC()
    {
        Camera cam = Camera.main;

        Rigidbody2D rb = player[0].GetComponent<Rigidbody2D>();

        Vector3 originalPos = cam.transform.position;
        float originalSize = cam.orthographicSize;

        Vector3 zoomPos = new(player[0].transform.position.x, player[0].transform.position.y + 0.5f, player[0].transform.position.z);
        zoomPos.z = originalPos.z;

        player[0].GetComponent<CharBase>().can_control = false;
        rb.linearVelocity = Vector2.zero;

        //ズーム
        //SE
        audioSource.PlayOneShot(se);

        float targetSize = 2.2f;
        float t = 0;

        while (t < 0.1f)
        {
            t += Time.deltaTime;

            cam.transform.position =
                Vector3.Lerp(originalPos, zoomPos, t / 0.1f);

            cam.orthographicSize =
                Mathf.Lerp(originalSize, targetSize, t / 0.1f);

            yield return null;
        }

        //ス〇ブラ風シェイク
        float shakeTime = 2.0f;

        while (shakeTime > 0)
        {
            shakeTime -= Time.deltaTime;

            Vector2 shake =
                UnityEngine.Random.insideUnitCircle * 0.25f;

            cam.transform.position =
                zoomPos + new Vector3(shake.x, shake.y, 0);

            yield return null;
        }
        audioSource.PlayOneShot(se1);

        // エフェクト生成
        Instantiate(
            deathEffect,
            player[0].transform.position,
            Quaternion.identity);

        // プレイヤーを消す
        player[0].transform.position = new Vector3(1000, 1000, 0);
        // または
        // loser.SetActive(false);

        // KO表示
        yield return ShowKO();

        yield return new WaitForSeconds(4.5f);

        // シーン切り替え
        SoloBattleResult.name = datas[0].data.char_name;
        SoloBattleResult.score = score;
        SoloBattleResult.img = datas[0].GetDefaultImage();
        SoloBattleResult.win = false;
        SceneManager.LoadScene("ResultScene-pve");
    }

    IEnumerator ShowKO()
    {
        koText.gameObject.SetActive(true);

        Color c = koText.color;
        c.a = 0;
        koText.color = c;

        koText.transform.localScale = Vector3.one * 4f;

        float t = 0;

        while (t < 0.2f)
        {
            t += Time.deltaTime;

            float p = t / 0.2f;

            koText.transform.localScale =
                Vector3.Lerp(Vector3.one * 4f, Vector3.one, p);

            c.a = p;
            koText.color = c;

            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        t = 0;

        while (t < 0.3f)
        {
            t += Time.deltaTime;

            c.a = 1 - t / 0.3f;
            koText.color = c;

            yield return null;
        }

        koText.gameObject.SetActive(false);
    }

    protected override IEnumerator StartBattleEffect()
    {
        Camera cam = Camera.main;
        datas[0].can_control = false;

        Vector3 originalPos = defaultCameraPos;
        float originalSize = defaultCameraSize;

        float zoomSize = 2.5f;

        // Player1
        yield return ZoomToPlayer(
            player[0].transform.position,
            zoomSize,
            0.5f);

        yield return new WaitForSeconds(0.4f);

        // Player2
        yield return ZoomToPlayer(
            player[0].transform.position,
            zoomSize,
            0.5f);

        yield return new WaitForSeconds(0.4f);

        yield return MoveCamera(
       defaultCameraPos,
       defaultCameraSize,
       0.6f);

        yield return ShowReady();

        yield return ShowGo();
        datas[0].can_control = true;
        
    }

    void OnDestroy()
    {
        timer.OnFinish -= Finish;
        CharBase.OnPlayerDies -= Death;
    }

    /// <summary> バトルを終了させる </summary>
    /// <param name="id"> プレイヤーの識別id </param>
    public void Finish()
    {
        SoloBattleResult.name = datas[0].data.char_name;
        SoloBattleResult.score = score;
        SoloBattleResult.img = datas[0].GetDefaultImage();
        SoloBattleResult.win = true;
        SceneManager.LoadScene(SceneName.RESULT_PVE);
    }

    Vector2 SetDirect(DIRECT direct)
    {
        if (direct == DIRECT.RIGHT) return new(1.0f, 0.0f);
        if (direct == DIRECT.LEFT) return new(-1.0f, 0.0f);
        if (direct == DIRECT.UP) return new(0.0f, 1.0f);
        if (direct == DIRECT.DOWN) return new(0.0f, -1.0f);
        return Vector2.zero;
    }

    public void CalculateScore(int value)
    {
        // 計算（0未満になるなら調整）
        score += score + value < 0 ? -score : Mathf.RoundToInt(value * now_score_bonus);
        score_text.text = score.ToString();
        score_circle.fillAmount = 1;
        now_score_bonus += 0.1f;
        score_bonus.text = "x " + now_score_bonus.ToString("N1");
        bonus_timer = 0;
    }
}

static public class SoloBattleResult
{
    static public string name = "オレ";
    static public int score = 2000;
    static public Sprite img;
    static public bool win = false;
}