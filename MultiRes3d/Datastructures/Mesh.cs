using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiRes3d {
	/// <summary>
	/// Repräsentiert ein (progressives) Polygonnetz.
	/// </summary>
	public class Mesh {
		/// <summary>
		/// Die Vertices des Polgyonnetzes.
		/// </summary>
		public IList<Vertex> Vertices {
			get;
			private set;
		}

		/// <summary>
		/// Die Facetten des Polygonnetzes.
		/// </summary>
		public IList<Triangle> Faces {
			get;
			private set;
		}

		/// <summary>
		/// Die mit der Mesh assoziierten Vertex-Splits.
		/// </summary>
		public Queue<VertexSplit> Splits {
			get;
			private set;
		}

		/// <summary>
		/// Initialisiert eine neue Instanz der Mesh Klasse.
		/// </summary>
		/// <param name="vertices">
		/// Eine Abzählung von Vertices, die der Mesh hinzugefügt werden sollen.
		/// </param>
		/// <param name="faces">
		/// Eine Abzählung von Facetten, die der Mesh hinzugefügt werden sollen.
		/// </param>
		/// <param name="splits">
		/// Eine Abzählung von Vertex-Splits, die der Mesh hinzugefügt werden sollen.
		/// </param>
		public Mesh(IEnumerable<Vertex> vertices = null, IEnumerable<Triangle> faces = null,
			IEnumerable<VertexSplit> splits = null) {
			Vertices = new List<Vertex>();
			Faces = new List<Triangle>();
			Splits = new Queue<VertexSplit>();
			if (vertices != null) {
				foreach (var v in vertices)
					Vertices.Add(v);
			}
			if (faces != null) {
				foreach (var f in faces)
					Faces.Add(f);
			}
			if (splits != null) {
				foreach (var s in splits)
					Splits.Enqueue(s);
			}
		}
	}
}
