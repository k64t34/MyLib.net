using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using LumiSoft.UI.Controls.WOutlookBar;

namespace TestUI
{
	/// <summary>
	/// Main app form.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private LumiSoft.UI.Controls.WFrame wFrame1;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem menuItem_ShowHittedObj;
		private System.Windows.Forms.ImageList imgSmall;
		private System.Windows.Forms.MenuItem menuItem_Clear;
		private System.Windows.Forms.MenuItem menuItem_ClearABarItems;

		private LumiSoft.UI.Controls.WOutlookBar.WOutlookBar outlookBar = null;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			InitBar();

			wFrame1.Frame_BarControl = outlookBar;
			wFrame1.Frame_Form = new Form1(wFrame1);

			outlookBar.ContextMenu = contextMenu1;
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
			this.wFrame1 = new LumiSoft.UI.Controls.WFrame();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.menuItem_ShowHittedObj = new System.Windows.Forms.MenuItem();
			this.menuItem_Clear = new System.Windows.Forms.MenuItem();
			this.imgSmall = new System.Windows.Forms.ImageList(this.components);
			this.menuItem_ClearABarItems = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// wFrame1
			// 
			this.wFrame1.ControlPaneWidth = 140;
			this.wFrame1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.wFrame1.FormFrameBorder = System.Windows.Forms.BorderStyle.Fixed3D;
			this.wFrame1.Name = "wFrame1";
			this.wFrame1.Size = new System.Drawing.Size(688, 485);
			this.wFrame1.SplitterColor = System.Drawing.SystemColors.Control;
			this.wFrame1.SplitterMinExtra = 0;
			this.wFrame1.SplitterMinSize = 0;
			this.wFrame1.TabIndex = 0;
			this.wFrame1.TopPaneBkColor = System.Drawing.SystemColors.Control;
			this.wFrame1.TopPaneHeight = 25;
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(32, 32);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Fuchsia;
			// 
			// contextMenu1
			// 
			this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menuItem_ShowHittedObj,
																						 this.menuItem_Clear,
																						 this.menuItem_ClearABarItems});
			// 
			// menuItem_ShowHittedObj
			// 
			this.menuItem_ShowHittedObj.Index = 0;
			this.menuItem_ShowHittedObj.Text = "Get hitted object";
			this.menuItem_ShowHittedObj.Click += new System.EventHandler(this.menuItem_ShowHittedObj_Click);
			// 
			// menuItem_Clear
			// 
			this.menuItem_Clear.Index = 1;
			this.menuItem_Clear.Text = "Clear outlook bar";
			this.menuItem_Clear.Click += new System.EventHandler(this.menuItem_Clear_Click);
			// 
			// imgSmall
			// 
			this.imgSmall.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imgSmall.ImageSize = new System.Drawing.Size(16, 16);
			this.imgSmall.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgSmall.ImageStream")));
			this.imgSmall.TransparentColor = System.Drawing.Color.Fuchsia;
			// 
			// menuItem_ClearABarItems
			// 
			this.menuItem_ClearABarItems.Index = 2;
			this.menuItem_ClearABarItems.Text = "Clear active bar items";
			this.menuItem_ClearABarItems.Click += new System.EventHandler(this.menuItem_ClearABarItems_Click);
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(688, 485);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.wFrame1});
			this.Name = "MainForm";
			this.Text = "MainForm";
			this.ResumeLayout(false);

		}
		#endregion


		#region Events handling

		#region OutlookBar Item Click stuff

		/// <summary>
		/// OutlookBar Click
		/// </summary>
		private void wOutlookBar_ItemClicked(object sender,LumiSoft.UI.Controls.WOutlookBar.ItemClicked_EventArgs e)
		{			
			wFrame1.FormFrameBorder = BorderStyle.Fixed3D;

			if(e.Item.Tag == null){
				if(this.Visible){
					wFrame1.Frame_Form = new Form1(wFrame1);
					MessageBox.Show("Item clicked:" + e.Item.Caption);
				}
			}
			else{
				wFrame1.Frame_ToolBar = null;
				switch(e.Item.Tag.ToString())
				{
					case "tab":
						wFrame1.FormFrameBorder = BorderStyle.None;
						wFrame1.Frame_Form = new TabMain();						
						break;

					default:
						wFrame1.Frame_Form = new Form1(wFrame1);
						break;
				}
			}
		}

		#endregion

		#region OutlookBar Bar Click stuff

		/// <summary>
		/// OutlookBar Click
		/// </summary>
		private void wOutlookBar_BarClicked(object sender,LumiSoft.UI.Controls.WOutlookBar.BarClicked_EventArgs e)
		{
			if(this.Visible){
				MessageBox.Show("Bar clicked:" + e.Bar.Caption);
			}
		}

		#endregion

		#region function menuItem_ShowHittedObj_Click

		private void menuItem_ShowHittedObj_Click(object sender, System.EventArgs e)
		{
			LumiSoft.UI.Controls.WOutlookBar.HitInfo hit = outlookBar.GetHittedObject();
			MessageBox.Show(hit.HittedObject.ToString());
		}

		#endregion

		#region function menuItem_Clear_Click

		private void menuItem_Clear_Click(object sender, System.EventArgs e)
		{
			outlookBar.Bars.Clear();
		}

		#endregion

		#region function menuItem_ClearABarItems_Click

		private void menuItem_ClearABarItems_Click(object sender, System.EventArgs e)
		{
			if(outlookBar.ActiveBar != null){
				outlookBar.ActiveBar.Items.Clear();
			}
		}

		#endregion

		#endregion


		#region function InitBar

		private void InitBar()
		{
			outlookBar = new LumiSoft.UI.Controls.WOutlookBar.WOutlookBar();
			outlookBar.ImageList = this.imageList1;
			outlookBar.ImageListSmall = this.imgSmall;
			outlookBar.ItemClicked += new LumiSoft.UI.Controls.WOutlookBar.ItemClickedEventHandler(this.wOutlookBar_ItemClicked);
			outlookBar.BarClicked += new LumiSoft.UI.Controls.WOutlookBar.BarClickedEventHandler(this.wOutlookBar_BarClicked);
			
			Item it = null;
			Bar bar = null;

			Bar stuckingTest = outlookBar.Bars.Add("Stucking test");
			stuckingTest.Items.Add("Can stuck",0);
			stuckingTest.Items.Add("Can stuck",0);

			it = stuckingTest.Items.Add("Can't stuck",0);
			it.AllowStuck = false;

			stuckingTest.Items.Add("Can stuck",0);

			bar = outlookBar.Bars.Add("Full item select");
			bar.ItemsStyle = ItemsStyle.FullSelect; // Force to use FullSelect(don't use ViewStyle)
			it = bar.Items.Add("Item a",0);
			it = bar.Items.Add("Item b",0);

			bar = outlookBar.Bars.Add("Small icons");
			bar.ItemsStyle = ItemsStyle.SmallIcon; // Force to use SmallIcon(don't use ViewStyle)
			it = bar.Items.Add("Item a",0);
			it = bar.Items.Add("Item b",0);

			bar = outlookBar.Bars.Add("This is multi line bar text test");
			bar.Items.Add("This is multiline item caption test",0);
			bar.Items.Add("For some reason many comercical Outlook bars wont do it.",0);
			outlookBar.StuckenItem = bar.Items.Add("Is it nicer to see ...",0);

			bar = outlookBar.Bars.Add("Test controls");
			it = bar.Items.Add("Tab",0);
			it.Tag = "tab";

		}

		#endregion
						
	}
}
