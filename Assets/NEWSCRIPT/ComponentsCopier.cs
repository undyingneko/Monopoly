#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ComponentsCopier
{
    static List<Component> lastCopiedComponents; // Added this line

    [MenuItem("GameObject/Copy all components %&C")]
    static void Copy()
    {
        if (UnityEditor.Selection.activeGameObject == null)
            return;

        var allComponents = UnityEditor.Selection.activeGameObject.GetComponents<Component>();

        // Filter out Transform and SpriteRenderer components
        lastCopiedComponents = allComponents.Where(comp => !(comp is Transform || comp is SpriteRenderer)).ToList();
    }

    [MenuItem("GameObject/Paste all components %&P")]
    static void Paste()
    {
        if (lastCopiedComponents == null)
        {
            Debug.LogError("Nothing is copied!");
            return;
        }

        foreach (var targetGameObject in UnityEditor.Selection.gameObjects)
        {
            if (!targetGameObject)
                continue;

            Undo.RegisterCompleteObjectUndo(targetGameObject, targetGameObject.name + ": Paste All Components");

            foreach (var copiedComponent in lastCopiedComponents) // Modified this line
            {
                if (!copiedComponent)
                    continue;

                UnityEditorInternal.ComponentUtility.CopyComponent(copiedComponent);

                var targetComponent = targetGameObject.GetComponent(copiedComponent.GetType());

                if (targetComponent)
                {
                    if (UnityEditorInternal.ComponentUtility.PasteComponentValues(targetComponent))
                    {
                        Debug.Log("Successfully pasted: " + copiedComponent.GetType());
                    }
                    else
                    {
                        Debug.LogError("Failed to copy: " + copiedComponent.GetType());
                    }
                }
                else
                {
                    if (UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetGameObject))
                    {
                        Debug.Log("Successfully pasted: " + copiedComponent.GetType());
                    }
                    else
                    {
                        Debug.LogError("Failed to copy: " + copiedComponent.GetType());
                    }
                }
            }
        }
    }
}
#endif
