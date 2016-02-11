using UnityEngine;

namespace Sense.PropertyAttributes 
{
    public class MaxAttribute : PropertyAttribute
    {
        public float FloatValue;
        public int IntValue;

        public enum ValueTypes
        {
            Int,
            Float,
        };

        public ValueTypes ValueType;


        public MaxAttribute(float value)
        {
            FloatValue = value;
            ValueType = ValueTypes.Float;
        }


        public MaxAttribute(int value)
        {
            IntValue = value;
            ValueType = ValueTypes.Int;
        }
    }
}

