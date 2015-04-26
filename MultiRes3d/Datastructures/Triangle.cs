
using SlimDX;
namespace MultiRes3d {
	/// <summary>
	/// Repräsentiert eine Facette.
	/// </summary>
	/// <remarks>
	/// Indices sind uint, da D3D Indexbuffer nur mit unsigned Typen arbeiten.
	/// </remarks>
	public class Triangle {
		/// <summary>
		/// Die Indices der Vertices des Triangles.
		/// </summary>
		public readonly uint[] Indices = new uint[3];

		/// <summary>
		/// Der Normalenvektor der Facette.
		/// </summary>
		public Vector3 Normal;

		/// <summary>
		/// Initialisiert eine neue Instanz der Triangle Klasse.
		/// </summary>
		/// <param name="index1">
		/// Der erste Index.
		/// </param>
		/// <param name="index2">
		/// Der zweite Index.
		/// </param>
		/// <param name="index3">
		/// Der dritte Index.
		/// </param>
		public Triangle(uint index1, uint index2, uint index3) {
			Indices[0] = index1;
			Indices[1] = index2;
			Indices[2] = index3;
		}

		/// <summary>
		/// Initialisiert eine neue Instanz der Triangle Klasse.
		/// </summary>
		/// <param name="indices">
		/// Die Indices der Vertices, die das Triangle ausmachen.
		/// </param>
		public Triangle(uint[] indices)
			: this(indices[0], indices[1], indices[2]) {
		}
	}
}
