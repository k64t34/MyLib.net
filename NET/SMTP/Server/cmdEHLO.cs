using System;
namespace k64t.Net.SMTP.Server{
public partial class SMTP_Session{
#region function EHLO

		private void EHLO(string argsText)
		{		
			/* Rfc 2821 4.1.1.1
			These commands, and a "250 OK" reply to one of them, confirm that
			both the SMTP client and the SMTP server are in the initial state,
			that is, there is no transaction in progress and all state tables and
			buffers are cleared.
			*/

			ResetState();

			string reply = "" +
				"250-" + System.Net.Dns.GetHostName() + " Hello [" + m_RemoteHostIp + "]\r\n" +
				"250-PIPELINING\r\n" +
				"250-SIZE " + Server.MaxMessageSize + "\r\n" +
		//		"250-DSN\r\n"  +
		//		"250-HELP\r\n" +
				"250-8BITMIME\r\n" +
				"250-BINARYMIME\r\n" +
				"250-CHUNKING\r\n" +
				"250-AUTH LOGIN CRAM-MD5\r\n" + //CRAM-MD5 DIGEST-MD5
			    "250 Ok\r\n";
			
			SendData(reply);
				
			m_CmdValidator.Helo_ok = true;
		}

		#endregion
}
}

