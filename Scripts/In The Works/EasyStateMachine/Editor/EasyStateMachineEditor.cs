using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR
[CustomEditor(typeof(EasyStateMachine))]
public class EasyStateMachineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var easyStateMachine = target as EasyStateMachine;

        EditorGUILayout.BeginHorizontal();
        {
            var option = GUILayout.MinWidth(30.0f);
            EditorGUILayout.LabelField("Graph Editor", option);

            if (DrawButton("Open", "Open Graph Editor", true, 60.0f))
            {
                easyStateMachine.ShowEditorWindow();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private static bool DrawButton(string title, string tooltip, bool enabled, float width)
    {
        if (enabled)
        {
            return GUILayout.Button(new GUIContent(title, tooltip), GUILayout.Width(width));
        }

        var color = GUI.color;
        GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.25f);
        GUILayout.Button(new GUIContent(title, tooltip), GUILayout.Width(width));
        GUI.color = color;
        return false;
    }
}



#endif
