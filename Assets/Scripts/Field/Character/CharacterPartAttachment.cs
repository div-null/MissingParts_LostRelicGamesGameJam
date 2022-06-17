using System.Collections.Generic;
using Assets.Scripts.Field.Cell;
using UnityEngine;

public class CharacterPartAttachment : MonoBehaviour
{
    private CharacterPart _characterPart;
    private Field _field;

    public void Initialize(CharacterPart characterPart, Field field)
    {
        _characterPart = characterPart;
        _field = field;
    }

    public void AttachParts()
    {
        HashSet<CharacterPart> visited = new HashSet<CharacterPart>();

        void visitNode(CharacterPart part)
        {
            if (part == null) return;
            if (visited.Contains(part)) return;
            visited.Add(part);
            
            visitNode(part.Left);
            visitNode(part.Right);
            visitNode(part.Up);
            visitNode(part.Down);

            if (part.IsLeaf())
                part.TryJoinAllDirections();
        }

        visitNode(_characterPart);
    }
    
    public void CheckForPits()
    {
        HashSet<CharacterPart> visited = new HashSet<CharacterPart>();

        void visitNode(CharacterPart part)
        {
            if (part == null) return;
            if (visited.Contains(part)) return;
            visited.Add(part);

            //if (HasPitInCell(part.Position))
            //    _characterPart.PutAway();

            visitNode(part.Left);
            visitNode(part.Right);
            visitNode(part.Up);
            visitNode(part.Down);
        }

        visitNode(_characterPart);
    }

    public CharacterPart DetachParts()
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
            return null;
        }

        //Unite them in groups and remove united parts from list
        List<CharacterPart> unitedParts = UniteCharacterPartsInGroups(remainingParts);


        //Choose max size chain as main character
        int maxSize = MaxSizeOfDifferentChainsOfCharacterParts(unitedParts, out int index);
            
        //Other chains will turn inactive
        if (unitedParts.Count > 1)
        {
            for (int i = 0; i < unitedParts.Count; i++)
            {
                if (i != index)
                {
                    unitedParts[i].SetActiveToAllParts(false);
                    unitedParts[i].CharacterPartAttachment.AttachParts();
                }
            }
        }
            
        if (maxSize > 0)
            return unitedParts[index];
        else
            return null;
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

        separateRemainingNodesFromDeleting(_characterPart);
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