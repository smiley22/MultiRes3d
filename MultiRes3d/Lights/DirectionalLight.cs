using SlimDX;
using System;
using System.Runtime.InteropServices;

namespace MultiRes3d {
	/// <summary>
	/// Repräsentiert ein direktionales Licht.
	/// </summary>
	public class DirectionalLight : IDisposable {
		readonly byte[] buffer = new byte[_marshalStruct.Size];
		IntPtr unmanagedMemory = Marshal.AllocHGlobal(_marshalStruct.Size);
		_marshalStruct str;
		bool disposed;

		/// <summary>
		/// The Größe einer <c>DirectionalLight</c> Instanz wenn sie in eine Sequenz von
		/// Bytes serialisiert wird.
		/// </summary>
		public static readonly int Size = _marshalStruct.Size;

		/// <summary>
		/// Die Ambient Komponente des Lichts.
		/// </summary>
		public Color4 Ambient;

		/// <summary>
		/// Die Diffuse Komponente des Lichts.
		/// </summary>
		public Color4 Diffuse;

		/// <summary>
		/// Die Specular Komponente des Lichts.
		/// </summary>
		public Color4 Specular;

		/// <summary>
		/// Die Richtung der Lichtstrahlen.
		/// </summary>
		public Vector3 Direction;

		/// <summary>
		/// Wird benötigt, um die Instanz in einen Bytestrom zu serialisieren.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		struct _marshalStruct {
			public static int Size = Marshal.SizeOf(typeof(_marshalStruct));
			public Color4 Ambient;
			public Color4 Diffuse;
			public Color4 Specular;
			public Vector3 Direction;
			// Struktur auf 64-Bytes padden, weil sie exakt der Größe der
			// entsprechenden Struktur im Shader entsprechen muss!
			float Pad;
		}

		/// <summary>
		/// Serialisiert diese Instanz in eine Bytesequenz.
		/// </summary>
		/// <returns>
		/// Eine Bytesequenz die diese Instanz repräsentiert.
		/// </returns>
		public byte[] ToBytes() {
			str.Ambient = Ambient;
			str.Diffuse = Diffuse;
			str.Specular = Specular;
			str.Direction = Direction;
			Marshal.StructureToPtr(str, unmanagedMemory, true);
			Marshal.Copy(unmanagedMemory, buffer, 0, _marshalStruct.Size);
			return buffer;
		}

		/// <summary>
		/// Gibt alle Resourcen dieser Instanz frei.
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Gibt alle Resourcen dieser Instanz frei, optionalerweise auch managed Resourcen.
		/// </summary>
		/// <param name="disposing">
		/// true, um managed Resourcen freizugeben; andernfalls false.
		/// </param>
		protected virtual void Dispose(bool disposing) {
			if (!disposed) {
				// Merken, daß die Instanz freigegeben wurde.
				disposed = true;
				// Managed Resourcen freigeben.
				if (disposing) {
					Marshal.FreeHGlobal(unmanagedMemory);
				}
				// Hier unmanaged Resourcen freigeben.
			}
		}
	}
}
