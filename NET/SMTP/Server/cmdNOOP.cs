﻿using System;
namespace k64t.Net.SMTP.Server{
public partial class SMTP_Session{
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
}
}
