using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Text;

namespace UI_rem_ISupportInitialize
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox m_pDir;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			label1.Text = "Jus select directory and press convert. \n\n NOTE: DON'T USE CONVERT TO ACTUAL PROJECTS, MAKE COPY AND TRY TO CONVERT COPY !!!\nConverts C# and VB only !";
		}

		#region method Dispose

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.button1 = new System.Windows.Forms.Button();
			this.m_pDir = new System.Windows.Forms.TextBox();
			this.button2 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(328, 120);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(64, 32);
			this.button1.TabIndex = 0;
			this.button1.Text = "Convert";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// m_pDir
			// 
			this.m_pDir.Location = new System.Drawing.Point(8, 88);
			this.m_pDir.Name = "m_pDir";
			this.m_pDir.Size = new System.Drawing.Size(384, 20);
			this.m_pDir.TabIndex = 1;
			this.m_pDir.Text = "";
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(400, 88);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(24, 24);
			this.button2.TabIndex = 2;
			this.button2.Text = "..";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(432, 72);
			this.label1.TabIndex = 3;
			this.label1.Text = "label1";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(448, 157);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.m_pDir);
			this.Controls.Add(this.button1);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Convert";
			this.ResumeLayout(false);

		}
		#endregion


		#region Events handling

		private void button1_Click(object sender, System.EventArgs e)
		{	
			if(File.Exists(Application.StartupPath + "\\notHandled.txt")){
				File.Delete(Application.StartupPath + "\\notHandled.txt");
			}

			Cursor = Cursors.WaitCursor;

			if(Directory.Exists(m_pDir.Text)){
				ArrayList files = new ArrayList();
				AppendDirFiles(m_pDir.Text,files,"*.cs");
				foreach(string file in files){
					ConverFile_CS(file);
				}

				files.Clear();
				AppendDirFiles(m_pDir.Text,files,"*.vb");
				foreach(string file in files){
					ConverFile_VB(file);
				}
				
			}
			else{
				MessageBox.Show("Invalid Directory !");
			}
		
			Cursor = Cursors.Default;

			MessageBox.Show(this,"Not handled ISupportInitialize rows are written to notHandled.txt.\n\nPlease check manually what are derived or related to Lumisoft ...  and remove them manually.","Info",MessageBoxButtons.OK,MessageBoxIcon.Information);
		}

		private void AppendDirFiles(string path,ArrayList filesAr,string fileSPattern)
		{
			string[] files = Directory.GetFiles(path,fileSPattern);
			foreach(string file in files){
				filesAr.Add(file);
			}

			string[] dirs = Directory.GetDirectories(path);
			foreach(string dir in dirs){
				AppendDirFiles(dir,filesAr,fileSPattern);
			}
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			using(FolderBrowserDialog dlg = new FolderBrowserDialog()){
				if(dlg.ShowDialog() == DialogResult.OK){
					m_pDir.Text = dlg.SelectedPath;
				}
			}
		}

		#endregion


		#region method ConverFile_CS

		private void ConverFile_CS(string file)
		{
			Hashtable ctrls = new Hashtable();

			// Scan Lumisoft.UI controls and their names
			using(TextReader r = (StreamReader)File.OpenText(file)){
				string line = "";
				while(line != null){
					if(line.ToLower().IndexOf("private lumisoft.ui") > -1){
						string buf          = "";
						string controlName  = "";
						string controlClass = "";
						
						line.Trim();

						// remove private keyword
						buf = line.Trim().Substring(7).Trim();

						// read control class
						controlClass = buf.Substring(0,buf.IndexOf(" ")).Trim();
						buf = buf.Substring(buf.IndexOf(" ")).Trim();

						// read control name from first ' '. Name is terminated with = or ; .
						if(buf.IndexOf("=") > 1){
							controlName = buf.Substring(0,buf.IndexOf("=")).Trim();
						}
						else{
							if(buf.IndexOf(";") > -1){
								controlName = buf.Substring(0,buf.IndexOf(";")).Trim();
							}
							else{
						//		MessageBox.Show(buf);
							}
						}

						if(IsISupportInitialize(controlClass)){
							ctrls.Add(controlName,controlName);
				//			MessageBox.Show(controlName);
						}				
					}

					line = r.ReadLine();
				}
			}

			ArrayList dataLines = new ArrayList();
			string skippedLines = "";
			// Remove LumiSoft ISupportInitialize lines			
			using(TextReader r = new StreamReader(file,Encoding.Default)){
				int nLine   = 0;
				string line = r.ReadLine();
				while(line != null){			
					if(line.ToLower().IndexOf("isupportinitialize") > -1){
						// Skip(remove) ISupportInitialize line for LumiSoft controls
						if(!LineContainsCtrl(ctrls,line)){
							skippedLines += file + " line=" + nLine.ToString() + " " + line + "\n";
							dataLines.Add(line);
						}
					}
					else{
						dataLines.Add(line);
					}

					line = r.ReadLine();
					nLine++;
				}
			}
			
			//MessageBox.Show(skippedLines);
			using(TextWriter wr = new StreamWriter(Application.StartupPath + "\\notHandled.txt",true,Encoding.Default)){
				wr.WriteLine(skippedLines);
			}

			if(ctrls.Count > 0){
				// Delete old file and create new
				File.Delete(file);

				using(TextWriter wr = new StreamWriter(file,false,Encoding.Default)){
					foreach(string line in dataLines){
						wr.WriteLine(line);
					}
				}
			}

		}

		#endregion

		#region method ConverFile_VB

		private void ConverFile_VB(string file)
		{
			Hashtable ctrls = new Hashtable();

			// Scan Lumisoft.UI controls and their names
			using(TextReader r = (StreamReader)File.OpenText(file)){
				string line = "";
				while(line != null){
					if(line.ToLower().IndexOf("friend withevents") > -1 && line.ToLower().IndexOf("lumisoft.ui") > 1){
						string buf          = "";
						string controlName  = "";
						string controlClass = "";
						
						line.Trim();

						// remove friend withevents keywords
						buf = line.Trim().Substring(17).Trim();

						// read control name to as keyword. Name is terminated with = or ; .
						if(buf.ToLower().IndexOf("as") > 1){
							controlName = buf.Substring(0,buf.ToLower().IndexOf("as")).Trim();
							controlClass = buf.Substring(buf.ToLower().IndexOf("as") + 2).Trim();
						}

						if(IsISupportInitialize(controlClass)){
							ctrls.Add(controlName,controlName);
					//		MessageBox.Show(controlName);
						}				
					}

					line = r.ReadLine();
				}
			}

			ArrayList dataLines = new ArrayList();
			string skippedLines = "";
			// Remove LumiSoft ISupportInitialize lines			
			using(TextReader r = new StreamReader(file,Encoding.Default)){
				int nLine   = 0;
				string line = r.ReadLine();
				while(line != null){			
					if(line.ToLower().IndexOf("isupportinitialize") > -1){
						// Skip(remove) ISupportInitialize line for LumiSoft controls
						if(!LineContainsCtrl(ctrls,line)){
							skippedLines += file + " line=" + nLine.ToString() + " " + line + "\n";
							dataLines.Add(line);
						}
					}
					else{
						dataLines.Add(line);
					}

					line = r.ReadLine();
					nLine++;
				}
			}
			
			//MessageBox.Show(skippedLines);
			using(TextWriter wr = new StreamWriter(Application.StartupPath + "\\notHandled.txt",true,Encoding.Default)){
				wr.WriteLine(skippedLines);
			}

			if(ctrls.Count > 0){
				// Delete old file and create new
				File.Delete(file);

				using(TextWriter wr = new StreamWriter(file,false,Encoding.Default)){
					foreach(string line in dataLines){
						wr.WriteLine(line);
					}
				}
			}

		}

		#endregion

		#region method IsISupportInitialize

		private bool IsISupportInitialize(string ctrlClass)
		{				
			switch(ctrlClass)
			{
				case "LumiSoft.UI.Controls.WButton":
					return true;

				case "LumiSoft.UI.Controls.WButtonEdit":
					return true;

				case "LumiSoft.UI.Controls.WCheckBox.WCheckBox":
					return true;

				case "LumiSoft.UI.Controls.WComboBox":
					return true;

				case "LumiSoft.UI.Controls.WDatePicker.WDatePicker":
					return true;

				case "LumiSoft.UI.Controls.WEditBox":
					return true;

				case "LumiSoft.UI.Controls.WImageDropDown":
					return true;

				case "LumiSoft.UI.Controls.WListBox":
					return true;

				case "LumiSoft.UI.Controls.WSpinEdit":
					return true;

				default:
					return false;
			}
		}

		#endregion

		#region method LineContainsCtrl

		private bool LineContainsCtrl(Hashtable ctrls,string line)
		{
			foreach(string s in ctrls.Values){
				if(line.IndexOf(s) > -1){
					return true;
				}
			}

			return false;
		}

		#endregion


		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}		
	}
}
