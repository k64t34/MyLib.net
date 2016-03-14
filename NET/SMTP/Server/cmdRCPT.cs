using System;
namespace k64t.Net.SMTP.Server{
public partial class SMTP_Session{
#region function RCPT

private void RCPT(string argsText)
{
/* RFC 2821 4.1.1.3 RCPT
NOTE:
	This command is used to identify an individual recipient of the mail
	data; multiple recipients are specified by multiple use of this
	command.  The argument field contains a forward-path and may contain
	optional parameters.
	
	Relay hosts SHOULD strip or ignore source routes, and
	names MUST NOT be copied into the reverse-path.  
	
	Example:
		RCPT TO:<@hosta.int,@jkl.org:userc@d.bar.org>

		will normally be sent directly on to host d.bar.org with envelope
		commands

		RCPT TO:<userc@d.bar.org>
		RCPT TO:<userc@d.bar.org> SIZE=40000
			
	RCPT TO:<forward-path> [ SP <rcpt-parameters> ] <CRLF>			
*/

/* RFC 2821 3.3
	If a RCPT command appears without a previous MAIL command, 
	the server MUST return a 503 "Bad sequence of commands" response.
*/
if(!m_CmdValidator.MayHandle_RCPT){
	SendData("503 Bad sequence of commands\r\n");				
	return;
}


// Check that recipient count isn't exceeded
if(Forward_path.Count > Server.vMaxRecipients){
	SendData("452 Too many recipients\r\n");
	return;
}


//------ Parse parameters -------------------------------------------------------------------//
string forward_path   = "";
string recipientEmail = "";
long   messageSize    = 0;
bool   isToParam      = false;

//--- regex param parse strings
string[] exps = new string[2];
exps[0] = @"(?<param>TO)[\s]{0,}:\s{0,}<?\s{0,}(?<value>[\w\@\.\-\*\+\=\#\/]*)\s{0,}>?(\s|$)";
exps[1] = @"(?<param>SIZE)[\s]{0,}=\s{0,}(?<value>[\w]*)(\s|$)"; 

_Parameter[] param = _ParamParser.Paramparser_NameValue(argsText,exps);
foreach(_Parameter parameter in param){
	// Possible params:
	// TO:
	// SIZE=				
	switch(parameter.ParamName.ToUpper()) // paramInf[0] because of param syntax: pramName =/: value
	{
		//------ Required paramters -----//
		case "TO":
			if(parameter.ParamValue.Length == 0){
				SendData("501 Recipient address isn't specified. Syntax:{RCPT TO:<address> [SIZE=msgSize]}\r\n");
				return;
			}
			else{
				forward_path = parameter.ParamValue;
				isToParam = true;
			}
			break;

		//------ Optional parameters ---------------------//
		case "SIZE":
			if(parameter.ParamValue.Length == 0){
				SendData("501 Size parameter isn't specified. Syntax:{RCPT TO:<address> [SIZE=msgSize]}\r\n");
				return;
			}
			else{
				if(Core.IsNumber(parameter.ParamValue)){
					messageSize = Convert.ToInt64(parameter.ParamValue);
				}
				else{
					SendData("501 SIZE parameter value is invalid. Syntax:{RCPT TO:<address> [SIZE=msgSize]}\r\n");
				}
			}
			break;

		default:
			SendData("501 Error in parameters. Syntax:{RCPT TO:<address> [SIZE=msgSize]}\r\n");
			return;
	}
}

// If required parameter 'TO:' is missing
if(!isToParam){
	SendData("501 Required param TO: is missing. Syntax:<RCPT TO:{address> [SIZE=msgSize]}\r\n");
	return;
}

// Parse recipient's email address
recipientEmail = forward_path;
//---------------------------------------------------------------------------------------------//


// Check message size
if(Server.MaxMessageSize <= messageSize){SendData("552 Message exceeds allowed size\r\n");}
// Check if email address is ok
//else if(!Server.OnValidate_MailTo(this,forward_path,recipientEmail,m_Authenticated)){SendData("550 <" + recipientEmail + "> No such user here\r\n");}
// Check if mailbox size isn't exceeded
//else if (!Server.Validate_MailBoxSize(this,recipientEmail,messageSize)){SendData("552 Mailbox size limit exceeded\r\n");}
// Store reciptient
if(!Forward_path.Contains(recipientEmail)){Forward_path.Add(recipientEmail,forward_path);}
SendData("250 OK <" + recipientEmail + "> Recipient ok\r\n");
m_CmdValidator.RcptTo_ok = true;		

}



#endregion
}
}

