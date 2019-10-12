using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace DYTA.Dialogue
{
    [DataContract]
    public class Situation
    {
        [DataMember]
        public string LocationName { get; set; } = "LocationName";

        [DataMember]
        public string FirstDescription { get; set; } = "FirstDescription";

        [DataMember]
        public string Description { get; set; } = "Description";

        [DataMember]
        // cmd -> trans
        public Dictionary<string, Transition> SituationTransitions { get; set; } = new Dictionary<string, Transition>(StringComparer.OrdinalIgnoreCase);

        [DataMember]
        // action when reach this situation the first time
        public List<SetGlobalVariable> OneTimeSetValueAction { get; set; } = new List<SetGlobalVariable>();

        [DataMember]
        // action when every time reach this situation
        public List<SetGlobalVariable> AlwaysSetValueAction { get; set; } = new List<SetGlobalVariable>();
    }
}
