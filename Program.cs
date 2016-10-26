using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LogFileOpener
{
    class Program
    {
        private static readonly List<string> _validFiles = new List<string>();
        private static readonly List<string> _invalidFiles = new List<string>();
        private static string _filename;
        static void Main(string[] args)
        {
            try
            {
                ValidateArgs(args);
                LoadFilename(args);
                LoadLogFiles();
                OpenLogs();
            }  
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private static void OpenLogs()
        {
            foreach (var validFile in _validFiles)
            {
                Process.Start(validFile);
            }

            if (_invalidFiles.Count > 1)
            {
                MessageBox.Show(string.Format("There are {0} file(s) that are invalid: {1}", _invalidFiles.Count, string.Join(", ", _invalidFiles)));
            }
        }

        private static void LoadLogFiles()
        {
            var logFiles = File.ReadLines(_filename);
            foreach (var logFile in logFiles)
            {
                if (!IsValidFile(logFile))
                {
                    _invalidFiles.Add(logFile);
                    continue;
                }

                if (File.GetAttributes(logFile).HasFlag(FileAttributes.Directory))
                {
                    var d = new DirectoryInfo(logFile);
                    foreach (var fileInDirectory in d.GetFiles("*.*", SearchOption.TopDirectoryOnly))
                    {
                        _validFiles.Add(fileInDirectory.FullName);
                    }
                }
                else
                {
                    _validFiles.Add(logFile);
                }
            }
        }

        private static bool IsValidFile(string filename)
        {
            if (filename == null || filename.Trim() == "")
                return false;

            return File.Exists(filename) || Directory.Exists(filename);
        }

        private static bool IsFileType(string filename, params string[] extensions)
        {
            if (filename == null || filename.Trim() == "")
                return false;

            var exists = File.Exists(filename);
            var extension = Path.GetExtension(filename).Trim('.');
            var isValidExt = extensions.ToList().Exists(ext => ext.ToLower().Trim() == extension.ToLower().Trim());
            return exists && isValidExt;
        }

        private static void ValidateArgs(string[] args)
        {
            if (args.Length != 1)
            {
                throw new Exception("File is not defined");
            }
        }

        private static void LoadFilename(string[] args)
        {
            _filename = args[0];

            var isSrlog = IsFileType(_filename, "srlog");

            if (!isSrlog)
            {
                throw new Exception("Input needs to be a .srlog file");
            }
        }
    }
}

