using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RequiredAttribute))]
public class RequiredAttributePropertyDrawer : PropertyDrawer
{
    readonly private Color _errorColor = new Color(1.0f, 0.2f, 0.2f, 0.1f);
    readonly private float _offsetAmount = 2.0f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if(IsFieldEmpty(property))
        {
            float height = EditorGUIUtility.singleLineHeight * _offsetAmount;
            height += base.GetPropertyHeight(property, label);

            return height;
        }

        return base.GetPropertyHeight(property, label);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!IsFieldSupported(property))
        {
            Debug.LogError("Required Attribute placed on incompatible field type.");
            return;
        }

        if(IsFieldEmpty(property))
        {
            float doubleLineHeight = EditorGUIUtility.singleLineHeight * _offsetAmount;
            float propertyHeight = base.GetPropertyHeight(property, label);

            position.height = doubleLineHeight;
            position.height += propertyHeight;

            EditorGUI.HelpBox(position, "Required", MessageType.Error);
            EditorGUI.DrawRect(position, _errorColor);

            position.height = propertyHeight;
            position.y += doubleLineHeight;
        }

        EditorGUI.PropertyField(position, property, label);
    }

    private bool IsFieldSupported(SerializedProperty property)
    {
        if (property.propertyType == SerializedPropertyType.ObjectReference
            || property.propertyType == SerializedPropertyType.String)
        {
            return true;
        }

        return false;
    }

    private bool IsFieldEmpty(SerializedProperty property)
    {
        if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null)
        {
            return true;
        }

        if (property.propertyType == SerializedPropertyType.String && string.IsNullOrEmpty(property.stringValue))
        {
            return true;
        }

        return false;
    }
}
