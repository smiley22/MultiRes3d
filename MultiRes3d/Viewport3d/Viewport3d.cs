using SlimDX;
using SlimDX.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace MultiRes3d {
	/// <summary>
	/// Implementiert ein WinForms Control für das Rendern einer 3D Szene.
	/// </summary>
	public class Viewport3d : D3D11Control {
		/// <summary>
		/// Verarbeitet Maus- u. Tastatureingaben des Controls.
		/// </summary>
		InputProcessor inputProcessor;

		/// <summary>
		/// Rendererkomponente, die für das eigentliche Rendern zuständig ist.
		/// </summary>
		Renderer renderer;

		/// <summary>
		/// Die Kamera, die den Blickpunkt innerhalb der 3D Szene darstellt.
		/// </summary>
		[Browsable(false)]
		public Camera Camera {
			get;
			private set;
		}

		/// <summary>
		/// Das globale direktionale Licht der 3D Szene.
		/// </summary>
		[Browsable(false)]
		public DirectionalLight DirectionalLight {
			get {
				return renderer.DirectionalLight;
			}
		}

		/// <summary>
		/// Der Effect zum Setzen der Shader Variablen.
		/// </summary>
		[Browsable(false)]
		public BasicEffect Effect {
			get {
				return renderer != null ? renderer.Effect : null;
			}
		}

		/// <summary>
		/// Eine Punktlichtquelle innerhalb der 3D Szene.
		/// </summary>
		public PointLight PointLight {
			get {
				return renderer.PointLight;
			}
		}

		/// <summary>
		/// Die Liste der Objekte der Szene.
		/// </summary>
		public IList<Entity> Entities {
			get {
				return renderer != null ? renderer.Entities : null;
			}
		}

		/// <summary>
		/// Die Hintergrundfarbe des Controls.
		/// </summary>
		public override Color BackColor {
			get {
				return renderer != null ? renderer.ClearColor : base.BackColor;
			}
			set {
				base.BackColor = value;
				if (renderer != null)
					renderer.ClearColor = value;
			}
		}

		/// <summary>
		/// Initialisiert eine neue Instanz der Viewport3d Szene.
		/// </summary>
		public Viewport3d()
			: base() {
			Camera = new Camera();
			renderer = new Renderer(this, Camera);
			inputProcessor = new InputProcessor(this);
		}

		/// <summary>
		/// Rendert die 3D Szene ins Fenster des Controls.
		/// </summary>
		public void Render() {
			renderer.Render();
			// Swap the buffers.
			Present();
		}

		/// <summary>
		/// Rendert den angegebenen Text an der angegebenen Position in der angegebenen
		/// Farbe.
		/// </summary>
		/// <param name="text">
		/// Der Text.
		/// </param>
		/// <param name="position">
		/// Die Position in absoluten Koordinaten.
		/// </param>
		/// <param name="color">
		/// Die Farbe in der der Text gezeichnet werden soll.
		/// </param>
		public void DrawString(string text, Vector2 position, Color color) {
			renderer.DrawString(text, position, color);
		}

		/// <summary>
		/// Event Methode die ausgelöst wird, wenn sich das Seitenverhältnis des
		/// Controls geändert hat.
		/// </summary>
		/// <param name="e">
		/// Die Parameter des Events.
		/// </param>
		protected override void OnAspectRatioChanged(EventArgs e) {
			base.OnAspectRatioChanged(e);
			float aspectRatio = ClientSize.Width / (float) ClientSize.Height;
			if (renderer != null)
				renderer.OnAspectRatioChanged(aspectRatio);
		}

		/// <summary> 
		/// Gibt die Resourcen der Instanz frei.
		/// </summary>
		/// <param name="disposing">
		/// true, um managed Resourcen freizugeben; andernfalls false.
		/// </param>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (renderer != null)
					renderer.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Input Processing
		/// <summary>
		/// Die Event Methode die aufgerufen wird, wenn das Control eine KeyDown
		/// Notification erhält.
		/// </summary>
		/// <param name="e">
		/// Die Argumente für den KeyDown Event.
		/// </param>
		protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e) {
			base.OnKeyDown(e);
			inputProcessor.OnKeyDown(e);
		}

		/// <summary>
		/// Die Event Methode die aufgerufen wird, wenn das Control eine KeyPress
		/// Notification erhält.
		/// </summary>
		/// <param name="e">
		/// Die Argumente für den Keypress Event.
		/// </param>
		protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e) {
			base.OnKeyPress(e);
			inputProcessor.OnKeyPress(e);
		}

		/// <summary>
		/// Die Event Methode die aufgerufen wird, wenn das Control eine KeyUp
		/// Notification erhält.
		/// </summary>
		/// <param name="e">
		/// Die Argumente für den KeyUp Event.
		/// </param>
		protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e) {
			base.OnKeyUp(e);
			inputProcessor.OnKeyUp(e);
		}

		/// <summary>
		/// Die Event Methode die aufgerufen wird, wenn das Control eine MouseClick
		/// Notification erhält.
		/// </summary>
		/// <param name="e">
		/// Die Argumente für den MouseClick Event.
		/// </param>
		protected override void OnMouseClick(System.Windows.Forms.MouseEventArgs e) {
			base.OnMouseClick(e);
			inputProcessor.OnMouseClick(e);
		}

		/// <summary>
		/// Die Event Methode die aufgerufen wird, wenn das Control eine MouseDoubleClick
		/// Notification erhält.
		/// </summary>
		/// <param name="e">
		/// Die Argumente für den MouseDoubleClick Event.
		/// </param>
		protected override void OnMouseDoubleClick(System.Windows.Forms.MouseEventArgs e) {
			base.OnMouseDoubleClick(e);
			inputProcessor.OnMouseDoubleClick(e);
		}

		/// <summary>
		/// Die Event Methode die aufgerufen wird, wenn das Control eine MouseWheel
		/// Notification erhält.
		/// </summary>
		/// <param name="e">
		/// Die Argumente für den MouseWheel Event.
		/// </param>
		protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e) {
			base.OnMouseWheel(e);
			inputProcessor.OnMouseWheel(e);
		}

		/// <summary>
		/// Die Event Methode die aufgerufen wird, wenn das Control eine MouseDown
		/// Notification erhält.
		/// </summary>
		/// <param name="e">
		/// Die Argumente für den MouseDown Event.
		/// </param>
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e) {
			base.OnMouseDown(e);
			inputProcessor.OnMouseDown(e);
		}

		/// <summary>
		/// Die Event Methode die aufgerufen wird, wenn das Control eine MouseUp
		/// Notification erhält.
		/// </summary>
		/// <param name="e">
		/// Die Argumente für den MouseUp Event.
		/// </param>
		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e) {
			base.OnMouseUp(e);
			inputProcessor.OnMouseUp(e);
		}

		/// <summary>
		/// Die Event Methode die aufgerufen wird, wenn das Control eine MouseMove
		/// Notification erhält.
		/// </summary>
		/// <param name="e">
		/// Die Argumente für den MouseMove Event.
		/// </param>
		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e) {
			base.OnMouseMove(e);
			inputProcessor.OnMouseMove(e);
		}
		#endregion
	}
}
