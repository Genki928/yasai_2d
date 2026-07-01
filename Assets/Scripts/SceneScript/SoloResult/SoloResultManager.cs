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
    public Text yourScoreIs;
    public Text scoreUI;
    public Text yourRankIs;
    public Text rankUI;
    public GameObject made;
    public Text nameUI;
    public GameObject cursor;
    [SerializeField] List<int> ranks = new();
    [SerializeField] int arrow_max = 1;
    [SerializeField] int arrow_min = 0;
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
        //for (int i = 0; i < ranks.Count; i++)
        //{
        //    if (SoloBattleResult.socre < ranks[i].score)
        //    {
        //        rankUI.text = ranks[1].rank;
        //        break;
        //    }
        ////}
        rankUI.gameObject.SetActive(true);

        // 製作者
        yield return new WaitForSeconds(1.0f);
        made.gameObject.SetActive(true);
        nameUI.text = SoloBattleResult.name;
        canInput = true;
        cursor.SetActive(true);
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