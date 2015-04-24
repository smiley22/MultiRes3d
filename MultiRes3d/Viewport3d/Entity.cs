using SlimDX;
using SlimDX.Direct3D11;

namespace MultiRes3d {
	/// <summary>
	/// Repräsentiert ein renderbares Objekt.
	/// </summary>
	public abstract class Entity {
		Quaternion orientation;
		float scale;
		Vector3 position;
		Matrix transform;

		/// <summary>
		/// Liefert oder setzt die Orientierung des Objekts.
		/// </summary>
		public Quaternion Orientation {
			get {
				return orientation;
			}
			set {
				orientation = value;
				UpdateTransformationMatrix();
			}
		}

		/// <summary>
		/// Liefer oder setzt den Skalierungsfaktor des Objekts.
		/// </summary>
		public float Scale {
			get {
				return scale;
			}
			set {
				scale = value;
				UpdateTransformationMatrix();
			}
		}

		/// <summary>
		/// Liefer oder Setzt die Position des Objekts.
		/// </summary>
		public Vector3 Position {
			get {
				return position;
			}
			set {
				position = value;
				UpdateTransformationMatrix();
			}
		}

		/// <summary>
		/// Liefert die aus Positon, Skalierung und Orientierung berechnete Transformationsmatrix
		/// des Objekts.
		/// </summary>
		public Matrix Transform {
			get {
				return transform;
			}
		}

		/// <summary>
		/// Rendert das Objekt.
		/// </summary>
		/// <param name="context">
		/// Der <c>DeviceContext</c> der zugehörigen D3D11 Device.
		/// </param>
		/// <param name="effect">
		/// Der benutzte Effekt zum Setzen benötigter Shader-Variablen.
		/// </param>
		public abstract void Render(DeviceContext context, BasicEffect effect);

		/// <summary>
		/// Initialisiert eine neue Instanz der Entity-Klasse.
		/// </summary>
		protected Entity() {
			Scale = 1.0f;
			Orientation = Quaternion.Identity;
		}

		/// <summary>
		/// Aktualisiert die Transformationsmatrix des Objekts.
		/// </summary>
		void UpdateTransformationMatrix() {
			var rotCenter = Vector3.Zero;
			Matrix.AffineTransformation(scale, ref rotCenter, ref orientation, ref position,
				out transform);
		}
	}
}