using Const;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoloResultManager : MonoBehaviour
{
    // ----- 定数 ----- //
    [SerializeField] float START_SE_DELAY;
    [SerializeField] float DRAM_TO_TEXT_DELAY;
    [SerializeField] float DISPLAY_SCORE_DELAY;
    [SerializeField] float SCORE_TO_RANK_DELAY;

    // ----- プロパティ ----- //
    bool IsFinishedDirection(InputAction.CallbackContext ctx)
    {
        return ctx.performed &&
            (currentProcess == PvEResultMenuProcess.Menu || currentProcess == PvEResultMenuProcess.Score);
    }

    bool IsMoveCursor(InputAction.CallbackContext ctx)
    {
        return ctx.performed &&
            currentProcess == PvEResultMenuProcess.Menu;
    }

    // ----- 変数 ----- //
    [Header("◇Result")]
    [SerializeField] Transform rankLayer;
    [SerializeField] Text yourScoreIs;
    [SerializeField] Text scoreUI;
    [SerializeField] Text yourRankIs;
    [SerializeField] Text rankUI;
    [SerializeField] List<SoloResultRank> ranks = new();
    int displayCnt = 0;
    float sSpace = 1.0f;

    [Header("◇Made in ...")]
    [SerializeField] GameObject made;
    [SerializeField] SpriteRenderer skin;
    [SerializeField] Text nameUI;
    [SerializeField] Text winText;
    List<GameObject> rankObj = new();

    [Header("◇Curosr")]
    [SerializeField] GameObject cursor;
    [SerializeField] List<GameObject> option = new();
    int currentOption = 0;
    PvEResultMenuProcess currentProcess = PvEResultMenuProcess.direction;

    [Header("◇Score")]
    [SerializeField] GameObject scoreBackground;
    [SerializeField] Text characterHighscore;
    [SerializeField] Text characterHighscoreTitle;
    [SerializeField] Text allCharacterHighscore;

    [Header("◇Sound")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip cursorChanged;
    [SerializeField] AudioClip interact;
    [SerializeField] AudioClip dram;
    [SerializeField] AudioClip shortDram;
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip loseSound;

    enum PvEResultMenuProcess
    {
        direction,
        Menu,
        Loading,
        Score
    }

    [Serializable]
    class SoloResultRank
    {
        public int needScore = 0;
        public int sCount = 0;
    }

    void Start()
    {
        // オブジェクトの無効化
        yourScoreIs.gameObject.SetActive(false);
        scoreUI.gameObject.SetActive(false);
        yourRankIs.gameObject.SetActive(false);
        made.gameObject.SetActive(false);
        cursor.SetActive(false);
        scoreBackground.SetActive(false);
        characterHighscore.gameObject.SetActive(false);
        characterHighscoreTitle.gameObject.SetActive(false);
        allCharacterHighscore.gameObject.SetActive(false);
        foreach (GameObject op in option) op.SetActive(false);

        // リザルトを生産者マークに適用
        skin.sprite = SoloBattleResult.img;
        nameUI.text = SoloBattleResult.name;
        if (!SoloBattleResult.win) winText.text = "頑張りました";

        //ハイスコアの更新
        int id = CharPickData.id;
        Highscore.character[id].score.Add(SoloBattleResult.score);
        Highscore.character[id].score.Sort();
        Highscore.character[id].score.Reverse();
        characterHighscore.text =
            $"1.{Highscore.character[id].score[0]}\n2.{Highscore.character[id].score[1]}\n3.{Highscore.character[id].score[2]}\n" +
            $"----------\n" +
            $"今回.{SoloBattleResult.score}";

        Highscore.allCharacter.score.Add(SoloBattleResult.score);
        Highscore.allCharacter.score.Sort();
        Highscore.allCharacter.score.Reverse();
        allCharacterHighscore.text =
            $"1.{Highscore.allCharacter.score[0]}\n2.{Highscore.allCharacter.score[1]}\n3.{Highscore.allCharacter.score[2]}\n" +
            $"----------\n" +
            $"Aボタンで戻る";

        // 演出の開始
        StartCoroutine(Result());
    }

    void Update()
    {
        ;
    }

    IEnumerator Result()
    {
        // 「あなたのスコアは...」
        yield return new WaitForSeconds(START_SE_DELAY);
        audioSource.PlayOneShot(dram);
        yield return new WaitForSeconds(DRAM_TO_TEXT_DELAY);
        yourScoreIs.gameObject.SetActive(true);

        // 「○○点！」
        yield return new WaitForSeconds(DISPLAY_SCORE_DELAY);
        scoreUI.gameObject.SetActive(true);
        scoreUI.text = $"{SoloBattleResult.score}";

        // 「あなたの野菜ランクは...」
        yield return new WaitForSeconds(SCORE_TO_RANK_DELAY);
        audioSource.PlayOneShot(dram);

        // 「○○ランク！」
        yield return new WaitForSeconds(DRAM_TO_TEXT_DELAY);
        yourRankIs.gameObject.SetActive(true);

        // いくつランクを表示させるか数える
        for (int i = 0; i < ranks.Count; i++)
        {
            if (ranks[i].needScore <= SoloBattleResult.score)
            {
                displayCnt = ranks[i].sCount;
            }
        }

        // ランクを表示させる
        yield return new WaitForSeconds(DISPLAY_SCORE_DELAY);
        StartCoroutine(DisplayRank());
    }

    IEnumerator DisplayRank(int n = 0)
    {
        // ランクを表示する処理
        if (n++ < displayCnt)
        {
            // オブジェクトを生成 -> 位置を調整
            rankObj.Add(Instantiate(rankUI).gameObject);
            GameObject go = rankObj[rankObj.Count - 1].gameObject;
            go.transform.SetParent(rankLayer, false);
            go.transform.position = new(transform.position.x + sSpace, transform.position.y);
            sSpace += 1.2f;

            // 再帰
            if (n > 0) audioSource.PlayOneShot(shortDram);
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(DisplayRank(n));
        }
        else
        {
            yield return new WaitForSeconds(1.0f);

            // サウンド
            if (SoloBattleResult.win == true) audioSource.PlayOneShot(winSound);
            else audioSource.PlayOneShot(loseSound);

            // 生産者マークの表示
            made.gameObject.SetActive(true);

            // 選択肢、カーソルの描画
            foreach (GameObject op in option) op.SetActive(true);
            currentProcess = PvEResultMenuProcess.Menu;
            cursor.SetActive(true);
            Draw();
        }
    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        // 演出中なら中断
        if (!IsFinishedDirection(ctx)) return;

        audioSource.PlayOneShot(interact);
        if (currentProcess == PvEResultMenuProcess.Score)
        {
            currentProcess = PvEResultMenuProcess.Menu;
            DisplayHighscore(false);
            return;
        }

        // カーソル位置によって処理を分岐
        switch (currentOption)
        {
            // ハイスコアの表示
            case 0:
                currentProcess = PvEResultMenuProcess.Score;
                DisplayHighscore(true);
                break;

            // キャラクター選択シーンに移動
            case 1:
                StartCoroutine(WaitAndLoadScene(interact, SceneName.CHARACTER_PICK_PVE));
                break;

            // タイトル画面に移動
            case 2:
                StartCoroutine(WaitAndLoadScene(interact, SceneName.TITLE));
                break;
        }
    }


    public void Up(InputAction.CallbackContext ctx)
    {
        if (IsMoveCursor(ctx))
        {
            // はみ出さないように調整
            if (--currentOption < 0)
                currentOption = option.Count - 1;

            // 描画、SE
            audioSource.PlayOneShot(cursorChanged);
            Draw();
        }
    }

    public void Down(InputAction.CallbackContext ctx)
    {
        if (IsMoveCursor(ctx))
        {
            // はみ出さないように調整
            if (++currentOption >= option.Count)
                currentOption = 0;

            // 描画、SE
            audioSource.PlayOneShot(cursorChanged);
            Draw();
        }
    }

    /// <summary> カーソルの描画 </summary>
    void Draw()
    {
        Vector2 pos = new(option[currentOption].transform.position.x + -5.0f, option[currentOption].transform.position.y + 0.2f);
        cursor.transform.position = pos;
    }

    /// <summary> サウンドが再生し終わるまで、シーンの遷移を待機 </summary>
    /// <param name="clip"> 待機させるサウンド </param>
    /// <param name="sceneName"> 遷移するシーン名 </param>
    IEnumerator WaitAndLoadScene(AudioClip clip, string sceneName)
    {
        currentProcess = PvEResultMenuProcess.Loading;
        yield return new WaitForSeconds(clip.length);
        SceneManager.LoadScene(sceneName);
    }

    void DisplayHighscore(bool display)
    {
        scoreBackground.SetActive(display);

        // 名前の適用
        characterHighscoreTitle.text = SoloBattleResult.name;
        characterHighscoreTitle.gameObject.SetActive(display);

        // スコアの適用
        characterHighscore.gameObject.SetActive(display);
        allCharacterHighscore.gameObject.SetActive(display);
    }
}

public static class Highscore
{
    public static CharacterScore[] character =
        Enumerable.Range(0, 6).Select(_ => new CharacterScore()).ToArray();

    public static CharacterScore allCharacter = new();

    public class CharacterScore
    {
        public List<int> score = new() { 0, 0, 0 };
    }
}