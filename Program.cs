using System;
using System.Diagnostics;
using System.Windows.Forms;
using CIACS;
using System.IO;

namespace CIACS_win_client
{
	public partial class Program
	{
		static Computer computer = null;
		static StreamWriter logs = null;
		static string documentRoot = null;
		static string ip = null;

		/// <summary>
		/// Главная точка входа для приложения.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			var watch = System.Diagnostics.Stopwatch.StartNew();
			try
			{
				documentRoot = GetDocumentFolder();
				if (!Directory.Exists(documentRoot)) Directory.CreateDirectory(documentRoot);
				logs = File.CreateText(documentRoot + "logs.txt");
				ip = ReadIP();

				if (args.Length == 0)
				{
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					Application.Run(new Form1());
				}
				else
				{
					if (ip == null) throw new Exception("Server ip address is not specified");

					logs.WriteLine($"\"{args[0]}\" command received.");
					switch (args[0])
					{
						case "get":
							computer = new Computer();

							GetHardware();
							GetSoftware();
							string JSON = SerializeJSON(computer);
							watch.Stop();
							logs.WriteLine("\n\n" + watch.ElapsedMilliseconds + "\n\n");

							//OpenConnection();
							//SendMessage(JSON);
							//CloseConnection();

							logs.WriteLine("Message sent successfully to server. Message was:\n\n" + JSON);
								break;
						default:
							throw new Exception("Wrong arguments");
					}
				}
			}
			catch (Exception ex)
			{
				logs.WriteLine($"A problem was encountered: {ex.Message}\n");
				var trace = new System.Diagnostics.StackTrace(ex, true);
				for (int i = 0; i < trace.FrameCount; i++)
				{
					StackFrame sf = trace.GetFrame(i);
					logs.WriteLine(i + ": method {0}", sf.GetMethod());
					logs.WriteLine("at line: {0}\n", sf.GetFileLineNumber());
				}
			}
			finally
			{
				logs.Flush();
				logs.Close();
			}
		}
	}
}