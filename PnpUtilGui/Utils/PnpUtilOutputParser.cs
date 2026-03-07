using PnpUtilGui.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PnpUtilGui.Utils
{
    internal class PnpUtilOutputParser
    {
        [DllImport("kernel32.dll")]
        static extern uint GetLastError();
        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryEx", SetLastError = true)]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int LoadString(IntPtr hInstance, uint uID, StringBuilder lpBuffer, int nBufferMax);
        public List<Driver> ParseEnumDriverOutput(IEnumerable<string> enumerable, bool legacy)
        {
            var mui = LoadLibraryEx(CultureInfo.CurrentUICulture.Name + "\\pnputil.exe.mui", IntPtr.Zero, 2);

            var driverList = new List<Driver>();
            var stringArr = enumerable.ToArray();

            StringBuilder sb = new StringBuilder(100);
            LoadString(mui, legacy ? 4600u : 2201u, sb, sb.Capacity);
            var PublishedName = sb.ToString();
            LoadString(mui, legacy ? 4600u : 2202u, sb, sb.Capacity);
            var OriginalName = sb.ToString();
            LoadString(mui, legacy ? 4200u : 2204u, sb, sb.Capacity);
            var ProviderName = sb.ToString();
            LoadString(mui, legacy ? 4300u : 2205u, sb, sb.Capacity);
            var ClassName = sb.ToString();
            LoadString(mui, legacy ? 4600u : 2207u, sb, sb.Capacity);
            var ClassGUID = sb.ToString();
            LoadString(mui, legacy ? 4400u : 2210u, sb, sb.Capacity);
            var DriverVersion = sb.ToString();
            LoadString(mui, legacy ? 4500u : 2211u, sb, sb.Capacity);
            var SignerName = sb.ToString();

            for (var i = 2; i < stringArr.Length - 1; ++i)
            {
                var driverInfo = new string[8];

                do
                {
                    driverInfo
                    [
                        stringArr[i].StartsWith(PublishedName) ? 0 :
                        stringArr[i].StartsWith(OriginalName) ? 1 :
                        stringArr[i].StartsWith(ProviderName) ? 2 :
                        stringArr[i].StartsWith(ClassName) ? 3 :
                        stringArr[i].StartsWith(ClassGUID) ? 4 :
                        stringArr[i].StartsWith(DriverVersion) ? 5 :
                        stringArr[i].StartsWith(SignerName) ? 6 :
                        7
                    ] = stringArr[i];
                    ++i;
                } while (!string.IsNullOrEmpty(stringArr[i]));

                var driver = new Driver
                {
                    FileName = GetLineValue(driverInfo[0]),
                    SourceName = GetLineValue(driverInfo[1]),
                    Publisher = GetLineValue(driverInfo[2]),
                    DriverClass = GetLineValue(driverInfo[3]),
                    ClassGuid = GetLineValue(driverInfo[4]),
                    DateAndVersion = GetLineValue(driverInfo[5]),
                    CertificateSignerName = GetLineValue(driverInfo[6]),
                };

                driverList.Add(driver);
            }

            return driverList;
        }

        private static string GetLineValue(string line)
        {
            return line?.Split(':')[1].Trim();
        }
    }
}