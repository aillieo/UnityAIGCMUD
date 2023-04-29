namespace AillieoUtils.AIGC
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using UnityEngine;

    public static class PythonHelper
    {
        public static void RunPython(string script)
        {
            var directory = Path.GetDirectoryName(script);
            var filemame = Path.GetFileName(script);

            var executableFile = FindPython();

            if (string.IsNullOrEmpty(executableFile))
            {
                UnityEngine.Debug.LogError("找不到匹配的Python目录");
                return;
            }

            var startInfo = new ProcessStartInfo();

            startInfo.WorkingDirectory = directory;
            startInfo.FileName = executableFile;
            startInfo.Arguments = filemame;

            startInfo.UseShellExecute = true;

            using (var process = Process.Start(startInfo))
            {
            }
        }

        private static string executableName
        {
            get
            {
                switch (Application.platform)
                {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return "python.exe";
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return "python";
                default:
                    throw new NotImplementedException();
                }
            }
        }

        private static char environmentVariableSeparator
        {
            get
            {
                switch (Application.platform)
                {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return ';';
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return ':';
                default:
                    throw new NotImplementedException();
                }
            }
        }

        private static string environmentVariableName
        {
            get
            {
                switch (Application.platform)
                {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return "Path";
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return "PATH";
                default:
                    throw new NotImplementedException();
                }
            }
        }

        internal static string FindPython()
        {
            foreach (EnvironmentVariableTarget target in Enum.GetValues(typeof(EnvironmentVariableTarget)))
            {
                var pathValue = Environment.GetEnvironmentVariable(environmentVariableName, target);
                if (!string.IsNullOrEmpty(pathValue))
                {
                    var pathEntries = pathValue.Split(new char[] { environmentVariableSeparator }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var path in pathEntries)
                    {
                        try
                        {
                            var fullPath = Path.Combine(path, executableName);

                            if (File.Exists(fullPath))
                            {
                                return fullPath;
                            }
                        }
                        catch (Exception e)
                        {
                            UnityEngine.Debug.Log(e);
                        }
                    }
                }
            }

            return null;
        }
    }
}
