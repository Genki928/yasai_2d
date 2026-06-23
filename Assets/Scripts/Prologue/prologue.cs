using Const;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class prologue : MonoBehaviour
{
    public float speed = 50f; // 1•b‚ ‚½‚è‚ÌˆÚ“®—Ê

    RectTransform rect;

    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        rect.anchoredPosition += Vector2.up * speed * Time.deltaTime;
    }
    public void SceneChange_Titele(InputAction.CallbackContext ctx)
    {
                SceneManager.LoadScene(SceneName.TITLE);
                Winner.sprite = null;
     }
}