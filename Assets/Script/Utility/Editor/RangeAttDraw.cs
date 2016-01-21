using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RangeAtt))]
public class RangeAttDraw : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        RangeAtt range = (RangeAtt)base.attribute;
        property.intValue = Mathf.Clamp(EditorGUI.IntField(position, label.text, property.intValue), range.min, range.max);
    }
}
