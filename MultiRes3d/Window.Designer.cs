namespace MultiRes3d {
	partial class Window {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuOpen = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.MenuExit = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuSettings = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuWireframe = new System.Windows.Forms.ToolStripMenuItem();
			this.lightColourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuColourNeutral = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuColourRed = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuColourGreen = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuColourBlue = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuShowHelp = new System.Windows.Forms.ToolStripMenuItem();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.viewport3d = new MultiRes3d.Viewport3d();
			this.menuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.MenuSettings});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(633, 24);
			this.menuStrip.TabIndex = 0;
			this.menuStrip.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuOpen,
            this.toolStripSeparator1,
            this.MenuExit});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// MenuOpen
			// 
			this.MenuOpen.Name = "MenuOpen";
			this.MenuOpen.Size = new System.Drawing.Size(112, 22);
			this.MenuOpen.Text = "Open...";
			this.MenuOpen.Click += new System.EventHandler(this.OnClickMenuOpen);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(109, 6);
			// 
			// MenuExit
			// 
			this.MenuExit.Name = "MenuExit";
			this.MenuExit.Size = new System.Drawing.Size(112, 22);
			this.MenuExit.Text = "Exit";
			this.MenuExit.Click += new System.EventHandler(this.OnClickMenuExit);
			// 
			// MenuSettings
			// 
			this.MenuSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuWireframe,
            this.lightColourToolStripMenuItem,
            this.MenuShowHelp});
			this.MenuSettings.Name = "MenuSettings";
			this.MenuSettings.Size = new System.Drawing.Size(61, 20);
			this.MenuSettings.Text = "Settings";
			// 
			// MenuWireframe
			// 
			this.MenuWireframe.CheckOnClick = true;
			this.MenuWireframe.Name = "MenuWireframe";
			this.MenuWireframe.Size = new System.Drawing.Size(140, 22);
			this.MenuWireframe.Text = "Wireframe";
			this.MenuWireframe.Click += new System.EventHandler(this.OnClickMenuWireframe);
			// 
			// lightColourToolStripMenuItem
			// 
			this.lightColourToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuColourNeutral,
            this.MenuColourRed,
            this.MenuColourGreen,
            this.MenuColourBlue});
			this.lightColourToolStripMenuItem.Name = "lightColourToolStripMenuItem";
			this.lightColourToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
			this.lightColourToolStripMenuItem.Text = "Light Colour";
			// 
			// MenuColourNeutral
			// 
			this.MenuColourNeutral.Checked = true;
			this.MenuColourNeutral.CheckState = System.Windows.Forms.CheckState.Checked;
			this.MenuColourNeutral.Name = "MenuColourNeutral";
			this.MenuColourNeutral.Size = new System.Drawing.Size(113, 22);
			this.MenuColourNeutral.Text = "Neutral";
			this.MenuColourNeutral.Click += new System.EventHandler(this.OnClickMenuColour);
			// 
			// MenuColourRed
			// 
			this.MenuColourRed.Name = "MenuColourRed";
			this.MenuColourRed.Size = new System.Drawing.Size(113, 22);
			this.MenuColourRed.Text = "Red";
			this.MenuColourRed.Click += new System.EventHandler(this.OnClickMenuColour);
			// 
			// MenuColourGreen
			// 
			this.MenuColourGreen.Name = "MenuColourGreen";
			this.MenuColourGreen.Size = new System.Drawing.Size(113, 22);
			this.MenuColourGreen.Text = "Green";
			this.MenuColourGreen.Click += new System.EventHandler(this.OnClickMenuColour);
			// 
			// MenuColourBlue
			// 
			this.MenuColourBlue.Name = "MenuColourBlue";
			this.MenuColourBlue.Size = new System.Drawing.Size(113, 22);
			this.MenuColourBlue.Text = "Blue";
			this.MenuColourBlue.Click += new System.EventHandler(this.OnClickMenuColour);
			// 
			// MenuShowHelp
			// 
			this.MenuShowHelp.Checked = true;
			this.MenuShowHelp.CheckOnClick = true;
			this.MenuShowHelp.CheckState = System.Windows.Forms.CheckState.Checked;
			this.MenuShowHelp.Name = "MenuShowHelp";
			this.MenuShowHelp.Size = new System.Drawing.Size(140, 22);
			this.MenuShowHelp.Text = "Show Help";
			this.MenuShowHelp.Click += new System.EventHandler(this.OnClickShowHelp);
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "Obj files|*.obj|All files|*.*";
			// 
			// viewport3d
			// 
			this.viewport3d.BackColor = System.Drawing.Color.DarkGray;
			this.viewport3d.Dock = System.Windows.Forms.DockStyle.Fill;
			this.viewport3d.Location = new System.Drawing.Point(0, 24);
			this.viewport3d.Name = "viewport3d";
			this.viewport3d.Size = new System.Drawing.Size(633, 430);
			this.viewport3d.TabIndex = 1;
			this.viewport3d.VSync = true;
			this.viewport3d.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
			// 
			// Window
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(633, 454);
			this.Controls.Add(this.viewport3d);
			this.Controls.Add(this.menuStrip);
			this.MainMenuStrip = this.menuStrip;
			this.Name = "Window";
			this.Text = "Window";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClosed);
			this.Load += new System.EventHandler(this.OnLoad);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip;
		private Viewport3d viewport3d;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem MenuOpen;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem MenuExit;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.ToolStripMenuItem MenuSettings;
		private System.Windows.Forms.ToolStripMenuItem MenuWireframe;
		private System.Windows.Forms.ToolStripMenuItem lightColourToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem MenuColourNeutral;
		private System.Windows.Forms.ToolStripMenuItem MenuColourRed;
		private System.Windows.Forms.ToolStripMenuItem MenuColourGreen;
		private System.Windows.Forms.ToolStripMenuItem MenuColourBlue;
		private System.Windows.Forms.ToolStripMenuItem MenuShowHelp;
	}
}

