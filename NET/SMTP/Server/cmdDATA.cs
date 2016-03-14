using System;
using System.IO;
// using System.Net.Sockets;
//using System.Collections;
// using System.Threading;
// using System.Text.RegularExpressions;
namespace k64t.Net.SMTP.Server{
public partial class SMTP_Session{
#region function DATA

private void DATA(string argsText){
	/* RFC 2821 4.1.1
	NOTE:
		Several commands (RSET, DATA, QUIT) are specified as not permitting
		parameters.  In the absence of specific extensions offered by the
		server and accepted by the client, clients MUST NOT send such
		parameters and servers SHOULD reject commands containing them as
		having invalid syntax.
	*/

	if(argsText.Length > 0){
		SendData("500 Syntax error. Syntax:{DATA}\r\n");
		return;
	}


	/* RFC 2821 4.1.1.4 DATA
	NOTE:
		If accepted, the SMTP server returns a 354 Intermediate reply and
		considers all succeeding lines up to but not including the end of
		mail data indicator to be the message text.  When the end of text is
		successfully received and stored the SMTP-receiver sends a 250 OK
		reply.
		
		The mail data is terminated by a line containing only a period, that
		is, the character sequence "<CRLF>.<CRLF>" (see section 4.5.2).  This
		is the end of mail data indication.
			
		
		When the SMTP server accepts a message either for relaying or for
		final delivery, it inserts a trace record (also referred to
		interchangeably as a "time stamp line" or "Received" line) at the top
		of the mail data.  This trace record indicates the identity of the
		host that sent the message, the identity of the host that received
		the message (and is inserting this time stamp), and the date and time
		the message was received.  Relayed messages will have multiple time
		stamp lines.  Details for formation of these lines, including their
		syntax, is specified in section 4.4.

	*/


	/* RFC 2821 DATA
	NOTE:
		If there was no MAIL, or no RCPT, command, or all such commands
		were rejected, the server MAY return a "command out of sequence"
		(503) or "no valid recipients" (554) reply in response to the DATA
		command.
	*/
	if(!m_CmdValidator.MayHandle_DATA){
		SendData("503 Bad sequence of commands\r\n");
		return;
	}

	if(Forward_path.Count == 0){
		SendData("554 no valid recipients given\r\n");
		return;
	}

	// reply: 354 Start mail input
	SendData("354 Start mail input; end with <CRLF>.<CRLF>\r\n");
		
	//---- Construct server headers -------------------------------------------------------------------//
	byte[] headers = null;
	string header  = "Received: from " + this.m_RemoteHostName + " (" + this.m_RemoteHostIp + ")\r\n"; 
		   header += "\tby " + System.Net.Dns.GetHostName() + " with SMTP; " + DateTime.Now.ToUniversalTime().ToString("r",System.Globalization.DateTimeFormatInfo.InvariantInfo) + "\r\n";
			
	headers = System.Text.Encoding.ASCII.GetBytes(header.ToCharArray());
	//-------------------------------------------------------------------------------------------------//

	MemoryStream reply = null;
	ReadReplyCode replyCode = k64t.Net.SocketIO.ReadData(ClientSocket,out reply,headers,Server.MaxMessageSize,Server.vSessionTimeout,"\r\n.\r\n",".\r\n");
	if(replyCode == ReadReplyCode.Ok){
		long recivedCount = reply.Length;
		//------- Do period handling and raise store event  --------//
		// If line starts with '.', mail client adds additional '.',
		// remove them.
		using(MemoryStream msgStream = Core.DoPeriodHandling(reply,false)){
			reply.Close();

			// Raise NewMail event
			Server.OnStoreMessage(this,msgStream);			
			Server.ServerLog(String.Format("big binary " + recivedCount.ToString() + " bytes" + "",this.SessionID,m_RemoteHostIp,"S"));		

			// Send ok - got message
			SendData("250 OK\r\n");
		}
		//----------------------------------------------------------//
								
		
		/* RFC 2821 4.1.1.4 DATA
		NOTE:
			Receipt of the end of mail data indication requires the server to
			process the stored mail transaction information.  This processing
			consumes the information in the reverse-path buffer, the forward-path
			buffer, and the mail data buffer, and on the completion of this
			command these buffers are cleared.
		*/
		ResetState();
	}
	else{
		if(replyCode == ReadReplyCode.LengthExceeded){
			SendData("552 Requested mail action aborted: exceeded storage allocation\r\n");
		}
		else{
			SendData("500 Error mail not terminated with '.'\r\n");						
		}
	}
}

#endregion
}
}