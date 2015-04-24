using System;
using System.Drawing;
using System.Windows.Forms;

namespace MultiRes3d {
	public partial class Window : Form {
		FrameCounter frameCounter = new FrameCounter();

		public Window() {
			frameCounter.FPSCalculated += OnFPSCalculated;
			InitializeComponent();
		}

		public void Run() {
			if (WindowState == FormWindowState.Minimized)
				return;
			frameCounter.Count();
			Update(frameCounter.DeltaTime);
			viewport3d.Render();
		}

		void Update(double deltaTime) {
			viewport3d.DrawString("Hello World", new SlimDX.Vector2(320, 100), Color.Green);
		}

		void OnFPSCalculated(object sender, int e) {
			var mspf = 1000.0 / e;
			Text = String.Format("FPS: {0}, MSPF: {1} ms", e, mspf.ToString("0.000"));
		}

		void OnClickMenuOpen(object sender, EventArgs e) {
			if (openFileDialog.ShowDialog() == DialogResult.OK) {
				MessageBox.Show(openFileDialog.FileName);
			}
		}

		void OnClickMenuExit(object sender, EventArgs e) {
			Close();
		}

	}
}
