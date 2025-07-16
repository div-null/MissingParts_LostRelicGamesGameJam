using System.Linq;
using Game.Character;
using Game.Level;
using UnityEngine;

namespace Game.Systems
{
    public class MoveSystem
    {
        private readonly Field _field;

        public MoveSystem(Field field)
        {
            _field = field;
        }

        public bool CanMove(CharacterPart graph, DirectionType direction)
        {
            var vector2Int = direction.ToVector2Int();
            return CanMove(graph, vector2Int);
        }

        public bool CanMove(CharacterPart graph, Vector2Int deltaPosition)
        {
            return graph.All(part => !HasWallIn(part.Position + deltaPosition));
        }

        public bool Move(CharacterPart graph, DirectionType direction)
        {
            if (!CanMove(graph, direction)) return false;

            foreach (CharacterPart part in graph) 
                MovePart(part, direction);

            Debug.Log("Parts moved");
            return true;
        }

        public void MovePart(CharacterPart characterPart, DirectionType direction)
        {
            var destination = characterPart.Position + direction.ToVector2Int();
            MovePart(characterPart, destination);
        }

        public void MovePart(CharacterPart characterPart, Vector2Int destination) =>
            SetPosition(characterPart, destination);

        public bool CanPartMove(CharacterPart characterPart, DirectionType direction)
        {
            Vector2Int destination = characterPart.Position + direction.ToVector2Int();
            return !HasWallIn(destination);
        }

        private void SetPosition(CharacterPart part, Vector2Int destination)
        {
            _field.Replace(part, destination);
            part.Position = destination;
        }

        private bool HasWallIn(Vector2Int position)
        {
            if (_field.TryGet(position, out Cell cell))
                return cell.IsWall();

            return false;
        }
    }
}