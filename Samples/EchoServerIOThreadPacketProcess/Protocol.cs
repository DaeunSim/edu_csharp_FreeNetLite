using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoServerIOThreadPacketProcess
{
	public enum PROTOCOL_ID : short
	{
		BEGIN = 0,

		ECHO_REQ = 1,
		ECHO_ACK = 2,

		END
	}
}
