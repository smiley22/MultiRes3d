//
// Ein einfacher Rendering-Effekt der momentan Transformationen, Lichtberechnungen und
// Texturen unterstützt.
//
// Basiert auf BasicEffect.fx aus dem "Introduction to 3D Game Programming with DirectX 11"
// von Frank Luna.
//

#include "LightHelper.fx"

// *************************                                       *************************
// *************** Werte die auf einer per-Objekt Basis aktualisiert werden. ***************
// *************************                                       *************************
cbuffer cbPerObject {
	//
	// Die Weltmatrix des Objekts.
	//
	float4x4 gWorld;
	//
	// Die world-view-projection Matrix des Objekts.
	//
	float4x4 gWorldViewProj;
	//
	// Die Farbe des Objekts, falls es keine Texture Map benutzt.
	//
	float4 gColor;
}

//
// Nicht-numerische Werte wie Texture2D können nicht in einem cbuffer zusammengefasst werden. Die
// folgenden Werte werden auch auf einer per-Objekt Basis aktualisiert.
//

//
// Die Diffuse-Map (d.h. Texture oder 'Skin') des Objekts.
//
Texture2D gDiffuseMap;

//
// Die Specular-Map des Objekts, wenn es eine benutzt.
//
Texture2D gSpecularMap;



// *************************                                      *************************
// *************** Werte die auf einer per-Frame Basis aktualisiert werden. ***************
// *************************                                      *************************
cbuffer cbPerFrame {
	//
	// Das globale direktionale Light der Szene (z.b. die Sonne).
	//
	DirectionalLight gDirLight;
	//
	// Unser Point Light.
	//
	PointLight gPointLight;
	//
	// Der Augpunkt des Betrachters (d.h. der Kamera), in Weltkoordinaten.
	//
	float3 gEyePosW;
}



//
// Der Sampler-State, der zum Samplen von Texturen benutzt werden soll.
//
SamplerState sampler0 {
	Filter			= ANISOTROPIC;
	MaxAnisotropy	= 4;
};

//
// Die per-Vertex Daten, die an den Vertex Shader übergeben werden.
//
struct VertexIn {
	float3 PosL		: Position;
	float3 NormalL	: Normal;
	float2 Tex		: TexCoord;
};

//
// Der Rückgabewert des Vertex Shader, der anschließend als Eingabe für den Pixel Shader dient.
//
struct VertexOut {
	float4 PosH		: SV_POSITION;
	float3 PosW		: Position;
	float3 NormalW	: Normal;
	float2 Tex		: TexCoord;
};

//
// Einstiegspunkt des Vertex Shaders.
//
// @vin:
//  Eine Variable vom Typ VertexIn, die die Eingabedaten für diesen Aufruf des Vertex Shaders
//  enthält.
// @return
//  Eine Variable vom Typ VertexOut, die die transformierten Koordinaten und restliche
//  Daten beeinhaltet, die anschließend für Aufrufe des Pixel Shaders als Eingaben dienen.
//
VertexOut VS(VertexIn vin) {
	VertexOut vout;
	// Vertex Positon in Weltkoordinatensytem transformieren.
	vout.PosW		= mul(float4(vin.PosL, 1.0f), gWorld).xyz;
	// Vertex Position in Homogenen Clip Space transformieren.
	vout.PosH		= mul(float4(vin.PosL, 1.0f), gWorldViewProj);
	// Normale in Weltkoordinatensystem transformieren. Da wir keine Scherung durchführen,
	// können wir hier einfach die normal Weltmatrix anstatt der invers Transponierten
	// für die Transformation benutzen.
	vout.NormalW	= mul(vin.NormalL, (float3x3)gWorld);
	vout.Tex		= vin.Tex;

	return vout;
}

//
// Einstiegspunkt des Pixel Shaders.
//
// @pin
//  Eine Variable vom Typ VertexOut, die die Eingaben für diesen Aufruf des Pixel Shaders
//  enthält.
// @useLighting
//  Eine compile-time Variable (ähnlich wie pre-processor) die angibt, ob Lichtberechnung
//  durchgeführt werden soll oder nicht.
// @useSpecMap
//  Eine compile-time Variable (ähnlich wie pre-processor) die angibt, ob die Specular Map
//  eines Objekts für die Lichtberechnung berücksichtigt werden soll oder nicht.
// @return
//  Der berechnete Farbwert des Pixels.
//
float4 PS(VertexOut pin, uniform bool useLighting, uniform bool useSpecMap) : SV_Target {
	float4 litColor = gDiffuseMap.Sample( sampler0, pin.Tex );
if (useLighting) {
	// Interpolierte Normale muss ggf. erneut normalisiert werden. (Warum nochmal?)
//	pin.NormalW       = normalize(pin.NormalW);

	float4 ambient    = float4(0.0f, 0.0f, 0.0f, 0.0f);
	float4 diffuse    = float4(0.0f, 0.0f, 0.0f, 0.0f);
	float4 spec       = float4(0.0f, 0.0f, 0.0f, 0.0f);

	float4 specSample = float4(0.0f, 0.0f, 0.0f, 0.0f);
if(useSpecMap) {
	specSample = gSpecularMap.Sample( sampler0, pin.Tex );
}
	float3 toEyeW = normalize(gEyePosW - pin.PosW);
	// Die Beiträge jeder Lichtquelle aufsummieren.
	float4 A, D, S;
	ComputeDirectionalLight(specSample, gDirLight, pin.NormalW, toEyeW, A, D, S);
	ambient += A;  
	diffuse += D;
	spec    += S;

	// Unser Point Light berücksichtigen.
	ComputePointLight(specSample, gPointLight, pin.PosW, pin.NormalW, toEyeW, A, D, S);
	ambient += A;
	diffuse += D;
	spec    += S;

	litColor = litColor*(ambient + diffuse) + spec;
}
	return litColor;
}


struct ColorVertexIn {
	float3 PosL		: Position;
};

struct ColorVertexOut {
	float4 PosH		: SV_POSITION;
};


ColorVertexOut ColorVS(ColorVertexIn vin) {
	ColorVertexOut vout;
	// In Homogenen Clip Space transformieren.
	vout.PosH		= mul(float4(vin.PosL, 1.0f), gWorldViewProj);
	return vout;
}

float4 ColorPS(ColorVertexOut pin) : SV_Target {
//	return pin.Color;
	return gColor;
}

// *************************                                      *************************
// *************************           Effect Techniken           *************************
// *************************                                      *************************

//
// Diffuse Map zum Texturieren benutzen. Specular Map ignorieren und keinerlei Licht.
//
technique11 TexTech {
	pass P0 {
		SetVertexShader(CompileShader(vs_4_0, VS()));
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PS(false, false)));
	}
}

//
// Diffuse Map zum Texturieren benutzen. Specular Map ignorieren aber Licht berechnen.
//
technique11 TexLitTech {
	pass P0 {
		SetVertexShader(CompileShader(vs_4_0, VS()));
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PS(true, false)));
	}
}

//
// Diffuse Map zum Texturieren benutzen. Specular Map benutzen und Licht berechnen.
//
technique11 TexSpecLitTech {
	pass P0 {
		SetVertexShader(CompileShader(vs_4_0, VS()));
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PS(true, true)));
	}
}

//
// Farbwerte des Vertex benutzen.
//
technique11 ColorTech {
	pass P0 {
		SetVertexShader(CompileShader(vs_4_0, ColorVS()));
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, ColorPS()));
	}
}