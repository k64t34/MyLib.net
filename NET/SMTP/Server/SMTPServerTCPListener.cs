//-> Синхронизация логов от сессий.
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;


//using System.Net.NetworkInformation;
//using System.Linq;
//using System.Text;



	
//*******************************************
//http://www.codeproject.com/Articles/456380/A-Csharp-SMTP-server-receiver 
//http://kbss.ru/blog/lang_c_sharp/197.html

public enum ServerLogLevel {None,Critical,Error,Warning,Information,Debug}

namespace k64t.Net.SMTP{


public class SMTPServer	{
//*******************************************
private TcpListener	Server =null;
public ServerLogLevel FileLogLevel=ServerLogLevel.None;
//public LogLevel {get{return vLogLevel;}set{vLogLevel=value;}};
private bool lWriteFileLog=true;
private bool lWriteConsoleLog=true;
private string LogPath="";
private string LogFile="";
private StreamWriter objLogFile=null;

private int vPort=25;	
public int Port	{get{ return vPort;}set{ vPort = value;}}


private IPAddress ServerIP=IPAddress.Any;

private int MaxThreads = 8;
private Hashtable SessionTable = null;
public /*const*/ long TicksPerSecond = 10000000;
public int  SessionTimeout = 60; 	//{/*get{ return vSessionTimeout/10000000;*/}set{ vSessionTimeout = long(value)*10000000;}} // in sec
public long vSessionTimeout ; // in system tick. 1 ms = 10.000 ticks, 1 s = 10.000.000 tiks

public int    MaxMessageSize     = 1000000;

public int    vMaxRecipients      = 100;     // Holds maximum recipient count.


#region Event declarations
public event ErrorEventHandler SysError = null;
#endregion
	
//*******************************************
public SMTPServer()	{
//*******************************************
vSessionTimeout =SessionTimeout*TicksPerSecond;

this.Server = new TcpListener(ServerIP, vPort);
this.SessionTable = new Hashtable();

if (lWriteFileLog)
{
	LogFile= String.Format("{0}{1:ddMMyyyyHHmmss}.log",LogPath,DateTime.Now);
	objLogFile = new StreamWriter(LogFile);
	if (objLogFile!=null)
		{
		objLogFile.AutoFlush=true;
		ServerLog(String.Format("{0} Log file created",DateTime.Now));
		//objLogFile.WriteLine("Log file created {0}",DateTime.Now);		
		}
	else 
		{
		lWriteFileLog=false;
		//-> Record to OS log		
		}
}
}
//*******************************************
public void Start()    {
//*******************************************	
#if (DEBUG)
Console.WriteLine("Start");
#endif
ServerLog(String.Format("{0} SMTP Server starting",DateTime.Now));
LogServerStatus();
try{
	Server.Start();
	ServerLog(String.Format("{0} SMTP Server listening",DateTime.Now));
	while(true)
		{
		if(SessionTable.Count <= MaxThreads)
			{
			Socket clientSocket = Server.AcceptSocket();
			#if (DEBUG)
			Console.WriteLine("AcceptSocket");
			#endif
			string sessionID = clientSocket.GetHashCode().ToString();
			k64t.Net.SMTP.Server.SMTP_Session session = new k64t.Net.SMTP.Server.SMTP_Session(clientSocket,this,sessionID);
			Thread clientThread = new Thread(new ThreadStart(session.StartProcessing));			
			AddSession(sessionID,session/*,logWriter*/); // Add session to session list			
			clientThread.Start();// Start proccessing			
			}
		else 
			{
			ServerLog(String.Format("{0} SMTP Server threads rize to max count",DateTime.Now));
			Thread.Sleep(10000);	
			}
		}
}
catch(Exception x){
	ServerLog(String.Format("{0} SMTP Server start faild",DateTime.Now));
	OnSysError(x,new System.Diagnostics.StackTrace());
}
}
//*******************************************
public void Stop()    {
//*******************************************	
#if (DEBUG)
Console.WriteLine("Stop");
#endif
if(Server != null)
{
	ServerLog(String.Format("{0} SMTP Server stop",DateTime.Now));
	this.Server.Stop();
}
}
//*******************************************	
internal void AddSession(string sessionID,k64t.Net.SMTP.Server.SMTP_Session session/*,_LogWriter logWriter*/)		{
//*******************************************		
SessionTable.Add(sessionID,session);
ServerLog(String.Format("{0} SMTP Server:Session {1} added. Client IP {2}:{3}",DateTime.Now,sessionID,((IPEndPoint)session.ClientSocket.RemoteEndPoint).Address,((IPEndPoint)session.ClientSocket.RemoteEndPoint).Port));
}
//*******************************************		
internal void RemoveSession(string sessionID/*,_LogWriter logWriter*/)		{
//*******************************************			
lock(SessionTable)
	{
	if(!SessionTable.Contains(sessionID)){
		OnSysError(new Exception("Session '" + sessionID + "' doesn't exist."),new System.Diagnostics.StackTrace());
		return;
	}
	SessionTable.Remove(sessionID);				
	}

ServerLog(String.Format("{0} SMTP Server:Session {1} removed.",DateTime.Now,sessionID));
}
//*********************************************
internal void OnSysError(Exception x,StackTrace stackTrace)		{
//*********************************************	
/*if(this.SysError != null)
	{
	this.SysError(this,new Error_EventArgs(x,stackTrace));
	}*/
}
//*********************************************
internal void ServerLog(string Msg){
//*********************************************	
if (lWriteFileLog)objLogFile.WriteLine(Msg);
if (lWriteConsoleLog)Console.WriteLine(Msg);
}
//*********************************************
internal void LogServerStatus(){
//*********************************************	
if (lWriteFileLog)
{	
	
	if (Server==null)
	{	objLogFile.WriteLine("{0} SMTP Server Status:The server is not initialized.",DateTime.Now);	}
	else {
	objLogFile.WriteLine("{0} SMTP Server Status:",DateTime.Now);
	objLogFile.WriteLine("\tIP\t\t\t{0}",IPAddress.Parse(((IPEndPoint)Server.LocalEndpoint).Address.ToString()));
	objLogFile.WriteLine("\tPort\t\t{0}",vPort);		
	objLogFile.WriteLine("\tServerLogLevel\t{0}",FileLogLevel);		
	objLogFile.WriteLine("\tLogFile\t\t{0}",LogFile);		
	//->objLogFile.WriteLine("\tLogFileSize\t\t{0}",objLogFile.);		
	objLogFile.WriteLine("\tMaxThreads\t{0}",MaxThreads);		
	objLogFile.WriteLine("\tSessionCount\t{0}",SessionTable.Count);			
	objLogFile.WriteLine("\tThreadsCount\t{0}",System.Diagnostics.Process.GetCurrentProcess().Threads.Count);	
	//foreach (s int SessionTable)
	//	{
	//	objLogFile.WriteLine("\tThreadsCount\t{0}",s.);	
    //	}
	//-> Состояние каждого процесса
	//-> Кол-во памяти http://msdn.microsoft.com/ru-ru/library/system.diagnostics.process.virtualmemorysize(v=vs.110).aspx
	
	//-> Кол-во сокетов
	
	}	
}
	
}
//*********************************************	
internal void OnStoreMessage(k64t.Net.SMTP.Server.SMTP_Session session,MemoryStream msgStream)		{			
//*********************************************		
//->Сделать сохранение письма
//if(session.StoreMessage != null){
//	NewMail_EventArgs oArg = new NewMail_EventArgs(session,msgStream);
//	session.StoreMessage(this,oArg);
//	}						
}
}






	
	



	

}