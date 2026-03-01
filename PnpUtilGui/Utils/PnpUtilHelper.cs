using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PnpUtilGui.Models;

namespace PnpUtilGui.Utils
{
    internal class PnpUtilHelper
    {
        private static readonly PnpUtilOutputParser Parser = new PnpUtilOutputParser();

        public static Task<List<Driver>> EnumDrivers()
        {
            return Task.Run(() =>
            {
                var output = ExecutePnpUtil(new[] { "/enum-drivers" });

                var legacy = output == null;
                if (legacy)
                    output = ExecutePnpUtil(new[] { "/e" });

                return Parser.ParseEnumDriverOutput(output, legacy);
            });
        }

        public static Task<string> DeleteDriver(string fileName, bool force = false)
        {
            return Task.Run(() =>
            {
                var output = ExecutePnpUtil(new[] { "/d", fileName, force ? "/f" : "" });

                return string.Join(Environment.NewLine, output.Skip(2).Take(1));
            });
        }

        public static Task<string> ExportDriver(string fileName, string targetDir)
        {
            return Task.Run(
                () =>
                {
                    var output = ExecutePnpUtil(new[] {"/export-driver", fileName, targetDir});

                    return string.Join(Environment.NewLine, output.Skip(2));
                });
        }

        private static List<string> ExecutePnpUtil(string[] args)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "pnputil.exe",
                    Arguments = string.Join(" ", args),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            using (proc)
            {
                var list = new List<string>();
                string line;
                while ((line = proc.StandardOutput.ReadLine()) != null)
                {
                    list.Add(line);
                }

                return proc.ExitCode == 0 ? list : null;
            }
        }
    }
}
