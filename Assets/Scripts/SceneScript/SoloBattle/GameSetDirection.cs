using Const;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSetDirection : MonoBehaviour
{
    List<TargetBase> tbs;
    bool init = false;
    [SerializeField] Text wintext;
    AudioSource audioSource;
    [SerializeField] AudioClip se1;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        ;
    }

    public void Init(List<TargetBase> targets)
    {
        if (init) return;

        tbs = targets;
        init = true;
        for (int i = 0; i < tbs.Count; i++)
        {
            if (tbs[i] == null) continue;
            Rigidbody2D rb = tbs[i].GetComponent<Rigidbody2D>();
            tbs[i].escape = true;
            StartCoroutine(Direction());
        }
    }

    IEnumerator Direction()
    {
        // ââèo
        wintext.gameObject.SetActive(true);
        audioSource.PlayOneShot(se1);

        Color c = wintext.color;
        c.a = 0;
        wintext.color = c;

        wintext.transform.localScale = Vector3.one * 4f;

        float t = 0;

        while (t < 0.2f)
        {
            t += Time.deltaTime;

            float p = t / 0.2f;

            wintext.transform.localScale =
                Vector3.Lerp(Vector3.one * 4f, Vector3.one, p);

            c.a = p;
            wintext.color = c;

            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        t = 0;

        while (t < 0.3f)
        {
            t += Time.deltaTime;

            c.a = 1 - t / 0.3f;
            wintext.color = c;

            yield return null;
        }
        yield return new WaitForSeconds(2.0f);

        SceneManager.LoadScene(SceneName.RESULT_PVE);
    }

}
