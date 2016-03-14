using System;
using System.Runtime.InteropServices;

namespace LumiSoft.UI
{
	/// <summary>
	/// Summary description for User32.
	/// </summary>
	public class User32
	{
		/// <summary>
		/// 
		/// </summary>
		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern int ShowWindow(IntPtr hWnd, short cmdShow);

		/// <summary>
		/// 
		/// </summary>
		public User32()
		{
		}
	}
}
