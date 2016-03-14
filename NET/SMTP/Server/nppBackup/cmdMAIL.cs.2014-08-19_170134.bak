// using System;
// using System.Net.Sockets;
// using System.Collections;
// using System.Threading;
// using System.Text.RegularExpressions;
namespace k64t.Net.SMTP.Server{
public partial class SMTP_Session{
//*********************************************	
private void HELO(string argsText)		{
//*********************************************		
/* Rfc 2821 4.1.1.1
These commands, and a "250 OK" reply to one of them, confirm that
both the SMTP client and the SMTP server are in the initial state,
that is, there is no transaction in progress and all state tables and
buffers are cleared.
Syntax:
	 "HELO" SP Domain CRLF
*/

ResetState();

SendData("250 " + System.Net.Dns.GetHostName() + " Hello [" + m_RemoteHostIp + "]\r\n");
m_CmdValidator.Helo_ok = true;
}
}
}