using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Text;
using LumiSoft.Net;
using LumiSoft.Net.SMTP;

namespace LumiSoft.Net.SMTP.Server
{
	/// <summary>
	/// SMTP Session.
	/// </summary>
	public class SMTP_Session
	{
		private Socket        m_pClientSocket     = null;    // Referance to client Socket.
		private SMTP_Server   m_pSMTP_Server      = null;    // Referance to SMTP server.
		private string        m_SessionID         = "";      // Holds session ID.
		private string        m_UserName          = "";      // Holds loggedIn UserName.
		private string        m_ConnectedIp       = "";      // Holds connected computer's IP.
		private string        m_ConnectedHostName = "";      // Holds connected computer's name.
		private string        m_Reverse_path      = "";      // Holds sender's reverse path.
		private Hashtable     m_Forward_path      = null;    // Holds Mail to.
		private bool          m_Authenticated     = false;   // Holds authentication flag.
		private int           m_BadCmdCount       = 0;       // Holds number of bad commands.
		private BodyType      m_BodyType;
		private MemoryStream  m_MsgStream         = null;
		private bool          m_ChunkingErrors   = false;
//		private bool          m_Pipelining        = false;
		private DateTime      m_SessionStartTime;
		private SMTP_Cmd_Validator m_CmdValidator = null;
		private _LogWriter    m_pLogWriter        = null;
		private object        m_Tag               = null;

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="clientSocket">Referance to socket.</param>
		/// <param name="server">Referance to SMTP server.</param>
		/// <param name="sessionID">Session ID which is assigned to this session.</param>
		/// <param name="logWriter">Log writer.</param>
		internal SMTP_Session(Socket clientSocket,SMTP_Server server,string sessionID,_LogWriter logWriter)
		{			
			m_pClientSocket    = clientSocket;
			m_pSMTP_Server     = server;
			m_SessionID        = sessionID;
			m_BodyType         = BodyType.x7_bit;
			m_Forward_path     = new Hashtable();
			m_CmdValidator     = new SMTP_Cmd_Validator();
			m_pLogWriter       = logWriter;
			m_SessionStartTime = DateTime.Now;
		}

		
		#region function StartProcessing

		/// <summary>
		/// Starts session processing.
		/// </summary>
		public void StartProcessing()
		{								
			try
			{
				// Store client ip and host name.
				m_ConnectedIp = Core.ParseIP_from_EndPoint(m_pClientSocket.RemoteEndPoint.ToString());				
				m_ConnectedHostName = Core.GetHostName(m_ConnectedIp);
				
				// Check if ip is allowed to connect this computer
				if(m_pSMTP_Server.OnValidate_IpAddress(this.RemoteEndPoint)){
 
					// Notify that server is ready
					SendData("220 " + System.Net.Dns.GetHostName() + " Service ready\r\n");
			
					//------ Create command loop --------------------------------//
					// Loop while QUIT cmd or Session TimeOut.
					long lastCmdTime = DateTime.Now.Ticks;
					string lastCmd  = "";
					while(true)
					{	
						// If there is any data available, begin command reading.			
						if(m_pClientSocket.Available > 0){
							try
							{
								lastCmd = ReadLine();
								if(SwitchCommand(lastCmd)){
									break;
								}
							}							
							catch(ReadException rX){
								//---- Check that maximum bad commands count isn't exceeded ---------------//
								if(m_BadCmdCount > m_pSMTP_Server.MaxBadCommands-1){
									SendData("421 Too many bad commands, closing transmission channel\r\n");
									break;
								}
								m_BadCmdCount++;
								//-------------------------------------------------------------------------//

								switch(rX.ReadReplyCode){
									case ReadReplyCode.LengthExceeded:
										SendData("500 Line too long.\r\n");
										break;

									case ReadReplyCode.TimeOut:
										SendData("500 Command timeout.\r\n");
										break;

									case ReadReplyCode.UnKnownError:
										SendData("500 UnKnown Error.\r\n");

										m_pSMTP_Server.OnSysError(new Exception(rX.Message),new System.Diagnostics.StackTrace());										
										break;
								}
							}
							catch(Exception x){
								// Connection lost
								if(!m_pClientSocket.Connected){
									break;
								}
								
								SendData("500 Unkown temp error\r\n");
								m_pSMTP_Server.OnSysError(x,new System.Diagnostics.StackTrace());
							}

							// reset last command time
							lastCmdTime = DateTime.Now.Ticks;
						}
						else{
							//----- Session timeout stuff ------------------------------------------------//
							if(DateTime.Now.Ticks > lastCmdTime + ((long)(m_pSMTP_Server.SessionIdleTimeOut)) * 10000){						
								// Notify for closing
								SendData("421 Session timeout, closing transmission channel\r\n");
								break;
							}
						
							// Wait 100ms to save CPU, otherwise while loop may take 100% CPU. 
							Thread.Sleep(100);
							//---------------------------------------------------------------------------//
						}
					}
				}							
			}
			catch(ThreadInterruptedException e){
				string dummy = e.Message;     // Needed for to remove compile warning
			}
			catch(Exception x){
				if(m_pClientSocket.Connected){
					SendData("421 Service not available, closing transmission channel\r\n");

					m_pSMTP_Server.OnSysError(x,new System.Diagnostics.StackTrace());
				}
				else{
					m_pLogWriter.AddEntry("Connection is aborted by client machine",this.SessionID,m_ConnectedIp,"x");
				}				
			}
			finally{				
				m_pSMTP_Server.RemoveSession(this.SessionID,m_pLogWriter);
				
				// Write logs to log file, if needed
				if(m_pSMTP_Server.LogCommands){
					m_pLogWriter.Flush();
				}

				if(m_pClientSocket.Connected){
					m_pClientSocket.Close();
				}
			}
		}

