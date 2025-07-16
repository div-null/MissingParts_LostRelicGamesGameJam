namespace Game.Level
{
    public struct CellData
    {
        public readonly DirectionType Borders;
        public readonly CellType Type;

        public CellData(CellType type, DirectionType borders)
        {
            Type = type;
            Borders = borders;
        }
    }
}