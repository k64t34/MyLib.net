using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace TestUI
{
	/// <summary>
	/// Summary description for TabMain.
	/// </summary>
	public class TabMain : System.Windows.Forms.Form
	{
		private LumiSoft.UI.Controls.WTabs.WTab wTab1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public TabMain()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			// Add tabpages
			wTab1.AddTab(new TabPage1(),"Tab1");
			wTab1.AddTab(new TabPage2(),"Tab2");

			wTab1.SelectFirstTab();
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
			this.wTab1 = new LumiSoft.UI.Controls.WTabs.WTab();
			this.SuspendLayout();
			// 
			// wTab1
			// 
			this.wTab1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.wTab1.Name = "wTab1";
			this.wTab1.SelectedTab = null;
			this.wTab1.Size = new System.Drawing.Size(292, 273);
			this.wTab1.TabIndex = 0;
			// 
			// TabMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.wTab1});
			this.Name = "TabMain";
			this.Text = "TabMain";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
