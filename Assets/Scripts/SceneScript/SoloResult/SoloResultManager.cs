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
    [Header("UI")]
    public Text yourScoreIs;
    public Text scoreUI;
    public Text yourRankIs;
    public Text rankUI;
    public GameObject made;
    public Text nameUI;
    [SerializeField] List<SoloResultRank> ranks = new();
    public GameObject cursor;
    int arrow_max = 1;
    int arrow_min = 0;
    bool canInput = false;
    int displayCnt = 0;
    [SerializeField] Transform canvas;
    float sSpace = 1.0f;
    List<GameObject> rankObj = new();
    [SerializeField] SpriteRenderer skin;
    [SerializeField] Text yasaiName;
    [SerializeField] Text winText;

    void Start()
    {
        yourScoreIs.gameObject.SetActive(false);
        scoreUI.gameObject.SetActive(false);
        yourRankIs.gameObject.SetActive(false);
        made.gameObject.SetActive(false);
        cursor.SetActive(false);
        StartCoroutine(Result());
        skin.sprite = SoloBattleResult.img;
        yasaiName.text = SoloBattleResult.name;
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
        yield return new WaitForSeconds(1.0f);
        yourScoreIs.gameObject.SetActive(true);

        // 「○○点！」
        yield return new WaitForSeconds(1.0f);
        scoreUI.gameObject.SetActive(true);
        scoreUI.text = $"{SoloBattleResult.score}";

        // 「あなたの野菜ランクは...」
        yield return new WaitForSeconds(1.0f);
        yourRankIs.gameObject.SetActive(true);

        // 「○○ランク！」
        for (int i = 0; i < ranks.Count; i++)
        {
            if (ranks[i].needScore < SoloBattleResult.score)
            {
                displayCnt = ranks[i].sCount;
            }
        }
        StartCoroutine(DisplayRank());
    }

    IEnumerator DisplayRank(int n = 0)
    {
        yield return new WaitForSeconds(0.5f);
        if (n++ < displayCnt)
        {
            rankObj.Add(Instantiate(rankUI).gameObject);
            GameObject go = rankObj[rankObj.Count - 1].gameObject;
            go.transform.SetParent(canvas, false);
            go.transform.position = new(transform.position.x + sSpace, transform.position.y);
            sSpace += 1.2f;
            StartCoroutine(DisplayRank(n));
        }
        else
        {

            // 製作者
            made.gameObject.SetActive(true);
            nameUI.text = SoloBattleResult.name;
            canInput = true;
            cursor.SetActive(true);
        }
    }

    public void SceneChange_CharacterPickScene(InputAction.CallbackContext ctx)
    {
        if (canInput)
        {
            if (ctx.performed && TextArrow.instance.arrow_pos == 0)
            {
                SceneManager.LoadScene(SceneName.CHARACTER_PICK_PVE);
                Winner.sprite = null;
            }
            else if (ctx.performed && TextArrow.instance.arrow_pos == 1)
            {
                Debug.Log("titleに行く、よ");
                SceneManager.LoadScene(SceneName.TITLE);
            }
        }
    }


    public void Up(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            TextArrow.instance.arrow_pos++;
            if (TextArrow.instance.arrow_pos > arrow_max)
                TextArrow.instance.arrow_pos = arrow_min;
        }
    }
    public void Down(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            TextArrow.instance.arrow_pos--;
            if (TextArrow.instance.arrow_pos < arrow_min)
                TextArrow.instance.arrow_pos = arrow_max;
        }
    }
}

[Serializable]
public class SoloResultRank
{
    public int needScore = 0;
    public int sCount = 0;
}