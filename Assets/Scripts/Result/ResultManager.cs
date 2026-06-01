using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ResultManager : MonoBehaviour
{
    [SerializeField] int arrow_max=1;
    [SerializeField] int arrow_min = 0;

    [NonSerialized] public string winner = Winner.w_name;
    [NonSerialized] public int id = Winner.w_id;
    [SerializeField] GameObject winner_name;
    [NonSerialized] public bool canInput=false;


    // Start is called once before the first execution ofUpdate after the MonoBehaviour is created
    public IEnumerator Start() 
    {
        yield return new WaitForSeconds(1f);
        canInput = true;
    } 
    // Update is called once per frame
    void Update() 
    {
        winner_name.GetComponent<Text>().text = winner;
    } 
    public void SceneChange_CharacterPickScene(InputAction.CallbackContext ctx) 
    {
        if (canInput)
        {
            if (ctx.performed && TextArrow.instance.arrow_pos == 0)
            {
                Debug.Log(Winner.sprite);
                SceneManager.LoadScene("CharacterPickScene");
                Winner.sprite = null;
            }
            else if (ctx.performed && TextArrow.instance.arrow_pos == 1)
            {
                Debug.Log("title‚És‚­A‚æ");
                SceneManager.LoadScene("TitleScene");
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

