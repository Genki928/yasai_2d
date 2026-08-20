using Const;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoloResultManager : MonoBehaviour
{
    // ----- プロパティ ----- //
    bool IsFinishedDirection(InputAction.CallbackContext ctx) { return canInput && ctx.performed && !sceneLoading; }

    // ----- 定数 ----- //
    [SerializeField] float START_SE_DELAY;
    [SerializeField] float DRAM_TO_TEXT_DELAY;
    [SerializeField] float DISPLAY_SCORE_DELAY;
    [SerializeField] float SCORE_TO_RANK_DELAY;

    // ----- 変数 ----- //
    [Header("◇Result")]
    [SerializeField] Transform canvas;
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
    [SerializeField] GameObject optionCanvas;
    [SerializeField] List<GameObject> option = new();
    int currentOption = 0;
    bool canInput = false;
    bool sceneLoading = false;

    [Header("◇Sound")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip cursorChanged;
    [SerializeField] AudioClip interact;
    [SerializeField] AudioClip dram;

    void Start()
    {
        yourScoreIs.gameObject.SetActive(false);
        scoreUI.gameObject.SetActive(false);
        yourRankIs.gameObject.SetActive(false);
        made.gameObject.SetActive(false);
        optionCanvas.SetActive(false);
        cursor.SetActive(false);
        StartCoroutine(Result());
        skin.sprite = SoloBattleResult.img;
        nameUI.text = SoloBattleResult.name;
        if (!SoloBattleResult.win) winText.text = "頑張りました";
    }

    // Update is called once per frame
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
        yield return new WaitForSeconds(DRAM_TO_TEXT_DELAY);
        yourRankIs.gameObject.SetActive(true);

        // 「○○ランク！」
        yield return new WaitForSeconds(DISPLAY_SCORE_DELAY);
        for (int i = 0; i < ranks.Count; i++)
        {
            if (ranks[i].needScore <= SoloBattleResult.score)
            {
                displayCnt = ranks[i].sCount;
            }
        }
        StartCoroutine(DisplayRank());
    }

    IEnumerator DisplayRank(int n = 0)
    {
        if (n++ < displayCnt)
        {
            rankObj.Add(Instantiate(rankUI).gameObject);
            GameObject go = rankObj[rankObj.Count - 1].gameObject;
            go.transform.SetParent(canvas, false);
            go.transform.position = new(transform.position.x + sSpace, transform.position.y);
            sSpace += 1.2f;
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(DisplayRank(n));
        }
        else
        {
            // 製作者
            yield return new WaitForSeconds(0.5f);
            made.gameObject.SetActive(true);
            nameUI.text = SoloBattleResult.name;
            canInput = true;
            optionCanvas.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            cursor.SetActive(true);
            Draw();
        }
    }

    public void SceneChange_CharacterPickScene(InputAction.CallbackContext ctx)
    {
        if (IsFinishedDirection(ctx))
        {
            switch (currentOption)
            {
                case 0:
                    break;

                case 1:
                    StartCoroutine(WaitAndLoadScene(interact, SceneName.CHARACTER_PICK_PVE));
                    break;

                case 2:
                    StartCoroutine(WaitAndLoadScene(interact, SceneName.CHARACTER_PICK_PVE));
                    break;
            }
        }
    }


    public void Up(InputAction.CallbackContext ctx)
    {
        if (IsFinishedDirection(ctx))
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
        if (IsFinishedDirection(ctx))
        {
            // はみ出さないように調整
            if (++currentOption >= option.Count)
                currentOption = 0;

            // 描画、SE
            audioSource.PlayOneShot(cursorChanged);
            Draw();
        }
    }

    void Draw()
    {
        Vector2 pos = new(option[currentOption].transform.position.x + -5.0f, option[currentOption].transform.position.y + 0.2f);
        cursor.transform.position = pos;
    }

    IEnumerator WaitAndLoadScene(AudioClip clip, string sceneName)
    {
        sceneLoading = true;
        audioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        SceneManager.LoadScene(sceneName);
    }
}

[Serializable]
public class SoloResultRank
{
    public int needScore = 0;
    public int sCount = 0;
}
