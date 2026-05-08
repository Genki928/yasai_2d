using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] List<GameObject> chars = new List<GameObject>();
    GameObject[] player = new GameObject[2];
    int[] pick_nums = { 1, 0 };

    void Awake()
    {
        CharBase.OnPlayerDies += Finish;
    }

    void Start()
    {
        // キャラクターを生成
        for (int i = 0; i < 2; i++) {
            player[i] = Instantiate(chars[pick_nums[i]]);
        }
    }

    /// <summary> バトルを終了させる </summary>
    /// <param name="id"> プレイヤーの識別id </param>
    void Finish(int id)
    {
        Debug.Log("Player " + id + " won!");
    }
}