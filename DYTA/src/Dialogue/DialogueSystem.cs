using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace DYTA.Dialogue
{

    public class DialogueSystem
    {
        private static DialogueSystem s_Instance = null;
        public static DialogueSystem Instance
        {
            get
            {
                //lock (s_Lock)
                {
                    if (s_Instance == null)
                    {
                        s_Instance = new DialogueSystem();
                    }
                    return s_Instance;
                }
            }
        }

        public Dictionary<string, int> GlobalVariables { get; private set; }

        public DialogueTree Tree { get; private set; }

        public DialogueSystem()
        {
            GlobalVariables = new Dictionary<string, int>();
        }

        public void debug()
        {
            Tree = new DialogueTree();
            var sit0 = new Situation();
            sit0.LocationName = new string("SIT-00_Location");
            sit0.FirstDescription = new string("SIT-00_FDescr");
            sit0.Description = new string("SIT-00_Descr");

            sit0.OneTimeSetValueAction = new List<SetGlobalVariable>();
            sit0.OneTimeSetValueAction.Add(new SetGlobalVariable());

            sit0.AlwaysSetValueAction = new List<SetGlobalVariable>();
            sit0.AlwaysSetValueAction.Add(new SetGlobalVariable());
            sit0.AlwaysSetValueAction.Add(new SetGlobalVariable());

            var tran0 = new Transition();
            tran0.TargetSituationName = "SIT-01";
            tran0.OnFailDescription = "TRAN-00-FD";
            tran0.Conditions = new List<VariableCondition>();

            var tran1 = new Transition();
            tran1.TargetSituationName = "SIT-02";
            tran1.OnFailDescription = "TRAN-01-FD";
            tran1.Conditions = new List<VariableCondition>();
            tran1.Conditions.Add(new VariableCondition());

            sit0.SituationTransitions = new Dictionary<string, Transition>();
            sit0.SituationTransitions.Add("TRAN-00", tran0);
            sit0.SituationTransitions.Add("TRAN-01", tran1);

            //
            var sit1 = new Situation();
            sit1.LocationName = new string("SIT-01_Location");
            sit1.FirstDescription = new string("SIT-01_FDescr");
            sit1.Description = new string("SIT-01_Descr");

            var sit2 = new Situation();
            sit2.LocationName = new string("SIT-02_Location");
            sit2.FirstDescription = new string("SIT-02_FDescr");
            sit2.Description = new string("SIT-02_Descr");

            Tree.SituationTables = new Dictionary<string, Situation>();
            Tree.SituationTables.Add("SIT-00", sit0);
            Tree.SituationTables.Add("SIT-01", sit1);
            Tree.SituationTables.Add("SIT-02", sit2);

            var stream = File.Create("./test.json");
            DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
            var serializer = new DataContractJsonSerializer(typeof(DialogueTree));

            serializer.WriteObject(stream, Tree);

            stream.Close();
        }

        public void LoadScriptsFromFile(string path)
        {
            var serializer = new DataContractJsonSerializer(typeof(DialogueTree));
            var stream = File.Open(path, FileMode.Open);

            Tree = (DialogueTree)serializer.ReadObject(stream);

            stream.Close();
        }

        public int GetVariableValue(string variableName)
        {
            int value = 0;
            if (GlobalVariables.TryGetValue(variableName, out value))
            {
                return value;
            }
            else
            {
                GlobalVariables.Add(variableName, value);
                return value;
            }
        }

        public void SetVariableValue(string variableName, int targetValue)
        {
            if (GlobalVariables.ContainsKey(variableName))
            {
                GlobalVariables[variableName] = targetValue;
            }
            else
            {
                GlobalVariables.Add(variableName, targetValue);
            }
        }
    }
}