		#endregion

		#region function SwitchCommand

		/// <summary>
		/// Executes SMTP command.
		/// </summary>
		/// <param name="SMTP_commandTxt">Original command text.</param>
		/// <returns>Returns true if must end session(command loop).</returns>
		private bool SwitchCommand(string SMTP_commandTxt)
		{
			//---- Parse command --------------------------------------------------//
			string[] cmdParts = SMTP_commandTxt.TrimStart().Split(new char[]{' '});
			string SMTP_command = cmdParts[0].ToUpper().Trim();
			string argsText = Core.GetArgsText(SMTP_commandTxt,SMTP_command);
			//---------------------------------------------------------------------//

			switch(SMTP_command)
			{
				case "HELO":
					HELO(argsText);
					break;

				case "EHLO":
					EHLO(argsText);
					break;

				case "AUTH":
					AUTH(argsText);
					break;

				case "MAIL":
					MAIL(argsText);
					break;
					
				case "RCPT":
					RCPT(argsText);
					break;

				case "DATA":
					DATA(argsText);
					break;

				case "BDAT":
					BDAT(argsText);
					break;

				case "RSET":
					RSET(argsText);
					break;

				case "VRFY":
					VRFY();
					break;

				case "EXPN":
					EXPN();
					break;

				case "HELP":
					HELP();
					break;

				case "NOOP":
					NOOP();
				break;
				
				case "QUIT":
					QUIT(argsText);
					return true;
										
				default:					
					SendData("500 command unrecognized\r\n");

					//---- Check that maximum bad commands count isn't exceeded ---------------//
					if(m_BadCmdCount > m_pSMTP_Server.MaxBadCommands-1){
						SendData("421 Too many bad commands, closing transmission channel\r\n");
						return true;
					}
					m_BadCmdCount++;
					//-------------------------------------------------------------------------//

					break;				
			}
			
			return false;
		}

		#endregion


		#region function HELO

		private void HELO(string argsText)
		{
			/* Rfc 2821 4.1.1.1
			These commands, and a "250 OK" reply to one of them, confirm that
			both the SMTP client and the SMTP server are in the initial state,
			that is, there is no transaction in progress and all state tables and
			buffers are cleared.
			
			Syntax:
				 "HELO" SP Domain CRLF
			*/

			ResetState();

			SendData("250 " + System.Net.Dns.GetHostName() + " Hello [" + m_ConnectedIp + "]\r\n");
			m_CmdValidator.Helo_ok = true;
		}

