using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

[CustomEditor(typeof(EvaluationProceadure))]
public class EnumsAsButtonGroups : Editor
{
    #region Public Variables

    public int EnumButtonsPerRow = 1; // How many buttons per row in each Enum button group

    #endregion

    #region Private Variables

    private Dictionary<string, bool> foldouts = new Dictionary<string, bool>();
    private Dictionary<string, Dictionary<int, int>> enumValueToIndex = new Dictionary<string, Dictionary<int, int>>();
    private Dictionary<string, Dictionary<int, int>> enumIndexToValue = new Dictionary<string, Dictionary<int, int>>();
    private Dictionary<string, string[]> enumNames = new Dictionary<string, string[]>();

    #endregion

    #region Private Methods
    /// <summary>
    /// Draws an ENUM field as a GridSelection (Grid of buttons).
    /// </summary>
    /// <param name="field">The ENUM field in the original class.</param>
    /// <param name="buttonsPerRow">How many buttons per row of the grid. (default = 1)</param>
    private void DrawEnumAsButtonGroup(FieldInfo field, int buttonsPerRow = 1)
    {
        if (!field.FieldType.IsEnum) return;

        object value = field.GetValue(target);
        object underlyingValue = Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()));

        bool initialized = enumValueToIndex.ContainsKey(field.Name) &&
                           enumIndexToValue.ContainsKey(field.Name) &&
                           enumNames.ContainsKey(field.Name);

        if (!initialized)
        {
            System.Array values = Enum.GetValues(field.FieldType);
            enumValueToIndex.Add(field.Name, new Dictionary<int, int>());
            enumIndexToValue.Add(field.Name, new Dictionary<int, int>());
            enumNames[field.Name] = new string[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                int l = (int)Convert.ChangeType(values.GetValue(i), Enum.GetUnderlyingType(value.GetType()));
                enumValueToIndex[field.Name].Add(l, i);
                enumIndexToValue[field.Name].Add(i, l);
                enumNames[field.Name][i] = values.GetValue(i).ToString();
            }

            initialized = true;
        }

        // Init the foldout
        if (!foldouts.ContainsKey(field.Name)) foldouts.Add(field.Name, true);
        foldouts[field.Name] = EditorGUILayout.Foldout(foldouts[field.Name], 
                                                       field.Name + " (" + value.ToString() + ")", 
                                                       true);
        // Show contents if foldout is open
        if (foldouts[field.Name])
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            bool validIndex = enumValueToIndex[field.Name].ContainsKey((int)underlyingValue);
            int index = GUILayout.SelectionGrid(validIndex ? enumValueToIndex[field.Name][(int)underlyingValue] : 0,
                                                enumNames[field.Name], buttonsPerRow);
            Debug.Log(index);
            target.GetType()
                    .GetField(field.Name)
                    .SetValue(target, enumIndexToValue[field.Name][index]);
            EditorGUILayout.EndVertical();
        }
    }
    #endregion

    #region Unity Methods
    public override void OnInspectorGUI()
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
        FieldInfo[] fields = target.GetType().GetFields(flags);
        foreach (FieldInfo fieldInfo in fields)
        {
            if (fieldInfo.FieldType.IsEnum) DrawEnumAsButtonGroup(fieldInfo, EnumButtonsPerRow);
        }
        DrawDefaultInspector();
    }
    #endregion
}