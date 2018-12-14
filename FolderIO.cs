using System;
using System.IO;
public class FolderIO
{
const string Version="1.1"	;
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
//******************************************************************************
public static string FolderDifference(string Minuend  ,string Subtrahend){
//******************************************************************************
if (Minuend.StartsWith(Subtrahend))
	return Minuend.Substring(Subtrahend.Length);
else
	return Minuend;
}
//******************************************************************************
private static void CopyDirectory(string sourcePath, string destinationPath)	{
//******************************************************************************		
//-----------------------------------------------------------------------
/*System.IO.DirectoryInfo sourceDirectoryInfo = new System.IO.DirectoryInfo(sourcePath);

// If the destination folder don't exist then create it
if (!System.IO.Directory.Exists(destinationPath)) {
	System.IO.Directory.CreateDirectory(destinationPath);
}

System.IO.FileSystemInfo fileSystemInfo = null;
foreach (FileSystemInfo fileSystemInfo_loopVariable in sourceDirectoryInfo.GetFileSystemInfos()) {
	fileSystemInfo = fileSystemInfo_loopVariable;
	string destinationFileName = System.IO.Path.Combine(destinationPath, fileSystemInfo.Name);

	// Now check whether its a file or a folder and take action accordingly
	if (fileSystemInfo is System.IO.FileInfo) {
		System.IO.File.Copy(fileSystemInfo.FullName, destinationFileName, true);Console.WriteLine(
			FolderDifference(sourcePath,PluginFolder)+"\\"+fileSystemInfo);
	} else {
		// Recursively call the mothod to copy all the neste folders
		CopyDirectory(fileSystemInfo.FullName, destinationFileName);
	}
}*/
}
//******************************************************************************
public static string TrimEndBackslash(string Folder){return Folder.TrimEnd(new char[]{'\\'});}
//******************************************************************************	
	public static string ParentFolder(string Folder)	{
//******************************************************************************		
//Folder=Folder.TrimEnd(new char[]{'\\'});
//if (Folder.EndsWith("\\")) Folder.Remove(Folder.Length-2,1);
//Console.WriteLine("Folder=\t\t\"{0}\"",Folder);		
//Console.WriteLine("ParentFolder=\t{0}",System.IO.Directory.GetParent(Folder).ToString());		
//Console.WriteLine("ParentFolder=\t{0}",Path.GetDirectoryName(Folder));		
return System.IO.Directory.GetParent(Folder.TrimEnd(new char[]{'\\'})).ToString()+"\\";
}















}


