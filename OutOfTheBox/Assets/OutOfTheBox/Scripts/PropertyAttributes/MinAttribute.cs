using UnityEngine;

namespace Sense.PropertyAttributes 
{
    public class MinAttribute : PropertyAttribute
    {
        public float FloatValue;
        public int IntValue;

        public enum ValueTypes
        {
            Int,
            Float,
        };

        public ValueTypes ValueType;


        public MinAttribute(float value)
        {
            FloatValue = value;
            ValueType = ValueTypes.Float;
        }


        public MinAttribute(int value)
        {
            IntValue = value;
            ValueType = ValueTypes.Int;
        }
    }
}

