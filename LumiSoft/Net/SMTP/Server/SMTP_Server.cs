using System;
using System.IO;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using LumiSoft.Net;

namespace LumiSoft.Net.SMTP.Server
{	
	#region Event delegates

	/// <summary>
	/// Represents the method that will handle the AuthUser event for SMTP_Server.
	/// </summary>
	/// <param name="sender">The source of the event. </param>
	/// <param name="e">A AuthUser_EventArgs that contains the event data.</param>
	public delegate void AuthUserEventHandler(object sender,AuthUser_EventArgs e);

	/// <summary>
	/// Represents the method that will handle the ValidateMailFrom event for POP3_Server.
	/// </summary>
	/// <param name="sender">The source of the event. </param>
	/// <param name="e">A ValidateSender_EventArgs that contains the event data.</param>
	public delegate void ValidateMailFromHandler(object sender,ValidateSender_EventArgs e);

	/// <summary>
	/// Represents the method that will handle the ValidateMailTo event for POP3_Server.
	/// </summary>
	/// <param name="sender">The source of the event. </param>
	/// <param name="e">A ValidateRecipient_EventArgs that contains the event data.</param>
	public delegate void ValidateMailToHandler(object sender,ValidateRecipient_EventArgs e);

	/// <summary>
	/// Represents the method that will handle the ValidateMailboxSize event for POP3_Server.
	/// </summary>
	/// <param name="sender">The source of the event. </param>
	/// <param name="e">A ValidateMailboxSize_EventArgs that contains the event data.</param>
	public delegate void ValidateMailboxSize(object sender,ValidateMailboxSize_EventArgs e);

	/// <summary>
	/// Represents the method that will handle the StoreMessage event for POP3_Server.
	/// </summary>
	/// <param name="sender">The source of the event. </param>
	/// <param name="e">A NewMail_EventArgs that contains the event data.</param>
	public delegate void NewMailEventHandler(object sender,NewMail_EventArgs e);

	#endregion
	
	/// <summary>
	/// SMTP server component.
	/// </summary>
	public class SMTP_Server : System.ComponentModel.Component
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private TcpListener SMTP_Listener  = null;
		private Hashtable   m_SessionTable = null;
		
		private string m_IPAddress          = "ALL";   // Holds IP Address, which to listen incoming calls.
		private int    m_port               = 25;      // Holds port number, which to listen incoming calls.
		private int    m_MaxThreads         = 20;      // Holds maximum allowed Worker Threads.
        private bool   m_enabled            = false;   // If true listens incoming calls.
		private bool   m_LogCmds            = false;   // If true, writes POP3 commands to log file.
		private int    m_SessionIdleTimeOut = 80000;   // Holds session idle timeout.
		private int    m_CommandIdleTimeOut = 60000;   // Holds command ilde timeout.
		private int    m_MaxMessageSize     = 1000000; // Hold maximum message size.
		private int    m_MaxRecipients      = 100;     // Holds maximum recipient count.
		private int    m_MaxBadCommands     = 8;       // Holds maximum bad commands allowed to session.
      
		#region Event declarations

		/// <summary>
		/// Occurs when new computer connected to POP3 server.
		/// </summary>
		public event ValidateIPHandler ValidateIPAddress = null;

		/// <summary>
		/// Occurs when connected user tryes to authenticate.
		/// </summary>
		public event AuthUserEventHandler AuthUser = null;

		/// <summary>
		/// Occurs when server needs to validate sender.
		/// </summary>
		public event ValidateMailFromHandler ValidateMailFrom = null;

		/// <summary>
		/// Occurs when server needs to validate recipient.
		/// </summary>
		public event ValidateMailToHandler ValidateMailTo = null;

		/// <summary>
		/// Occurs when server needs to validate recipient mailbox size.
		/// </summary>
		public event ValidateMailboxSize ValidateMailboxSize = null;

		/// <summary>
		/// Occurs when server has accepted message to store.
		/// </summary>
		public event NewMailEventHandler StoreMessage = null;

		/// <summary>
		/// Occurs when server has system error(Unknown error).
		/// </summary>
		public event ErrorEventHandler SysError = null;

		/// <summary>
		/// Occurs when SMTP session has finished and session log is available.
		/// </summary>
		public event LogEventHandler SessionLog = null;

		#endregion


		#region Constructors

		/// <summary>
		/// 
		/// </summary>
		/// <param name="container"></param>
		public SMTP_Server(System.ComponentModel.IContainer container)
		{
			// Required for Windows.Forms Class Composition Designer support
			container.Add(this);
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//			
		}

