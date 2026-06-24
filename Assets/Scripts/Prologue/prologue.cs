using Const;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class prologue : MonoBehaviour
{
    public bool isLoad = false;
    public float speed = 50f; // 1•b‚ ‚½‚è‚ÌˆÚ“®—Ê

    [SerializeField] float endY = 4300f;

    RectTransform rect;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        rect.anchoredPosition += Vector2.up * speed * Time.deltaTime;

        if (rect.anchoredPosition.y >= endY)
        {
            Winner.sprite = null;
            SceneManager.LoadScene(SceneName.TITLE);
        }
    }
    public void SceneChange_Titele(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene(SceneName.TITLE);
        Winner.sprite = null;
    }
}