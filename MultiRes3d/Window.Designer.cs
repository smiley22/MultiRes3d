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
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.viewport3d = new MultiRes3d.Viewport3d();
			this.menuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
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
			// openFileDialog
			// 
			this.openFileDialog.Filter = "Obj files|*.obj|All files|*.*";
			// 
			// viewport3d
			// 
			this.viewport3d.BackColor = System.Drawing.SystemColors.ControlLight;
			this.viewport3d.Dock = System.Windows.Forms.DockStyle.Fill;
			this.viewport3d.Location = new System.Drawing.Point(0, 24);
			this.viewport3d.Name = "viewport3d";
			this.viewport3d.Size = new System.Drawing.Size(633, 430);
			this.viewport3d.TabIndex = 1;
			this.viewport3d.VSync = true;
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
	}
}

