<%@ Inherits="System.Web.HttpApplication" %>
<script runat="server" language="C#">
    protected void Application_BeginRequest(Object sender, EventArgs e)
	{   
               // Specify mailserver settings path
               this.Application["SettingsPath"] = "D:\\LumiSoft\\MailServer\\Application\\Release\\Settings\\";
	}
</script>
