using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace DYTA.Dialogue
{
    [DataContract]
    public class Transition
    {
        [DataMember]
        public string TargetSituationName { get; set; } = "TargetSituationName";

        [DataMember]
        public string OnFailDescription { get; set; } = "OnFailDescription";

        [DataMember]
        public List<VariableCondition> Conditions { get; set; } = new List<VariableCondition>();

        public bool IfReachConditions()
        {
            foreach (var condition in Conditions)
            {
                if (!condition.Evaluate())
                {
                    return false;
                }
            }
            return true;
        }
    }
}
