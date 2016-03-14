using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace TestUI
{
	/// <summary>
	/// Summary description for TabPage1.
	/// </summary>
	public class TabPage1 : System.Windows.Forms.Form
	{
		private LumiSoft.UI.Controls.WLabel wLabel1;
		private LumiSoft.UI.Controls.WEditBox wEditBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TabPage1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.wLabel1 = new LumiSoft.UI.Controls.WLabel();
			this.wEditBox1 = new LumiSoft.UI.Controls.WEditBox();
			this.SuspendLayout();
			// 
			// wLabel1
			// 
			this.wLabel1.Location = new System.Drawing.Point(16, 40);
			this.wLabel1.Name = "wLabel1";
			this.wLabel1.Size = new System.Drawing.Size(192, 40);
			this.wLabel1.TabIndex = 0;
			this.wLabel1.Text = "TabPage 1";
			// 
			// wLabel1.ViewStyle
			// 
			// 
			// wEditBox1
			// 
			this.wEditBox1.Location = new System.Drawing.Point(40, 120);
			this.wEditBox1.Name = "wEditBox1";
			this.wEditBox1.Size = new System.Drawing.Size(88, 24);
			this.wEditBox1.TabIndex = 1;
			this.wEditBox1.Text = "wEditBox1";
			// 
			// wEditBox1.ViewStyle
			// 
			// 
			// TabPage1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.wEditBox1);
			this.Controls.Add(this.wLabel1);
			this.Name = "TabPage1";
			this.Text = "TabPage1";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
