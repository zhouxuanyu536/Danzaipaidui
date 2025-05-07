using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
[CustomEditor(typeof(TrailElevatorAutoMove))]
public class TrailElevatorAutoMoveEditor: Editor
{
    SerializedProperty PrimaryMoveType;
    SerializedProperty ChangedPrimaryMoveType;
    SerializedProperty ChangeActionProperty;

    private TrailElevatorAutoMove autoMove;
    private void OnEnable()
    {
        autoMove = target as TrailElevatorAutoMove;
        PrimaryMoveType = serializedObject.FindProperty("primaryMoveType");
        ChangedPrimaryMoveType = serializedObject.FindProperty("ChangedPrimaryMoveType");
        ChangeActionProperty = serializedObject.FindProperty("ChangeAction");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((TrailElevatorAutoMove)target), typeof(MonoScript), false);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.PropertyField(PrimaryMoveType);
        if (autoMove.primaryMoveType == MoveType.NoMove)
        {
            EditorGUI.indentLevel++; //增加缩进
            //ChangedPrimaryMoveType
            MoveType currentType = (MoveType)ChangedPrimaryMoveType.enumValueIndex;
            string[] moveTypeNames = Enum.GetNames(typeof(MoveType));
            var filteredNames = new List<string>(moveTypeNames);
            filteredNames.Remove("NoMove");
            int currentIndex = Mathf.Max(0, Array.IndexOf(moveTypeNames, currentType.ToString()));
            int newIndex = EditorGUILayout.Popup("Changed Primary MoveType", currentIndex, filteredNames.ToArray());
            if (newIndex >= 0 && newIndex < filteredNames.Count)
            {
                ChangedPrimaryMoveType.enumValueIndex = (int)Enum.Parse(typeof(MoveType), filteredNames[newIndex]);
            }

            //ChangedAction
            int ChangeActionIndex = -1;
            int newActionIndex = 0;
            var ActionNames = new List<string>() { "OnPlayerOnBoard" };
            EditorGUILayout.Popup("ChangeAction", newActionIndex, ActionNames.ToArray());
            if (ChangeActionIndex != newActionIndex)
            {
                ChangeActionIndex = newActionIndex;
                ChangeActionProperty.stringValue = ActionNames[ChangeActionIndex]; // 更新字符串变量
            }

            EditorGUI.indentLevel--; //减小缩进
        }

        EditorGUILayout.Space(); // 添加间距
        DrawPropertiesExcluding(serializedObject,"m_Script","primaryMoveType", "ChangedPrimaryMoveType","ChangeAction");
        serializedObject.ApplyModifiedProperties();

    }
}
