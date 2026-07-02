using Const;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
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

    void Start()
    {
        yourScoreIs.gameObject.SetActive(false);
        scoreUI.gameObject.SetActive(false);
        yourRankIs.gameObject.SetActive(false);
        rankUI.gameObject.SetActive(false);
        made.gameObject.SetActive(false);
        cursor.SetActive(false);
        StartCoroutine(Result());
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
        scoreUI.text = $"{SoloBattleResult.socre}";

        // 「あなたの野菜ランクは...」
        yield return new WaitForSeconds(1.0f);
        yourRankIs.gameObject.SetActive(true);

        // 「○○ランク！」
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(DisplayRank());

        // 製作者
        yield return new WaitForSeconds(1.0f);
        made.gameObject.SetActive(true);
        nameUI.text = SoloBattleResult.name;
        canInput = true;
        cursor.SetActive(true);
    }

    IEnumerator DisplayRank()
    {
        StartCoroutine(DisplayRank());
        yield return new WaitForSeconds(1.0f);
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