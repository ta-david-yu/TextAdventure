using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace DYTA.Dialogue
{
    [DataContract]
    public class VariableCondition
    {
        public enum VariableComparator
        {
            Equal,
            NotEqual,
            LargerThan,
            SmallerThan
        }

        [DataMember]
        public string VariableName { get; set; } = "VariableName";

        [DataMember]
        public VariableComparator Comparator { get; set; }

        [DataMember]
        public int ComparisonValue { get; set; }

        public bool Evaluate()
        {
            int value = DialogueSystem.Instance.GetVariableValue(VariableName);

            switch (Comparator)
            {
                case VariableComparator.Equal:
                    return value == ComparisonValue;
                case VariableComparator.NotEqual:
                    return value != ComparisonValue;
                case VariableComparator.LargerThan:
                    return value > ComparisonValue;
                case VariableComparator.SmallerThan:
                    return value < ComparisonValue;
                default:
                    return false;
            }
        }
    }
}
