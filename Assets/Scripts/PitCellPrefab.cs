using UnityEngine;

[CreateAssetMenu(fileName = "PitSprites", menuName = "Create pit Sprites", order = 0)]
public class PitCellPrefab : ScriptableObject
{
    public GameObject InnerCorner;
    public GameObject OuterCorner;
    public GameObject Vertical;
}