		#endregion

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
				"250-" + System.Net.Dns.GetHostName() + " Hello [" + m_ConnectedIp + "]\r\n" +
				"250-PIPELINING\r\n" +
				"250-SIZE " + m_pSMTP_Server.MaxMessageSize + "\r\n" +
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

		#region function AUTH

		private void AUTH(string argsText)
		{
			/* Rfc 2554 AUTH --------------------------------------------------//
			Restrictions:
		         After an AUTH command has successfully completed, no more AUTH
				 commands may be issued in the same session.  After a successful
				 AUTH command completes, a server MUST reject any further AUTH
				 commands with a 503 reply.
				 
			Remarks: 
				If an AUTH command fails, the server MUST behave the same as if
				the client had not issued the AUTH command.
			*/
			if(m_Authenticated){
				SendData("503 already authenticated\r\n");
				return;
			}
			
				
			//------ Parse parameters -------------------------------------//
			string userName = "";
			string password = "";

			string[] param = argsText.Split(new char[]{' '});
			switch(param[0].ToUpper())
			{
				case "PLAIN":
					SendData("504 Unrecognized authentication type.\r\n");
					break;

				case "LOGIN":

					#region LOGIN authentication

				    //---- AUTH = LOGIN ------------------------------
					/* Login
					C: AUTH LOGIN-MD5
					S: 334 VXNlcm5hbWU6
					C: username_in_base64
					S: 334 UGFzc3dvcmQ6
					C: password_in_base64
					
					   VXNlcm5hbWU6 base64_decoded= USERNAME
					   UGFzc3dvcmQ6 base64_decoded= PASSWORD
					*/
					// Note: all strings are base64 strings eg. VXNlcm5hbWU6 = UserName.
			
					
					// Query UserName
					SendData("334 VXNlcm5hbWU6\r\n");

					string userNameLine = ReadLine();
					// Encode username from base64
					if(userNameLine.Length > 0){
						userName = System.Text.Encoding.Default.GetString(Convert.FromBase64String(userNameLine));
					}
						
					// Query Password
					SendData("334 UGFzc3dvcmQ6\r\n");

					string passwordLine = ReadLine();
					// Encode password from base64
					if(passwordLine.Length > 0){
						password = System.Text.Encoding.Default.GetString(Convert.FromBase64String(passwordLine));
					}
																							
					if(m_pSMTP_Server.OnAuthUser(this,userName,password,"",AuthType.Plain)){
						SendData("235 Authentication successful.\r\n");
						m_Authenticated = true;
						m_UserName = userName;
					}
					else{
						SendData("535 Authentication failed\r\n");
					}

					#endregion

					break;

				case "CRAM-MD5":
					
					#region CRAM-MD5 authentication

					/* Cram-M5
					C: AUTH CRAM-MD5
					S: 334 <md5_calculation_hash_in_base64>
					C: base64(decoded:username password_hash)
					*/
					
					string md5Hash = "<" + Guid.NewGuid().ToString().ToLower() + ">";
					SendData("334 " + Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(md5Hash)) + "\r\n");

					string reply = this.ReadLine();

					reply = System.Text.Encoding.Default.GetString(Convert.FromBase64String(reply));
					string[] replyArgs = reply.Split(' ');
					userName = replyArgs[0];
					
					if(m_pSMTP_Server.OnAuthUser(this,userName,replyArgs[1],md5Hash,AuthType.CRAM_MD5)){
						SendData("235 Authentication successful.\r\n");
						m_Authenticated = true;
						m_UserName = userName;
					}
					else{
						SendData("535 Authentication failed\r\n");
					}

					#endregion

					break;

				case "DIGEST-MD5":

					/*	string md5Hash1 = "<" + Guid.NewGuid().ToString().ToLower() + ">";
						SendData("334 " + Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(md5Hash1)) + "\r\n");

						string reply1 = ReadLine();

						reply1 = System.Text.Encoding.Default.GetString(Convert.FromBase64String(reply1));
						m_pLogWriter.AddEntry(reply1 + "<CRLF>",this.SessionID,m_ConnectedIp,"C");
*/
					// ToDo: can't find any examples ???
					SendData("504 Unrecognized authentication type.\r\n");
					break;

				default:
					SendData("504 Unrecognized authentication type.\r\n");
					break;
			}
			//-----------------------------------------------------------------//
		}

