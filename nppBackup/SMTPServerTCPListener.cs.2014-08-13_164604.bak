// НЕ ДОДЕЛАНО
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

using System.Net.NetworkInformation;
using System.Threading;

//*******************************************
//Простой многопоточный сервер на сокете 
public class SMTPServerSocket	{
//*******************************************
private Socket	_serverSocket;
private Thread _acceptThread;
private class ConnectionInfo {
        public Socket Socket;
        public Thread Thread;}
private List<ConnectionInfo> _connections =  new List<ConnectionInfo>();        
public SMTPServerSocket()	{}
//*******************************************
private void SetupServerSocket()    {
//*******************************************	
#if (DEBUG)
Console.WriteLine("SetupServerSocket");
#endif
// Получаем информацию о локальном компьютере
IPHostEntry localMachineInfo =  Dns.GetHostEntry(Dns.GetHostName());
IPEndPoint myEndpoint = new IPEndPoint( /*localMachineInfo.AddressList[0]*/IPAddress.Any,25);
// Создаем сокет, привязываем его к адресу и начинаем прослушивание
_serverSocket = new Socket(
    myEndpoint.Address.AddressFamily,
    SocketType.Stream, ProtocolType.Tcp);
_serverSocket.Bind(myEndpoint);
_serverSocket.Listen((int) SocketOptionName.MaxConnections);
}
//*******************************************	
private void AcceptConnections() {
//*******************************************		
#if (DEBUG)
Console.WriteLine("AcceptConnections");
#endif
while (true)
	{
    // Принимаем соединение
    Socket socket = _serverSocket.Accept();
    ConnectionInfo connection = new ConnectionInfo();
    connection.Socket = socket;

    // Создаем поток для получения данных
    connection.Thread = new Thread(ProcessConnection);
    connection.Thread.IsBackground = true;
    connection.Thread.Start(connection);

    // Сохраняем сокет
    lock (_connections) _connections.Add(connection);
	}
}
//*******************************************
private void ProcessConnection(object state)  {
//*******************************************	
#if (DEBUG)
Console.WriteLine("ProcessConnection");
#endif

ConnectionInfo connection = (ConnectionInfo)state;
byte[] buffer = new byte[255];
try
{
    while (true)
    {
        int bytesRead = connection.Socket.Receive(
            buffer);
        if (bytesRead > 0)
        {
        	#if (DEBUG)
			Console.WriteLine("bytesRead={0} ",bytesRead);
			for (int i=0;i<bytesRead;i++)
				Console.Write(Char.ConvertFromUtf32(buffer[i]));

			
			
			#endif
			lock (_connections);
			connection.Socket.Send( buffer, bytesRead,
                            SocketFlags.None);
            
            {
                /*foreach (ConnectionInfo conn in
                    _connections)
                {
                    if (conn != connection)
                    {
                        conn.Socket.Send(
                            buffer, bytesRead,
                            SocketFlags.None);
                    }
                }*/
            }
        }
        else if (bytesRead == 0) return;
    }
}
catch (SocketException exc)
{
	#if (DEBUG)
    Console.WriteLine("Socket exception: " +
        exc.SocketErrorCode);
	#endif
}
catch (Exception exc)
{
     Console.WriteLine("Exception: " + exc);
}
finally
{
    connection.Socket.Close();
    lock (_connections) _connections.Remove(
        connection);
}
}
//*******************************************
public void Start(){
//*******************************************	
#if (DEBUG)
Console.WriteLine("Start");
#endif
SetupServerSocket();
_acceptThread = new Thread(AcceptConnections);
_acceptThread.IsBackground = true;
_acceptThread.Start();
}
}



