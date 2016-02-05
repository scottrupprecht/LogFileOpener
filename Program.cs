using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogFileOpener
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                MessageBox.Show("File is not defined");
                return;
            }
            
            var filename = args[0];
            
            var isSrlog = IsFileType(filename, new List<string> { "srlog" });
            
            if (isSrlog)
            {
                OpenLogs(filename);
                return;
            }
            MessageBox.Show("Input needs to be a .srlog file");
        }

        private static void OpenLogs(string filepath)
        {
            var logFiles = File.ReadLines(filepath);
            var validFiles = new List<string>();
            var invalidFiles = new List<string>();
            var acceptableFileTypes = GetAcceptableFileTypes();
            foreach (var logFile in logFiles)
            {
                if(IsFileType(logFile, acceptableFileTypes))
                    validFiles.Add(logFile);
                else
                    invalidFiles.Add(logFile);
            }

            foreach (var validFile in validFiles)
            {
                Process.Start(validFile);
            }

            if (invalidFiles.Count > 1)
            {
                MessageBox.Show(string.Format("There are {0} file(s) that are invalid: {1}", invalidFiles.Count, string.Join(", ", invalidFiles)));
            }
        }

        private static bool IsFileType(string filename, List<string> extensions)
        {
            if (filename == null || filename.Trim() == "")
                return false;

            var exists = File.Exists(filename);
            var extension = Path.GetExtension(filename).Trim('.');
            var isValidExt = extensions.Exists(ext => ext.ToLower().Trim() == extension.ToLower().Trim());
            return exists && isValidExt;
        }

        private static List<string> GetAcceptableFileTypes()
        {
            try
            {
                string path = Assembly.GetExecutingAssembly().Location;
                var directory = Path.GetDirectoryName(path);
                var fileTypesPath = directory + "\\" + "filetypes.txt";
                if (File.Exists(fileTypesPath))
                {
                    var text = File.ReadAllText(fileTypesPath);
                    var fileList = new List<string>(text.Split(','));
                    if (fileList.Count > 0)
                        return fileList;
                }
                else
                {
                    using (StreamWriter writetext = new StreamWriter(fileTypesPath))
                    {
                        writetext.WriteLine("log,txt");
                        MessageBox.Show("filetypes.txt not found, created new file");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return new List<string> {"log", "txt"};
        } 
    }
}
