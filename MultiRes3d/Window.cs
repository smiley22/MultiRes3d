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
		static string version = string.Format("{0} {1}", Application.ProductName,
			Application.ProductVersion.Substring(0, 3));
		bool showHelp = true;

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
			if (showHelp)
				DrawHelpText();
			if(pm != null)
				DrawInfoText();
			// Pointlight bewegt sich mit uns mit.
			viewport3d.PointLight.Position = viewport3d.Camera.Eye;
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
		/// Rendert eine kleine "Bedienungsanleitung" zur Benutzung der Anwendung.
		/// </summary>
		void DrawHelpText() {
			var dict = new Dictionary<string, string>() {
				{ "1", "Solid Mode" },
				{ "2", "Wireframe Mode" },
				{ "W", "Increase Object Scale" },
				{ "S", "Decrease Object Scale" },
				{ "↑", "Move Up Object" },
				{ "↓", "Move Down Object" },
				{ "R", "Reset Settings" },
				{ "+", "Increase Detail" },
				{ "-", "Reduce Detail" },
				{ "e", "Expand to max Detail" }
			};
			int xKey = 10,
				xText = 40,
				y = 40,
				lineHeight = 18;
			foreach (var pair in dict) {
				viewport3d.DrawString(pair.Key, new Vector2(xKey, y), Color.FromArgb(0, 255, 0));
				viewport3d.DrawString(pair.Value, new Vector2(xText, y), Color.White);
				y += lineHeight;
			}
			viewport3d.DrawString(version, new Vector2(10, 10), Color.White);
		}

		/// <summary>
		/// Rendert einige Informationen über die Mesh wie Vertex- u. Facettenanzahl.
		/// </summary>
		void DrawInfoText() {
			var percent = pm.NumberOfSplits == 0 ? 100 :
				(100 * pm.CurrentSplit / pm.NumberOfSplits);
			var progression = string.Format("{0}/{1} ({2} %)", pm.CurrentSplit,
				pm.NumberOfSplits, percent);
			viewport3d.DrawString("Number of Vertices", new Vector2(10, 257), Color.White);
			viewport3d.DrawString(pm.NumberOfVertices.ToString(), new Vector2(160, 257),
				Color.Turquoise);
			viewport3d.DrawString("Number of Faces", new Vector2(10, 275), Color.White);
			viewport3d.DrawString(pm.NumberOfFaces.ToString(), new Vector2(160, 275),
				Color.Turquoise);
			viewport3d.DrawString("Level of Progression", new Vector2(10, 305), Color.White);
			viewport3d.DrawString(progression, new Vector2(160, 305), Color.LawnGreen);
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
			
			var testMesh = ObjIO.Load("Testdata/pm-cow.obj");

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
			var moveStep = .1f;
			var scaleStep = .01f;
			var increaseStep = .01;
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
				case Keys.Oemplus:
					pm.ProgressTo(pm.Progress + increaseStep);
					break;
				case Keys.OemMinus:
					pm.ProgressTo(pm.Progress - increaseStep);
					break;
				case Keys.E:
					pm.ProgressTo(1.0);
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

		/// <summary>
		/// Event Methode, die aufgerufen wird, wenn der Menüpunkt "Settings -> Show Help"
		/// betätigt wird.
		/// </summary>
		/// <param name="sender">
		/// Der Sender des Events.
		/// </param>
		/// <param name="e">
		/// Die Event Parameter.
		/// </param>
		void OnClickShowHelp(object sender, EventArgs e) {
			showHelp = MenuShowHelp.Checked;
		}
	}
}
