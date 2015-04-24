//
// Beinhaltet Strukturen und Funktionen zur Licht Berechnung.
//
// Basiert auf LightHelper.fx aus "Introduction to 3D Game Programming with DirectX 11"
// von Frank Luna.
//

//
// Repräsentert ein direktionales Licht. Diese Struktur _muss_ synchron gehalten werden mit
// der entsprechenden DirectionalLight Klasse im Projekt!
//
// Siehe DirectionalLight Klasse für Beschreibung der einzelnen Felder.
//
struct DirectionalLight {
	float4	Ambient;
	float4	Diffuse;
	float4	Specular;
	float3	Direction;
	// Shader Strukturen müssen immer ein Vielfaches von 16-Byte groß sein.
	float	pad;
};

//
// Repräsentert ein Point Licht. Diese Struktur _muss_ synchron gehalten werden mit
// der entsprechenden DirectionalLight Klasse im Projekt!
//
// Siehe DirectionalLight Klasse für Beschreibung der einzelnen Felder.
//
struct PointLight {
	float4	Ambient;
	float4	Diffuse;
	float4	Specular;
	float3	Position;
	float	Range;
	float3	Att;
	// Shader Strukturen müssen immer ein Vielfaches von 16-Byte groß sein.
	float	pad;
};

//
// Berechnet die Ambient, Diffuse und Specular Anteile der Lichtgleichung für ein
// direktionales Licht.
//
// @specMap:
//  Der gesamplete Wert der Specular Map an der entsprechenden Texel Position.
// @L:
//  Das direktionale Licht, für welches die Berechnung durchgeführt werden soll.
// @normal:
//  Die transformierte Oberflächennormale.
// @toEye:
//  Der normierte Richtungsvektor von der transformierten Vertex Position zu der
//  Position des Augpunkts des Betrachters (der Kamera).
// @ambient:
//  Der out-Parameter der den berechneten Ambient Anteil erhält.
// @diffuse
//  Der out-Parameter der den berechneten Diffuse Anteil erhält.
// @spec
//  Der out-Parameter der den berechnetn Specular Anteil erhält.
//
void ComputeDirectionalLight(float4 specMap, DirectionalLight L, float3 normal, float3 toEye,
							 out float4 ambient, out float4 diffuse, out float4 spec) {
	// Werte initialisieren.
	ambient	= L.Ambient;
	diffuse	= float4(0.0f, 0.0f, 0.0f, 0.0f);
	spec	= float4(0.0f, 0.0f, 0.0f, 0.0f);
	// Der Lichtvektor zeigt in die entgegengesetzte Richtung.
	float3 lightVec = -L.Direction;
	// Diffuse und Specular Anteile berechnen.
	float diffuseFactor = dot(lightVec, normal);
	// Dynamisches Branching im Shader verhindern.
	[flatten]
	if( diffuseFactor > 0.0f ) {
		float3 v         = reflect(-lightVec, normal);
		// Alpha Channel der Specular Map enthält die 'Shininess'.
		// FIXME: Einfach den Red Channel benutzen.
		float specFactor = pow(max(dot(v, toEye), 0.0f), specMap.r * 18.0f);

		diffuse = diffuseFactor * L.Diffuse;
		spec    = specFactor * specMap * L.Specular;
	}
}

//
// Berechnet die Ambient, Diffuse und Specular Anteile der Lichtgleichung für ein
// Point Light.
//
// @specMap:
//  Der gesamplete Wert der Specular Map an der entsprechenden Texel Position.
// @L:
//  Das Point Light, für welches die Berechnung durchgeführt werden soll.
// @pos:
//  Die Position an welcher das Licht berechnet werden soll.
// @normal:
//  Die transformierte Oberflächennormale.
// @toEye:
//  Der normierte Richtungsvektor von der transformierten Vertex Position zu der
//  Position des Augpunkts des Betrachters (der Kamera).
// @ambient:
//  Der out-Parameter der den berechneten Ambient Anteil erhält.
// @diffuse
//  Der out-Parameter der den berechneten Diffuse Anteil erhält.
// @spec
//  Der out-Parameter der den berechnetn Specular Anteil erhält.
//
void ComputePointLight(float4 specMap, PointLight L, float3 pos, float3 normal, float3 toEye,
				   out float4 ambient, out float4 diffuse, out float4 spec) {
	// Werte initialisieren.
	ambient = float4(0.0f, 0.0f, 0.0f, 0.0f);
	diffuse = float4(0.0f, 0.0f, 0.0f, 0.0f);
	spec    = float4(0.0f, 0.0f, 0.0f, 0.0f);
	// Der Vektor von dem Punkt der. Oberfläche zur Lichtquelle.
	float3 lightVec = L.Position - pos;
	// Die Distanz zwischen Punkt und Lichtquelle.
	float d = length(lightVec);
	// Point Lights haben nur eine fixe Reichweite.
	if( d > L.Range )
		return;
	// Light Vektor normieren.
	lightVec /= d; 
	// Ambient Anteil.
	ambient = L.Ambient;
	// Diffuse und Specular Anteile berechnen.
	float diffuseFactor = dot(lightVec, normal);
	// Dynamisches Branching im Shader verhindern.
	[flatten]
	if( diffuseFactor > 0.0f ) {
		float3 v         = reflect(-lightVec, normal);
		// Alpha Channel der Specular Map enthält die 'Shininess'.
		// FIXME: Einfach den Red Channel benutzen.
		float specFactor = pow(max(dot(v, toEye), 0.0f), specMap.r * 18.0f);
					
		diffuse = diffuseFactor * L.Diffuse;
		spec    = specFactor * specMap * L.Specular;
	}
	// Dämpfung berücksichtigen.
	float att = 1.0f / dot(L.Att, float3(1.0f, d, d*d));
	diffuse *= att;
	spec    *= att;
}
