using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    [SerializeField]
    private GameObject trianglePrefab;

    private List<GameObject> triangles = new();

    void Start()
    {
        float offset = 3f;

        for (int i = 0; i < 3; i++)
        {
            GameObject tri = Instantiate(
                trianglePrefab,
                transform);

            tri.transform.localPosition =
                Vector3.right * offset * i;

            triangles.Add(tri);
        }

        // F‚ðÝ’è
        triangles[0].GetComponent<SpriteRenderer>().color =
            Color.red;

        triangles[1].GetComponent<SpriteRenderer>().color =
            Color.yellow;

        triangles[2].GetComponent<SpriteRenderer>().color =
            Color.green;
    }

    void Update()
    {
        Vector3 move =
            Vector3.left * 10f * Time.deltaTime;

        foreach (GameObject tri in triangles)
        {
            tri.transform.position += move;
        }
    }
}