		#endregion

		#region function MAIL

		private void MAIL(string argsText)
		{
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
			if(m_pSMTP_Server.MaxMessageSize > messageSize){
				// Check if sender is ok
				if(m_pSMTP_Server.OnValidate_MailFrom(this,reverse_path,senderEmail)){		
					SendData("250 OK <" + senderEmail + "> Sender ok\r\n");
										
					// See note above
					ResetState();

					// Store reverse path
					m_Reverse_path = reverse_path;
					m_CmdValidator.MailFrom_ok = true;

					//-- Store params
					m_BodyType = bodyType;
				}			
				else{				
					SendData("550 You are refused to send mail here\r\n");
				}
			}
			else{
				SendData("552 Message exceeds allowed size\r\n");
			}			
		}

		#endregion

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
			if(m_Forward_path.Count > m_pSMTP_Server.MaxRecipients){
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
			if(m_pSMTP_Server.MaxMessageSize > messageSize){
				// Check if email address is ok
				if(m_pSMTP_Server.OnValidate_MailTo(this,forward_path,recipientEmail,m_Authenticated)){

					// Check if mailbox size isn't exceeded
					if(m_pSMTP_Server.Validate_MailBoxSize(this,recipientEmail,messageSize)){
						// Store reciptient
						if(!m_Forward_path.Contains(recipientEmail)){
							m_Forward_path.Add(recipientEmail,forward_path);
						}				
						
						SendData("250 OK <" + recipientEmail + "> Recipient ok\r\n");
						m_CmdValidator.RcptTo_ok = true;
					}
					else{					
						SendData("552 Mailbox size limit exceeded\r\n");
					}
				}
				else{				
					SendData("550 <" + recipientEmail + "> No such user here\r\n");				
				}
			}
			else{
				SendData("552 Message exceeds allowed size\r\n");
			}
		}

		#endregion

		#region function DATA

		private void DATA(string argsText)
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

			if(m_Forward_path.Count == 0){
				SendData("554 no valid recipients given\r\n");
				return;
			}

			// reply: 354 Start mail input
			SendData("354 Start mail input; end with <CRLF>.<CRLF>\r\n");
				
			//---- Construct server headers -------------------------------------------------------------------//
			byte[] headers = null;
			string header  = "Received: from " + this.m_ConnectedHostName + " (" + this.m_ConnectedIp + ")\r\n"; 
				   header += "\tby " + System.Net.Dns.GetHostName() + " with SMTP; " + DateTime.Now.ToUniversalTime().ToString("r",System.Globalization.DateTimeFormatInfo.InvariantInfo) + "\r\n";
				    
			headers = System.Text.Encoding.ASCII.GetBytes(header.ToCharArray());
			//-------------------------------------------------------------------------------------------------//

