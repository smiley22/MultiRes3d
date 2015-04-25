using SlimDX;
using SlimDX.Direct3D11;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MultiRes3d {
	/// <summary>
	/// Repräsentiert das Fenster der Anwendung.
	/// </summary>
	public partial class Window : Form {
		FrameCounter frameCounter = new FrameCounter();

		/// <summary>
		/// Initialisiert eine neue Instanz der Window-Klasse.
		/// </summary>
		public Window() {
			frameCounter.FPSCalculated += OnFPSCalculated;
			InitializeComponent();
		}

		/// <summary>
		/// Wird kontinuierlich vom System aufgerufen, wenn keine anderweitigen Nachrichten
		/// verarbeitet werden müssen.
		/// </summary>
		public void Run() {
			if (WindowState == FormWindowState.Minimized)
				return;
			frameCounter.Count();
			Update(frameCounter.DeltaTime);
			viewport3d.Render();
		}

		/// <summary>
		/// Hier werden die Szenen Objekte aktualisiert usw.
		/// </summary>
		/// <param name="deltaTime">
		/// Die Zeitspanne, die seit dem letzten Aufruf vergangen ist.
		/// </param>
		void Update(double deltaTime) {
		}

		/// <summary>
		/// Event Methode, die regelmäßig von der <c>FrameCounter</c> Instanz aufgerufen
		/// wird, um die FPS-Anzeige zu aktualisieren.
		/// </summary>
		/// <param name="sender">
		/// Der Sender des Events.
		/// </param>
		/// <param name="e">
		/// Die Frames pro Sekunde.
		/// </param>
		void OnFPSCalculated(object sender, int e) {
			var mspf = 1000.0 / e;
			Text = String.Format("FPS: {0}, MSPF: {1} ms", e, mspf.ToString("0.000"));
		}

		/// <summary>
		/// Event Methode, die aufgerufen wird, wenn der Menüpunkt "File -> Open..."
		/// betätigt wird.
		/// </summary>
		/// <param name="sender">
		/// Der Sender des Events.
		/// </param>
		/// <param name="e">
		/// Die Event Parameter.
		/// </param>
		void OnClickMenuOpen(object sender, EventArgs e) {
			if (openFileDialog.ShowDialog() == DialogResult.OK) {
				MessageBox.Show(openFileDialog.FileName);
			}
		}

		/// <summary>
		/// Event Methode, die aufgerufen wird, wenn der Menüpunkt "File -> Close"
		/// betätigt wird.
		/// </summary>
		/// <param name="sender">
		/// Der Sender des Events.
		/// </param>
		/// <param name="e">
		/// Die Event Parameter.
		/// </param>
		void OnClickMenuExit(object sender, EventArgs e) {
			// Form schließen und damit Anwendung beenden.
			Close();
		}

		Mesh testMesh;
		PM pm;

		/// <summary>
		/// Event Methode, die aufgerufen wird, wenn das Fenster fertig initialisiert
		/// wurde.
		/// </summary>
		/// <param name="sender">
		/// Der Sender des Events.
		/// </param>
		/// <param name="e">
		/// Die Event Parameter.
		/// </param>
		void OnLoad(object sender, EventArgs e) {
			testMesh = ObjIO.Load("Testdata/bunny.obj");

			// 1. PM aus testMesh erstellen.
			pm = new PM(viewport3d, testMesh);
			pm.Scale = 0.15f;

			// 2. PM viewport.Entities hinzufügen.
			viewport3d.Entities.Add(pm);

			viewport3d.PointLight.Ambient = new Color4(0.34f, 0.34f, 0.34f);

			viewport3d.PointLight.Diffuse = new Color4(0.7f, 0.5f, 0.3f);
			viewport3d.PointLight.Position = new Vector3(0.8f, 1f, -0.5f);
			viewport3d.PointLight.Attenuation = new Vector3(0.0f, 0.5f, 0.0f);
			viewport3d.PointLight.Range = 3.03f;
		}

		/// <summary>
		/// Event Methode, die aufgerufn wird, wenn das Fenster geschlossen wurde.
		/// </summary>
		/// <param name="sender">
		/// Der Sender des Events.
		/// </param>
		/// <param name="e">
		/// Die Event Parameter.
		/// </param>
		void OnClosed(object sender, FormClosedEventArgs e) {
			if (pm != null) {
				viewport3d.Entities.Remove(pm);
				pm.Dispose();
			}

		}
	}
}
