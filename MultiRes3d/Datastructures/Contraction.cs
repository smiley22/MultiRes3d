using SlimDX;

namespace MultiRes3d {
	/// <summary>
	/// Repräsentiert eine Pair-Contract Operation, also das "Verschmelzen" zweier
	/// Vertices zu einem.
	/// </summary>
	public class Contraction {
		/// <summary>
		/// Der Index des ersten Vertex.
		/// </summary>
		public uint S;

		/// <summary>
		/// Die Position an die der erste Vertex verschoben wird.
		/// </summary>
		public Vector3 Position;

		/// <summary>
		/// Der Offset in das Array der Facetten-Indices, an das der Zeiger verschoben
		/// werden soll.
		/// </summary>
		public int faceOffset;

		/// <summary>
		/// Der zugehörige VertexSplit, der ausgeführt werden muss, um die Contraction
		/// rückgängig zu machen.
		/// </summary>
		public VertexSplit VertexSplit;
	}
}
