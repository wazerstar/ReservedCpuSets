using Microsoft.Win32;
using System;

namespace ReservedCpuSets {
    internal class SharedFunctions {
        public static int GetWindowsBuildNumber() {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion")) {
                return int.Parse(key.GetValue("CurrentBuildNumber") as string);
            }
        }

        public static string GetReservedCpuSets() {
            string bitmask;

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel")) {
                try {
                    byte[] current_config = key.GetValue("ReservedCpuSets") as byte[];
                    Array.Reverse(current_config); // big to little endian
                    string hex_string = BitConverter.ToString(current_config).Replace("-", "");
                    // convert to binary
                    bitmask = Convert.ToString(Convert.ToInt32(hex_string, 16), 2);
                } catch (ArgumentException) {
                    bitmask = "";
                }
            }
            // sterilize string and ensure it is length of core count
            return bitmask.TrimStart('0').PadLeft(Environment.ProcessorCount, '0');
        }
    }
}
