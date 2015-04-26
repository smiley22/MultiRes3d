using SlimDX;
using System;
using System.Collections.Generic;
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
			var normals = new List<Vector3>();
			var verts = new List<Vertex>();
			var faces = new List<Triangle>();
			var splits = new Queue<VertexSplit>();
			using (var sr = File.OpenText(path)) {
				string l = string.Empty;
				while ((l = sr.ReadLine()) != null) {
					if (l.StartsWith("v ")) {
						verts.Add(ParseVertex(l));
						normals.Add(Vector3.Zero);
					} else if (l.StartsWith("f ")) {
						var face = ParseFace(l);
						ComputeNormals(face, verts);
						faces.Add(face);
					} else if (l.StartsWith("#vsplit ")) {
						splits.Enqueue(ParseVertexSplit(l));
					}
				}
			}
			return new Mesh(verts, faces, splits);
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
					float.Parse(p[3], CultureInfo.InvariantCulture)),
				Normal = Vector3.Zero
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
				uint.Parse(p[1]) - 1,
				uint.Parse(p[2]) - 1,
				uint.Parse(p[3]) - 1
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
				// FIXME: Fixup S index!!!!!
				S = uint.Parse(m.Groups[1].Value),
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
					uint.Parse(_m.Groups[1].Value) - 1,
					uint.Parse(_m.Groups[2].Value) - 1,
					uint.Parse(_m.Groups[3].Value) - 1
				};
				s.Faces.Add(new Triangle(indices));
			}
			return s;
		}

		/// <summary>
		/// Fügt die Normale der Facette den Normalen der Vertices anteilig hinzu.
		/// </summary>
		/// <param name="f">
		/// Die Facette.
		/// </param>
		/// <param name="v">
		/// Die Liste der Vertices.
		/// </param>
		static void ComputeNormals(Triangle f, IList<Vertex> v) {
			var d1 = v[(int) f.Indices[1]].Position - v[(int) f.Indices[0]].Position;
			var d2 = v[(int) f.Indices[2]].Position - v[(int) f.Indices[0]].Position;
			d1.Normalize();
			d2.Normalize();
			var normal = Vector3.Cross(d1, d2);
			for (int i = 0; i < 3; i++) {
				var vertex = v[(int) f.Indices[i]];
				v[(int) f.Indices[i]] = new Vertex() {
					Position = vertex.Position,
					Normal = (vertex.Normal + normal).Normalized()
				};
			}
		}
	}
}
