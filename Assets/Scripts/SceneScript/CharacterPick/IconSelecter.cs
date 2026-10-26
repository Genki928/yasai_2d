using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class IconSelecter : MonoBehaviour
{
    public List<IconSelecter> Icons => icons;
    public SelectedData CharacterData => data;
    [SerializeField] List<IconSelecter> icons = new();
    [SerializeField] SelectedData data;

    void Start()
    {
        
    }

    void Update()
    {

    }

}

[Serializable]
public class SelectedData
{
    public string Name => name;
    public PickData Lore => lore;
    public Vector2 Size => size;
    public Sprite Sprite => sprite;
    public Sprite InteractedSprite => interactedSprite;
    public CharId Id => id;
    [SerializeField] string name;
    [SerializeField] PickData lore;
    [SerializeField] CharId id;
    [SerializeField] Vector2 size;
    [SerializeField] Sprite sprite;
    [SerializeField] Sprite interactedSprite;
}