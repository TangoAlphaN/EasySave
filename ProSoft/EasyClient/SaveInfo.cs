using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace EasyClient
{
    /// <summary>
    /// Save information class
    /// </summary>
    [Serializable]
    public partial class SaveInfo : ObservableObject
    {

        /// <summary>
        /// Save name
        /// </summary>
        public string SaveName { get; set; }

        /// <summary>
        /// Save uuid
        /// </summary>
        private Guid _guid;

        /// <summary>
        /// Save status, observable
        /// </summary>
        [ObservableProperty]
        private string _status;

        /// <summary>
        /// Save progress, observable
        /// </summary>
        [ObservableProperty]
        private int _progress;

        /// <summary>
        /// Static instance of save data
        /// </summary>
        private static readonly Dictionary<string, SaveInfo> dictionnaryInstance = new Dictionary<string, SaveInfo>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="saveName">name</param>
        /// <param name="status">status</param>
        /// <param name="progress">progress</param>
        /// <param name="guid">guid</param>
        public SaveInfo(string saveName, string status, int progress, Guid guid)
        {
            SaveName = saveName;
            Status = status;
            Progress = progress;
            _guid = guid;
        }

        /// <summary>
        /// Create SaveInfo from EasySave.src.Models.Data.Save
        /// </summary>
        /// <param name="s">save</param>
        /// <returns>SaveInfo instance</returns>
        public static SaveInfo FromSave(dynamic s)
        {
            return new SaveInfo($"{s.GetName()} - {s.uuid}", s.GetStatus().ToString(), s.CalculateProgress(), s.uuid);
        }

        /// <summary>
        /// Create SaveInfo from Json
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns>DictionnaryInstance</returns>
        public static object Create(JObject jsonData)
        {
            //Update save if exist, else create it
            foreach (var save in jsonData.Properties())
            {
                if (dictionnaryInstance.ContainsKey(save.Name.ToString()))
                {
                    if (save.Value["progression"] != null)
                        dictionnaryInstance[save.Name.ToString()] = dictionnaryInstance[save.Name.ToString()].Update($"{save.Value["name"]} - {save.Name}", save.Value["state"].ToString(), int.Parse(save.Value["progression"].ToString()));
                    else
                        dictionnaryInstance[save.Name.ToString()] = dictionnaryInstance[save.Name.ToString()].Update($"{save.Value["name"]} - {save.Name}", save.Value["state"].ToString(), 0);
                }
                else
                {
                    SaveInfo s;
                    if (save.Value["progression"] != null)
                        s = new SaveInfo($"{save.Value["name"]} - {save.Name}", save.Value["state"].ToString(), int.Parse(save.Value["progression"].ToString()), Guid.Parse(save.Name));
                    else
                        s = new SaveInfo($"{save.Value["name"]} - {save.Name}", save.Value["state"].ToString(), 0, Guid.Parse(save.Name));
                    dictionnaryInstance.Add(save.Name.ToString(), s);
                }
            }
            return dictionnaryInstance;
        }

        /// <summary>
        /// Update a SaveInformation instance
        /// </summary>
        /// <param name="name">newName</param>
        /// <param name="status">newStatus</param>
        /// <param name="progress">newProgress</param>
        /// <returns>SaveInfo instance</returns>
        private SaveInfo Update(string name, string status, int progress)
        {
            SaveName = name;
            Status = status;
            Progress = progress;
            return this;
        }

        /// <summary>
        /// Override equals
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>true if equals</returns>
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

        /// <summary>
        /// Override GetHashCode
        /// </summary>
        /// <returns>int hashed</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(_guid);
        }
    }
}
