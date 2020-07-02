using System.Text;
using System.Net;
using System.Net.Sockets;
using CIACS;
using Newtonsoft.Json;

namespace CIACS_win_client
{
	public partial class Program
	{
		static Socket socket = null;
		public static void OpenConnection()
		{
			IPAddress ipAddress = IPAddress.Parse(ip);
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, 65100);

			socket = new Socket(ipAddress.AddressFamily,
				SocketType.Stream, ProtocolType.Tcp);

			socket.Connect(remoteEP);
		}

		public static ServerInfo ReceiveLocation()
		{
			OpenConnection();

			SendMessage("GET locations");
			string serverinfo = ReceiveMessage();

			CloseConnection();

			return JsonConvert.DeserializeObject<ServerInfo>(serverinfo);
		}
		static string ReceiveMessage()
		{
			byte[] receive = new byte[1024];
			int bytesRec = socket.Receive(receive);

			return Encoding.UTF8.GetString(receive, 0, bytesRec);
		}
		static void SendMessage(string message)
		{
			byte[] msg = Encoding.UTF8.GetBytes(message);
			socket.Send(msg);
		}
		static public void RequestUpdate()
		{
			OpenConnection();

			SendMessage("POST request");

			CloseConnection();
		}
		static public void UpdateLocation(string JSON)
		{
			OpenConnection();

			SendMessage("POST location");
			SendMessage(JSON);

			CloseConnection();
		}
		static void CloseConnection()
		{
			socket.Shutdown(SocketShutdown.Both);
			socket.Close();
			socket.Dispose();
		}
	}
}
