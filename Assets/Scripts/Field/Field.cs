using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Field.Cell;
using UnityEngine;

public class Field : MonoBehaviour
{
    public static Field Instance;
    
    public Cell[,] Cells;
    private List<CharacterPart> CharacterParts;

    void LoadField()
    {
        //TODO: read field + initialize field and character
    }
    
    //TODO: Join all CharacterParts to each other at start like how it implement in Character.cs in TryAttachCells

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
