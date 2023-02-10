﻿using EasySave.src.Models.Data;
using EasySave.src.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasySave.src
{
    public class ViewModel
    {

        internal protected ViewModel()
        {

        }

        public HashSet<string> GetSaves()
        {
            HashSet<string> data = new HashSet<string>();
            foreach (Save s in Save.GetSaves())
                data.Add(s.ToString());
            return data;
        }

        public Save CreateSave(string name, string src, string dest, SaveType type)
        {
            return Save.CreateSave(name, src, dest, type);
        }

        public void EditSave(Save s, string name)
        {
            s.Rename(name);
        }

        public void DeleteSave(Save s)
        {
            Save.Delete(s.uuid);
        }

        public Save GetSaveByUuid(string name)
        {
            return Save.GetSaves().First(save => save.ToString() == name);
        }

        internal void RunSave(Save save)
        {
            save.Run();
        }

        internal void StopAllSaves()
        {
            foreach(Save s in Save.GetSaves())
            {
                s.Stop();
            }
            LogUtils.LogSaves();
        }

        public static bool IsUpdated()
        {
            string version = "V"+Assembly.GetExecutingAssembly().GetName().Version;
           
            //to be modified later
            string latest = "V1.0";

            return latest == version;
        }
    }
}
