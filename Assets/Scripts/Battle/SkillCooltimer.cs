using UnityEngine;
using UnityEngine.UI;

public class SkillCooltimer : MonoBehaviour
{
    Image image;
    public CharBase character;

    void Start()
    {
        image = GetComponent<Image>();
    }

    void RefreshCooltimer(int cooltimer, int cooltime)
    {
        image.fillAmount = 1 - cooltimer / (float)cooltime;
    }
}
