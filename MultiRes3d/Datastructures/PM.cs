using SlimDX.Direct3D11;
using SlimDX.DXGI;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Buffer = SlimDX.Direct3D11.Buffer;
using Math = MultiRes3d.Math;
using Device = SlimDX.Direct3D11.Device;
using System.Drawing;
using System.Collections.Generic;
using SlimDX;
using System.Linq;

namespace MultiRes3d {
	/// <summary>
	/// Repräsentiert ein renderbares Progressive Mesh Objekt.
	/// </summary>
	public class PM : Entity, IDisposable {
		bool disposed;
		Buffer vertexBuffer, indexBuffer;
		InputLayout inputLayout;
		VertexBufferBinding vertexBufferBinding;
		Viewport3d viewport3d;

		/// <summary>
		/// Initialisiert eine neue Instanz der PM Klasse.
		/// </summary>
		/// <param name="viewport3d">
		/// Das D3D11 Viewport3d Control für welche die Instanz erzeugt wird.
		/// </param>
		/// <param name="m">
		/// Eine Mesh Instanz mit deren Daten die Progressiv Mesh initialisiert werden soll.
		/// </param>
		public PM(Viewport3d viewport3d, Mesh m)
			: base() {
				this.viewport3d = viewport3d;
				vertexBuffer = CreateVertexBuffer(m.Vertices.Count, m.Vertices);
				indexBuffer = CreateIndexBuffer(m.Faces.Count * 3, m.Faces);
				inputLayout = CreateInputLayout();
				vertexBufferBinding = new VertexBufferBinding(vertexBuffer, Vertex.Size, 0);

				numIndices = m.Faces.Count * 3;

		}
		int numIndices;
		/// <summary>
		/// Rendert die Instanz.
		/// </summary>
		/// <param name="context">
		/// Der D3D11 Device Context.
		/// </param>
		/// <param name="effect">
		/// Die Effekt Instanz zum Setzen der benötigten Shader Variablen.
		/// </param>
		public override void Render(DeviceContext context, BasicEffect effect) {
			// D3D11 Input-Assembler konfigurieren.

			ApplyRenderState(context, effect);

			// Nun können wir die Daten in unseren Buffern rendern.
			var tech = effect.ColorLitTech;
			for (int i = 0; i < tech.Description.PassCount; i++) {
				tech.GetPassByIndex(i).Apply(context);

				context.DrawIndexed(numIndices, 0, 0);
			}
		}

		/// <summary>
		/// Gibt die Resourcen der Instanz frei.
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary> 
		/// Gibt die Resourcen der Instanz frei.
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
					if (vertexBuffer != null)
						vertexBuffer.Dispose();
					if (indexBuffer != null)
						indexBuffer.Dispose();
					if (inputLayout != null)
						inputLayout.Dispose();
				}
				// Hier Unmanaged Resourcen freigeben.
			}
		}

		void ApplyRenderState(DeviceContext context, BasicEffect effect) {
			context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
			context.InputAssembler.InputLayout = inputLayout;
			context.InputAssembler.SetVertexBuffers(0, vertexBufferBinding);
				context.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);
			effect.SetColor(Color.Gray);
		}


		#region D3D11 Initialisierungen
		Buffer CreateVertexBuffer(int maxVertices, IList<Vertex> vertices) {
			var desc = new BufferDescription(Vertex.Size * maxVertices,
				/*ResourceUsage.Dynamic*/ ResourceUsage.Immutable, BindFlags.VertexBuffer, /*CpuAccessFlags.Write*/ CpuAccessFlags.None,
				ResourceOptionFlags.None, 0);
//			return new Buffer(viewport3d.Device, desc);
			return new Buffer(viewport3d.Device, new DataStream(vertices.ToArray(), true, false), desc);
		}

		Buffer CreateIndexBuffer(int maxIndices, IList<Triangle> faces) {
			var desc = new BufferDescription(sizeof(uint) * maxIndices,
				/*ResourceUsage.Dynamic*/ ResourceUsage.Immutable, BindFlags.IndexBuffer, /*CpuAccessFlags.Write*/ CpuAccessFlags.None,
				ResourceOptionFlags.None, 0);
//			return new Buffer(viewport3d.Device, desc);
			var indices = new uint[maxIndices];
			for(int i = 0; i < faces.Count; i++) {
				for(int c = 0; c < 3; c++) {
					indices[i * 3 + c] = Convert.ToUInt32(faces[i].Indices[c]);
				}
			}
			return new Buffer(viewport3d.Device, new DataStream(indices, false, false), desc);
		}

		InputLayout CreateInputLayout() {
			// Unser Input-Layout ist sehr einfach: Pro Vertex gibt es nur einen Ortsvektor
			// und sonst nix.
			var elements = new[] {
				new InputElement("Position", 0, Format.R32G32B32_Float, 0, 0, 
					InputClassification.PerVertexData, 0),
				new InputElement("Normal", 0, Format.R32G32B32_Float, 4*3, 0,
					InputClassification.PerVertexData, 0)
			};
			// Input-Layout wird gegen die Signatur der Shader-Technique geprüft, um
			// sicherzustellen, daß unsere Datenstruktur für den Vertex Shader und
			// die im Shader/Effect definierte übereinstimmen.
			var sig = viewport3d.Effect.ColorLitTech.GetPassByIndex(0).Description.Signature;

			return new InputLayout(viewport3d.Device, sig, elements);
		}
		#endregion

	}
}
