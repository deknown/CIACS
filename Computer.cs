using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CIACS;
using System.Management;
using Microsoft.Win32;

namespace CIACS_win_client
{
	public partial class Program
	{
		static ManagementObjectCollection GetWMICollection(string key, string scope = "root\\CIMV2")
		{
			ManagementObjectCollection searchResults = null;
			using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, "SELECT * FROM " + key))
			{
				searchResults = searcher.Get();
				try
				{
					if (searchResults.Count == 0) return null;
				}
				catch (System.Management.ManagementException)
				{
					return null;
				}
				return searchResults;
			}
		}
		static void GetHardware()
		{
			computer.hardware = new HardWare();

			ManagementObjectCollection WMICollection = null;
			/// Motherboard
			WMICollection = GetWMICollection("Win32_BaseBoard");
			if (WMICollection != null)
			{
				computer.hardware.motherboards = new List<MotherBoard>();
				foreach (ManagementObject item in WMICollection)
				{
					MotherBoard mboard = new MotherBoard();

					mboard.manufacturer = (string)item["Manufacturer"];
					mboard.name = (string)item["Product"];

					computer.hardware.motherboards.Add(mboard);
				}
			}
			/// CPU
			WMICollection = GetWMICollection("Win32_Processor");
			if (WMICollection != null)
			{
				computer.hardware.cpus = new List<CPU>();
				foreach (ManagementObject item in WMICollection)
				{
					CPU cpu = new CPU();

					cpu.manufacturer = (string)item["Manufacturer"];
					string name = (string)item["Name"];
					{
						int deleteIndex = name.LastIndexOf('@');
						if (deleteIndex > 0) name = name.Remove(deleteIndex);
					}
					cpu.name = name;
					cpu.cores = Convert.ToInt32(item["NumberOfCores"]);
					cpu.threads = Convert.ToInt32(item["ThreadCount"]);
					cpu.speed = Convert.ToInt32(item["MaxClockSpeed"]);
					cpu.bits = Convert.ToInt32(item["DataWidth"]);

					computer.hardware.cpus.Add(cpu);

				}
			}
			/// GPU
			using (RegistryKey subKeys = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}\"))
			{
				if (subKeys != null)
				{
					computer.hardware.gpus = new List<GPU>();
					foreach (var subKey in subKeys.GetSubKeyNames().OrderBy((a) => a).ToArray())
					{
						RegistryKey key = null;
						if (subKey[0] == '0')
						{
							key = subKeys.OpenSubKey(subKey);
							if (key != null)
							{
								var adapterString = key.GetValue("HardwareInformation.AdapterString");
								if (adapterString != null)
								{
									GPU gpu = new GPU();

									UInt64 qwMemorySize = Convert.ToUInt64(key.GetValue("HardwareInformation.qwMemorySize"));
									if (qwMemorySize != 0)
									{
										gpu.capacity = (int)(qwMemorySize / 1024 / 1024);
									}
									else if (key.GetValueKind("HardwareInformation.MemorySize") == RegistryValueKind.Binary)
									{
										gpu.capacity = BitConverter.ToInt32((byte[])key.GetValue("HardwareInformation.MemorySize"), 0);
									}
									else
									{
										gpu.capacity = (int)((UInt32)key.GetValue("HardwareInformation.MemorySize") / 1024 / 1024);
									}
									gpu.manufacturer = (string)key.GetValue("ProviderName");

									string name = null;
									if (key.GetValueKind("HardwareInformation.AdapterString") == RegistryValueKind.Binary)
									{
										name = Encoding.Unicode.GetString((byte[])adapterString);
									}
									else name = (string)adapterString;

									if (name != null)
									{
										int deleteIndex = name.IndexOf('\0');
										if (deleteIndex > 0) name = name.Remove(deleteIndex);

										gpu.name = name;
									}

									computer.hardware.gpus.Add(gpu);
								}
							}
						}
						else break;
					}
				}
			}
			/// Disk
			WMICollection = GetWMICollection("MSFT_PhysicalDisk", "ROOT\\Microsoft\\Windows\\Storage");
			computer.hardware.disks = new List<Disk>();
			if (WMICollection != null)
			{
				foreach (ManagementObject item in WMICollection)
				{
					Disk disk = new Disk();

					int bus = (UInt16)item["BusType"];
					switch (bus)
					{
						case 7: //  USB
						case 12: // SD
							goto diskExit;
						default:
							break;
					}
					int media = (UInt16)item["MediaType"];
					switch (media)
					{
						case 3:
							disk.type = "HDD";
							break;
						case 4:
							disk.type = "SSD";
							break;
						case 5:
							disk.type = "SCM";
							break;
						default:
							break;
					}
					disk.capacity = (int)((UInt64)item["Size"] / 1024 / 1024);

					computer.hardware.disks.Add(disk);

				diskExit:;
				}
			}
			else
			{
				WMICollection = GetWMICollection("Win32_DiskDrive");
				if (WMICollection != null)
				{
					foreach (ManagementObject item in WMICollection)
					{
						Disk disk = new Disk();

						disk.capacity = (int)((UInt64)item["Size"] / 1024 / 1024);

						computer.hardware.disks.Add(disk);
					}
				}
			}
			/// RAM
			WMICollection = GetWMICollection("Win32_PhysicalMemory");
			if (WMICollection != null)
			{
				computer.hardware.rams = new List<RAM>();
				foreach (ManagementObject item in WMICollection)
				{
					RAM ram = new RAM();

					ram.manufacturer = (string)item["Manufacturer"];
					ram.name = (string)item["PartNumber"];

					int type = Convert.ToInt32(item["SMBIOSMemoryType"]);
					Dictionary<int, string> types = new Dictionary<int, string>() {
						{2, "DRAM"}, {3, "Synchronous DRAM"}, {4, "Cache DRAM"}, {5, "EDO"}, {6, "EDRAM"}, {7, "VRAM"}, {8, "SRAM"},
						{9, "RAM"}, {10, "ROM"}, {11, "Flash"}, {12, "EEPROM"}, {13, "FEPROM"}, {14, "EPROM"}, {15, "CDRAM"},
						{16, "3DRAM"}, {17, "SDRAM"}, {18, "SGRAM"}, {19, "RDRAM"}, {20, "DDR"}, {21, "DDR2"}, {22, "DDR2 FB-DIMM"},
						{24, "DDR3"}, {25, "FBD2"}, {26, "DDR4"}
					};  // WILL BE NULL: 0, 1, 23, >26 
					types.TryGetValue(type, out ram.type);

					ram.capacity = (int)((UInt64)item["Capacity"] / 1024 / 1024);

					computer.hardware.rams.Add(ram);
				}
			}
			/// OS
			WMICollection = GetWMICollection("Win32_OperatingSystem");
			if (WMICollection != null)
			{
				foreach (ManagementObject item in WMICollection)
				{
					computer.os.name = (string)item["Caption"];
					computer.os.build = (string)item["BuildNumber"];
					string bits = (string)item["OSArchitecture"];
					{
						int deleteIndex = bits.IndexOf('-');
						if (deleteIndex > 0) bits = bits.Remove(deleteIndex);
					}
					computer.os.bits = Int32.Parse(bits);

					break;
				}
			}
			///// getter for active IP address
			//using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
			//{
			//	socket.Connect("8.8.8.8", 65530);
			//	IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
			//	computer.ip = endPoint.Address.ToString();
			//}
		}
		static void GetSoftware()
		{
			List<SoftWare> software = new List<SoftWare>();

			using (RegistryKey subKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
			{
				if (subKeys != null)
				{
					foreach (var subKey in subKeys.GetSubKeyNames())
					{
						RegistryKey key = subKeys.OpenSubKey(subKey);
						if (key != null)
						{
							string name = (string)key.GetValue("DisplayName");
							if (name != null)
							{
								SoftWare soft = new SoftWare();
								soft.name = name;
								soft.version = (string)key.GetValue("DisplayVersion");
								software.Add(soft);
							}
						}
					}
				}
			}
			List<SoftWare> software32 = new List<SoftWare>();
			if (computer.os.bits == 64)
			{
				using (RegistryKey subKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall"))
				{
					if (subKeys != null)
					{
						foreach (var subKey in subKeys.GetSubKeyNames())
						{
							RegistryKey key = subKeys.OpenSubKey(subKey);
							if (key != null)
							{
								string name = (string)key.GetValue("DisplayName");
								if (name != null)
								{
									SoftWare soft = new SoftWare();
									soft.name = name;
									soft.version = (string)key.GetValue("DisplayVersion");
									software32.Add(soft);
								}
							}
						}
					}
				}
			}
			if (software.Count > 0)
			{
				software = software.RemoveVersion();
				software.Sort();
			};
			if (software32.Count > 0)
			{
				software32 = software32.RemoveVersion();
				software32.Sort();
			}

			computer.software = software.DistinctUnionSorted(software32, (x, y) => {
				int result = String.Compare(x.name, y.name);
				if (result == 0) result = String.Compare(y.version, x.version);
				return result;
			}, (x, y) => {
				int result = String.Compare(x.name, y.name);
				if (result == 0) result = String.Compare(x.version, y.version);
				return result;
			}).ToList();
		}
	}
}
