#if UNITY_EDITOR
using Sense.PropertyAttributes;
using UnityEditor;
using UnityEngine;


namespace Sense.Editor.PropertyAttributes
{
    [CustomPropertyDrawer(typeof(Clamp01Attribute))]
    public class Clamp01AttributeDrawer : PropertyDrawer 
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
        {
            var newValue = EditorGUI.Slider(position, property.name, property.floatValue, 0f, 1f);
            property.floatValue = Mathf.Clamp01(newValue);
        }
    }
}
#endif
