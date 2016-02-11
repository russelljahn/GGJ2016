#if UNITY_EDITOR
using Sense.PropertyAttributes;
using UnityEditor;
using UnityEngine;


namespace Sense.Editor.PropertyAttributes
{
    [CustomPropertyDrawer(typeof(ReadonlyOnPlayAttribute))]
    public class ReadonlyOnPlayAttributeDrawer : PropertyDrawer 
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
        {
            GUI.enabled = !Application.isPlaying;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) 
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
#endif
