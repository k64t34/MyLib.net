
using System;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Collections;
namespace k64t.Net
{
//*********************************************
public class SocketIO	{
//*********************************************			
public static string ReadLine(Socket socket)		{			return ReadLine(socket,500,600000000);		}	
public static string ReadLine(Socket socket,int maxLen,long idleTimeOut){
try
{
	long lastDataTime   = DateTime.Now.Ticks;
	ArrayList lineBuf   = new ArrayList();
	byte      prevByte  = 0;
	
	while(true){
		if(socket.Available > 0){
			// Read next byte
			byte[] currByte = new byte[1];
			int countRecieved = socket.Receive(currByte,1,SocketFlags.None);
			// Count must be equal. Eg. some computers won't give byte at first read attempt
			if(countRecieved == 1){
				lineBuf.Add(currByte[0]);

				// Line found
				if((prevByte == (byte)'\r' && currByte[0] == (byte)'\n')){
					byte[] retVal = new byte[lineBuf.Count-2];    // Remove <CRLF> 
					lineBuf.CopyTo(0,retVal,0,lineBuf.Count-2);
				
					return System.Text.Encoding.Default.GetString(retVal).Trim();
				}
			
				// Store byte
				prevByte = currByte[0];

				// Check if maximum length is exceeded
				if(lineBuf.Count > maxLen){
					throw new ReadException(ReadReplyCode.LengthExceeded,"Maximum line length exceeded");
				}

				// reset last data time
				lastDataTime = DateTime.Now.Ticks;
			}						
		}
		else{
			//---- Time out stuff -----------------------//
			if(DateTime.Now.Ticks > lastDataTime + idleTimeOut){
				#if (DEBUG)
				Console.WriteLine("SMTP_attendant.ReadLine.timeout");
				#endif				
				throw new ReadException(ReadReplyCode.TimeOut,"Read timeout");
			}					
			System.Threading.Thread.Sleep(100);									
			//------------------------------------------//
		}
	}
}
catch(Exception x){
	if(x is ReadException){
		throw x;
	}
	throw new ReadException(ReadReplyCode.UnKnownError,x.Message);
}
}
//*********************************************
public static string GetArgsText(string input,string cmdTxtToRemove)		{
//*********************************************	
string buff = input.Trim();
if(buff.Length >= cmdTxtToRemove.Length){
	buff = buff.Substring(cmdTxtToRemove.Length);
}
buff = buff.Trim();

return buff;
}
//*********************************************
public static string GetHostName(string IP)		{
//*********************************************	
try{	return System.Net.Dns.GetHostByAddress(IP).HostName;}
catch{	return "UnkownHost";}
}
//*********************************************
public static string ParseIP_from_EndPoint(string endpoint)		{
//*********************************************	
string retVal = endpoint;
int index = endpoint.IndexOf(":");
if(index > 1){	retVal = endpoint.Substring(0,index);	}
return retVal;
}
//*********************************************	
public static byte[] ReadData(Socket socket,string terminator,string removeFromEnd){
//*********************************************		
MemoryStream storeStream = new MemoryStream();
ReadReplyCode code = ReadData(socket,out storeStream,null,10000000,300000,terminator,removeFromEnd);

if(code != ReadReplyCode.Ok){
	throw new Exception("Error:" + code.ToString());
}
return storeStream.ToArray();
}

//*********************************************		
public static ReadReplyCode ReadData(Socket socket,long count,Stream storeStrm,bool storeToStream,long cmdIdleTimeOut){
//*********************************************			
ReadReplyCode replyCode = ReadReplyCode.Ok;

try
{
	long lastDataTime = DateTime.Now.Ticks;
	long readedCount  = 0;
	while(readedCount < count){
		int countAvailable = socket.Available;
		if(countAvailable > 0){
			byte[] b = null;						
			if((readedCount + countAvailable) <= count){
				b = new byte[countAvailable];								
			}
			// There are more data in socket than we need, just read as much we need
			else{
				b = new byte[count - readedCount];
			}

			int countRecieved = socket.Receive(b,0,b.Length,SocketFlags.None);
			readedCount += countRecieved;

			if(storeToStream && countRecieved > 0){
				storeStrm.Write(b,0,countRecieved);
			}

			// reset last data time
			lastDataTime = DateTime.Now.Ticks;
		}
		else{					
			//---- Idle and time out stuff ----------------------------------------//
			if(DateTime.Now.Ticks > lastDataTime + cmdIdleTimeOut){
				replyCode = ReadReplyCode.TimeOut;
				break;
			}
			System.Threading.Thread.Sleep(50);
			//---------------------------------------------------------------------//
		}
	}
}
catch{
	replyCode = ReadReplyCode.UnKnownError;
}

return replyCode;
}
//*********************************************		
public static ReadReplyCode ReadData(Socket socket,out MemoryStream replyData,byte[] addData,int maxLength,long cmdIdleTimeOut,string terminator,string removeFromEnd){
//*********************************************			
ReadReplyCode replyCode = ReadReplyCode.Ok;	
replyData = null;

try
{
	replyData = new MemoryStream();
	_FixedStack stack = new _FixedStack(terminator);
	int nextReadWriteLen = 1;

	long lastDataTime = DateTime.Now.Ticks;
	while(nextReadWriteLen > 0){
		if(socket.Available >= nextReadWriteLen){
			//Read byte(s)
			byte[] b = new byte[nextReadWriteLen];
			int countRecieved = socket.Receive(b);

			// Write byte(s) to buffer, if length isn't exceeded.
			if(replyCode != ReadReplyCode.LengthExceeded){							
				replyData.Write(b,0,countRecieved);
			}

			// Write to stack(terminator checker)
			nextReadWriteLen = stack.Push(b,countRecieved);

			//---- Check if maximum length is exceeded ---------------------------------//
			if(replyCode != ReadReplyCode.LengthExceeded && replyData.Length > maxLength){
				replyCode = ReadReplyCode.LengthExceeded;
			}
			//--------------------------------------------------------------------------//

			// reset last data time
			lastDataTime = DateTime.Now.Ticks;
		}
		else{					
			//---- Idle and time out stuff ----------------------------------------//
			if(DateTime.Now.Ticks > lastDataTime + cmdIdleTimeOut){
				replyCode = ReadReplyCode.TimeOut;
				break;
			}
			System.Threading.Thread.Sleep(50);
			//---------------------------------------------------------------------//
		}
	}

	// If reply is ok then remove chars if any specified by 'removeFromEnd'.
	if(replyCode == ReadReplyCode.Ok && removeFromEnd.Length > 0){					
		replyData.SetLength(replyData.Length - removeFromEnd.Length);				
	}
}
catch{
	replyCode = ReadReplyCode.UnKnownError;				
}

return replyCode;
}
}

	
	

//*********************************************
// Error_EventArgs
//*********************************************
public class Error_EventArgs
{
private Exception  m_pException  = null;
private StackTrace m_pStackTrace = null;
public Error_EventArgs(Exception x,StackTrace stackTrace){
	m_pException  = x;
	m_pStackTrace = stackTrace;
	}
public Exception Exception  {get{ return m_pException;  }}
public StackTrace StackTrace{get{ return m_pStackTrace; }}
}




//*********************************************
//  Exception
//*********************************************
public enum ReadReplyCode{
Ok             = 0,// Read completed successfully.
TimeOut        = 1,// Read timed out.
LengthExceeded = 2,// Maximum allowed Length exceeded.
UnKnownError   = 3,// UnKnown error, eception raised.
}
//*********************************************
//  ReadException 
//*********************************************
public class ReadException : System.Exception{
private ReadReplyCode m_ReadReplyCode;
public ReadException(ReadReplyCode code,string message) : base(message){		m_ReadReplyCode = code;}
public ReadReplyCode ReadReplyCode{	get{ return m_ReadReplyCode; }}

}




	
	


