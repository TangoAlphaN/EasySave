using EasySave.src.Models.Data;
using System.Collections.Generic;

namespace EasySave.src
{
    public class ViewModel
    {

        protected ViewModel()
        {

        }

        public static HashSet<string> GetSaves()
        {
            HashSet<string> data = new HashSet<string>();
            foreach (Save s in Save.GetSaves())
                data.Add(s.ToString());
            return data;
        }
        
    }
}
