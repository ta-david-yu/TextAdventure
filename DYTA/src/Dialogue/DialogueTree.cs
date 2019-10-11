using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DYTA.Dialogue
{
    [DataContract]
    public class DialogueTree
    {
        [DataMember]

        // name -> situation
        public Dictionary<string, Situation> SituationTables { get; set; }
    }
}
