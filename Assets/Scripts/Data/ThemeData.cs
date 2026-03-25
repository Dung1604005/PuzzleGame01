using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ThemeData", menuName = "Scriptable Objects/ThemeData")]
public class ThemeData : ScriptableObject
{
    public string ThemeName; // "Magic Forest", "Lava Cave"...
    public List<Sprite> BlockSprites;

    public Sprite GridSprite;

    public Sprite buttonIcon;

    public Color colorGridBorder;
    public Color colorBackground;

    public Color comboLowEffectColor;

    public Color comboHighEffectColor;

    public Color scoreFloatingText;

    public Color comboFloatingText;
    public AudioClip ThemeMusic;

    public Sprite GetRandomSprite()
    {
        int randomIndex = Random.Range(0, BlockSprites.Count);
        return BlockSprites[randomIndex];
    }
}