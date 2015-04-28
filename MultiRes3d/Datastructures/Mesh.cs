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
		/// Der Stack von VertexSplits.
		/// </summary>
		/// <remarks>
		/// Die Mesh verwaltet zwei zueinander komplementäre Stacks. Einen Stack
		/// mit VertexSplit Einträgen und einen Stack mit den Umkehroperationen
		/// (Contractions).
		/// Jedes mal wenn von dem VertexSplit Stack ein VertexSplit entnommen
		/// und auf der Mesh durchgeführt wird, wird ein neuer Eintrag auf dem
		/// Contractions Stack angelegt, der die Umkehroperation zu dem VertexSplit
		/// darstellt. Analog wird verfahren, wenn eine Contraction durchgeführt
		/// wird. Auf diese Weise kann der Detailgrad der Mesh in beide Richtungen
		/// variiert werden.
		/// </remarks>
		public Stack<VertexSplit> Splits {
			get;
			private set;
		}

		/// <summary>
		/// Der Stack von Contractions.
		/// </summary>
		public Stack<Contraction> Contractions {
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
		/// Eine Liste von Vertex-Splits, die der Mesh hinzugefügt werden sollen.
		/// </param>
		public Mesh(IList<Vertex> vertices, IList<Triangle> faces, IList<VertexSplit> splits) {
			Vertices = new Vertex[vertices.Count + splits.Count];
			NumberOfVertices = vertices.Count;
			for (int i = 0; i < vertices.Count; i++)
				Vertices[i] = vertices[i];
#if false
			// FIXME: Geht davon aus, daß pro VertexSplit höchstens 2 neue Facetten
			//        entstehen. Dies trifft auf "normale" Meshes auch zu, aber
			//        bei manchen nicht-wohlgeformten Meshes geht diese Rechnung
 			//        nicht auf.
			int maxNewFacesPerSplit = 2;
#else
			// Provisorische Notlösung: Zur Sicherheit mehr Speicher allokieren als
			// notwendig sein sollte.
			int maxNewFacesPerSplit = 3;
#endif
			FlatFaces = new uint[(faces.Count + maxNewFacesPerSplit * splits.Count) * 3];
			NumberOfFaces = faces.Count;
			Splits = new Stack<VertexSplit>();
			Contractions = new Stack<Contraction>();
			for (int i = 0; i < faces.Count; i++)
				_faces.Add(new Face(faces[i].Indices, FlatFaces, i * 3));
			for (int i = splits.Count; i > 0; i--)
				Splits.Push(splits[i - 1]);
		}

		/// <summary>
		/// Entfernt den obersten VertexSplit von dem VertexSplit Stack und führt ihn
		/// durch.
		/// </summary>
		/// <returns>
		/// true, wenn ein VertexSplit aus dem Stack entnommen wurde und durchgeführt
		/// wurde; andernfalls false, d.h. der Stack war schon leer.
		/// </returns>
		/// <remarks>
		/// Das Durchführen einer VertexSplit Operation hat immer das Pushen seiner
		/// Umkehroperation auf den Contraction Stack zur Folge.
		/// </remarks>
		public bool PerformVertexSplit() {
			if (Splits.Count == 0)
				return false;
			if (incidentFaces == null) {
				incidentFaces = ComputeIncidentFaces();
				// Einmalig alle Vertexnormalen bestimmen.
//				ComputeNormals(_faces);
			}
			var split = Splits.Pop();
			// 1. Vertex s wird an neue Position verschoben.
			Vertices[split.S] = new Vertex() { Position = split.SPosition };
			// 2. Vertex t wird neu zur Mesh hinzugefügt.
			Vertices[NumberOfVertices] = new Vertex() { Position = split.TPosition };
			uint t = (uint) NumberOfVertices;
			// 3. Umkehroperation des VertexSplits auf Contraction Stack pushen.
			Contractions.Push(new Contraction() { S = split.S,
				Position = Vertices[split.S].Position, faceOffset = NumberOfFaces,
				VertexSplit = split
			});
			NumberOfVertices++;
			// 4. Alle Facetten von s, die ursprünglich t "gehört" haben, auf t zurückbiegen.
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
			// 5. Etwaige gemeinsame Facetten von s und t der Mesh neu hinzufügen.
			foreach (var f in split.Faces) {
				if (!f.Indices.Contains(split.S))
					continue;
				var newFace = new Face(f.Indices, FlatFaces, NumberOfFaces * 3);
				NumberOfFaces++;
				for (int c = 0; c < 3; c++)
					incidentFaces[newFace[c]].Add(newFace);
			}
			// Normalen aller betroffenen Vertices neuberechnen.
			ComputeNormals(incidentFaces[split.S].Union(incidentFaces[t]));
			return true;
		}

		/// <summary>
		/// Entfernt die oberste Contraction von dem Contraction Stack und führt sie
		/// durch.
		/// </summary>
		/// <returns>
		/// true, wenn eine Contraction aus dem Stack entnommen wurde und durchgeführt
		/// wurde; andernfalls false, d.h. der Stack war schon leer.
		/// </returns>
		/// <remarks>
		/// Das Durchführen einer Contraction Operation hat immer das Pushen ihrer
		/// Umkehroperation auf den VertexSplit Stack zur Folge.
		/// </remarks>
		public bool PerformContraction() {
			if (Contractions.Count == 0)
				return false;
			var contraction = Contractions.Pop();
			// 1. Vertex s wird an neue Position verschoben.
			Vertices[contraction.S] = new Vertex() { Position = contraction.Position };
			NumberOfFaces = contraction.faceOffset;
			NumberOfVertices--;
			uint t = (uint) NumberOfVertices;
			// 2. Alle Facetten von t auf s umbiegen.
			foreach (var f in incidentFaces[t]) {
				bool remove = false;
				for (int i = 0; i < 3; i++) {
					if (f[i] == contraction.S)
						remove = true;
					else if (f[i] == t)
						f[i] = contraction.S;
				}
				if (remove) {
					for (int c = 0; c < 3; c++)
						incidentFaces[f[c]].Remove(f);
				} else {
					incidentFaces[contraction.S].Add(f);
				}
			}
			incidentFaces.Remove(t);
			// 3. Umkehroperation der Contraction auf den VertexSplit Stack pushen.
			Splits.Push(contraction.VertexSplit);
			// Normalen aller betroffenen Vertices neuberechnen.
			ComputeNormals(incidentFaces[contraction.S]);
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
		/// Berechnet die Normalen der angegebenen Facetten und all ihrer inzidenten
		/// Vertices.
		/// </summary>
		/// <param name="faces">
		/// Die Facetten.
		/// </param>
		void ComputeNormals(IEnumerable<Face> faces) {
			var recompute = new HashSet<uint>();
			// Facettennormalen berechnen und dabei Vertex Indices einsammeln.
			foreach (var f in faces) {
				ComputeFaceNormal(f);
				for (int i = 0; i < 3; i++)
					recompute.Add(f[i]);
			}
			// Für jeden eingesammelten Vertex Normale berechnen.
			foreach (var index in recompute)
				ComputeVertexNormal(index);
		}

		/// <summary>
		/// Berechnet den Normalenvektor des angegebenen Vertex.
		/// </summary>
		/// <param name="v">
		/// Der Vertex.
		/// </param>
		void ComputeVertexNormal(uint v) {
			var normal = Vector3.Zero;
			foreach (var face in incidentFaces[v])
				normal = normal + face.Normal;
			normal.Normalize();
			var oldPos = Vertices[v].Position;
			Vertices[v] = new Vertex() {
				Position = oldPos,
				Normal = normal
			};
		}

		/// <summary>
		/// Berechnet den Normalenvektor der angegebenen Facette.
		/// </summary>
		/// <param name="f">
		/// Die Facette.
		/// </param>
		void ComputeFaceNormal(Face f) {
			var d1 = Vertices[f[1]].Position - Vertices[f[0]].Position;
			var d2 = Vertices[f[2]].Position - Vertices[f[0]].Position;
			f.Normal = Vector3.Cross(d1, d2);
			f.Normal.Normalize();
		}
	}
}
