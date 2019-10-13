using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace Sandbox
{
    [DataContract]
    public class PlayerProgress
    {
        [DataMember]
        public string Situation { get; set; }

        [DataMember]
        public Dictionary<string, int> GlobalVariables { get; set; }

        [DataMember]
        public HashSet<string> VisitedSituation { get; set; }

        public static PlayerProgress LoadFromFile(string path)
        {
            var serializer = new DataContractJsonSerializer(typeof(PlayerProgress));
            var stream = File.Open(path, FileMode.Open);

            PlayerProgress progress = (PlayerProgress)serializer.ReadObject(stream);

            stream.Close();

            return progress;
        }

        public static void SaveToFile(PlayerProgress progress, string path)
        {
            var stream = File.Create(path);
            var serializer = new DataContractJsonSerializer(typeof(PlayerProgress));

            serializer.WriteObject(stream, progress);

            stream.Close();
        }
    }
}
