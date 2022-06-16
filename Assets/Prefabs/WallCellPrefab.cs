using UnityEngine;

[CreateAssetMenu(fileName = "WallSprites", menuName = "Create wallSprites", order = 0)]
public class WallCellPrefab : ScriptableObject
{
    public GameObject InnerCorner;
    public GameObject OuterCorner;
    public GameObject VerticalWall;
}