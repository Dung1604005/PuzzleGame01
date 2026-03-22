using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ThemeData", menuName = "Scriptable Objects/ThemeData")]
public class ThemeData : ScriptableObject
{
    public string ThemeName; // "Magic Forest", "Lava Cave"...
    public List<Sprite> BlockSprites;

    public Sprite GridSprite;
    public Color colorTheme;
    public AudioClip ThemeMusic;

    public Sprite GetRandomSprite()
    {
        int randomIndex = Random.Range(0, BlockSprites.Count);
        return BlockSprites[randomIndex];
    }
}