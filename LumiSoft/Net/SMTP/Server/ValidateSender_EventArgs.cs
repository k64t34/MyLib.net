using System;

namespace LumiSoft.Net.SMTP.Server
{
	/// <summary>
	/// Provides data for the ValidateMailFrom event.
	/// </summary>
	public class ValidateSender_EventArgs
	{
		private SMTP_Session m_pSession  = null;
		private string       m_MailFrom  = "";
		private bool         m_Validated = true;

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="session">Reference to smtp session.</param>
		/// <param name="mailFrom">Sender email address.</param>
		public ValidateSender_EventArgs(SMTP_Session session,string mailFrom)
		{
			m_pSession = session;
			m_MailFrom = mailFrom;
		}


		#region Properties Implementation

		/// <summary>
		/// Gets reference to smtp session.
		/// </summary>
		public SMTP_Session Session
		{
			get{ return m_pSession; }
		}

		/// <summary>
		/// Sender's email address.
		/// </summary>
		public string MailFrom
		{
			get{ return m_MailFrom; }
		}

		/// <summary>
		/// Gets or sets if sender is ok.
		/// </summary>
		public bool Validated
		{
			get{ return m_Validated; }

			set{ m_Validated = value; }
		}

		#endregion

	}
}