			MemoryStream reply = null;
			ReadReplyCode replyCode = Core.ReadData(m_pClientSocket,out reply,headers,m_pSMTP_Server.MaxMessageSize,m_pSMTP_Server.CommandIdleTimeOut,"\r\n.\r\n",".\r\n");
			if(replyCode == ReadReplyCode.Ok){
				long recivedCount = reply.Length;
				//------- Do period handling and raise store event  --------//
				// If line starts with '.', mail client adds additional '.',
				// remove them.
				using(MemoryStream msgStream = Core.DoPeriodHandling(reply,false)){
					reply.Close();

					// Raise NewMail event
					m_pSMTP_Server.OnStoreMessage(this,msgStream);

					if(m_pSMTP_Server.LogCommands){
						m_pLogWriter.AddEntry("big binary " + recivedCount.ToString() + " bytes" + "",this.SessionID,m_ConnectedIp,"S");
					}

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

		#region function BDAT

		private void BDAT(string argsText)
		{
			/*RFC 3030 2
				The BDAT verb takes two arguments.  The
				first argument indicates the length, in octets, of the binary data
				chunk.  The second optional argument indicates that the data chunk
				is the last.
				
				The message data is sent immediately after the trailing <CR>
				<LF> of the BDAT command line.  Once the receiver-SMTP receives the
				specified number of octets, it will return a 250 reply code.

				The optional LAST parameter on the BDAT command indicates that this
				is the last chunk of message data to be sent.  The last BDAT command
				MAY have a byte-count of zero indicating there is no additional data
				to be sent.  Any BDAT command sent after the BDAT LAST is illegal and
				MUST be replied to with a 503 "Bad sequence of commands" reply code.
				The state resulting from this error is indeterminate.  A RSET command
				MUST be sent to clear the transaction before continuing.
				
				A 250 response MUST be sent to each successful BDAT data block within
				a mail transaction.

				bdat-cmd   ::= "BDAT" SP chunk-size [ SP end-marker ] CR LF
				chunk-size ::= 1*DIGIT
				end-marker ::= "LAST"
			*/

			if(!m_CmdValidator.MayHandle_BDAT){
				SendData("503 Bad sequence of commands\r\n");
				return;
			}
			
			string[] param = argsText.Split(new char[]{' '});
			if(param.Length > 0 && param.Length < 3){				
				if(Core.IsNumber(param[0])){
					long chunkSize = Convert.ToInt64(param[0]);

					if(m_MsgStream == null){
						m_MsgStream = new MemoryStream();
					}

					// Check if message size isn't exceeded
					if(!m_ChunkingErrors && ((m_MsgStream.Length + chunkSize) > m_pSMTP_Server.MaxMessageSize)){
						m_ChunkingErrors = true;
						SendData("552 Requested mail action aborted: exceeded storage allocation\r\n");
					}

					// Append chunck part to stream
					ReadReplyCode replyCode = Core.ReadData(m_pClientSocket,chunkSize,m_MsgStream,!m_ChunkingErrors,m_pSMTP_Server.CommandIdleTimeOut);
					if(!m_ChunkingErrors){
						switch(replyCode){
							case ReadReplyCode.Ok:
								SendData("250 " + chunkSize.ToString() + " octets received\r\n");
								break;
							case ReadReplyCode.TimeOut:
								m_ChunkingErrors = true;
								SendData("500 TimeOut\r\n");
								break;
							case ReadReplyCode.UnKnownError:
								m_ChunkingErrors = true;
								SendData("500 UnKnownError\r\n");
								break;
						}
					}
					else{
						SendData("500 See previous chuncking errors\r\n");
					}

					// LAST specified
					if(param.Length == 2){
						m_CmdValidator.BDAT_Last_ok = true;
						if(replyCode == ReadReplyCode.Ok != m_ChunkingErrors){
							// Raise NewMail event
							m_pSMTP_Server.OnStoreMessage(this,m_MsgStream);
						}

						// Close stream
						m_MsgStream.Close();
						m_MsgStream = null;

						// 
						ResetState();
					}
				}
				else{
					SendData("500 Syntax error. Syntax:{BDAT chunk-size [LAST]}\r\n");
				}
			}
			else{
				SendData("500 Syntax error. Syntax:{BDAT chunk-size [LAST]}\r\n");
			}

		}

		#endregion

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

		#region function VRFY

		private void VRFY()
		{
			/* RFC 821 VRFY 
			Example:
				S: VRFY Lumi
				R: 250 Ivar Lumi <ivx@lumisoft.ee>
				
				S: VRFY lum
				R: 550 String does not match anything.			 
			*/

			// ToDo: Parse user, add new event for cheking user

		//	SendData("250 OK\r\n");

			SendData("502 Command not implemented\r\n");
		}

		#endregion

		#region function NOOP

		private void NOOP()
		{
			/* RFC 2821 4.1.1.9 NOOP (NOOP)
			NOTE:
				This command does not affect any parameters or previously entered
				commands.  It specifies no action other than that the receiver send
				an OK reply.
			*/

			SendData("250 OK\r\n");
		}

		#endregion

		#region function QUIT

		private void QUIT(string argsText)
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

		#endregion


		//---- Optional commands
		
		#region function EXPN

		private void EXPN()
		{
			/* RFC 821 EXPN 
			NOTE:
				This command asks the receiver to confirm that the argument
				identifies a mailing list, and if so, to return the
				membership of that list.  The full name of the users (if
				known) and the fully specified mailboxes are returned in a
				multiline reply.
			
			Example:
				S: EXPN lsAll
				R: 250-ivar lumi <ivx@lumisoft.ee>
				R: 250-<willy@lumisoft.ee>
				R: 250 <kaido@lumisoft.ee>
			*/

	//		SendData("250 OK\r\n");

			SendData("502 Command not implemented\r\n");
		}

		#endregion

		#region function HELP

		private void HELP()
		{
			/* RFC 821 HELP
			NOTE:
				This command causes the receiver to send helpful information
				to the sender of the HELP command.  The command may take an
				argument (e.g., any command name) and return more specific
				information as a response.
			*/

	//		SendData("250 OK\r\n");

			SendData("502 Command not implemented\r\n");
		}

		#endregion


		#region function ResetState

		private void ResetState()
		{
			//--- Reset variables
			m_BodyType = BodyType.x7_bit;
			m_Forward_path.Clear();
			m_Reverse_path  = "";
	//		m_Authenticated = false; // ??? must clear or not, no info.
			m_CmdValidator.Reset();
			m_CmdValidator.Helo_ok = true;
		}

		#endregion


		#region function SendData
			
		/// <summary>
		/// Sends data to socket.
		/// </summary>
		/// <param name="data">String data wich to send.</param>
		private void SendData(string data)
		{	
			Byte[] byte_data = System.Text.Encoding.ASCII.GetBytes(data.ToCharArray());
			
			int nCount = m_pClientSocket.Send(byte_data,byte_data.Length,0);
			if(nCount != byte_data.Length){
				throw new Exception("Smtp.SendData sended less data than requested !");
			}

			if(m_pSMTP_Server.LogCommands){
				data = data.Replace("\r\n","<CRLF>");
				m_pLogWriter.AddEntry(data,this.SessionID,m_ConnectedIp,"S");
			}
		}

		#endregion

		#region function ReadLine

		/// <summary>
		/// Reads line from socket.
		/// </summary>
		/// <returns></returns>
		private string ReadLine()
		{
			string line = Core.ReadLine(m_pClientSocket,500,m_pSMTP_Server.CommandIdleTimeOut);
				
			if(m_pSMTP_Server.LogCommands){
				m_pLogWriter.AddEntry(line + "<CRLF>",this.SessionID,m_ConnectedIp,"C");
			}

			return line;
		}

		#endregion
	

		#region Properties Implementation
		
		/// <summary>
		/// Gets session ID.
		/// </summary>
		public string SessionID
		{
			get{ return m_SessionID; }
		}

		/// <summary>
		/// Gets if session authenticated.
		/// </summary>
		public bool Authenticated
		{
			get{ return m_Authenticated; }
		}

		/// <summary>
		/// Gets loggded in user name (session owner).
		/// </summary>
		public string UserName
		{
			get{ return m_UserName; }
		}

		/// <summary>
		/// Gets body type.
		/// </summary>
		public BodyType BodyType
		{
			get{ return m_BodyType; }
		}

		/// <summary>
		/// Gets local EndPoint which accepted client(connected host).
		/// </summary>
		public EndPoint LocalEndPoint
		{
			get{ return m_pClientSocket.LocalEndPoint; }
		}

		/// <summary>
		/// Gets connected Host(client) EndPoint.
		/// </summary>
		public EndPoint RemoteEndPoint
		{
			get{ return m_pClientSocket.RemoteEndPoint; }
		}

		/// <summary>
		/// Gets sender.
		/// </summary>
		public string MailFrom
		{
			get{ return m_Reverse_path; }
		}

		/// <summary>
		/// Gets recipients.
		/// </summary>
		public string[] MailTo
		{
			get{
				string[] to = new string[m_Forward_path.Count];
				m_Forward_path.Values.CopyTo(to,0);

				return to; 
			}
		}

		/// <summary>
		/// Gets session start time.
		/// </summary>
		public DateTime SessionStartTime
		{
			get{ return m_SessionStartTime; }
		}

		/// <summary>
		/// Gets or sets custom user data.
		/// </summary>
		public object Tag
		{
			get{ return m_Tag; }

			set{ m_Tag = value; }
		}

		#endregion

	}
}
