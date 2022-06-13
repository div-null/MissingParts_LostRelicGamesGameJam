using VContainer.Unity;

namespace Infrastructure.Scope
{
    public class LevelEntryPoint : IStartable
    {
        private Field _field;
        private Character _character;

        public LevelEntryPoint(Field field, Character character)
        {
            _character = character;
            _field = field;
        }

        public void Start()
        {
        }
    }
}