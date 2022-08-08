namespace LevelEditor.Editor
{
    // public class CellEditor : BindableElement
    // {
    //     public new class UxmlFactory : UxmlFactory<CellEditor>
    //     {
    //     }
    //
    //     private CellType _type;
    //     private VisualElement cell;
    //
    //     public CellEditor()
    //     {
    //         SetCell(CellType.Wall);
    //         cell = new VisualElement();
    //         cell.AddToClassList("cell");
    //         cell.AddToClassList(stateToClass());
    //     }
    //
    //     public void SetCell(CellType type)
    //     {
    //         _type = type;
    //         
    //         AddToClassList("cell");
    //         AddToClassList(stateToClass());
    //     }
    //
    //     string stateToClass()
    //     {
    //         return _type switch
    //         {
    //             CellType.Wall => "cell-wall",
    //             CellType.Floor => "cell-floor",
    //             CellType.Pit => "cell-pit",
    //             CellType.Finish => "cell-finish",
    //         };
    //     }
    // }
}