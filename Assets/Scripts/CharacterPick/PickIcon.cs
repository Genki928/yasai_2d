using UnityEngine;

public class PickIcon : MonoBehaviour
{
    public SpriteRenderer icon;

    public void SetIcon(Sprite img)
    {
        icon.sprite = img;
    }
}
