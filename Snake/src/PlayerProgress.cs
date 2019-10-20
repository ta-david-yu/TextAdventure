using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace NSShaft
{
    public class PlayerProgress
    {
        public const string c_SaveFilePath = "lb.sav";

        [Serializable]
        public class ScoreData
        {
            public List<Tuple<string, int>> SinglePlayerScores { get; set; } = new List<Tuple<string, int>>();
            public List<Tuple<string, int>> TwoPlayerScores { get; set; } = new List<Tuple<string, int>>();
        }

        public ScoreData Data { get; private set; }

        public void Load()
        {
            if (!File.Exists(c_SaveFilePath))
            {
                var file = System.IO.File.Create(c_SaveFilePath);
                Data = new ScoreData();
                file.Close();
            }
            else
            {
                var bytes = System.IO.File.ReadAllBytes(c_SaveFilePath);

                if (bytes.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        var bf = new BinaryFormatter();
                        ms.Write(bytes, 0, bytes.Length);
                        ms.Seek(0, SeekOrigin.Begin);
                        Data = bf.Deserialize(ms) as ScoreData;
                    }
                }
                else
                {
                    Data = new ScoreData();
                }
            }
        }

        public void Save()
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, Data);
                var bytes = ms.ToArray();
                File.WriteAllBytes(c_SaveFilePath, bytes);
            }
        }

        public void AddRecord(GameMode mode, string name, int level)
        {
            if (mode == GameMode.SinglePlayer)
            {
                Data.SinglePlayerScores.Add(new Tuple<string, int>(name, level));
                Data.SinglePlayerScores.OrderBy(tuple => -tuple.Item2);
            }
            else if (mode == GameMode.TwoPlayers)
            {
                Data.TwoPlayerScores.Add(new Tuple<string, int>(name, level));
                Data.TwoPlayerScores.OrderBy(tuple => -tuple.Item2);
            }

            Save();
        }
    }
}
