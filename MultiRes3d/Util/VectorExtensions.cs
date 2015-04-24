using SlimDX;

namespace MultiRes3d {
	/// <summary>
	/// Beinhaltet Extension Methods für die SlimDX.Vector3 Struktur.
	/// </summary>
	public static class VectorExtensions {
		/// <summary>
		/// Transformiert einen Vektor durch die angegebene Matrix. 
		/// </summary>
		/// <param name="v">
		/// Der Vektor, der transformiert werden soll.
		/// </param>
		/// <param name="m">
		/// Die transformierende Matrix.
		/// </param>
		/// <returns>
		/// Der transformierte Vektor.
		/// </returns>
		public static Vector3 Transform(this Vector3 v, Matrix m) {
			var vec4 = Vector3.Transform(v, m);
			return new Vector3(vec4.X, vec4.Y, vec4.Z);
		}

		/// <summary>
		/// Liefert eine Kopies des Vektors in Einheitslänge zurück.
		/// </summary>
		/// <param name="v">
		/// Der zu normierende Vektor.
		/// </param>
		/// <returns>
		/// Der normierte Vektor.
		/// </returns>
		public static Vector3 Normalized(this Vector3 v) {
			v.Normalize();
			return v;
		}
	}
}
