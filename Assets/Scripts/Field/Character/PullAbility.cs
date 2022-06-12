using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullAbility : Ability
{
    public DirectionType PullDirection;
    private Field _field;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Apply()
    {
        if (_characterPart.HasPartInDirection(PullDirection))
        {
            //TryToAttach
        }
        else
        {
            //TryToDettach
        }
    }
    
    //TryToAttach: Is there a characterPart on the field in this direction within 4 cells? If not, then nothing can be attached.
    //TryToDetach: Can dettachable part move front? If not, then can main part move back? If not, then it cant be dettached
}
