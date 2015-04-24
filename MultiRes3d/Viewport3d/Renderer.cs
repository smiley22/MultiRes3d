using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DirectWrite;
using SlimDX.Windows;
using SpriteTextRenderer.SlimDX;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MultiRes3d {
	/// <summary>
	/// Implementiert die Komponente, die für das eigentliche Rendern der Szene
	/// verantwortlich ist.
	/// </summary>
	internal class Renderer : IDisposable {
		#region Private Felder
		const float DefaultFieldOfView = Math.PiOver4;
		const float DefaultNearClippingPlaneDistance = 0.1f;
		const float DefaultFarClippingPlaneDistance = 10.0f;
		static readonly Color DefaultClearColor = Color.Black;
		IList<Entity> entities = new List<Entity>();
		bool disposed;
		D3D11Control control;
		DeviceContext context;
		BasicEffect effect;
		Camera camera;
		Matrix projectionMatrix;
		Color clearColor = DefaultClearColor;
		bool updateProjectionMatrix = true;
		float fov = DefaultFieldOfView;
		float nearClippingPlaneDistance = DefaultNearClippingPlaneDistance;
		float farClippingPlaneDistance = DefaultFarClippingPlaneDistance;
		DirectionalLight directionalLight = new DirectionalLight();
		PointLight pointLight = new PointLight();
		float aspectRatio;
		SpriteRenderer spriteRenderer;
		TextBlockRenderer font;
		#endregion

		/// <summary>
		/// Liefer oder setzt die Clear Color für den Color Buffer.
		/// </summary>
		public Color ClearColor {
			get {
				return clearColor;
			}
			set {
				clearColor = value;
			}
		}

		/// <summary>
		/// Liefert oder setzt das Blickfeld, in Radiant.
		/// </summary>
		public float Fov {
			get {
				return fov;
			}
			set {
				fov = value;
				updateProjectionMatrix = true;
			}
		}

		/// <summary>
		/// Die Distanz der Near Clipping Plane zum Blickpunkt.
		/// </summary>
		public float NearClippingPlaneDistance {
			get {
				return nearClippingPlaneDistance;
			}
			set {
				nearClippingPlaneDistance = value;
				updateProjectionMatrix = true;
			}
		}

		/// <summary>
		/// Die Distanz der Far Clipping Plane zum Blickpunkt.
		/// </summary>
		public float FarClippingPlaneDistance {
			get {
				return farClippingPlaneDistance;
			}
			set {
				farClippingPlaneDistance = value;
				updateProjectionMatrix = true;
			}
		}

		/// <summary>
		/// Die globale direktionale Lichtquelle der Szene.
		/// </summary>
		public DirectionalLight DirectionalLight {
			get {
				return directionalLight;
			}
		}

		/// <summary>
		/// Die Punktlichtquelle der Szene.
		/// </summary>
		public PointLight PointLight {
			get {
				return pointLight;
			}
		}

		/// <summary>
		/// Die Objekte, die dem Renderer bekannt sind.
		/// </summary>
		public IList<Entity> Entities {
			get {
				return entities;
			}
		}

		/// <summary>
		/// Initialisiert eine neue Instanz der Renderer Klasse.
		/// </summary>
		/// <param name="control">
		/// Die <c>D3D11Control</c> Instanz für die die Renderer Instanz erstellt wird.
		/// </param>
		/// <param name="camera">
		/// Die Kamera aus deren Perspektive der Renderer die Szene rendern soll.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Der control Parameter oder der Kamera Parameter ist null.
		/// </exception>
		public Renderer(D3D11Control control, Camera camera) {
			control.ThrowIfNull("control");
			camera.ThrowIfNull("camera");
			this.control = control;
			this.camera = camera;
			if (control.Device != null) {
				context = control.Context;
				effect = new BasicEffect(control.Device, "Effects/Basic.fx");
			}
		}

		/// <summary>
		/// Rendert den angegebenen Text an der angegebenen Position in der angegebenen
		/// Farbe.
		/// </summary>
		/// <param name="text">
		/// Der Text.
		/// </param>
		/// <param name="position">
		/// Die Position in absoluten Koordinaten.
		/// </param>
		/// <param name="color">
		/// Die Farbe in der der Text gezeichnet werden soll.
		/// </param>
		public void DrawString(string text, Vector2 position, Color color) {
			font.DrawString(text, position, color);
		}

		/// <summary>
		/// Event Method, die aufgerufen wird, wenn sich das Seitenverhältnis des zugehörigen
		/// Controls geändert hat.
		/// </summary>
		/// <param name="aspectRatio">
		/// Das neue Seitenverhältnis.
		/// </param>
		public void OnAspectRatioChanged(float aspectRatio) {
			if (spriteRenderer == null)
				InitSpriteRenderer();
			else
				spriteRenderer.RefreshViewport();
			this.aspectRatio = aspectRatio;
			updateProjectionMatrix = true;
			// Neurendern der Szene erzwingen.
			Render();
		}

		/// <summary>
		/// Rendert die Szene.
		/// </summary>
		public void Render() {
			// Color und Depthbuffer löschen.
			control.Clear(clearColor);
			// Müssen wir die Projektionsmatrix neu berechnen?
			if (updateProjectionMatrix) {
				projectionMatrix = Matrix.PerspectiveFovLH(fov, aspectRatio,
					nearClippingPlaneDistance, farClippingPlaneDistance);
				updateProjectionMatrix = false;
			}
			var viewMatrix = camera.ViewMatrix;
			// Per-Frame Variablen des Effekts aktualisieren.
			effect.SetDirectionalLight(directionalLight);
			effect.SetPointLight(pointLight);
			effect.SetEyePosition(camera.Eye);
			
			//  3. Render Entities (Refactor into private method: RenderEntities).
			Matrix model, modelView, modelViewProjection;
			foreach (var ent in entities) {
				model = ent.Transform;
				Matrix.Multiply(ref model, ref viewMatrix, out modelView);
				Matrix.Multiply(ref modelView, ref projectionMatrix, out modelViewProjection);
				// TODO: Hier Render Methode alle registrierten Entity Instanzen aufrufen.
				ent.Render(context, effect);
			}
			// Sprite-Renderer flushen, um evtl. gecachete Sprites zu zeichnen.
			spriteRenderer.Flush();
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
					if (directionalLight != null)
						directionalLight.Dispose();
					if (effect != null)
						effect.Dispose();
					if (font != null)
						font.Dispose();
					if (spriteRenderer != null)
						spriteRenderer.Dispose();
				}
				// Hier Unmanaged Resourcen freigeben.
			}
		}

		/// <summary>
		/// Initialisiert den Sprite-Renderer zum Rendern von Bitmap Fonts.
		/// </summary>
		void InitSpriteRenderer() {
			if (spriteRenderer != null) {
				throw new InvalidOperationException("Die spriteRenderer Instanz wurde " +
					"bereits initialisiert.");
			}
			spriteRenderer = new SpriteRenderer(control.Device);
			font = new TextBlockRenderer(spriteRenderer, "Arial", FontWeight.Normal,
				SlimDX.DirectWrite.FontStyle.Normal, FontStretch.Normal, 14);
			// Virtuelle Auflösung von 640x480 zum Positionieren von Text benutzen.
			spriteRenderer.ScreenSize = new SpriteTextRenderer.STRVector(640.0f, 480.0f);
		}
	}
}
