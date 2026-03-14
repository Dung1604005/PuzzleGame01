using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyConfig", menuName = "Scriptable Objects/DifficultyConfig")]
public class DifficultyConfig : ScriptableObject
{
    [SerializeField]private BagComposition level1 = new BagComposition();

    public BagComposition Level1 => level1;
    [SerializeField]private BagComposition level2 = new BagComposition();

    public BagComposition Level2 => level2;
    [SerializeField]private BagComposition level3 = new BagComposition();

    public BagComposition Level3 => level3;
}
[System.Serializable]
public class BagComposition
{
    
    public int easyCount = 1;
    public int mediumCount = 1;
    public int hardCount = 1;
}