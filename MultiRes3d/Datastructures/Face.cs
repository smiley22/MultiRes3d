using SlimDX;

namespace MultiRes3d {
	/// <summary>
	/// Repräsentiert eine Facette der PM.
	/// </summary>
	/// <remarks>
	/// Die Klasse ist speziell angepasst, um ein möglichst effizientes Kopieren
	/// der Facetten Indices in den Grafikspeicher, der ein flaches Index-Array
	/// als Eingabe erfordert, zu ermöglichen. 
	/// </remarks>
	public class Face {
		uint[] storage;
		int offset;

		/// <summary>
		/// Der Normalenvektor der Facette.
		/// </summary>
		public Vector3 Normal;

		/// <summary>
		/// Überladener Index-Operator zum Zugriff auf die Indices der Facette.
		/// </summary>
		/// <param name="index">
		/// Der Index auf den zugegriffen wird.
		/// </param>
		/// <returns>
		/// Der jeweilige Index der Facette.
		/// </returns>
		public uint this[int index] {
			get {
				return storage[offset + index];
			}
			set {
				storage[offset + index] = value;
			}
		}

		/// <summary>
		/// Initialisiert eine neue Instanz der Face Klasse.
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
		/// <param name="storage">
		/// Der zugrundeliegene flache Speicher, in dem die Indices gespeichert
		/// werden.
		/// </param>
		/// <param name="offset">
		/// Der Offset in den Speicher an dem die Indices gespeichert werden.
		/// </param>
		public Face(uint index1, uint index2, uint index3, uint[] storage,
			int offset) {
			this.storage = storage;
			this.offset = offset;
			storage[offset + 0] = index1;
			storage[offset + 1] = index2;
			storage[offset + 2] = index3;
		}

		/// <summary>
		/// Initialisiert eine neue Instanz der Face Klasse.
		/// </summary>
		/// <param name="indices">
		/// Die Indices der Vertices, die das Triangle ausmachen.
		/// </param>
		/// <param name="storage">
		/// Der zugrundeliegene flache Speicher, in dem die Indices gespeichert
		/// werden.
		/// </param>
		/// <param name="offset">
		/// Der Offset in den Speicher an dem die Indices gespeichert werden.
		/// </param>
		public Face(uint[] indices, uint[] storage, int offset)
			: this(indices[0], indices[1], indices[2], storage, offset) {
		}
	}
}
