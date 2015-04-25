using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MultiRes3d {
	/// <summary>
	/// Implementiert eine bequeme Schnittstelle zum Setzen der Variablen des Bassic.fx Effekts.
	/// </summary>
	public class BasicEffect : IDisposable {
		bool disposed;
		readonly Effect effect;
		EffectMatrixVariable world;
		EffectMatrixVariable worldViewProjection;
		EffectResourceVariable diffuseMap, specularMap;
		EffectVariable dirLight;
		// FIXME: Immoment wird nur ein einzelnes Point-Light unterstützt. Das ist für unsere
		//        Zwecke erstmal ausreichend, aber ein flexibles System wäre hier schön.
		EffectVariable pointLight;
		EffectVectorVariable eyePosition;
		EffectVectorVariable color;

		/// <summary>
		/// Liefert eine <c>EffectTechnique</c> zur Benutzung einer Diffuse-Map (also
		/// einer Texture oder einem 'Skin') ohne Lichtberechnung.
		/// </summary>
		public EffectTechnique TexTech {
			get;
			private set;
		}

		/// <summary>
		/// Liefert eine <c>EffectTechnique</c> zur Benutzung einer Diffuse-Map (also
		/// einer Texture oder einem 'Skin') mit Lichtberechnung.
		/// </summary>
		public EffectTechnique TexLitTech {
			get;
			private set;
		}

		/// <summary>
		/// Liefert eine <c>EffectTechnique</c> zur Benutzung einer Diffuse-Map (also
		/// einer Texture oder einem 'Skin') und einer Specular-Map mit Lichtberechnung.
		/// </summary>
		public EffectTechnique TexSpecLitTech {
			get;
			private set;
		}

		/// <summary>
		/// Liefert eine <c>EffectTechnique</c> um Farbinformationen auf einer per-Vertex
		/// Basis zu benutzen.
		/// </summary>
		public EffectTechnique ColorTech {
			get;
			private set;
		}

		/// <summary>
		/// Liefert eine <c>EffectTechnique</c> um Farbinformationen auf einer per-Vertex
		/// Basis zu benutzen mit Lichtberechnung.
		/// </summary>
		public EffectTechnique ColorLitTech {
			get;
			private set;
		}

		/// <summary>
		/// Initialisiert eine neue Instanz der BasicEffect-Klasse für die angegebene
		/// Direct3d Device.
		/// </summary>
		/// <param name="device">
		/// Die <c>Device</c> Instanz für die die BasicEffect instance erstellt werden soll.
		/// </param>
		/// <param name="path">
		/// Der Pfad zu der Quellcode-Datei aus welcher die BasicEffect Instanz erzeugt werden
		/// soll.
		/// </param>
		public BasicEffect(Device device, string path) {
			effect = CreateEffect(device, path, false);
			// 'Pointer' für die jeweiligen Effekt- bzw. Shadervariablen und Techniques
			// setzen.
			SetupEffectVariables();
			SetupTechniques();
		}

		/// <summary>
		/// Setzt die 'gWorld' Matrix Variable des Effekts.
		/// </summary>
		/// <param name="m">
		/// Die Transformationsmatrix auf welche die 'gWorld' Variable gesetzt werden soll.
		/// </param>
		/// <remarks>
		/// Diese Matrix wird benutzt, um Vertices aus ihrem lokalen Koordinatensystem ins
		/// 'Weltkoordinatensystem' zu transformieren.
		/// </remarks>
		public void SetWorld(Matrix m) {
			world.SetMatrix(m);
		}

		/// <summary>
		/// Setzt die 'gWorldViewProj' Matrix Variable des Effekts.
		/// </summary>
		/// <param name="m">
		/// Die Transformationsmatrix auf welche die 'gWorldViewProj' Variable gesetzt
		/// werden soll.
		/// </param>
		/// <remarks>
		/// Diese Matrix wird benutzt, um Vertices aus ihrem lokalen Koordinatensystem
		/// in den homogenen clip space zu transformieren.
		/// </remarks>
		public void SetWorldViewProjection(Matrix m) {
			worldViewProjection.SetMatrix(m);
		}

		/// <summary>
		/// Setzt die 'gDiffuseMap' Variable des Effekts.
		/// </summary>
		/// <param name="diffuseMapView">
		/// Die <c>ShaderResourceView</c> Instanz auf welche die 'gDiffuseMap' Variable
		/// gesetzt werden soll.
		/// </param>
		public void SetDiffuseMap(ShaderResourceView diffuseMapView) {
			diffuseMap.SetResource(diffuseMapView);
		}

		/// <summary>
		/// Setzt die 'gSpecularMap' Variable des Effekts.
		/// </summary>
		/// <param name="specularMapView">
		/// Die <c>ShaderResourceView</c> Instanz auf welche die 'gSpecularMap' Variable
		/// gesetzt werden soll.
		/// </param>
		public void SetSpecularMap(ShaderResourceView specularMapView) {
			specularMap.SetResource(specularMapView);
		}

		/// <summary>
		/// Setzt die 'gDirLight' Variable des Effekts.
		/// </summary>
		/// <param name="light">
		/// Die <c>DirectionalLight</c> Instanz auf welche die 'gDirLight' Variable gesetzt
		/// werden soll.
		/// </param>
		public void SetDirectionalLight(DirectionalLight light) {
			dirLight.SetRawValue(new DataStream(light.ToBytes(), false, false),
				DirectionalLight.Size);
		}

		/// <summary>
		/// Setzt die 'gPointLight' Variable des Effekts.
		/// </summary>
		/// <param name="light">
		/// Die <c>PointLight</c> Instanz auf welche die 'gPointLight' Variable gesetzt
		/// werden soll.
		/// </param>
		public void SetPointLight(PointLight light) {
			pointLight.SetRawValue(new DataStream(light.ToBytes(), false, false),
				PointLight.Size);
		}

		/// <summary>
		/// Setzt die 'gEyePosW' Variable des Effekts.
		/// </summary>
		/// <param name="position">
		/// Die Augpunktposition der Kamera im Weltkoordinatensystem auf welche die 'gEyePosW'
		/// Variable gesetzt werden soll.
		/// </param>
		public void SetEyePosition(Vector3 position) {
			eyePosition.Set(position);
		}

		/// <summary>
		/// Setzt die 'gColor' Variable des Effekts.
		/// </summary>
		/// <param name="color">
		/// Die Farbe auf welche die 'gColor' Variable gesetzt werden soll.
		/// </param>
		public void SetColor(Color color) {
			this.color.Set(color);
		}

		/// <summary>
		/// Gibt alle Resourcen der Instanz frei.
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Gibt alle Resourcen der Instanz frei, optionaler auch die managed Resourcen.
		/// </summary>
		/// <param name="disposing">
		/// true, um die managed Resourcen der Instanz freizugeben; andernfalls false.
		/// </param>
		protected virtual void Dispose(bool disposing) {
			if (!disposed) {
				// Merken, daß die Instanz freigegeben wurde.
				disposed = true;
				// Managed Resourcen freigeben.
				if (disposing) {
					if (effect != null)
						effect.Dispose();
				}
				// Hier unmanaged Resourcen freigeben.
			}
		}

		/// <summary>
		/// Erstellt einen Effekt aus der angegebenen Datei.
		/// </summary>
		/// <param name="device">
		/// Die Direct3D Device für welche die <c>Effekt</c> Instanz erstellt werden soll.
		/// </param>
		/// <param name="path">
		/// Der Pfad zu der Datei des Effekts.
		/// </param>
		/// <param name="precompiled">
		/// Falls true, gibt die unter <c>path</c> angegebene Datei eine vorkompilierte
		/// Binärdatei an; andernfalls gibt die Datei eine Effekt Quellcode Datei an.
		/// </param>
		/// <exception cref="Direct3D11Exception">
		/// Das Erstellen des Effekts ist fehlgeschlagen.
		/// </exception>
		static Effect CreateEffect(Device device, string path, bool precompiled = true) {
			ShaderBytecode bytecode = null;
			string errors = null;
			try {
				if (precompiled) {
					var bytes = File.ReadAllBytes(path);
					using (var ds = new DataStream(bytes.Length, true, true)) {
						ds.Write(bytes, 0, bytes.Length);
						bytecode = new ShaderBytecode(ds);
					}
				} else {
					bytecode = ShaderBytecode.CompileFromFile(path, null, "fx_5_0",
						ShaderFlags.None, EffectFlags.None, null, new IncludeFx(), out errors);
				}
				return new Effect(device, bytecode);
			} catch (Exception e) {
				throw new Direct3D11Exception("Effect creation failed: " + errors, e);
			} finally {
				if (bytecode != null)
					bytecode.Dispose();
			}
		}

		/// <summary>
		/// Assoziiert Referenzen mit den entsprechenden Variablen der Effekt-Instanz.
		/// </summary>
		/// <exception cref="Direct3D11Exception">
		/// Eine oder mehrere Variablen konnten nicht aufgelöst werden.
		/// </exception>
		void SetupEffectVariables() {
			IList<string> notFound = new List<string>();
			if ((world = effect.GetVariableByName("gWorld").AsMatrix()) == null)
				notFound.Add("gWorld");
			if ((worldViewProjection = effect.GetVariableByName("gWorldViewProj").AsMatrix()) == null)
				notFound.Add("gWorldViewProj");
			if ((diffuseMap = effect.GetVariableByName("gDiffuseMap").AsResource()) == null)
				notFound.Add("gDiffuseMap");
			if ((specularMap = effect.GetVariableByName("gSpecularMap").AsResource()) == null)
				notFound.Add("gSpecularMap");
			if ((dirLight = effect.GetVariableByName("gDirLight")) == null)
				notFound.Add("gDirLight");
			if ((pointLight = effect.GetVariableByName("gPointLight")) == null)
				notFound.Add("gPointLight");
			if((eyePosition = effect.GetVariableByName("gEyePosW").AsVector()) == null)
				notFound.Add("gEyePosW");
			if ((color = effect.GetVariableByName("gColor").AsVector()) == null)
				notFound.Add("gColor");
			if (notFound.Count > 0) {
				throw new Direct3D11Exception("The following effect variables could not be found: "
					+ string.Join(", ", notFound));
			}
		}

		/// <summary>
		/// Assoziiert Referenzen mit den verschiedenen Techniken des Effekts.
		/// </summary>
		/// <exception cref="Direct3D11Exception">
		/// Eine oder mehrere Techniken konnten nicht aufgelöst werden.
		/// </exception>
		void SetupTechniques() {
			IList<string> notFound = new List<string>();
			TexTech = effect.GetTechniqueByName("TexTech");
			if(!TexTech.IsValid)
				notFound.Add("TexTech");
			TexLitTech = effect.GetTechniqueByName("TexLitTech");
			if(!TexLitTech.IsValid)
				notFound.Add("TexLitTech");
			TexSpecLitTech = effect.GetTechniqueByName("TexSpecLitTech");
			if (!TexSpecLitTech.IsValid)
				notFound.Add("TexSpecLitTech");
			ColorTech = effect.GetTechniqueByName("ColorTech");
			if (!ColorTech.IsValid)
				notFound.Add("ColorTech");
			ColorLitTech = effect.GetTechniqueByName("ColorLitTech");
			if (!ColorLitTech.IsValid)
				notFound.Add("ColorLitTech");
			if (notFound.Count > 0) {
				throw new Direct3D11Exception("The following effect techniques could not be found: "
					+ string.Join(", ", notFound));
			}
		}
	}
}
