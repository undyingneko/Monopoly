// using UnityEditor;
// using UnityEngine;

// [CustomEditor(typeof(PropertyManager))]
// public class PropertyManagerEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         serializedObject.Update();

//         PropertyManager propertyManager = (PropertyManager)target;

//         // Display properties list
//         for (int i = 0; i < propertyManager.properties.Count; i++)
//         {
//             EditorGUILayout.BeginVertical(EditorStyles.helpBox);

//             EditorGUILayout.BeginHorizontal();
//             EditorGUILayout.LabelField("Index: " + propertyManager.properties[i].JSONwaypointIndex.ToString(), GUILayout.Width(70));
//             EditorGUILayout.LabelField("Name: " + propertyManager.properties[i].name);
//             EditorGUILayout.EndHorizontal();

//             EditorGUILayout.LabelField("Price Stall Base: " + propertyManager.properties[i].priceStallBase.ToString());

//             // Make Stage Prices list expandable
//             EditorGUILayout.PropertyField(serializedObject.FindProperty("properties").GetArrayElementAtIndex(i).FindPropertyRelative("stagePrices"), true);

//             // Make Rent Prices list expandable
//             EditorGUILayout.PropertyField(serializedObject.FindProperty("properties").GetArrayElementAtIndex(i).FindPropertyRelative("rentPrices"), true);

//             // Make Buyout Prices list expandable
//             EditorGUILayout.PropertyField(serializedObject.FindProperty("properties").GetArrayElementAtIndex(i).FindPropertyRelative("buyoutPrices"), true);

//             // Make Stage Indexes list expandable
//             EditorGUILayout.PropertyField(serializedObject.FindProperty("properties").GetArrayElementAtIndex(i).FindPropertyRelative("stageIndexes"), true);

//             EditorGUILayout.LabelField("Buyout Multiplier: " + propertyManager.properties[i].buyoutMultiplier.ToString());
//             EditorGUILayout.LabelField("Buyout Count: " + propertyManager.properties[i].buyoutCount.ToString());

//             // Display more data as needed
//             EditorGUILayout.LabelField("Owned: " + propertyManager.properties[i].owned.ToString());
//             EditorGUILayout.LabelField("Owner ID: " + propertyManager.properties[i].ownerID.ToString());
//             EditorGUILayout.LabelField("Team Owner ID: " + propertyManager.properties[i].teamownerID.ToString());

//             EditorGUILayout.EndVertical();
//         }

//         serializedObject.ApplyModifiedProperties();
//     }
// }
