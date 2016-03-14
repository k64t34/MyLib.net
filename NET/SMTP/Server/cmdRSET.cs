using System;
namespace k64t.Net.SMTP.Server{
public partial class SMTP_Session{
		#region function RSET

		private void RSET(string argsText)
		{
			/* RFC 2821 4.1.1
			NOTE:
				Several commands (RSET, DATA, QUIT) are specified as not permitting
				parameters.  In the absence of specific extensions offered by the
				server and accepted by the client, clients MUST NOT send such
				parameters and servers SHOULD reject commands containing them as
				having invalid syntax.
			*/

			if(argsText.Length > 0){
				SendData("500 Syntax error. Syntax:{RSET}\r\n");
				return;
			}

			/* RFC 2821 4.1.1.5 RESET (RSET)
			NOTE:
				This command specifies that the current mail transaction will be
				aborted.  Any stored sender, recipients, and mail data MUST be
				discarded, and all buffers and state tables cleared.  The receiver
				MUST send a "250 OK" reply to a RSET command with no arguments.
			*/
			
			ResetState();

			SendData("250 OK\r\n");
		}

		#endregion
}
}

