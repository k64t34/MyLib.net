using System;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using System.Text.RegularExpressions;
namespace k64t.Net.SMTP.Server{
//*********************************************
public partial class SMTP_Session{
//*********************************************
public Socket  ClientSocket     = null;
private string SessionID = null;
private SMTPServer Server = null;
private string        m_RemoteHostIp   = ""; 
private string        m_RemoteHostName = ""; 
//private string        m_Reverse_path      = "";      // Holds sender's reverse path.
private Hashtable     Forward_path      = null;    // Holds Mail to.
//private bool          m_Authenticated     = false;   // Holds authentication flag.
//private int           m_BadCmdCount       = 0;       // Holds number of bad commands.
private BodyType      m_BodyType;
//private MemoryStream  m_MsgStream         = null;
//private bool          m_ChunkingErrors   = false;
//		private bool          m_Pipelining        = false;
//private DateTime      m_SessionStartTime;
private SMTP_Cmd_Validator m_CmdValidator = null;
//private _LogWriter    m_pLogWriter        = null;
//private object        m_Tag               = null;

//*********************************************
internal SMTP_Session(Socket clientSocket,SMTPServer server,string sessionID)		{			
//*********************************************	
ClientSocket	= clientSocket;
Server   		= server;
SessionID       = sessionID;
//*m_BodyType         = BodyType.x7_bit;
Forward_path     = new Hashtable();
m_CmdValidator     = new SMTP_Cmd_Validator();
//m_pLogWriter       = logWriter;
//m_SessionStartTime = DateTime.Now;*/
}	
//*********************************************		
//public IPAddress GetClientIP() {
//*********************************************	
//}
//*********************************************
public void StartProcessing() {
//*********************************************	
#if (DEBUG)
Console.WriteLine("StartProcessing");
#endif
try
	{
	m_RemoteHostIp = k64t.Net.SocketIO.ParseIP_from_EndPoint(ClientSocket.RemoteEndPoint.ToString());				
	m_RemoteHostName = k64t.Net.SocketIO.GetHostName(m_RemoteHostIp);
	SendData("220 " + System.Net.Dns.GetHostName() + " Service ready\r\n");
	long lastCmdTime = DateTime.Now.Ticks;	
	string lastCmd  = "";
	while(true)
		{
		if(ClientSocket.Available > 0)
			{
			try
				{
				lastCmd = ReadLine();
				if(SwitchCommand(lastCmd))break;				
				}
			catch
				{
				}
			lastCmdTime = DateTime.Now.Ticks;
			}
		else{
			//----- Session timeout stuff ------------------------------------------------//
			if(DateTime.Now.Ticks > lastCmdTime + Server.vSessionTimeout){				
				// Notify for closing
				Server.ServerLog(String.Format("{0} SMTP Server session {1} timeout",DateTime.Now,SessionID));
				SendData("421 Session timeout, closing transmission channel\r\n");
				break;
			}
		
			// Wait 100ms to save CPU, otherwise while loop may take 100% CPU. 
			Thread.Sleep(100);
			//---------------------------------------------------------------------------//
			}
		}
	}
catch
	{
	}
finally
	{
	//->m_pSMTP_Server.RemoveSession(this.SessionID,m_pLogWriter);	
	if(ClientSocket.Connected)ClientSocket.Close();			
	}
}
//*********************************************		
private bool SwitchCommand(string SMTP_commandTxt)		{
//*********************************************			
//---- Parse command --------------------------------------------------//
string[] cmdParts = SMTP_commandTxt.TrimStart().Split(new char[]{' '});
string SMTP_command = cmdParts[0].ToUpper().Trim();
string argsText = k64t.Net.SocketIO.GetArgsText(SMTP_commandTxt,SMTP_command);
//---------------------------------------------------------------------//

switch(SMTP_command)
{
	case "HELO":
		HELO(argsText);
		break;
	case "EHLO":
		EHLO(argsText);
		break;
/*	case "AUTH":
		AUTH(argsText);
		break;*/
	case "MAIL":
		MAIL(argsText);
		break;		
	case "RCPT":
		RCPT(argsText);
		break;
	case "DATA":
		DATA(argsText);
		break;

/*	case "BDAT":
		BDAT(argsText);
		break;*/

	case "RSET":
		RSET(argsText);
		break;

/*	case "VRFY":
		VRFY();
		break;

	case "EXPN":
		EXPN();
		break;

	case "HELP":
		HELP();
		break;*/

	case "NOOP":
		NOOP();
	break;
	
	case "QUIT":
		QUIT(argsText);
		return true;
							
	default:					
		SendData("500 command unrecognized\r\n");

		//---- Check that maximum bad commands count isn't exceeded ---------------//
		/*if(m_BadCmdCount > m_pSMTP_Server.MaxBadCommands-1){
			SendData("421 Too many bad commands, closing transmission channel\r\n");
			return true;
		}
		m_BadCmdCount++;*/
		//-------------------------------------------------------------------------//

		break;				
}

return false;
}
//*********************************************		
private void ResetState()		{
//*********************************************			
//--- Reset variables
//m_BodyType = BodyType.x7_bit;
//m_Forward_path.Clear();
//m_Reverse_path  = "";
//		m_Authenticated = false; // ??? must clear or not, no info.
m_CmdValidator.Reset();
m_CmdValidator.Helo_ok = true;
}		
//*********************************************	
private string ReadLine()		{
//*********************************************		
string line = SocketIO.ReadLine(ClientSocket,500,Server.vSessionTimeout);
Server.ServerLog(String.Format("{0} SMTP Server sesion {1}. Recive data {2} bytes\n<-{3}",DateTime.Now,SessionID,line.Length,line/*.Substring(0,128)*/));
return line;
}
//*********************************************
private void SendData(string data)		{	
//*********************************************	
Byte[] byte_data = System.Text.Encoding.ASCII.GetBytes(data.ToCharArray());
Server.ServerLog(String.Format("{0} SMTP Server session {1} send data {2} bytes \n->{3}",DateTime.Now,SessionID,byte_data.Length,data.Replace("\r\n","<CRLF>")));
int nCount = ClientSocket.Send(byte_data,byte_data.Length,0);
if(nCount != byte_data.Length)
	{
	Server.ServerLog(String.Format("{0} SMTP Server sesion {1} Smtp.SendData sended {2} bytes less data than requested {3} bytes !",DateTime.Now,SessionID,nCount,byte_data.Length));
	throw new Exception("Smtp.SendData sended less data than requested !");	
	}
}
}















































































internal class SMTP_Cmd_Validator
	{
		private bool m_Helo_ok       = false;
		private bool m_Authenticated = false;
		private bool m_MailFrom_ok   = false;
		private bool m_RcptTo_ok     = false;
		private bool m_BdatLast_ok   = false;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public SMTP_Cmd_Validator()
		{			
		}


