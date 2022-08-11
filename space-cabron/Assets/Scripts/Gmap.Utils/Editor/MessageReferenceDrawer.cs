using System;
using System.Linq;
using System.Reflection;
using Gmap.Utils;
using UnityEditor;
using UnityEngine;

public class MessageReferenceDrawer<T> : PropertyDrawer
{
    private int _selectedTypeIndex = 0;
    private Type[] types = MessageReference<T>.GetMessageTypes();

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        Type type = GetTargetType(_selectedTypeIndex);
        int numberOfPropertiesInType = GetNumberOfProperties(type);
        return (1+numberOfPropertiesInType) * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // base.OnGUI(position, property, label);
        // EditorGUI.BeginProperty(position, label, property);
        EditorGUI.BeginProperty(position, label, property);

        string[] typeNames = types.Select(x=>x.FullName).ToArray();

        // shitty code ahead
        SerializedProperty propertyMessageToRaise = property.FindPropertyRelative("messageToRaise"); 
        string messageToRaise = propertyMessageToRaise.stringValue;
        if (string.IsNullOrEmpty(messageToRaise))
            _selectedTypeIndex = 0;
        else
            _selectedTypeIndex = Array.IndexOf(typeNames, messageToRaise);
        
        Rect popupSize = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        _selectedTypeIndex = EditorGUI.Popup(
            popupSize,
            _selectedTypeIndex, types.Select(x=>x.Name).ToArray()
        );
        propertyMessageToRaise.stringValue = types[_selectedTypeIndex].FullName;

        // List Parameters
        Type selectedType = types[_selectedTypeIndex];
        FieldInfo[] fields = selectedType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo field = fields[i];

            var pos = new Rect(
                position.x, 
                position.y + (i+1) * EditorGUIUtility.singleLineHeight, 
                position.width/2f, 
                EditorGUIUtility.singleLineHeight
            );

            var posField = pos;
            posField.x += pos.width;
            EditorGUI.LabelField(pos, $"{field.Name} ({field.FieldType.Name})");

            if (!SimpleSerializer.IsSupported(field.FieldType))
                continue;
            DrawField(property, field, i, posField);
        }

        EditorGUI.EndProperty();
    }

    private int GetNumberOfProperties(Type type)
    {
        return type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Length;
    }

    private Type GetTargetType(int selectedTypeIndex)
    {
        return types[selectedTypeIndex];
    }

    private Rect GetPropertyDrawerRect(Rect pos, int numberOfPropertiesInType)
    {
        Rect r = pos;
        r.height += 100+numberOfPropertiesInType * EditorGUIUtility.singleLineHeight;
        return r;
    }
    
    private void DrawField(SerializedProperty property, FieldInfo field, int i, Rect rect)
    {
        SerializedProperty serializedParametersArray = InitializeArrayProperty(property, "serializedParameters");
        SerializedProperty parameterNames = InitializeArrayProperty(property, "parameterNames");
        SerializedProperty parameterGameObjects = InitializeArrayProperty(property, "parameterGameObjects");

        string fieldName = field.Name;
        Type fieldType = field.FieldType;

        int parameterNameIndex = FindParameterNameAtArray(parameterNames, fieldName);
        if (parameterNameIndex == -1)
            parameterNames.GetArrayElementAtIndex(i).stringValue = fieldName;

        SerializedProperty serializedParameter  = serializedParametersArray.GetArrayElementAtIndex(i);
        if (string.IsNullOrEmpty(serializedParameter.stringValue))
            serializedParameter.stringValue = SimpleSerializer.SerializeDefault(fieldType);

        object deserializedParameter = SimpleSerializer.Deserialize(serializedParameter.stringValue);

        // If the deserialized parameter is null, it was probably being used as another type's
        // reference and we don't need it anymore, since the type changed.
        if (deserializedParameter == null || deserializedParameter.GetType() != fieldType)
            deserializedParameter = SimpleSerializer.Default(fieldType);


        string serializedNewValue = null;
        if (fieldType == typeof(string))
            serializedNewValue = DrawSerializedProperty<string>(rect, serializedParameter, deserializedParameter, EditorGUI.TextField);
        else if (fieldType == typeof(int))
            serializedNewValue = DrawSerializedProperty<int>(rect, serializedParameter, deserializedParameter, EditorGUI.IntField);
        else if (fieldType == typeof(float))
            serializedNewValue = DrawSerializedProperty<float>(rect, serializedParameter, deserializedParameter, EditorGUI.FloatField);
        else if (fieldType == typeof(Vector3))
            serializedNewValue = DrawSerializedProperty<Vector3>(rect, serializedParameter, deserializedParameter, (r, v) => EditorGUI.Vector3Field(r, "", v));
        else if (fieldType == typeof(bool))
            serializedNewValue = DrawSerializedProperty<bool>(rect, serializedParameter, deserializedParameter, EditorGUI.Toggle);
        else if (fieldType == typeof(GameObject))
        {
            serializedNewValue = DrawSerializedProperty<UnityEngine.Object>(
                rect, serializedParameter, deserializedParameter, 
                (r,v) => { 
                    EditorGUI.ObjectField(r, parameterGameObjects.GetArrayElementAtIndex(i), GUIContent.none);
                    return null;
            });
        }

        serializedParameter.stringValue = serializedNewValue;
    }

    private string DrawSerializedProperty<ParameterType>(
        Rect rect, 
        SerializedProperty serializedParameter,
        object deserializedParameter, 
        System.Func<Rect, ParameterType, ParameterType> drawMethod
    ) {
        ParameterType value = (ParameterType)deserializedParameter;
        return SimpleSerializer.Serialize(drawMethod(rect, value));
    }

    private int FindParameterNameAtArray(SerializedProperty parameterNames, string name)
    {
        for (int i = 0; i < parameterNames.arraySize; i++)
        {
            if (parameterNames.GetArrayElementAtIndex(i).stringValue == name)
                return i;
        }
        return -1;
    }

    private static SerializedProperty InitializeArrayProperty(
        SerializedProperty property, 
        string name, 
        int defaultSize=10
    ) {
        SerializedProperty array = property.FindPropertyRelative(name);
        if (!array.isArray)
            return null;
        if (array.arraySize == 0)
            array.arraySize = defaultSize;
        return array;
    }
}
