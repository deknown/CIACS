using System;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using CIACS;
using System.CodeDom;
using System.Runtime.Remoting.Messaging;

namespace CIACS_win_client
{
	public partial class Program
	{
		public static string SerializeJSON(object Object)
		{
			return JsonConvert.SerializeObject(Object);
		}
		public static string GetDocumentFolder()
		{
			return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CIACS\\";
		}
		public static string ReadIP()
		{
			if (File.Exists(documentRoot + "serverip")) {
				return File.ReadAllText(documentRoot + "serverip");
			}
			return null;
		}
		public static void WriteIP(string IP)
		{
			ip = IP;
			File.WriteAllText(documentRoot + "serverip", IP);
		}
		public static string GetServerIP()
		{
			return ip;
		}
	}
}
