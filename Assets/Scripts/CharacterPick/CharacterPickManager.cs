using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterPickManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            SceneManager.LoadScene("BattleScene");
    }

    public void CursorUP(InputAction.CallbackContext ctx)
    {
        ;
    }

    public void CursorDown(InputAction.CallbackContext ctx)
    {
        ;
    }

    public void CursorLeft(InputAction.CallbackContext ctx)
    {
        ;
    }

    public void CursorRight(InputAction.CallbackContext ctx)
    {
        ;
    }
}
