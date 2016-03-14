﻿// using System;
// using System.Net.Sockets;
// using System.Collections;
// using System.Threading;
// using System.Text.RegularExpressions;
namespace k64t.Net.SMTP.Server{
public partial class SMTP_Session{
//*********************************************	
private void QUIT(string argsText){
//*********************************************		
/* RFC 2821 4.1.1
NOTE:
	Several commands (RSET, DATA, QUIT) are specified as not permitting
	parameters.  In the absence of specific extensions offered by the
	server and accepted by the client, clients MUST NOT send such
	parameters and servers SHOULD reject commands containing them as
	having invalid syntax.
*/
if(argsText.Length > 0){
	SendData("500 Syntax error. Syntax:<QUIT>\r\n");
	return;
}
/* RFC 2821 4.1.1.10 QUIT (QUIT)
NOTE:
	This command specifies that the receiver MUST send an OK reply, and
	then close the transmission channel.
*/

// reply: 221 - Close transmission cannel
SendData("221 Service closing transmission channel\r\n");			
}
}
}