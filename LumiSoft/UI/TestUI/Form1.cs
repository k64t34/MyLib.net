using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using LumiSoft.UI;

namespace TestUI
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private LumiSoft.UI.Controls.WEditBox wEditBox1;
		private LumiSoft.UI.Controls.WSpinEdit wSpinEdit1;
		private LumiSoft.UI.Controls.WPictureBox wPictureBox1;
		private LumiSoft.UI.Controls.WButtonEdit wButtonEdit1;
		private LumiSoft.UI.Controls.WDatePicker.WDatePicker wDatePicker1;
		private LumiSoft.UI.Controls.WComboBox wComboBox1;
		private LumiSoft.UI.Controls.WCheckBox.WCheckBox wCheckBox1;
		private LumiSoft.UI.Controls.WToolBar toolBar1;
		private System.Windows.Forms.ToolBarButton toolBarm_pFlashControls;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.ToolBarButton toolBarButton3;
		private System.Windows.Forms.ToolBarButton toolBarButton4;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ToolBarButton toolBarButton5;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private LumiSoft.UI.Controls.WEditBox wEditBox2;
		private LumiSoft.UI.Controls.WEditBox wEditBox3;
		private LumiSoft.UI.Controls.WSpinEdit wSpinEdit2;
		private LumiSoft.UI.Controls.WSpinEdit wSpinEdit3;
		private LumiSoft.UI.Controls.WButtonEdit wButtonEdit2;
		private LumiSoft.UI.Controls.WButtonEdit wButtonEdit3;
		private LumiSoft.UI.Controls.WComboBox wComboBox2;
		private LumiSoft.UI.Controls.WComboBox wComboBox3;
		private LumiSoft.UI.Controls.WDatePicker.WDatePicker wDatePicker2;
		private LumiSoft.UI.Controls.WDatePicker.WDatePicker wDatePicker3;
		private LumiSoft.UI.Controls.WCheckBox.WCheckBox wCheckBox2;
		private LumiSoft.UI.Controls.WCheckBox.WCheckBox wCheckBox3;
		private LumiSoft.UI.Controls.WButton m_pFlashControls;
		private LumiSoft.UI.Controls.WTime wTime1;
		private LumiSoft.UI.Controls.WLabel wLabel1;
		private LumiSoft.UI.Controls.WLabel wLabel2;
		private LumiSoft.UI.Controls.WTabs.WTabBar wTabBar1;
		private LumiSoft.UI.Controls.WButton m_SaveToXml;
		private LumiSoft.UI.Controls.WButton m_pLoadFromXml;
		private LumiSoft.UI.Controls.WButtonEdit wButtonEdit4;
		private LumiSoft.UI.Controls.WComboBox wComboBox4;
		private LumiSoft.UI.Controls.WDatePicker.WDatePicker wDatePicker4;
		private System.ComponentModel.IContainer components;

		public Form1(LumiSoft.UI.Controls.WFrame wFrame)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			
			PropertyGrid propertyGrid1 = new PropertyGrid();
			propertyGrid1.CommandsVisibleIfAvailable = true;
			propertyGrid1.Location = new Point(5, 25);
			propertyGrid1.Size = new System.Drawing.Size(200, 400);
			propertyGrid1.TabIndex = 1;
			propertyGrid1.Dock = DockStyle.Left;
			propertyGrid1.Text = "Property Grid";

			this.Controls.Add(propertyGrid1);

			propertyGrid1.SelectedObject = LumiSoft.UI.ViewStyle.staticViewStyle;


			wComboBox1.Items.Add("fsg");
			wComboBox1.Items.Add("gddgddh");
			wComboBox1.Items.Add("fsdshg");
			wComboBox1.Items.Add("wrrurt");
			wComboBox1.Items.Add("iyytii","tag");

			wComboBox4.Items.Add("fsg");
			wComboBox4.Items.Add("gddgddh");
			wComboBox4.Items.Add("fsdshg");
			wComboBox4.Items.Add("wrrurt");
			wComboBox4.Items.Add("iyytii","tag");

			wFrame.Frame_ToolBar = this.toolBar1;

			wTabBar1.ImageList = this.imageList1;

			wTabBar1.Tabs.Add("Tab1","",0);
			(wTabBar1.Tabs.Add("Tab2","",0)).Enabled = false;
			wTabBar1.Tabs.Add("Tab3","",0);
			wTabBar1.Tabs.Add("Tab x...","",0);		
		
		}

		#region function Dispose

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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form1));
			this.wEditBox1 = new LumiSoft.UI.Controls.WEditBox();
			this.wSpinEdit1 = new LumiSoft.UI.Controls.WSpinEdit();
			this.wPictureBox1 = new LumiSoft.UI.Controls.WPictureBox();
			this.wButtonEdit1 = new LumiSoft.UI.Controls.WButtonEdit();
			this.wDatePicker1 = new LumiSoft.UI.Controls.WDatePicker.WDatePicker();
			this.wComboBox1 = new LumiSoft.UI.Controls.WComboBox();
			this.wCheckBox1 = new LumiSoft.UI.Controls.WCheckBox.WCheckBox();
			this.toolBar1 = new LumiSoft.UI.Controls.WToolBar();
			this.toolBarm_pFlashControls = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton4 = new System.Windows.Forms.ToolBarButton();
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.toolBarButton5 = new System.Windows.Forms.ToolBarButton();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.wEditBox2 = new LumiSoft.UI.Controls.WEditBox();
			this.wEditBox3 = new LumiSoft.UI.Controls.WEditBox();
			this.wSpinEdit2 = new LumiSoft.UI.Controls.WSpinEdit();
			this.wSpinEdit3 = new LumiSoft.UI.Controls.WSpinEdit();
			this.wButtonEdit2 = new LumiSoft.UI.Controls.WButtonEdit();
			this.wButtonEdit3 = new LumiSoft.UI.Controls.WButtonEdit();
			this.wComboBox2 = new LumiSoft.UI.Controls.WComboBox();
			this.wComboBox3 = new LumiSoft.UI.Controls.WComboBox();
			this.wDatePicker2 = new LumiSoft.UI.Controls.WDatePicker.WDatePicker();
			this.wDatePicker3 = new LumiSoft.UI.Controls.WDatePicker.WDatePicker();
			this.wCheckBox2 = new LumiSoft.UI.Controls.WCheckBox.WCheckBox();
			this.wCheckBox3 = new LumiSoft.UI.Controls.WCheckBox.WCheckBox();
			this.m_pFlashControls = new LumiSoft.UI.Controls.WButton();
			this.wTime1 = new LumiSoft.UI.Controls.WTime();
			this.wLabel1 = new LumiSoft.UI.Controls.WLabel();
			this.wLabel2 = new LumiSoft.UI.Controls.WLabel();
			this.wTabBar1 = new LumiSoft.UI.Controls.WTabs.WTabBar();
			this.m_SaveToXml = new LumiSoft.UI.Controls.WButton();
			this.m_pLoadFromXml = new LumiSoft.UI.Controls.WButton();
			this.wButtonEdit4 = new LumiSoft.UI.Controls.WButtonEdit();
			this.wComboBox4 = new LumiSoft.UI.Controls.WComboBox();
			this.wDatePicker4 = new LumiSoft.UI.Controls.WDatePicker.WDatePicker();
			this.SuspendLayout();
			// 
			// wEditBox1
			// 
			this.wEditBox1.Location = new System.Drawing.Point(240, 72);
			this.wEditBox1.Name = "wEditBox1";
			this.wEditBox1.TabIndex = 0;
			this.wEditBox1.Text = "Normal";
			this.wEditBox1.UseStaticViewStyle = true;
			// 
			// wEditBox1.ViewStyle
			// 
			// 
			// wSpinEdit1
			// 
			this.wSpinEdit1.BackColor = System.Drawing.Color.White;
			this.wSpinEdit1.ButtonsAlign = LumiSoft.UI.Controls.LeftRight.Right;
			this.wSpinEdit1.DecimalPlaces = 0;
			this.wSpinEdit1.DecMaxValue = new System.Decimal(new int[] {
																		   999999999,
																		   0,
																		   0,
																		   0});
			this.wSpinEdit1.DecMinValue = new System.Decimal(new int[] {
																		   999999999,
																		   0,
																		   0,
																		   -2147483648});
			this.wSpinEdit1.DecValue = new System.Decimal(new int[] {
																		0,
																		0,
																		0,
																		0});
			this.wSpinEdit1.Location = new System.Drawing.Point(240, 152);
			this.wSpinEdit1.Mask = LumiSoft.UI.Controls.WEditBox_Mask.Numeric;
			this.wSpinEdit1.MaxLength = 32767;
			this.wSpinEdit1.Name = "wSpinEdit1";
			this.wSpinEdit1.ReadOnly = false;
			this.wSpinEdit1.Size = new System.Drawing.Size(104, 20);
			this.wSpinEdit1.TabIndex = 1;
			this.wSpinEdit1.Text = "0";
			this.wSpinEdit1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			this.wSpinEdit1.UseStaticViewStyle = true;
			// 
			// wSpinEdit1.ViewStyle
			// 
			// 
			// wPictureBox1
			// 
			this.wPictureBox1.DrawBorder = true;
			this.wPictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("wPictureBox1.Image")));
			this.wPictureBox1.Location = new System.Drawing.Point(456, 280);
			this.wPictureBox1.Name = "wPictureBox1";
			this.wPictureBox1.Size = new System.Drawing.Size(72, 64);
			this.wPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.wPictureBox1.TabIndex = 2;
			this.wPictureBox1.UseStaticViewStyle = true;
			// 
			// wPictureBox1.ViewStyle
			// 
			// 
			// wButtonEdit1
			// 
			this.wButtonEdit1.Location = new System.Drawing.Point(240, 232);
			this.wButtonEdit1.Name = "wButtonEdit1";
			this.wButtonEdit1.TabIndex = 3;
			this.wButtonEdit1.Text = "Normal";
			this.wButtonEdit1.UseStaticViewStyle = true;
			// 
			// wButtonEdit1.ViewStyle
			// 
			this.wButtonEdit1.ButtonPressed += new LumiSoft.UI.Controls.ButtonPressedEventHandler(this.wButtonEdit1_ButtonPressed);
			// 
			// wDatePicker1
			// 
			this.wDatePicker1.Location = new System.Drawing.Point(384, 176);
			this.wDatePicker1.Name = "wDatePicker1";
			this.wDatePicker1.TabIndex = 4;
			this.wDatePicker1.UseStaticViewStyle = true;
			this.wDatePicker1.Value = new System.DateTime(2003, 1, 17, 0, 0, 0, 0);
			// 
			// wDatePicker1.ViewStyle
			// 
			// 
			// wComboBox1
			// 
			this.wComboBox1.DropDownWidth = 120;
			this.wComboBox1.Location = new System.Drawing.Point(384, 72);
			this.wComboBox1.Name = "wComboBox1";
			this.wComboBox1.SelectedIndex = -1;
			this.wComboBox1.SelectionLength = 0;
			this.wComboBox1.SelectionStart = 0;
			this.wComboBox1.Size = new System.Drawing.Size(120, 20);
			this.wComboBox1.TabIndex = 5;
			this.wComboBox1.Text = "Normal";
			this.wComboBox1.UseStaticViewStyle = true;
			// 
			// wComboBox1.ViewStyle
			// 
			this.wComboBox1.VisibleItems = 5;
			// 
			// wCheckBox1
			// 
			this.wCheckBox1.Checked = false;
			this.wCheckBox1.Location = new System.Drawing.Point(240, 328);
			this.wCheckBox1.Name = "wCheckBox1";
			this.wCheckBox1.ReadOnly = false;
			this.wCheckBox1.Size = new System.Drawing.Size(30, 22);
			this.wCheckBox1.TabIndex = 6;
			this.wCheckBox1.UseStaticViewStyle = true;
			// 
			// wCheckBox1.ViewStyle
			// 
			// 
			// toolBar1
			// 
			this.toolBar1.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																						this.toolBarm_pFlashControls,
																						this.toolBarButton2,
																						this.toolBarButton3,
																						this.toolBarButton4,
																						this.toolBarButton5});
			this.toolBar1.Divider = false;
			this.toolBar1.DropDownArrows = true;
			this.toolBar1.ImageList = this.imageList1;
			this.toolBar1.Location = new System.Drawing.Point(0, 0);
			this.toolBar1.Name = "toolBar1";
			this.toolBar1.ShowToolTips = true;
			this.toolBar1.Size = new System.Drawing.Size(536, 26);
			this.toolBar1.TabIndex = 7;
			// 
			// toolBarm_pFlashControls
			// 
			this.toolBarm_pFlashControls.Enabled = false;
			this.toolBarm_pFlashControls.ImageIndex = 0;
			// 
			// toolBarButton2
			// 
			this.toolBarButton2.Enabled = false;
			this.toolBarButton2.ImageIndex = 1;
			// 
			// toolBarButton3
			// 
			this.toolBarButton3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// toolBarButton4
			// 
			this.toolBarButton4.DropDownMenu = this.contextMenu1;
			this.toolBarButton4.ImageIndex = 3;
			this.toolBarButton4.Style = System.Windows.Forms.ToolBarButtonStyle.DropDownButton;
			// 
			// contextMenu1
			// 
			this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menuItem1,
																						 this.menuItem2});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.Text = "Enable print";
			this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.Text = "Disable print";
			this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// toolBarButton5
			// 
			this.toolBarButton5.ImageIndex = 2;
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Fuchsia;
			// 
			// wEditBox2
			// 
			this.wEditBox2.Location = new System.Drawing.Point(240, 96);
			this.wEditBox2.Name = "wEditBox2";
			this.wEditBox2.ReadOnly = true;
			this.wEditBox2.TabIndex = 8;
			this.wEditBox2.Text = "ReadOnly";
			this.wEditBox2.UseStaticViewStyle = true;
			// 
			// wEditBox2.ViewStyle
			// 
			// 
			// wEditBox3
			// 
			this.wEditBox3.Enabled = false;
			this.wEditBox3.Location = new System.Drawing.Point(240, 120);
			this.wEditBox3.Name = "wEditBox3";
			this.wEditBox3.TabIndex = 9;
			this.wEditBox3.Text = "Disabled";
			this.wEditBox3.UseStaticViewStyle = true;
			// 
			// wEditBox3.ViewStyle
			// 
			// 
			// wSpinEdit2
			// 
			this.wSpinEdit2.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(228)), ((System.Byte)(224)), ((System.Byte)(220)));
			this.wSpinEdit2.ButtonsAlign = LumiSoft.UI.Controls.LeftRight.Right;
			this.wSpinEdit2.DecimalPlaces = 0;
			this.wSpinEdit2.DecMaxValue = new System.Decimal(new int[] {
																		   999999999,
																		   0,
																		   0,
																		   0});
			this.wSpinEdit2.DecMinValue = new System.Decimal(new int[] {
																		   999999999,
																		   0,
																		   0,
																		   -2147483648});
			this.wSpinEdit2.DecValue = new System.Decimal(new int[] {
																		0,
																		0,
																		0,
																		0});
			this.wSpinEdit2.Location = new System.Drawing.Point(240, 176);
			this.wSpinEdit2.Mask = LumiSoft.UI.Controls.WEditBox_Mask.Numeric;
			this.wSpinEdit2.MaxLength = 32767;
			this.wSpinEdit2.Name = "wSpinEdit2";
			this.wSpinEdit2.ReadOnly = true;
			this.wSpinEdit2.Size = new System.Drawing.Size(104, 20);
			this.wSpinEdit2.TabIndex = 10;
			this.wSpinEdit2.Text = "0";
			this.wSpinEdit2.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			this.wSpinEdit2.UseStaticViewStyle = true;
			// 
			// wSpinEdit2.ViewStyle
			// 
			// 
			// wSpinEdit3
			// 
			this.wSpinEdit3.BackColor = System.Drawing.Color.White;
			this.wSpinEdit3.ButtonsAlign = LumiSoft.UI.Controls.LeftRight.Right;
			this.wSpinEdit3.DecimalPlaces = 0;
			this.wSpinEdit3.DecMaxValue = new System.Decimal(new int[] {
																		   999999999,
																		   0,
																		   0,
																		   0});
			this.wSpinEdit3.DecMinValue = new System.Decimal(new int[] {
																		   999999999,
																		   0,
																		   0,
																		   -2147483648});
			this.wSpinEdit3.DecValue = new System.Decimal(new int[] {
																		0,
																		0,
																		0,
																		0});
			this.wSpinEdit3.Enabled = false;
			this.wSpinEdit3.Location = new System.Drawing.Point(240, 200);
			this.wSpinEdit3.Mask = LumiSoft.UI.Controls.WEditBox_Mask.Numeric;
			this.wSpinEdit3.MaxLength = 32767;
			this.wSpinEdit3.Name = "wSpinEdit3";
			this.wSpinEdit3.ReadOnly = false;
			this.wSpinEdit3.Size = new System.Drawing.Size(104, 20);
			this.wSpinEdit3.TabIndex = 11;
			this.wSpinEdit3.Text = "0";
			this.wSpinEdit3.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			this.wSpinEdit3.UseStaticViewStyle = true;
			// 
			// wSpinEdit3.ViewStyle
			// 
			// 
			// wButtonEdit2
			// 
			this.wButtonEdit2.EditStyle = LumiSoft.UI.Controls.EditStyle.Selectable;
			this.wButtonEdit2.Location = new System.Drawing.Point(240, 256);
			this.wButtonEdit2.Name = "wButtonEdit2";
			this.wButtonEdit2.TabIndex = 12;
			this.wButtonEdit2.Text = "Selectable";
			this.wButtonEdit2.UseStaticViewStyle = true;
			// 
			// wButtonEdit2.ViewStyle
			// 
			this.wButtonEdit2.ButtonPressed += new LumiSoft.UI.Controls.ButtonPressedEventHandler(this.wButtonEdit1_ButtonPressed);
			// 
			// wButtonEdit3
			// 
			this.wButtonEdit3.Enabled = false;
			this.wButtonEdit3.Location = new System.Drawing.Point(240, 304);
			this.wButtonEdit3.Name = "wButtonEdit3";
			this.wButtonEdit3.TabIndex = 13;
			this.wButtonEdit3.Text = "Disabled";
			this.wButtonEdit3.UseStaticViewStyle = true;
			// 
			// wButtonEdit3.ViewStyle
			// 
			// 
			// wComboBox2
			// 
			this.wComboBox2.DropDownWidth = 120;
			this.wComboBox2.EditStyle = LumiSoft.UI.Controls.EditStyle.ReadOnly;
			this.wComboBox2.Location = new System.Drawing.Point(384, 120);
			this.wComboBox2.Name = "wComboBox2";
			this.wComboBox2.SelectedIndex = -1;
			this.wComboBox2.SelectionLength = 0;
			this.wComboBox2.SelectionStart = 0;
			this.wComboBox2.Size = new System.Drawing.Size(120, 20);
			this.wComboBox2.TabIndex = 14;
			this.wComboBox2.Text = "ReadOnly";
			this.wComboBox2.UseStaticViewStyle = true;
			// 
			// wComboBox2.ViewStyle
			// 
			this.wComboBox2.VisibleItems = 5;
			// 
			// wComboBox3
			// 
			this.wComboBox3.DropDownWidth = 120;
			this.wComboBox3.Enabled = false;
			this.wComboBox3.Location = new System.Drawing.Point(384, 144);
			this.wComboBox3.Name = "wComboBox3";
			this.wComboBox3.SelectedIndex = -1;
			this.wComboBox3.SelectionLength = 0;
			this.wComboBox3.SelectionStart = 0;
			this.wComboBox3.Size = new System.Drawing.Size(120, 20);
			this.wComboBox3.TabIndex = 15;
			this.wComboBox3.Text = "Disabled";
			this.wComboBox3.UseStaticViewStyle = true;
			// 
			// wComboBox3.ViewStyle
			// 
			this.wComboBox3.VisibleItems = 5;
			// 
			// wDatePicker2
			// 
			this.wDatePicker2.EditStyle = LumiSoft.UI.Controls.EditStyle.ReadOnly;
			this.wDatePicker2.Location = new System.Drawing.Point(384, 224);
			this.wDatePicker2.Name = "wDatePicker2";
			this.wDatePicker2.TabIndex = 16;
			this.wDatePicker2.UseStaticViewStyle = true;
			// 
			// wDatePicker2.ViewStyle
			// 
			// 
			// wDatePicker3
			// 
			this.wDatePicker3.EditStyle = LumiSoft.UI.Controls.EditStyle.Selectable;
			this.wDatePicker3.Location = new System.Drawing.Point(384, 200);
			this.wDatePicker3.Name = "wDatePicker3";
			this.wDatePicker3.TabIndex = 17;
			this.wDatePicker3.UseStaticViewStyle = true;
			// 
			// wDatePicker3.ViewStyle
			// 
			// 
			// wCheckBox2
			// 
			this.wCheckBox2.Checked = false;
			this.wCheckBox2.Location = new System.Drawing.Point(280, 328);
			this.wCheckBox2.Name = "wCheckBox2";
			this.wCheckBox2.ReadOnly = true;
			this.wCheckBox2.Size = new System.Drawing.Size(30, 22);
			this.wCheckBox2.TabIndex = 18;
			this.wCheckBox2.UseStaticViewStyle = true;
			// 
			// wCheckBox2.ViewStyle
			// 
			// 
			// wCheckBox3
			// 
			this.wCheckBox3.Checked = false;
			this.wCheckBox3.Enabled = false;
			this.wCheckBox3.Location = new System.Drawing.Point(320, 328);
			this.wCheckBox3.Name = "wCheckBox3";
			this.wCheckBox3.ReadOnly = false;
			this.wCheckBox3.Size = new System.Drawing.Size(30, 22);
			this.wCheckBox3.TabIndex = 19;
			this.wCheckBox3.UseStaticViewStyle = true;
			// 
			// wCheckBox3.ViewStyle
			// 
			// 
			// m_pFlashControls
			// 
			this.m_pFlashControls.Location = new System.Drawing.Point(456, 360);
			this.m_pFlashControls.Name = "m_pFlashControls";
			this.m_pFlashControls.Size = new System.Drawing.Size(64, 32);
			this.m_pFlashControls.TabIndex = 20;
			this.m_pFlashControls.Text = "Flash controls";
			// 
			// m_pFlashControls.ViewStyle
			// 
			this.m_pFlashControls.ButtonPressed += new LumiSoft.UI.Controls.ButtonPressedEventHandler(this.m_pFlashControls_Click);
			// 
			// wTime1
			// 
			this.wTime1.BackColor = System.Drawing.Color.White;
			this.wTime1.DrawBorder = true;
			this.wTime1.Location = new System.Drawing.Point(240, 376);
			this.wTime1.Name = "wTime1";
			this.wTime1.ShowSeconds = true;
			this.wTime1.Size = new System.Drawing.Size(80, 20);
			this.wTime1.TabIndex = 21;
			this.wTime1.UseStaticViewStyle = true;
			this.wTime1.Value = new System.DateTime(2003, 5, 29, 0, 0, 0, 0);
			// 
			// wTime1.ViewStyle
			// 
			// 
			// wLabel1
			// 
			this.wLabel1.Location = new System.Drawing.Point(240, 360);
			this.wLabel1.Name = "wLabel1";
			this.wLabel1.Size = new System.Drawing.Size(80, 16);
			this.wLabel1.TabIndex = 22;
			this.wLabel1.Text = "Time edit";
			this.wLabel1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			// 
			// wLabel1.ViewStyle
			// 
			// 
			// wLabel2
			// 
			this.wLabel2.Location = new System.Drawing.Point(368, 368);
			this.wLabel2.Name = "wLabel2";
			this.wLabel2.Size = new System.Drawing.Size(80, 16);
			this.wLabel2.TabIndex = 23;
			this.wLabel2.Text = "wLabel2";
			// 
			// wLabel2.ViewStyle
			// 
			// 
			// wTabBar1
			// 
			this.wTabBar1.Cursor = System.Windows.Forms.Cursors.Hand;
			this.wTabBar1.ImageList = null;
			this.wTabBar1.Location = new System.Drawing.Point(240, 32);
			this.wTabBar1.Name = "wTabBar1";
			this.wTabBar1.SelectedTab = null;
			this.wTabBar1.Size = new System.Drawing.Size(264, 24);
			this.wTabBar1.TabIndex = 24;
			// 
			// m_SaveToXml
			// 
			this.m_SaveToXml.Location = new System.Drawing.Point(240, 400);
			this.m_SaveToXml.Name = "m_SaveToXml";
			this.m_SaveToXml.Size = new System.Drawing.Size(128, 24);
			this.m_SaveToXml.TabIndex = 25;
			this.m_SaveToXml.Text = "Save ViewStyle to XML";
			// 
			// m_SaveToXml.ViewStyle
			// 
			this.m_SaveToXml.ButtonPressed += new LumiSoft.UI.Controls.ButtonPressedEventHandler(this.m_SaveToXml_ButtonPressed);
			// 
			// m_pLoadFromXml
			// 
			this.m_pLoadFromXml.Location = new System.Drawing.Point(384, 400);
			this.m_pLoadFromXml.Name = "m_pLoadFromXml";
			this.m_pLoadFromXml.Size = new System.Drawing.Size(136, 24);
			this.m_pLoadFromXml.TabIndex = 26;
			this.m_pLoadFromXml.Text = "Load ViewStyle from XML";
			// 
			// m_pLoadFromXml.ViewStyle
			// 
			this.m_pLoadFromXml.ButtonPressed += new LumiSoft.UI.Controls.ButtonPressedEventHandler(this.m_pLoadFromXml_ButtonPressed);
			// 
			// wButtonEdit4
			// 
			this.wButtonEdit4.EditStyle = LumiSoft.UI.Controls.EditStyle.ReadOnly;
			this.wButtonEdit4.Location = new System.Drawing.Point(240, 280);
			this.wButtonEdit4.Name = "wButtonEdit4";
			this.wButtonEdit4.TabIndex = 27;
			this.wButtonEdit4.Text = "ReadOnly";
			this.wButtonEdit4.UseStaticViewStyle = true;
			// 
			// wButtonEdit4.ViewStyle
			// 
			// 
			// wComboBox4
			// 
			this.wComboBox4.DropDownWidth = 120;
			this.wComboBox4.EditStyle = LumiSoft.UI.Controls.EditStyle.Selectable;
			this.wComboBox4.Location = new System.Drawing.Point(384, 96);
			this.wComboBox4.Name = "wComboBox4";
			this.wComboBox4.SelectedIndex = -1;
			this.wComboBox4.SelectionLength = 0;
			this.wComboBox4.SelectionStart = 0;
			this.wComboBox4.Size = new System.Drawing.Size(120, 20);
			this.wComboBox4.TabIndex = 28;
			this.wComboBox4.Text = "Selectable";
			this.wComboBox4.UseStaticViewStyle = true;
			// 
			// wComboBox4.ViewStyle
			// 
			this.wComboBox4.VisibleItems = 5;
			// 
			// wDatePicker4
			// 
			this.wDatePicker4.EditStyle = LumiSoft.UI.Controls.EditStyle.ReadOnly;
			this.wDatePicker4.Enabled = false;
			this.wDatePicker4.Location = new System.Drawing.Point(384, 248);
			this.wDatePicker4.Name = "wDatePicker4";
			this.wDatePicker4.TabIndex = 29;
			this.wDatePicker4.UseStaticViewStyle = true;
			// 
			// wDatePicker4.ViewStyle
			// 
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(536, 437);
			this.Controls.Add(this.wDatePicker4);
			this.Controls.Add(this.wComboBox4);
			this.Controls.Add(this.wButtonEdit4);
			this.Controls.Add(this.m_pLoadFromXml);
			this.Controls.Add(this.m_SaveToXml);
			this.Controls.Add(this.wTabBar1);
			this.Controls.Add(this.wLabel2);
			this.Controls.Add(this.wLabel1);
			this.Controls.Add(this.wTime1);
			this.Controls.Add(this.m_pFlashControls);
			this.Controls.Add(this.wCheckBox3);
			this.Controls.Add(this.wCheckBox2);
			this.Controls.Add(this.wDatePicker3);
			this.Controls.Add(this.wDatePicker2);
			this.Controls.Add(this.wComboBox3);
			this.Controls.Add(this.wComboBox2);
			this.Controls.Add(this.wButtonEdit3);
			this.Controls.Add(this.wButtonEdit2);
			this.Controls.Add(this.wSpinEdit3);
			this.Controls.Add(this.wSpinEdit2);
			this.Controls.Add(this.wEditBox3);
			this.Controls.Add(this.wEditBox2);
			this.Controls.Add(this.toolBar1);
			this.Controls.Add(this.wCheckBox1);
			this.Controls.Add(this.wComboBox1);
			this.Controls.Add(this.wDatePicker1);
			this.Controls.Add(this.wButtonEdit1);
			this.Controls.Add(this.wPictureBox1);
			this.Controls.Add(this.wSpinEdit1);
			this.Controls.Add(this.wEditBox1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}
		#endregion

		
		#region Events handling

		private void wButtonEdit1_ButtonPressed(object sender, System.EventArgs e)
		{
			MessageBox.Show("Button pressed");
		}

		private void m_pFlashControls_Click(object sender, System.EventArgs e)
		{
			wComboBox1.FlashControl();
			wEditBox1.FlashControl();
			wDatePicker1.FlashControl();
			wSpinEdit1.FlashControl();
			wButtonEdit1.FlashControl();
		}

		private void menuItem1_Click(object sender, System.EventArgs e)
		{
			toolBarm_pFlashControls.Enabled = true;
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			toolBarm_pFlashControls.Enabled = false;
		}

		#region function m_SaveToXml_ButtonPressed

		private void m_SaveToXml_ButtonPressed(object sender, System.EventArgs e)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.RestoreDirectory = true;
			dlg.Filter = "ViewSyle|*.vst";
			if(dlg.ShowDialog(this) == DialogResult.OK){
				using(FileStream fs = File.Create(dlg.FileName)){
					byte[] data = LumiSoft.UI.ViewStyle.staticViewStyle.SaveToXml();
					
					fs.Write(data,0,data.Length);
				}
			}			
		}

		#endregion

		#region function m_pLoadFromXml_ButtonPressed

		private void m_pLoadFromXml_ButtonPressed(object sender, System.EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.RestoreDirectory = true;
			dlg.Filter = "ViewSyle|*.vst";
			if(dlg.ShowDialog(this) == DialogResult.OK){
				using(FileStream fs = File.OpenRead(dlg.FileName)){
					byte[] data = new Byte[fs.Length];
					fs.Read(data,0,data.Length);

					LumiSoft.UI.ViewStyle.staticViewStyle.LoadFromXml(data);
				}
			}			
		}

		#endregion

		#endregion

	}
}
