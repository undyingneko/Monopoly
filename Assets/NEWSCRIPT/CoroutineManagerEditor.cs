using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(CoroutineManager))]
public class CoroutineManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CoroutineManager coroutineManager = (CoroutineManager)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Active Coroutines", EditorStyles.boldLabel);

        foreach (var coroutine in coroutineManager.activeCoroutines)
        {
            EditorGUILayout.LabelField(coroutine.Key);
        }
    }
}
