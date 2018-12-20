//version 1.0
using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
public class NetIO
	{
	const string Version="1.0"	;
	//****************************************************	
		public static IPEndPoint BusyTCPSocket(int Port,System.Net.IPAddress IPAddress){			
		//****************************************************
		IPEndPoint result = null;
		IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
		IPEndPoint[]  endPoints = (properties.GetActiveTcpListeners());
		int i_max=endPoints.Length;
		for (int i=0;i!=i_max;i++)
			{
			if (endPoints[i].Port==Port)
				{
				if (endPoints[i].Address.Equals(IPAddress) || endPoints[i].Address.Equals(System.Net.IPAddress.Parse("0.0.0.0")))
    				{
					result=endPoints[i];
    				break;
    				}
				}
			}
		return result;
		}
	//****************************************************	
	public static IPEndPoint BusyUDPSocket(int Port,System.Net.IPAddress IPAddress){
	//****************************************************
	IPEndPoint result = null;
	IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
	IPEndPoint[]  endPoints = (properties.GetActiveUdpListeners());
	int i_max=endPoints.Length;
	for (int i=0;i!=i_max;i++)
		{
		if (endPoints[i].Port==Port)
			{
			if (endPoints[i].Address.Equals(IPAddress) || endPoints[i].Address.Equals(System.Net.IPAddress.Parse("0.0.0.0")))
				{
				result=endPoints[i];
				break;
				}
			}
		}
	return result;
	}	
	//****************************************************
	public static int GetPortProcessID(int Port){
	//****************************************************
	//C# Sample to list all the active TCP and UDP connections using Windows Form appl
	//https://code.msdn.microsoft.com/windowsdesktop/C-Sample-to-list-all-the-4817b58f/view/Discussions#content			
	//Build your own netstat.exe with c#
	//https://timvw.be/2007/09/09/build-your-own-netstatexe-with-c/
	int result=0;
	Process netstat = new Process();
	netstat.StartInfo.RedirectStandardOutput = true;
	netstat.StartInfo.RedirectStandardError = true; //ComSpec=C:\Windows\system32\cmd.exe
	netstat.StartInfo.CreateNoWindow = true;
	netstat.StartInfo.WorkingDirectory="C:\\Windows\\system32\\";	//SystemRoot=C:\Windows, windir	c:\Windows\System32\findstr.exe c:\Windows\System32\netstat.exe	
	netstat.StartInfo.FileName = netstat.StartInfo.WorkingDirectory + "cmd.exe";			
	netstat.StartInfo.UseShellExecute=false;	//https://msdn.microsoft.com/ru-ru/library/system.diagnostics.processstartinfo.workingdirectory(v=vs.110).aspx			
	//ok netstat.StartInfo.Arguments+="/C netstat -nao | findstr 27015";
	netstat.StartInfo.Arguments+="/Q /C FOR /F \"tokens=5\" %p IN ('netstat -nao ^| findstr /i LISTENING ^| findstr "+Port+"') do echo %p ";			
	#if (DEBUG)
	Debug.WriteLine("GetPortProcessID WorkingDirectory={0}",netstat.StartInfo.WorkingDirectory);
	Debug.WriteLine("GetPortProcessID FileName={0}",netstat.StartInfo.FileName);
	Debug.WriteLine(netstat.StartInfo.Arguments);
    Console.WriteLine("GetPortProcessID WorkingDirectory={0}",netstat.StartInfo.WorkingDirectory);
	Console.WriteLine("GetPortProcessID FileName={0}",netstat.StartInfo.FileName);
	Console.WriteLine(netstat.StartInfo.Arguments);					
	#endif	
	try 
		{					
	        netstat.Start();
    	}        	
	catch (Exception e)
    	{
			#if (DEBUG)
    	    Debug.WriteLine(e.Message);
    	    Console.WriteLine("GetPortProcessID ERR {0}",e.Message);
    	    #endif
    	}
	
	string output =netstat.StandardOutput.ReadToEnd();
		
	string err =netstat.StandardError.ReadToEnd();	
	
	#if (DEBUG)
	Debug.WriteLine(output);			
	Debug.WriteLine(err);			
	#endif
	netstat.WaitForExit();
	if (netstat.ExitCode>0) 
    	{
		#if (DEBUG)
		Console.WriteLine(output);
	    Console.WriteLine(err);
    	Console.ForegroundColor=ConsoleColor.Red;
    	Console.WriteLine("ERRORLEVEL "+netstat.ExitCode);
    	Console.ResetColor();
    	#endif
    	}
	else
		result=Int32.Parse(output);
	return result;
	}

}