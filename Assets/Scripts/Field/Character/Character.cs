using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Field.Cell;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

public class Character : MonoBehaviour
{
    private CharacterPart _mainPart;
    private ColorType _characterColor;
    private Field _field;

    public event Action Moved;
    public event Action Died;

    public void Initialize(Field field)
    {
        _field = field;
    }
    
    public void AddParts(List<CharacterPart> parts)
    {
        // Взяли первый элемент
        _mainPart = parts.First();
    }
    
    public void DestroyCharacter()
    {
        _mainPart = null;
        //may be something else with events
    }

    public void Move(Vector2 vectorDirection)
    {
        _mainPart.CharacterPartMovement.Move(vectorDirection.ToDirection());
        _mainPart.CharacterPartAttachment.AttachParts();
        Moved?.Invoke();
        DetachPartsInPits();
    }

    //!!!May be its wrong because there is can be a situation when the max chain will be drop in the pits and the less chain will survive!!!
    //And because of this maybe it's better to first find out about all the falls into the pit and then unhook the necessary blocks?
    public void DetachPartsInPits()
    {
        //Find all parts in pits
        //Remove their links with other parts
        //Create lists for remaining parts and deleting parts
        List<CharacterPart> deletingParts;
        List<CharacterPart> remainingParts = SeparateCharacterPartsToRemainingAndDeleting(out deletingParts);
        
        //delete deleting parts
        foreach (var part in deletingParts)
        {
            part.Delete();
        }
        
        if (remainingParts.Count == 0)
        {
            Died?.Invoke();
            Debug.Log("Game Over!");
            return;
        }

        //Unite them in groups and remove united parts from list
        List<CharacterPart> unitedParts = UniteCharacterPartsInGroups(remainingParts);


        //Choose max size chain as main character
        int maxSize = MaxSizeOfDifferentChainsOfCharacterParts(unitedParts, out int index);
        if (maxSize > 0)
            _mainPart = unitedParts[index];
        else
            return;


        //Other chains will turn inactive
        if (unitedParts.Count > 1)
        {
            for (int i = 0; i < unitedParts.Count; i++)
            {
                if (i != index)
                    unitedParts[i].SetActiveToAllParts(false);
            }
        }
    }

    private List<CharacterPart> SeparateCharacterPartsToRemainingAndDeleting(out List<CharacterPart> deletingParts)
    {
        HashSet<CharacterPart> visited = new HashSet<CharacterPart>();
        List<CharacterPart> remaining = new List<CharacterPart>();
        List<CharacterPart> deleting = new List<CharacterPart>();

        void separateRemainingNodesFromDeleting(CharacterPart part)
        {
            if (part == null) return;
            if (visited.Contains(part)) return;
            visited.Add(part);

            separateRemainingNodesFromDeleting(part.Down);
            separateRemainingNodesFromDeleting(part.Up);
            separateRemainingNodesFromDeleting(part.Right);
            separateRemainingNodesFromDeleting(part.Left);

            Cell cell = _field.Get(part.Position);
            if (cell.IsPit())
            {
                deleting.Add(part);
                part.RemoveLinks();
            }
            else
                remaining.Add(part);
        }

        separateRemainingNodesFromDeleting(_mainPart);
        deletingParts = deleting;
        return remaining;
    }

    private static List<CharacterPart> UniteCharacterPartsInGroups(List<CharacterPart> remaining)
    {
        HashSet<CharacterPart> visited = new HashSet<CharacterPart>();
        List<CharacterPart> unitedParts = new List<CharacterPart>();
        foreach (var part in remaining)
        {
            if (!visited.Contains(part))
                unitedParts.Add(part);
            
            void visitNodes(CharacterPart part)
            {
                if (part == null) return;
                if (visited.Contains(part)) return;
                visited.Add(part);

                visitNodes(part.Down);
                visitNodes(part.Up);
                visitNodes(part.Right);
                visitNodes(part.Left);
            }

            visitNodes(part);
        }

        return unitedParts;
    }

    private int MaxSizeOfDifferentChainsOfCharacterParts(List<CharacterPart> unitedParts, out int index)
    {
        int maxSize = -1;
        index = -1;
        for (int i = 0; i < unitedParts.Count; i++)
        {
            int currentSize = SizeOfCharacterPartsChain(unitedParts[i]);
            if (currentSize > maxSize)
            {
                maxSize = currentSize;
                index = i;
            }
        }

        return maxSize;
    }

    //FindMax
    //ChangeMainPart
    //Get cell from field for logic
   
    public int SizeOfCharacterPartsChain(CharacterPart characterPart)
    {
        HashSet<CharacterPart> visited = new HashSet<CharacterPart>();

        void visitNode(CharacterPart part)
        {
            if (part == null) return;
            if (visited.Contains(part)) return;
            visited.Add(part);

            visitNode(part.Down);
            visitNode(part.Up);
            visitNode(part.Right);
            visitNode(part.Left);
        }

        visitNode(characterPart);
        return visited.Count;
    }
}