// Assets/02_Scripts/Data/ItemStorageSO.cs
using UnityEngine;

[CreateAssetMenu(menuName = "MyGame/ItemStorage")]
public class ItemStorageSO : ScriptableObject
{
    public GameObject lotsPrefab;
    public GameObject stackMapLayerPrefab;
    public GameObject stackMapPrefab;
    public GameObject noInkMapPrefab;
    public GameObject gradeColorPrefab;
}
