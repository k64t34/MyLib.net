using System;
public class FolderIO
{
//******************************************************************************
public static void CheckFolderString(ref string s){
//******************************************************************************
s=s.Trim();
if (!s.EndsWith("\\")) s+="\\";
}
public static void CheckFolderString(ref string s,string basepath)
{
s=s.Trim();
if (!s.EndsWith("\\")) s+="\\";
if (s.StartsWith(".."))
	{
	s=s.Remove(0,3);
	s=basepath+s;
	}
else
	{
	if (!s.StartsWith("\\") & !s.StartsWith("\\\\") & s.Substring(1,2)!=":\\")
		s=basepath+s;
	}

}
}


