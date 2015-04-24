using SlimDX;

namespace MultiRes3d {
	/// <summary>
	/// Repräsentiert eine Kamera.
	/// </summary>
	public class Camera {
		/// <summary>
		/// Die standarmäßige Position des Augpunkts.
		/// </summary>
		public static readonly Vector3 DefaultEye = new Vector3(0, 0, -5);

		/// <summary>
		/// Der standardmäßige Referenzpunkt der Kamera. 
		/// </summary>
		public static readonly Vector3 DefaultRef = new Vector3(0, 0, 0);

		/// <summary>
		/// Der standardmäßige Up-Vektor der Kamera.
		/// </summary>
		public static readonly Vector3 DefaultUp = new Vector3(0, 1, 0);

		/// <summary>
		/// Der standardmäßige Zoom-Faktor der Kamera.
		/// </summary>
		public const float DefaultZoomFactor = 1.0f;

		/// <summary>
		/// Der maximale Zoom der Kamera.
		/// </summary>
		public const float MaxZoomFactor = 1.5f;

		/// <summary>
		/// Der minimale Zoom der Kamera.
		/// </summary>
		public const float MinZoomFactor = 0.7f;

		/// <summary>
		/// Die initiale Position des Augpunkts.
		/// </summary>
		readonly Vector3 initialEye;

		/// <summary>
		/// Der initiale Referenzpunkt der Kamera.
		/// </summary>
		readonly Vector3 initialRef;

		/// <summary>
		/// Der initiale Up-Vektor der Kamera.
		/// </summary>
		readonly Vector3 initialUp;

		/// <summary>
		/// Gibt an, ob die View Matrix neu berechnet werden muss.
		/// </summary>
		bool updateViewMatrix = true;

		/// <summary>
		/// Die momentane View Matrix.
		/// </summary>
		Matrix viewMatrix = Matrix.Identity;

		/// <summary>
		/// Der Augpunkt.
		/// </summary>
		public Vector3 Eye {
			get;
			private set;
		}

		/// <summary>
		/// Der Referenzpunkt der Kamera.
		/// </summary>
		public Vector3 Ref {
			get;
			private set;
		}

		/// <summary>
		/// Der Up-Vektor der Kamera.
		/// </summary>
		public Vector3 Up {
			get;
			private set;
		}

		/// <summary>
		/// Der Zoom-Faktor der Kamera.
		/// </summary>
		public float ZoomFactor {
			get;
			private set;
		}

		/// <summary>
		/// Liefert die für die Kamera berechnete View Matrix zurück.
		/// </summary>
		public Matrix ViewMatrix {
			get {
				if (updateViewMatrix) {
					viewMatrix = Matrix.LookAtLH(Eye, Ref, Up);
					updateViewMatrix = false;
				}
				return viewMatrix;
			}
		}

		/// <summary>
		/// Initialisiert eine neue Instanz der Kamera Klasse.
		/// </summary>
		/// <param name="eye">
		/// Die initiale Position des Augpunkts.
		/// </param>
		/// <param name="ref">
		/// Der initiale Referenzpunkt der Kamera.
		/// </param>
		/// <param name="up">
		/// Der initiale Up-Vektor der Kamera.
		/// </param>
		public Camera(Vector3? eye = null, Vector3? @ref = null, Vector3? up = null) {
			Eye = eye.HasValue ? eye.Value : DefaultEye;
			Ref = @ref.HasValue ? @ref.Value : DefaultRef;
			Up = up.HasValue ? up.Value : DefaultUp;
			initialEye = Eye;
			initialRef = Ref;
			initialUp = Up;
			ZoomFactor = DefaultZoomFactor;
		}

		/// <summary>
		/// Setzt die Kamera auf ihre Standardwerte zurück.
		/// </summary>
		public void Reset() {
			Eye = initialEye;
			Ref = initialRef;
			Up = initialUp;
			ZoomFactor = DefaultZoomFactor;
			updateViewMatrix = true;
		}

