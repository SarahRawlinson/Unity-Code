using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class ResetSerializedFieldInEditor : MonoBehaviour
{
    [MenuItem("Tools/MyTool/Do It in C#")]
    static void DoIt()
    {
        EditorUtility.DisplayDialog("MyTool", "Do It in C# !", "OK", "");
    }
}