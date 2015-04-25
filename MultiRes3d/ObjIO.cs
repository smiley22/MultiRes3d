using SlimDX;
using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace MultiRes3d {
	/// <summary>
	/// Eine Klasse zum Laden von Wavefront .obj Dateien.
	/// </summary>
	/// <remarks>
	/// Es werden nur Dreiecksnetze unterstützt.
	/// </remarks>
	public static class ObjIO {
		/// <summary>
		/// Parsed die angegebene .obj Datei.
		/// </summary>
		/// <param name="path">
		/// Der Name der Datei, welche geparsed werden soll.
		/// </param>
		/// <returns>
		/// Eine Mesh Instanz, die aus den Daten der .obj Datei erzeugt wurde.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// Die angegebene .obj Datei ist ungültig oder enthält ein nicht-unterstütztes
		/// Meshformat.
		/// </exception>
		/// <exception cref="IOException">
		/// Die angegebene Datei konnte nicht gelesen werden.
		/// </exception>
		public static Mesh Load(string path) {
			var mesh = new Mesh();
			using (var sr = File.OpenText(path)) {
				string l = string.Empty;
				while ((l = sr.ReadLine()) != null) {
					if (l.StartsWith("v "))
						mesh.Vertices.Add(ParseVertex(l));
					else if (l.StartsWith("f "))
						mesh.Faces.Add(ParseFace(l));
					else if (l.StartsWith("#vsplit "))
						mesh.Splits.Enqueue(ParseVertexSplit(l));
				}
			}
			FixUpNormals(mesh);
			return mesh;
		}

		static void FixUpNormals(Mesh m) {
			foreach (var f in m.Faces) {
				var d1 = m.Vertices[f.Indices[1]].Position - m.Vertices[f.Indices[0]].Position;
				var d2 = m.Vertices[f.Indices[2]].Position - m.Vertices[f.Indices[0]].Position;

				d1.Normalize();
				d2.Normalize();
				var normal = Vector3.Cross(d1, d2);
				f.Normal = normal;
				for(int i = 0; i < 3; i++) {
					var v = m.Vertices[f.Indices[i]];

					m.Vertices[f.Indices[i]] = new Vertex() {
						Position = v.Position,
						Normal = normal
					};
				}

			}
		}

		/// <summary>
		/// Parsed eine Vertex Deklaration.
		/// </summary>
		/// <param name="l">
		/// Die Zeile, die die Vertex Deklaration enthält.
		/// </param>
		/// <returns>
		/// Der Vertex.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// Die Vertexdelaration ist ungültig.
		/// </exception>
		static Vertex ParseVertex(string l) {
			var p = l.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (p.Length != 4)
				throw new InvalidOperationException("Invalid vertex format: " + l);
			return new Vertex() {
				Position = new Vector3(
					float.Parse(p[1], CultureInfo.InvariantCulture),
					float.Parse(p[2], CultureInfo.InvariantCulture),
					float.Parse(p[3], CultureInfo.InvariantCulture))
			};
		}

		/// <summary>
		/// Parsed eine Facetten Deklaration.
		/// </summary>
		/// <param name="l">
		/// Die Zeile, die die Facetten Deklaration enthält.
		/// </param>
		/// <returns>
		/// Die Facette.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// Die Facettendeklaration ist ungültig.
		/// </exception>
		static Triangle ParseFace(string l) {
			var p = l.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (p.Length != 4)
				throw new InvalidOperationException("Invalid face: " + l);
			var indices = new[] {
				int.Parse(p[1]) - 1,
				int.Parse(p[2]) - 1,
				int.Parse(p[3]) - 1
			};
			return new Triangle(indices);
		}

		/// <summary>
		/// Parsed einen dreidimensionalen Vektor.
		/// </summary>
		/// <param name="l">
		/// Die Zeile, die den Vektor enthält.
		/// </param>
		/// <returns>
		/// Der Vektor.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// Die Vektordeklaration ist ungültig.
		/// </exception>
		static Vector3 ParseVector(string l) {
			var p = l.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (p.Length != 3)
				throw new InvalidOperationException("Invalid vector: " + l);
			return new Vector3(
				float.Parse(p[0], CultureInfo.InvariantCulture),
				float.Parse(p[1], CultureInfo.InvariantCulture),
				float.Parse(p[2], CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Parsed eine VertexSplit Deklaration.
		/// </summary>
		/// <param name="l">
		/// Die Zeile, die die VertexSplit Deklaration enthält.
		/// </param>
		/// <returns>
		/// Der Vertex-Split Eintrag.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// Die VertexSplit Deklaration ist ungültig.
		/// </exception>
		static VertexSplit ParseVertexSplit(string l) {
			var m = Regex.Match(l, @"^#vsplit\s+(\d+)\s+{(.*)}\s+{(.*)}\s+{(.*)}$");
			if (!m.Success)
				throw new InvalidOperationException("Invalid vsplit: " + l);
			var s = new VertexSplit() {
				S = int.Parse(m.Groups[1].Value),
				SPosition = ParseVector(m.Groups[2].Value),
				TPosition = ParseVector(m.Groups[3].Value)
			};
			var faceIndices = m.Groups[4].Value;
			var matches = Regex.Matches(faceIndices, @"\((-?\d+)\s+(-?\d+)\s+(-?\d+)\)");
			for (int i = 0; i < matches.Count; i++) {
				var _m = matches[i];
				if (!_m.Success)
					throw new InvalidOperationException("Invalid face index entry in vsplit: " + l);
				var indices = new[] {
					int.Parse(_m.Groups[1].Value) - 1,
					int.Parse(_m.Groups[2].Value) - 1,
					int.Parse(_m.Groups[3].Value) - 1
				};
				s.Faces.Add(new Triangle(indices));
			}
			return s;
		}
	}
}
