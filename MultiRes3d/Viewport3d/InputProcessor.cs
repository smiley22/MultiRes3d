using SlimDX;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MultiRes3d {
	/// <summary>
	/// Verarbeitet Tastatur- u. Mauseingaben für das Viewport3d Control.
	/// </summary>
	internal class InputProcessor {
		/// <summary>
		/// Der Anteil, um welchen die Kamera um ihre Pitch-Achse für eine einzelne Betätigung
		/// des Mausrads rotiert werden soll.
		/// </summary>
		const float mouseWheelNotch = 0.03f;

		/// <summary>
		/// Der Anteil, um welchen die Kamera um ihre Yaw-Achse für eine einzelne Bewegungseinheit
		/// der Maus rotiert werden soll.
		/// </summary>
		const float mouseMoveFactor = 0.03f;

		/// <summary>
		/// Die Kamera Instanz.
		/// </summary>
		Camera camera;

		/// <summary>
		/// Die Instanz des Viewport3d Controls, dessen Eingaben verarbeitet werden sollen.
		/// </summary>
		Viewport3d viewport3d;

		/// <summary>
		/// Die lezte Position der Maus.
		/// </summary>
		Point lastMousePosition;

		/// <summary>
		/// Die Mausbuttons, die momentan gedrückt werden.
		/// </summary>
		MouseButtons? currentButton;

		/// <summary>
		/// Initialisiert eine neue Instanz der InputProcessor Klasse.
		/// </summary>
		/// <param name="viewport3d">
		/// Die Viewport3d Instanz, deren Eingaben verarbeitet werden sollen.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Der Viewport3d Parameter ist null.
		/// </exception>
		public InputProcessor(Viewport3d viewport3d) {
			viewport3d.ThrowIfNull("viewport3d");
			camera = viewport3d.Camera;
			this.viewport3d = viewport3d;
		}

		/// <summary>
		/// Die Event Methode die aufgerufen wird, wenn das assoziierte Control eine
		/// KeyDown Notification erhält.
		/// </summary>
		/// <param name="e">
		/// Die Argumente für den KeyDown Event.
		/// </param>
		public void OnKeyDown(KeyEventArgs e) {
			switch (e.KeyCode) {
				case Keys.R:
					camera.Reset();
					break;
				default:
					return;
			}
			// Neurendern erzwingen.
			viewport3d.Render();
		}

		/// <summary>
		/// Die Event Methode die aufgerufen wird, wenn das assoziierte Control eine
		/// KeyPress Notification erhält.
		/// </summary>
		/// <param name="e">
		/// Die Argumente für den Keypress Event.
		/// </param>
		public void OnKeyPress(KeyPressEventArgs e) {
		}

		/// <summary>
		/// Die Event Methode die aufgerufen wird, wenn das assoziierte Control eine
		/// KeyUp Notification erhält.
		/// </summary>
		/// <param name="e">
		/// Die Argumente für den KeyUp Event.
		/// </param>
		public void OnKeyUp(KeyEventArgs e) {
		}

		/// <summary>
		/// Die Event Methode die aufgerufen wird, wenn das assoziierte Control eine
		/// MouseClick Notification erhält.
		/// </summary>
		/// <param name="e">
		/// Die Argumente für den MouseClick Event.
		/// </param>
		public void OnMouseClick(MouseEventArgs e) {
		}

		/// <summary>
		/// Die Event Methode die aufgerufen wird, wenn das assoziierte Control eine
		/// MouseDoubleClick Notification erhält.
		/// </summary>
		/// <param name="e">
		/// Die Argumente für den MouseDoubleClick Event.
		/// </param>
		public void OnMouseDoubleClick(MouseEventArgs e) {
		}

		/// <summary>
		/// Die Event Methode die aufgerufen wird, wenn das assoziierte Control eine
		/// MouseWheel Notification erhält.
		/// </summary>
		/// <param name="e">
		/// Die Argumente für den MouseWheel Event.
		/// </param>
		public void OnMouseWheel(MouseEventArgs e) {
			float amount = System.Math.Sign(e.Delta) * mouseWheelNotch;
			if (camera.Zoom(amount)) {
				viewport3d.Render();
			}
		}

		/// <summary>
		/// Die Event Methode die aufgerufen wird, wenn das assoziierte Control eine
		/// MouseDown Notification erhält.
		/// </summary>
		/// <param name="e">
		/// Die Argumente für den MouseDown Event.
		/// </param>
		public void OnMouseDown(MouseEventArgs e) {
			lastMousePosition = e.Location;
			currentButton = e.Button;
		}

		/// <summary>
		/// Die Event Methode die aufgerufen wird, wenn das assoziierte Control eine
		/// MouseUp Notification erhält.
		/// </summary>
		/// <param name="e">
		/// Die Argumente für den MouseUp Event.
		/// </param>
		public void OnMouseUp(MouseEventArgs e) {
			currentButton = null;
		}

		/// <summary>
		/// Die Event Methode die aufgerufen wird, wenn das assoziierte Control eine
		/// MouseMove Notification erhält.
		/// </summary>
		/// <param name="e">
		/// Die Argumente für den MouseMove Event.
		/// </param>
		public void OnMouseMove(MouseEventArgs e) {
			if (!currentButton.HasValue)
				return;
			if (currentButton.Value == MouseButtons.Left) {
				var delta = mouseMoveFactor * new Vector2(e.X - lastMousePosition.X, e.Y -
					lastMousePosition.Y);
				camera.Rotate(delta);
				lastMousePosition = e.Location;
//				viewport3d.Render();
			} else if (currentButton.Value == MouseButtons.Right) {
			}
		}
	}
}
