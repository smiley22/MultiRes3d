using SlimDX;
using System.Collections.Generic;
using System.Linq;

namespace MultiRes3d {
	/// <summary>
	/// Repräsentiert ein (progressives) Polygonnetz.
	/// </summary>
	/// <remarks>
	/// Basiert auf der Mesh-Klasse aus MeshSimplify, aber an D3D bzw. Grafikhardware
	/// angepaßt, um bessere Performanz zu erzielen.
	/// </remarks>
	public class Mesh {
		IDictionary<uint, ISet<Face>> incidentFaces;
		IList<Face> _faces = new List<Face>();

		/// <summary>
		/// Die Vertices des Polgyonnetzes.
		/// </summary>
		/// <remarks>
		/// Vertices werden in einem fixen Buffer verwaltet mit einem Zeiger der
		/// die Anzahl der im Buffer befindlichen Vertices angibt. So muss nicht
		/// nach jedem VertexSplit eine neue Array Instanz aus einer Liste erzeugt
		/// werden.
		/// </remarks>
		public Vertex[] Vertices {
			get;
			private set;
		}

		/// <summary>
		/// Die Anzahl der sich im <c>Vertices</c> Array befindlichen Vertices.
		/// </summary>
		public int NumberOfVertices {
			get;
			private set;
		}

		/// <summary>
		/// Die Faces des Polygonnetzes als sequentielle Liste von Indices.
		/// </summary>
		/// <remarks>
		/// Je 3 Indices repräsentieren eine Facette. Die Facetten werden als primitives
		/// uint-Array gespeichert, da sie so direkt in den Videospeicher der Grafikkarte
		/// kopiert werden können.
		/// </remarks>
		public uint[] FlatFaces {
			get;
			private set;
		}

		/// <summary>
		/// Die Anzahl der sich im <c>FlatFaces</c> Array befindlichen Facetten.
		/// </summary>
		public int NumberOfFaces {
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
		/// Eine Liste von Vertices, die der Mesh hinzugefügt werden sollen.
		/// </param>
		/// <param name="faces">
		/// Eine Liste von Facetten, die der Mesh hinzugefügt werden sollen.
		/// </param>
		/// <param name="splits">
		/// Eine Queue von Vertex-Splits, die der Mesh hinzugefügt werden sollen.
		/// </param>
		public Mesh(IList<Vertex> vertices, IList<Triangle> faces, Queue<VertexSplit> splits) {
			Vertices = new Vertex[vertices.Count + splits.Count];
			NumberOfVertices = vertices.Count;
			for (int i = 0; i < vertices.Count; i++)
				Vertices[i] = vertices[i];
			FlatFaces = new uint[(faces.Count + 2 * splits.Count) * 3];
			NumberOfFaces = faces.Count;
			Splits = new Queue<VertexSplit>();

			for (int i = 0; i < faces.Count; i++)
				_faces.Add(new Face(faces[i].Indices, FlatFaces, i * 3));
			foreach (var s in splits)
				Splits.Enqueue(s);
		}


		public bool PerformVertexSplit() {
			if (Splits.Count == 0)
				return false;
			if (incidentFaces == null)
				incidentFaces = ComputeIncidentFaces();
			var split = Splits.Dequeue();
			PerformVertexSplit(split);
			return true;
		}

		/// <summary>
		/// Berechnet für jeden Vertex die Menge seiner inzidenten Facetten.
		/// </summary>
		/// <returns>
		/// Eine Map die jedem Vertex der Mesh die Menge seiner inzidenten Facetten zuordnet.
		/// </returns>
		IDictionary<uint, ISet<Face>> ComputeIncidentFaces() {
			var dict = new Dictionary<uint, ISet<Face>>();
			for (uint i = 0; i < NumberOfVertices; i++)
				dict.Add(i, new HashSet<Face>());
			foreach (var f in _faces) {
				for (int c = 0; c < 3; c++) {
					dict[f[c]].Add(f);
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
		void PerformVertexSplit(VertexSplit split) {
			// 1. Vertex s wird an neue Position verschoben.
			Vertices[split.S] = new Vertex() { Position = split.SPosition };
			// 2. Vertex t wird neu zur Mesh hinzugefügt.
			Vertices[NumberOfVertices] = new Vertex() { Position = split.TPosition };
			uint t = (uint)NumberOfVertices;
			NumberOfVertices++;
			// 3. Alle Facetten von s, die ursprünglich t "gehört" haben, auf t zurückbiegen.
			var facesOfS = incidentFaces[split.S];
			var facesOfT = new HashSet<Face>();
			incidentFaces.Add(t, facesOfT);
			var removeFromS = new HashSet<Face>();
			foreach (var f in facesOfS) {
				var _c = IsOriginalFaceOfT(t, f, split);
				if (_c < 0)
					continue;
				f[_c] = t;
				facesOfT.Add(f);
				removeFromS.Add(f);
			}
			foreach (var r in removeFromS)
				facesOfS.Remove(r);
			// 4. Etwaige gemeinsame Facetten von s und t der Mesh neu hinzufügen.
			foreach (var f in split.Faces) {
				if (!f.Indices.Contains(split.S))
					continue;
				var newFace = new Face(f.Indices, FlatFaces, NumberOfFaces * 3);
				NumberOfFaces++;
				for (int c = 0; c < 3; c++)
					incidentFaces[newFace[c]].Add(newFace);
			}
			// Normalen von s und t neuberechnen. Eigentlich müssten auch die Normalen
			// der restlichen Vertices der inzidenten Facetten neuberechnet werden, aber
			// das hier reicht schon aus, damit es ganz gut aussieht.
			var oldPos = Vertices[split.S].Position;
			Vertices[split.S] = new Vertex() {
				Position = oldPos,
				Normal = ComputeVertexNormal(split.S)
			};
			oldPos = Vertices[t].Position;
			Vertices[t] = new Vertex() {
				Position = oldPos,
				Normal = ComputeVertexNormal(t)
			};
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
		int IsOriginalFaceOfT(uint t, Face face, VertexSplit split) {
			foreach (var f in split.Faces) {
				var index = -1;
				var isface = true;
				for (int i = 0; i < 3; i++) {
					if (f.Indices[i] == t && face[i] == split.S) {
						index = i;
					} else if (f.Indices[i] != face[i]) {
						isface = false;
					}
				}
				if (isface)
					return index;
			}
			return -1;
		}

		/// <summary>
		/// Berechnet den Normalenvektor des angegebenen Vertex.
		/// </summary>
		/// <param name="v">
		/// Der Vertex.
		/// </param>
		/// <returns>
		/// Der Normalenvektor des Vertex.
		/// </returns>
		Vector3 ComputeVertexNormal(uint v) {
			var normal = Vector3.Zero;
			foreach (var face in incidentFaces[v])
				normal = normal + ComputeFaceNormal(face);
			return normal.Normalized();
		}

		/// <summary>
		/// Berechnet den Normalenvektor der angegebenen Facette.
		/// </summary>
		/// <param name="f">
		/// Die Facette.
		/// </param>
		/// <returns>
		/// Der Normalenvektor der Facette.
		/// </returns>
		Vector3 ComputeFaceNormal(Face f) {
			var d1 = Vertices[f[1]].Position - Vertices[f[0]].Position;
			var d2 = Vertices[f[2]].Position - Vertices[f[0]].Position;
			return Vector3.Cross(d1, d2).Normalized();
		}
	}
}
