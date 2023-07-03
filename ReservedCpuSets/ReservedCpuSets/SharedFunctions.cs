using Microsoft.Win32;
using System;
using System.Linq;

namespace ReservedCpuSets {
    internal class SharedFunctions {
        public static string GetReservedCpuSets() {
            string bitmask;

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel")) {
                try {
                    byte[] current_config = key.GetValue("ReservedCpuSets") as byte[];
                    Array.Reverse(current_config); // big to little endian
                    // convert to binary
                    bitmask = string.Join("", current_config.Select(b => Convert.ToString(b, 2)));
                } catch (ArgumentException) {
                    bitmask = "";
                }
            }
            // sterilize string and ensure it is length of core count
            return bitmask.TrimStart('0').PadLeft(Environment.ProcessorCount, '0');
        }
    }
}
