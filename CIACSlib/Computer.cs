using System.Collections.Generic;

namespace CIACS
{
	public class Computer
	{
		//public string id;
		//public string ip;
		public OS os = new OS();
		public HardWare hardware;
		public List<SoftWare> software;
	}
}
