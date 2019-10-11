using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace DYTA.Dialogue
{
    [DataContract]
    public class SetGlobalVariable
    {
        public enum VariableOperator
        {
            Set = 0,
            Add,
            Subtract
        }

        [DataMember]
        public string VariableName { get; set; } = "VaraibleName";

        [DataMember]
        public VariableOperator Operator { get; set; }

        [DataMember]
        public int OperationValue { get; set; }

        public int Operate()
        {
            int value = DialogueSystem.Instance.GetVariableValue(VariableName);

            switch (Operator)
            {
                case VariableOperator.Set:
                    value = OperationValue;
                    break;
                case VariableOperator.Add:
                    value += OperationValue;
                    break;
                case VariableOperator.Subtract:
                    value -= OperationValue;
                    break;
                default:
                    break;
            }

            DialogueSystem.Instance.SetVariableValue(VariableName, value);
            return value;
        }
    }
}