		#region function Reset

		/// <summary>
		/// Resets state.
		/// </summary>
		public void Reset()
		{
			m_Helo_ok       = false;
			m_Authenticated = false;
			m_MailFrom_ok   = false;
			m_RcptTo_ok     = false;
			m_BdatLast_ok   = false;
		}

		#endregion


		#region Properties Implementation

		/// <summary>
		/// Gets if may handle MAIL command.
		/// </summary>
		public bool MayHandle_MAIL
		{
			get{ return m_Helo_ok && !MailFrom_ok; }
		}

		/// <summary>
		/// Gets if may handle RCPT command.
		/// </summary>
		public bool MayHandle_RCPT
		{
			get{ return MailFrom_ok; }
		}

		/// <summary>
		/// Gets if may handle DATA command.
		/// </summary>
		public bool MayHandle_DATA
		{
			get{ return RcptTo_ok; }
		}

		/// <summary>
		/// Gets if may handle BDAT command.
		/// </summary>
		public bool MayHandle_BDAT
		{
			get{ return RcptTo_ok && !m_BdatLast_ok; }
		}

		/// <summary>
		/// Gets if may handle AUTH command.
		/// </summary>
		public bool MayHandle_AUTH
		{
			get{ return !m_Authenticated; }
		}

		/// <summary>
		/// Gest or sets if HELO command handled.
		/// </summary>
		public bool Helo_ok
		{
			get{ return m_Helo_ok; }

			set{ m_Helo_ok = value; }
		}

		/// <summary>
		/// Gest or sets if AUTH command handled.
		/// </summary>
		public bool Authenticated
		{
			get{ return m_Authenticated; }

			set{ m_Authenticated = value; }
		}

		/// <summary>
		/// Gest or sets if MAIL command handled.
		/// </summary>
		public bool MailFrom_ok
		{
			get{ return m_MailFrom_ok; }

			set{ m_MailFrom_ok = value; }
		}

		/// <summary>
		/// Gest or sets if RCPT command handled.
		/// </summary>
		public bool RcptTo_ok
		{
			get{ return m_RcptTo_ok; }

			set{ m_RcptTo_ok = value; }
		}

		/// <summary>
		/// Gest or sets if BinaryMime.
		/// </summary>
		public bool BDAT_Last_ok
		{
			get{ return m_BdatLast_ok; }

			set{ m_BdatLast_ok = value; }
		}

		#endregion

	}

}