namespace LevelEditor.Editor
{
    // [CustomEditor(typeof(GameLevel))]
    // public class GameLevelEditor : UnityEditor.Editor
    // {
    //     public override VisualElement CreateInspectorGUI()
    //     {
    //         VisualElement inspector = new VisualElement();
    //
    //         inspector.Add(new Label("This is a custom inspector"));
    //
    //         var finishEnum = new EnumField("Finish Color", CellColor.Green);
    //         inspector.Add(finishEnum);
    //
    //         VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Level/Editor/Level_editor.uxml");
    //         VisualTreeAsset cellEditor = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Level/Editor/cell_editor.uxml");
    //
    //         visualTree.CloneTree(inspector);
    //         VisualElement mapContainer = inspector.Q("mapEditor");
    //
    //         var level = serializedObject.targetObject as GameLevel;
    //         if (level == null)
    //             return inspector;
    //
    //         for (int j = 0; j < level.MapHeight; j++)
    //         {
    //             for (int i = 0; i < level.MapWidth; i++)
    //             {
    //                 var editor = new CellEditor();
    //                 editor.SetCell(level.Cells[i][j]);
    //                 mapContainer.Add(editor);
    //             }
    //         }
    //
    //         return inspector;
    //     }
    // }
}