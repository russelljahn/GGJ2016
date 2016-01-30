#if UNITY_EDITOR
using Sense.Extensions;
using Sense.PropertyAttributes;
using UnityEditor;
using UnityEngine;

namespace Sense.Editor.PropertyAttributes
{
    [CustomPropertyDrawer(typeof (MinAttribute))]
    public class MinAttributeDrawer : PropertyDrawer
    {
        private MinAttribute _attributeValue;
        private MinAttribute AttributeValue
        {
            get { return _attributeValue.IsNotNull() ? _attributeValue : (_attributeValue = (MinAttribute) attribute); }
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (AttributeValue.ValueType)
            {
                case MinAttribute.ValueTypes.Float:
                    property.floatValue = Mathf.Max(AttributeValue.FloatValue, property.floatValue);
                    break;

                case MinAttribute.ValueTypes.Int:
                    property.intValue = Mathf.Max(AttributeValue.IntValue, property.intValue);
                    break;
            }
            EditorGUI.PropertyField(position, property);
        }
    }
}
#endif
