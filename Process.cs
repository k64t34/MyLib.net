//version 1.0
using System;
using System.Management;  // <=== Add Reference required!!
using System.Diagnostics;

namespace K64t
{
	class ProcessUtil
	{
		/// <summary>
        /// Gets the parent process id of the given process id.
        /// </summary>
        /// <returns>PID of the Process.</returns>
        ///
		public static uint GetParentProcessID(int PID){
		var query = string.Format("SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {0}", PID);
        var search = new ManagementObjectSearcher("root\\CIMV2", query);
        var results = search.Get().GetEnumerator();
        results.MoveNext();
        var queryObj = results.Current;
        var parentId = (uint)queryObj["ParentProcessId"];
        //var parent = Process.GetProcessById((int)parentId);
        //Console.WriteLine("I was started by {0} PID {1}", parent.ProcessName,parent.Id);
        //Console.ReadLine();	
		return parentId;
		}
        public static uint GetParentProcessID(uint PID){
        	return GetParentProcessID((int)PID);
		}
        
        
	}
	
}