		/// <summary>
		/// Ändert den Zoom der Kameralinse.
		/// </summary>
		/// <param name="delta">
		/// Ein Deltawert, der auf den momentanen Zoom-Faktor addiert wird.
		/// </param>
		/// <returns>
		/// true, wenn der Zoom-Faktor sich geändert hat; ansonsten false.
		/// </returns>
		public bool Zoom(float delta) {
			float oldZoomFactor = ZoomFactor;
			ZoomFactor = Math.Clamp(ZoomFactor + delta, MinZoomFactor, MaxZoomFactor);
			return oldZoomFactor != ZoomFactor;
		}

		/// <summary>
		/// Rotiert die Kamera um ihre Yaw und Pitch Achsen.
		/// </summary>
		/// <param name="yaw">
		/// Der Winkel, um welchen die Kamera um die Yaw-Achse entgegen dem Uhrzeigersinn
		/// rotiert werden soll, in Radiant.
		/// </param>
		/// Der Winkel, um welchen die Kamera um die Pitch-Achse entgegen dem Uhrzeigersinn
		/// rotiert werden soll, in Radiant.
		/// </param>
		public void Rotate(float yaw, float pitch) {
			RotateYaw(yaw);
			RotatePitch(pitch);
		}

		/// <summary>
		/// Rotiert die Kamera um ihre Yaw und Pitch Achsen.
		/// </summary>
		/// <param name="angles">
		/// Die Winkel, um welche die Kamera um die Yaw- u. Pitch-Achsen rotiert werden
		/// soll, in Radiant.
		/// </param>
		public void Rotate(Vector2 angles) {
			Rotate(angles.X, angles.Y);
		}

		/// <summary>
		/// Rotiert die Kamera um ihre Yaw-Ache, d.h., nach links bzw. rechts.
		/// </summary>
		/// <param name="yaw">
		/// Der Winkel, um welchen die Kamera um die Yaw-Achse entgegen dem Uhrzeigersinn
		/// rotiert werden soll, in Radiant.
		/// </param>
		public void RotateYaw(float angle) {
			var oldDirection = (Eye - Ref);
			var length = oldDirection.Length();
			oldDirection.Normalize();
			// Die Yaw-Achse ist der Up-Vektor der Kamera.
			var rotMatrix = Matrix.RotationAxis(Up, angle);
			var newDirection = oldDirection.Transform(rotMatrix).Normalized();
			Eye = Ref + newDirection * length;
			updateViewMatrix = true;
		}

		/// <summary>
		/// Rotiert die Kamera um ihre Pitch-Ache, d.h., nach oben bzw. unten.
		/// </summary>
		/// <param name="pitch">
		/// Der Winkel, um welchen die Kamera um die Pitch-Achse entgegen dem Uhrzeigersinn
		/// rotiert werden soll, in Radiant.
		/// </param>
		public void RotatePitch(float angle) {
			var oldDirection = Eye - Ref;
			var length = oldDirection.Length();
			oldDirection.Normalize();
			// Die Rotationsachse steht senkrecht auf der Blickrichtung und dem Up-Vektor
			// der Kamera.
			var axis = Vector3.Cross(oldDirection, Up);
			var rotMatrix = Matrix.RotationAxis(axis, angle);
			var newDirection = oldDirection.Transform(rotMatrix).Normalized();
			Eye = Ref + newDirection * length;
			Up = Vector3.Cross(axis, newDirection).Normalized();
			updateViewMatrix = true;
		}

		/// <summary>
		/// Bewegt den Augpunkt der Kamera an die angegebene Position.
		/// </summary>
		/// <param name="position">
		/// Die Position, an welche der Augpunkt gesetzt werden soll, in Weltkoordinaten.
		/// </param>
		public void Move(Vector3 position) {
			Eye = position;
			updateViewMatrix = true;
		}
	}
}
