using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField] List<GameObject> _rocks = new();
    int _rockCount = 4;

    void Start()
    {
        // –³Œø‰»
        foreach (var rock in _rocks)
            rock.SetActive(false);

        // n‰ñ”—LŒø‰»
        for (int i = 0; i < _rockCount; i++)
        {
            int n = Random.Range(0, _rocks.Count);
            _rocks[n].SetActive(true);
        }
    }
}
