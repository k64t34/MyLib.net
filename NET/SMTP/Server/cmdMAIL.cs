﻿using System;
namespace k64t.Net.SMTP.Server{
public partial class SMTP_Session{
private void MAIL(string argsText){
/* RFC 2821 3.3
NOTE:
	This command tells the SMTP-receiver that a new mail transaction is
	starting and to reset all its state tables and buffers, including any
	recipients or mail data.  The <reverse-path> portion of the first or
	only argument contains the source mailbox (between "<" and ">"
	brackets), which can be used to report errors (see section 4.2 for a
	discussion of error reporting).  If accepted, the SMTP server returns
	 a 250 OK reply.
	 
	MAIL FROM:<reverse-path> [SP <mail-parameters> ] <CRLF>
	reverse-path = "<" [ A-d-l ":" ] Mailbox ">"
	Mailbox = Local-part "@" Domain
	
	body-value ::= "7BIT" / "8BITMIME" / "BINARYMIME"
	
	Examples:
		C: MAIL FROM:<ned@thor.innosoft.com>
		C: MAIL FROM:<ned@thor.innosoft.com> SIZE=500000 BODY=8BITMIME
*/

	if(!m_CmdValidator.MayHandle_MAIL){
		if(m_CmdValidator.MailFrom_ok){
			SendData("503 Sender already specified\r\n");
		}
		else{
			SendData("503 Bad sequence of commands\r\n");
		}
		return;
	}

	//------ Parse parameters -------------------------------------------------------------------//
	string   reverse_path = "";
	string   senderEmail  = "";
	long     messageSize  = 0;
	BodyType bodyType     = BodyType.x7_bit;
	bool     isFromParam  = false;

	//--- regex param parse strings
	string[] exps = new string[3];
	exps[0] = @"(?<param>FROM)[\s]{0,}:\s{0,}<?\s{0,}(?<value>[\w\@\.\-\*\+\=\#\/]*)\s{0,}>?(\s|$)";
	exps[1] = @"(?<param>SIZE)[\s]{0,}=\s{0,}(?<value>[\w]*)(\s|$)"; 
	exps[2] = @"(?<param>BODY)[\s]{0,}=\s{0,}(?<value>[\w]*)(\s|$)";

	_Parameter[] param = _ParamParser.Paramparser_NameValue(argsText,exps);
	foreach(_Parameter parameter in param){
		// Possible params:
		// FROM:
		// SIZE=
		// BODY=
		switch(parameter.ParamName.ToUpper()) 
		{
			//------ Required paramters -----//
			case "FROM":
		//		if(parameter.ParamValue.Length == 0){
		//			SendData("501 Sender address isn't specified. Syntax:{MAIL FROM:<address> [SIZE=msgSize]}\r\n");
		//			return;
		//		}
		//		else{
					reverse_path = parameter.ParamValue;
					isFromParam = true;
		//		}
				break;

			//------ Optional parameters ---------------------//
			case "SIZE":
				if(parameter.ParamValue.Length == 0){
					SendData("501 SIZE parameter value isn't specified. Syntax:{MAIL FROM:<address> [SIZE=msgSize] [BODY=8BITMIME]}\r\n");
					return;
				}
				else{
					if(Core.IsNumber(parameter.ParamValue)){
						messageSize = Convert.ToInt64(parameter.ParamValue);
					}
					else{
						SendData("501 SIZE parameter value is invalid. Syntax:{MAIL FROM:<address> [SIZE=msgSize] [BODY=8BITMIME]}\r\n");
					}
				}
				break;

			case "BODY":
				if(parameter.ParamValue.Length == 0){
					SendData("501 BODY parameter value isn't specified. Syntax:{MAIL FROM:<address> [SIZE=msgSize] [BODY=8BITMIME]}\r\n");
					return;
				}
				else{
					switch(parameter.ParamValue.ToUpper()){
						case "7BIT":
							bodyType = BodyType.x7_bit;
							break;
						case "8BITMIME":
							bodyType = BodyType.x8_bit;
							break;
						case "BINARYMIME":
							bodyType = BodyType.binary;									
							break;
						default:
							SendData("501 BODY parameter value is invalid. Syntax:{MAIL FROM:<address> [BODY=(7BIT/8BITMIME)]}\r\n");
							return;
					}
				}
				break;

			default:
				SendData("501 Error in parameters. Syntax:{MAIL FROM:<address> [SIZE=msgSize] [BODY=8BITMIME]}\r\n");
				return;				
		}
	}
	
	// If required parameter 'FROM:' is missing
	if(!isFromParam){
		SendData("501 Required param FROM: is missing. Syntax:{MAIL FROM:<address> [SIZE=msgSize] [BODY=8BITMIME]}\r\n");
		return;
	}

	// Parse sender's email address
	senderEmail = reverse_path;
	//---------------------------------------------------------------------------------------------//
	
	//--- Check message size
	if(Server.MaxMessageSize > messageSize){
		// Check if sender is ok
		//if(Server.OnValidate_MailFrom(this,reverse_path,senderEmail)){		
			SendData("250 OK <" + senderEmail + "> Sender ok\r\n");
								
			// See note above
			ResetState();

			// Store reverse path
			//m_Reverse_path = reverse_path;
			m_CmdValidator.MailFrom_ok = true;

			//-- Store params
			m_BodyType = bodyType;
		//}			
		//else{				
		//	SendData("550 You are refused to send mail here\r\n");
		//}
	}
	else{
		SendData("552 Message exceeds allowed size\r\n");
	}			
}
}
}