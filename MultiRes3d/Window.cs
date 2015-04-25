using SlimDX;
using SlimDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MultiRes3d {
	/// <summary>
	/// Repräsentiert das Fenster der Anwendung.
	/// </summary>
	public partial class Window : Form {
		FrameCounter frameCounter = new FrameCounter();
		PM pm;

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
			viewport3d.PointLight.Position = viewport3d.Camera.Eye;
			//	+ viewport3d.Camera.Up.Normalized() * -2;

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
			if (openFileDialog.ShowDialog() != DialogResult.OK)
				return;
			try {
				var mesh = ObjIO.Load(openFileDialog.FileName);
				var test = new PM(viewport3d, mesh);

				SetRenderObject(test);
			} catch (Exception ex) {
				MessageBox.Show(ex.Message, "Fehler beim Einlesen der Mesh",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// Setzt das angegebene Objekt als neues Renderobjekt.
		/// </summary>
		/// <param name="mesh">
		/// Das Objekt, das als neues Renderobjekt gesetzt werden soll.
		/// </param>
		void SetRenderObject(PM mesh) {
			// Alte PM entfernen und entsorgen.
			if (pm != null) {
				viewport3d.Entities.Remove(pm);
				pm.Dispose();
			}
			pm = mesh;
			viewport3d.Entities.Add(pm);
			// Kamera, Skalierung, Licht usw. zurücksetzen.
			Reset();
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
			
			var testMesh = ObjIO.Load("Testdata/bunny.obj");

			// 1. PM aus testMesh erstellen.
			var test = new PM(viewport3d, testMesh);
			test.Scale = 0.15f;
			test.Position = new Vector3(test.Position.X, test.Position.Y - .5f, test.Position.Z);

			SetRenderObject(test);
			
			viewport3d.PointLight.Ambient = new Color4(0.34f, 0.34f, 0.34f);
			viewport3d.PointLight.Diffuse = Color.Gray;
			viewport3d.PointLight.Attenuation = new Vector3(0.5f, 0.1f, 0.0f);
			viewport3d.PointLight.Range = 7.03f;
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

		/// <summary>
		/// Event Methode, die aufgerufen wird, wenn eine Taste gedrückt wurde.
		/// </summary>
		/// <param name="sender">
		/// Der Sender des Events.
		/// </param>
		/// <param name="e">
		/// Die Event Parameter.
		/// </param>
		void OnKeyDown(object sender, KeyEventArgs e) {
			float moveStep = .1f;
			float scaleStep = .01f;
			if (pm == null)
				return;
			switch (e.KeyCode) {
				case Keys.D1:
					SetWireframe(false);
					break;
				case Keys.D2:
					SetWireframe(true);
					break;
				case Keys.Up:
					pm.Y += moveStep;
					break;
				case Keys.Down:
					pm.Y -= moveStep;
					break;
				case Keys.W:
					pm.Scale += scaleStep;
					break;
				case Keys.S:
					pm.Scale -= scaleStep;
					break;
				case Keys.R:
					Reset();
					break;
			}
		}

		/// <summary>
		/// Event Methode, die aufgerufen wird, wenn der Menüpunkt "Settings -> Wireframe"
		/// betätigt wird.
		/// </summary>
		/// <param name="sender">
		/// Der Sender des Events.
		/// </param>
		/// <param name="e">
		/// Die Event Parameter.
		/// </param>
		void OnClickMenuWireframe(object sender, EventArgs e) {
			SetWireframe(MenuWireframe.Checked);
		}

		/// <summary>
		/// Event Methode, die aufgerufen wird, wenn einer der Unterpunkte des Menüpunkts
		/// "Settings -> Colours" betätigt wird.
		/// </summary>
		/// <param name="sender">
		/// Der Sender des Events.
		/// </param>
		/// <param name="e">
		/// Die Event Parameter.
		/// </param>
		void OnClickMenuColour(object sender, EventArgs e) {
			var dict = new Dictionary<ToolStripMenuItem, Color4>() {
				{ MenuColourNeutral,	Color.Gray },
				{ MenuColourRed,		Color.Red },
				{ MenuColourGreen,		Color.Green },
				{ MenuColourBlue,		Color.CornflowerBlue }
			};
			viewport3d.PointLight.Diffuse = dict[sender as ToolStripMenuItem];
			foreach (var k in dict.Keys) {
				k.Checked = k == sender;
			}
		}

		/// <summary>
		/// Schaltet Wireframe Darstellung an oder aus.
		/// </summary>
		/// <param name="enable">
		/// true, um das Objekt in Wireframe Darstellung zu rendern; andernfalls false.
		/// </param>
		void SetWireframe(bool enable) {
			viewport3d.FillMode = enable ? FillMode.Wireframe : FillMode.Solid;
			MenuWireframe.Checked = enable;
		}

		/// <summary>
		/// Setzt alle Einstellungen auf ihre Standardwerte zurück.
		/// </summary>
		void Reset() {
			pm.Scale = 1.0f;
			pm.Position = Vector3.Zero;
			SetWireframe(false);
			// Hack: Einfach einen Klick simulieren, damit die Menüpunkte richtig
			//       abgehakt werden.
			OnClickMenuColour(MenuColourNeutral, EventArgs.Empty);
			// Kamera muss nicht zurückgesetzt werden, da dies bereits vom Viewport3d
			// Control erledigt wird (siehe InputProcessor).
		}
	}
}