	public class StreamLineReader
	{
		private Stream m_StrmSource = null;

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="strmSource"></param>
		public StreamLineReader(Stream strmSource)
		{
			m_StrmSource = strmSource;
		}


		#region function ReadLine

		/// <summary>
		/// Reads byte[] line from stream.
		/// </summary>
		/// <returns>Return null if end of stream reached.</returns>
		public byte[] ReadLine()
		{
			ArrayList lineBuf  = new ArrayList();
			byte      prevByte = 0;

			int currByteInt = m_StrmSource.ReadByte();
			while(currByteInt > -1){
				lineBuf.Add((byte)currByteInt);

				// Line found
				if((prevByte == (byte)'\r' && (byte)currByteInt == (byte)'\n')){
					byte[] retVal = new byte[lineBuf.Count-2];    // Remove <CRLF> 
					lineBuf.CopyTo(0,retVal,0,lineBuf.Count-2);

					return retVal;
				}
				
				// Store byte
				prevByte = (byte)currByteInt;

				// Read next byte
				currByteInt = m_StrmSource.ReadByte();				
			}

			// Line isn't terminated with <CRLF> and has some chars left, return them.
			if(lineBuf.Count > 0){
				byte[] retVal = new byte[lineBuf.Count];  
				lineBuf.CopyTo(0,retVal,0,lineBuf.Count);

				return retVal;
			}

			return null;
		}

		#endregion

	}

}