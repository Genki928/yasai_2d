using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class ResultManager : MonoBehaviour
{
    [SerializeField] int arrow_max=1;
    [SerializeField] int arrow_min = 0;



    // Start is called once before the first execution ofUpdate after the MonoBehaviour is created
    void Start() 
    { 

    } 
    // Update is called once per frame
    void Update() 
    {

    } 
    public void SceneChange_CharacterPickScene(InputAction.CallbackContext ctx) 
    {
        if (ctx.performed && TextArrow.instance.arrow_pos == 0) SceneManager.LoadScene("CharacterPickScene");
        else if (ctx.performed && TextArrow.instance.arrow_pos == 1)
        {
            Debug.Log("title‚És‚­A‚æ");
            SceneManager.LoadScene("TitleScene");
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