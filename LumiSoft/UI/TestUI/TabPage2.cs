using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace TestUI
{
	/// <summary>
	/// Summary description for TabPage2.
	/// </summary>
	public class TabPage2 : System.Windows.Forms.Form
	{
		private LumiSoft.UI.Controls.WLabel wLabel1;
		private LumiSoft.UI.Controls.WButton wButton1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TabPage2()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		#region function Dispose

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

		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.wLabel1 = new LumiSoft.UI.Controls.WLabel();
			this.wButton1 = new LumiSoft.UI.Controls.WButton();
			this.SuspendLayout();
			// 
			// wLabel1
			// 
			this.wLabel1.Location = new System.Drawing.Point(40, 56);
			this.wLabel1.Name = "wLabel1";
			this.wLabel1.Size = new System.Drawing.Size(192, 40);
			this.wLabel1.TabIndex = 1;
			this.wLabel1.Text = "TabPage 2";
			// 
			// wLabel1.ViewStyle
			// 
			// 
			// wButton1
			// 
			this.wButton1.Location = new System.Drawing.Point(48, 144);
			this.wButton1.Name = "wButton1";
			this.wButton1.Size = new System.Drawing.Size(72, 24);
			this.wButton1.TabIndex = 2;
			this.wButton1.Text = "wButton1";
			// 
			// wButton1.ViewStyle
			// 
			// 
			// TabPage2
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.wButton1);
			this.Controls.Add(this.wLabel1);
			this.Name = "TabPage2";
			this.Text = "TabPage2";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
