using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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

		IDictionary<int, ISet<Triangle>> _incidentFaces;

		public void PerformVertexSplit() {
			if (Splits.Count == 0)
				return;
			if (_incidentFaces == null)
				_incidentFaces = ComputeIncidentFaces();
			var split = Splits.Dequeue();
			PerformVertexSplit(split, _incidentFaces);
		}

		/// <summary>
		/// Berechnet für jeden Vertex die Menge seiner inzidenten Facetten.
		/// </summary>
		/// <returns>
		/// Eine Map die jedem Vertex der Mesh die Menge seiner inzidenten Facetten zuordnet.
		/// </returns>
		IDictionary<int, ISet<Triangle>> ComputeIncidentFaces() {
			var dict = new Dictionary<int, ISet<Triangle>>();
			for (int i = 0; i < Vertices.Count; i++)
				dict.Add(i, new HashSet<Triangle>());
			foreach (var f in Faces) {
				for (int c = 0; c < f.Indices.Length; c++) {
					dict[f.Indices[c]].Add(f);
				}
			}
			return dict;
		}

		/// <summary>
		/// Führt eine Vertex-Split Operation auf der Mesh aus.
		/// </summary>
		/// <param name="split">
		/// Die Vertex-Split Operation, die ausgeführt werden soll.
		/// </param>
		/// <param name="incidentFaces">
		/// Eine Map die jedem Vertex der Mesh die Menge seiner inzidenten Facetten zuordnet.
		/// </param>
		void PerformVertexSplit(VertexSplit split, IDictionary<int, ISet<Triangle>> incidentFaces) {
			// 1. Vertex s wird an neue Position verschoben.
			var oldNormal = Vertices[split.S].Normal;
			Vertices[split.S] = new Vertex() {
				Position = split.SPosition,
				Normal = oldNormal
			};
			// 2. Vertex t wird neu zur Mesh hinzugefügt.
			Vertices.Add(new Vertex() { Position = split.TPosition });
			var t = Vertices.Count - 1;
			// 3. Alle Facetten von s, die ursprünglich t "gehört" haben, auf t zurückbiegen.
			var facesOfS = incidentFaces[split.S];
			var facesOfT = new HashSet<Triangle>();
			incidentFaces.Add(t, facesOfT);
			var removeFromS = new HashSet<Triangle>();
			foreach (var f in facesOfS) {
				var _c = IsOriginalFaceOfT(t, f, split);
				if (_c < 0)
					continue;
				f.Indices[_c] = t;
				facesOfT.Add(f);
				removeFromS.Add(f);
			}
			foreach (var r in removeFromS)
				facesOfS.Remove(r);
			// 4. Etwaige gemeinsame Facetten von s und t der Mesh neu hinzufügen.
			foreach (var f in split.Faces) {
				if (!f.Indices.Contains(split.S))
					continue;
				var newFace = new Triangle(f.Indices);
				Faces.Add(newFace);
				for (int c = 0; c < newFace.Indices.Length; c++)
					incidentFaces[newFace.Indices[c]].Add(newFace);
			}
		}

		/// <summary>
		/// Prüft, ob die angegebene Facette ursprünglich dem angegebenen Vertex "gehörte".
		/// </summary>
		/// <param name="t">
		/// Der Vertex.
		/// </param>
		/// <param name="face">
		/// Die Facette.
		/// </param>
		/// <param name="split">
		/// Die zugehörige VertexSplit Instanz.
		/// </param>
		/// <returns>
		/// Der Facettenindex, an dessen Stelle der ursprüngliche Vertex eingetragen war,
		/// oder -1 falls die Facette nicht ursprünglich dem Vertex "gehörte".
		/// </returns>
		/// <remarks>
		/// Diese Methode könnte schöner sein und sollte eigentlich in 2 Methoden aufgeteilt
		/// werden: Eine Methode zum Prüfen mit Rückgabewert Bool und eine Methode zum
		/// Abfragen des Index. Aus Geschwindigkeitsgründen ist dies aber ungünstig.
		/// </remarks>
		int IsOriginalFaceOfT(int t, Triangle face, VertexSplit split) {
			foreach (var f in split.Faces) {
				var index = -1;
				var isface = true;
				for (int i = 0; i < 3; i++) {
					if (f.Indices[i] == t && face.Indices[i] == split.S) {
						index = i;
					} else if (f.Indices[i] != face.Indices[i]) {
						isface = false;
					}
				}
				if (isface)
					return index;
			}
			return -1;
		}

	}
}
