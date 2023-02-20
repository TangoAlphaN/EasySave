using EasySave.src.Models.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace EasyClient
{
    [Serializable]
    public class SaveInfo
    {
        public string SaveName { get; set; }
        private Guid _guid;
        public string Status { get; set; }
        public int Progress { get; set; }

        private static Dictionary<string, SaveInfo> dictionnaryInstance = new Dictionary<string, SaveInfo>();

        public string ProgressionString => $"{Progress}%";

        public SaveInfo(string saveName, string status, int progress, Guid guid)
        {
            SaveName = saveName;
            Status = status;
            Progress = progress;
            _guid = guid;
        }

        public static SaveInfo FromSave(Save s)
        {
            return new SaveInfo(s.GetName(), s.GetStatus().ToString(), s.CalculateProgress(), s.uuid);
        }

        public override string ToString()
        {
            return $"{SaveName} - {Status.ToString()} - {ProgressionString}";
        }

        public static object Create(JObject jsonData)
        {
            foreach (var save in jsonData.Properties())
            {
                if (dictionnaryInstance.ContainsKey(save.Name.ToString()))
                {
                    if (save.Value["save.Value[\"progression\"].ToString()"] != null)
                        dictionnaryInstance[save.Name.ToString()] = dictionnaryInstance[save.Name.ToString()].Update(save.Value["name"].ToString(), save.Value["state"].ToString(), int.Parse(save.Value["progression"].ToString()));
                    else dictionnaryInstance[save.Name.ToString()] = dictionnaryInstance[save.Name.ToString()].Update(save.Value["name"].ToString(), save.Value["state"].ToString(), 0);
                }
                else
                {
                    SaveInfo s;
                    if (save.Value["progression"] != null)
                        s = new SaveInfo(save.Value["name"].ToString(), save.Value["state"].ToString(), int.Parse(save.Value["progression"].ToString()), Guid.Parse(save.Name));
                    else
                        s = new SaveInfo(save.Value["name"].ToString(), save.Value["state"].ToString(), 0, Guid.Parse(save.Name));
                    dictionnaryInstance.Add(save.Name.ToString(), s);

                }
            }
            return dictionnaryInstance;
        }

        private SaveInfo Update(string name, string status, int progress)
        {
            SaveName = name;
            Status = status;
            Progress = progress;
            return this;
        }

        public override bool Equals(object obj)
        {
            // If the passed object is null, return False
            if (obj == null)
            {
                return false;
            }
            // If the passed object is not Customer Type, return False
            if (!(obj is SaveInfo))
            {
                return false;
            }
            return _guid == ((SaveInfo)obj)._guid;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_guid);
        }
    }
}
