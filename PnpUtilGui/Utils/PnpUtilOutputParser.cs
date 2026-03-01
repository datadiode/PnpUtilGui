using System;
using System.Collections.Generic;
using System.Linq;
using PnpUtilGui.Models;

namespace PnpUtilGui.Utils
{
    internal class PnpUtilOutputParser
    {
        public List<Driver> ParseEnumDriverOutput(IEnumerable<string> enumerable, bool legacy)
        {
            var driverList = new List<Driver>();
            var stringArr = enumerable.ToArray();

            for (var i = 2; i < stringArr.Length - 1; ++i)
            {
                var driverInfo = new string[8];

                do
                {
                    driverInfo
                    [
                        stringArr[i].StartsWith(legacy ? "Published name :" : "Published Name:") ? 0 :
                        stringArr[i].StartsWith("Original Name:") ? 1 :
                        stringArr[i].StartsWith(legacy ? "Driver package provider :" : "Provider Name:") ? 2 :
                        stringArr[i].StartsWith(legacy ? "Class :" : "Class Name:") ? 3 :
                        stringArr[i].StartsWith("Class GUID:") ? 4 :
                        stringArr[i].StartsWith(legacy ? "Driver date and version :" : "Driver Version:") ? 5 :
                        stringArr[i].StartsWith(legacy ? "Signer name :" : "Signer Name:") ? 6 :
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