		/// <summary>
		/// 
		/// </summary>
		public SMTP_Server()
		{
			// Required for Windows.Forms Class Composition Designer support
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		#endregion

		#region function Dispose

		/// <summary>
		/// Clean up any resources being used and STOPs SMTP server.
		/// </summary>
		public new void Dispose()
		{
			base.Dispose();

			Stop();				
		}

		#endregion

		        
		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion


		#region function Start

		/// <summary>
		/// Starts SMTP Server.
		/// </summary>
		private void Start()
		{
			try
			{
				if(!m_enabled && !this.DesignMode){
					m_SessionTable = new Hashtable();

					Thread startSMTPServer = new Thread(new ThreadStart(Run));
					startSMTPServer.Start();
				}
			}
			catch(Exception x)
			{
				OnSysError(x,new System.Diagnostics.StackTrace());
			}
		}

		#endregion

		#region function Stop

		/// <summary>
		/// Stops SMTP Server.
		/// </summary>
		private void Stop()
		{
			try
			{
				if(SMTP_Listener != null){
					SMTP_Listener.Stop();
				}
			}
			catch(Exception x)
			{
				OnSysError(x,new System.Diagnostics.StackTrace());
			}
		}

		#endregion


		#region function Run

		/// <summary>
		/// Starts server message loop.
		/// </summary>
		private void Run()
		{			
			try
			{
				// check which ip's to listen (all or assigned)
				if(m_IPAddress.ToLower().IndexOf("all") > -1){
					SMTP_Listener = new TcpListener(IPAddress.Any,m_port);
				}
				else{
					SMTP_Listener = new TcpListener(IPAddress.Parse(m_IPAddress),m_port);
				}
				// Start listening
				SMTP_Listener.Start();
                

				//-------- Main Server message loop --------------------------------//
				while(true){
					// Check if maximum allowed thread count isn't exceeded
					if(m_SessionTable.Count <= m_MaxThreads){

						// Thread is sleeping, until a client connects
						Socket clientSocket = SMTP_Listener.AcceptSocket();

						string sessionID = clientSocket.GetHashCode().ToString();

						//****
						_LogWriter logWriter = new _LogWriter(this.SessionLog);
						SMTP_Session session = new SMTP_Session(clientSocket,this,sessionID,logWriter);
						
						Thread clientThread = new Thread(new ThreadStart(session.StartProcessing));
						
						// Add session to session list
						AddSession(sessionID,session,logWriter);

						// Start proccessing
						clientThread.Start();
					}
					else{
						Thread.Sleep(100);
					}
				}
			}
			catch(ThreadInterruptedException e){
				string dummy = e.Message;     // Neede for to remove compile warning
				Thread.CurrentThread.Abort();
			}
			catch(Exception x)
			{
				if(x.Message != "A blocking operation was interrupted by a call to WSACancelBlockingCall"){
					OnSysError(x,new System.Diagnostics.StackTrace());
				}
			}
		}

		#endregion


		#region Session handling stuff

		#region function AddSession

		/// <summary>
		/// Adds session.
		/// </summary>
		/// <param name="sessionID">Session ID.</param>
		/// <param name="session">Session object.</param>
		/// <param name="logWriter">Log writer.</param>
		internal void AddSession(string sessionID,SMTP_Session session,_LogWriter logWriter)
		{
			m_SessionTable.Add(sessionID,session);

			if(m_LogCmds){
				logWriter.AddEntry("//----- Sys: 'Session:'" + sessionID + " added " + DateTime.Now);
			}
		}

		#endregion

		#region function RemoveSession

		/// <summary>
		/// Removes session.
		/// </summary>
		/// <param name="sessionID">Session ID.</param>
		/// <param name="logWriter">Log writer.</param>
		internal void RemoveSession(string sessionID,_LogWriter logWriter)
		{
			lock(m_SessionTable){
				if(!m_SessionTable.Contains(sessionID)){
					OnSysError(new Exception("Session '" + sessionID + "' doesn't exist."),new System.Diagnostics.StackTrace());
					return;
				}
				m_SessionTable.Remove(sessionID);				
			}

			if(m_LogCmds){
				logWriter.AddEntry("//----- Sys: 'Session:'" + sessionID + " removed " + DateTime.Now);
			}
		}

		#endregion

		#endregion


		#region Properties Implementaion

		/// <summary>
		/// Gets or sets which IP address to listen.
		/// </summary>
		[		
		Description("IP Address to Listen SMTP requests"),
		DefaultValue("ALL"),
		]
		public string IpAddress 
		{
			get{ return m_IPAddress; }

			set{ m_IPAddress = value; }
		}


		/// <summary>
		/// Gets or sets which port to listen.
		/// </summary>
		[		
		Description("Port to use for SMTP"),
		DefaultValue(25),
		]
		public int Port 
		{
			get{ return m_port;	}

			set{ m_port = value; }
		}


		/// <summary>
		/// Gets or sets maximum session threads.
		/// </summary>
		[		
		Description("Maximum Allowed threads"),
		DefaultValue(20),
		]
		public int Threads 
		{
			get{ return m_MaxThreads; }

			set{ m_MaxThreads = value; }
		}


		/// <summary>
		/// Runs and stops server.
		/// </summary>
		[		
		Description("Use this property to run and stop SMTP Server"),
		DefaultValue(false),
		]
		public bool Enabled 
		{
			get{ return m_enabled; }

			set{				
				if(value != m_enabled){
					if(value){
						Start();
					}
					else{
						Stop();
					}

					m_enabled = value;
				}
			}
		}
	
		/// <summary>
		/// Gets or sets if to log commands.
		/// </summary>
		public bool LogCommands
		{
			get{ return m_LogCmds; }

			set{ m_LogCmds = value; }
		}

		/// <summary>
		/// Session idle timeout in milliseconds.
		/// </summary>
		public int SessionIdleTimeOut 
		{
			get{ return m_SessionIdleTimeOut; }

			set{ m_SessionIdleTimeOut = value; }
		}

		/// <summary>
		/// Command idle timeout in milliseconds.
		/// </summary>
		public int CommandIdleTimeOut 
		{
			get{ return m_CommandIdleTimeOut; }

			set{ m_CommandIdleTimeOut = value; }
		}

		/// <summary>
		/// Maximum message size.
		/// </summary>
		public int MaxMessageSize 
		{
			get{ return m_MaxMessageSize; }

			set{ m_MaxMessageSize = value; }
		}

		/// <summary>
		/// Maximum recipients per message.
		/// </summary>
		public int MaxRecipients
		{
			get{ return m_MaxRecipients; }

			set{ m_MaxRecipients = value; }
		}

		/// <summary>
		/// Gets or sets maximum bad commands allowed to session.
		/// </summary>
		public int MaxBadCommands
		{
			get{ return m_MaxBadCommands; }

			set{ m_MaxBadCommands = value; }
		}
		
		#endregion

		#region Events Implementation

		#region function OnValidate_IpAddress
		
		/// <summary>
		/// Raises event ValidateIP.
		/// </summary>
		/// <param name="enpoint">Connected host EndPoint.</param>
		/// <returns></returns>
		internal bool OnValidate_IpAddress(EndPoint enpoint) 
		{			
			ValidateIP_EventArgs oArg = new ValidateIP_EventArgs(enpoint);
			if(this.ValidateIPAddress != null){
				this.ValidateIPAddress(this, oArg);
			}

			return oArg.Validated;						
		}

		#endregion

		#region function OnAuthUser

		/// <summary>
		/// Raises event AuthUser.
		/// </summary>
		/// <param name="session">Reference to current smtp session.</param>
		/// <param name="userName">User name.</param>
		/// <param name="passwordData">Password compare data,it depends of authentication type.</param>
		/// <param name="data">For md5 eg. md5 calculation hash.It depends of authentication type.</param>
		/// <param name="authType">Authentication type.</param>
		/// <returns></returns>
		internal bool OnAuthUser(SMTP_Session session,string userName,string passwordData,string data,AuthType authType)
		{
			AuthUser_EventArgs oArgs = new AuthUser_EventArgs(session,userName,passwordData,data,authType);
			if(this.AuthUser != null){
				this.AuthUser(this,oArgs);
			}

			return oArgs.Validated;
		}

		#endregion

		#region function OnValidate_MailFrom

		/// <summary>
		/// Raises event ValidateMailFrom.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="reverse_path"></param>
		/// <param name="email"></param>
		/// <returns></returns>
		internal bool OnValidate_MailFrom(SMTP_Session session,string reverse_path,string email) 
		{			
			ValidateSender_EventArgs oArg = new ValidateSender_EventArgs(session,email);
			if(this.ValidateMailFrom != null){
				this.ValidateMailFrom(this, oArg);
			}

			return true;						
		}

		#endregion

		#region function OnValidate_MailTo

		/// <summary>
		/// Raises event ValidateMailTo.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="forward_path"></param>
		/// <param name="email"></param>
		/// <param name="authenticated"></param>
		/// <returns></returns>
		internal bool OnValidate_MailTo(SMTP_Session session,string forward_path,string email,bool authenticated) 
		{			
			ValidateRecipient_EventArgs oArg = new ValidateRecipient_EventArgs(session,email,authenticated);
			if(this.ValidateMailTo != null){
				this.ValidateMailTo(this, oArg);
			}

			return oArg.Validated;						
		}

		#endregion

		#region function Validate_MailBoxSize

		/// <summary>
		/// Raises event ValidateMailboxSize.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="eAddress"></param>
		/// <param name="messageSize"></param>
		/// <returns></returns>
		internal bool Validate_MailBoxSize(SMTP_Session session,string eAddress,long messageSize)
		{
			ValidateMailboxSize_EventArgs oArgs = new ValidateMailboxSize_EventArgs(session,eAddress,messageSize);
			if(this.ValidateMailboxSize != null){
				this.ValidateMailboxSize(this,oArgs);
			}

			return oArgs.IsValid;
		}

		#endregion


		#region function OnNewMail

		/// <summary>
		/// Raises event StoreMessage.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="msgStream"></param>
		internal void OnStoreMessage(SMTP_Session session,MemoryStream msgStream) 
		{			
			if(this.StoreMessage != null){	
				NewMail_EventArgs oArg = new NewMail_EventArgs(session,msgStream);
				this.StoreMessage(this,oArg);
			}						
		}

		#endregion


		#region function OnSysError

		/// <summary>
		/// Raises SysError event.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="stackTrace"></param>
		internal void OnSysError(Exception x,StackTrace stackTrace)
		{
			if(this.SysError != null){
				this.SysError(this,new Error_EventArgs(x,stackTrace));
			}
		}

		#endregion

		#endregion
		
	}
}
