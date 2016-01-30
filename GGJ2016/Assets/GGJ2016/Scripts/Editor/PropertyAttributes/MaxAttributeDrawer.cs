#if UNITY_EDITOR
using Sense.Extensions;
using Sense.PropertyAttributes;
using UnityEditor;
using UnityEngine;

namespace Sense.Editor.PropertyAttributes
{
    [CustomPropertyDrawer(typeof (MaxAttribute))]
    public class MaxAttributeDrawer : PropertyDrawer
    {
        private MaxAttribute _attributeValue;
        private MaxAttribute AttributeValue
        {
            get { return _attributeValue.IsNotNull() ? _attributeValue : (_attributeValue = (MaxAttribute) attribute); }
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (AttributeValue.ValueType)
            {
                case MaxAttribute.ValueTypes.Float:
                    property.floatValue = Mathf.Min(AttributeValue.FloatValue, property.floatValue);
                    break;

                case MaxAttribute.ValueTypes.Int:
                    property.intValue = Mathf.Min(AttributeValue.IntValue, property.intValue);
                    break;
            }
            EditorGUI.PropertyField(position, property);
        }
    }
}


#endif
