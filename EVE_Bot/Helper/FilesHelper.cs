
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EVE_Bot.Helper
{
    public static class FilesHelper
    {
        public static void OutputJsonFile(string strFileName, string strContent)
        {
            string strBasePath = Application.StartupPath + @"\Lib\" + strFileName + ".json";
            if (!Directory.Exists(Path.GetDirectoryName(strBasePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(strBasePath));
            }
            using (var sw = new StreamWriter(strBasePath, false, Encoding.Unicode))
            {
                sw.Write(strContent);
            }
            //Environment.
        }

        public static string ReadJsonFile(string strFileName)
        {
            string strContents;
            string strBasePath = Application.StartupPath;
            string strFinalName = strBasePath + @"\Lib\" + strFileName + ".json";
            if (!File.Exists(strFinalName))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(strFinalName));
                File.Create(strBasePath + @"\Lib\" + strFileName + ".json").Close();
            }
            using (var sr = new StreamReader(strBasePath + @"\Lib\" + strFileName + ".json", Encoding.Unicode))
            {
                strContents = sr.ReadToEnd();
            }
            //Environment.
            return strContents;
        }
    }
}
