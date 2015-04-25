using SlimDX;
using System.Runtime.InteropServices;

namespace MultiRes3d {
	/// <summary>
	/// Repräsentiert einen Vertex eines Polygonnetzes.
	/// </summary>
	/// <remarks>
	/// Achtung, Vertex ist eine struct, d.h. ein Value Type!
	/// </remarks>
	[StructLayout(LayoutKind.Sequential)]
	public struct Vertex {
		/// <summary>
		/// Die Größe der Vertex Struktur, in Bytes.
		/// </summary>
		public static int Size = Marshal.SizeOf(typeof(Vertex));

		/// <summary>
		/// Die Position des Vertex.
		/// </summary>
		public Vector3 Position;

		/// <summary>
		/// Der Normalenvektor des Vertex.
		/// </summary>
		public Vector3 Normal;
	}
}
