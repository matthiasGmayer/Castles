#version 430
const int MAXLIGHTS = 12;

in vec4 clipSpace;
in vec2 tex;
in vec3 toCamera;
in vec3 toLight[MAXLIGHTS];
in vec4 positionToCamera;

uniform sampler2D reflectionTex;
uniform sampler2D refractionTex;
uniform sampler2D dudvTex;
uniform sampler2D normalTex;
uniform sampler2D depthTex;
uniform sampler2D skyTex;
uniform float waveStrength;
uniform float time;
uniform float near;
uniform float far;
uniform vec3 pointLightColor[MAXLIGHTS];
uniform vec3 dirLightColor[MAXLIGHTS];
uniform int pointLightNumber;
uniform int dirLightNumber;
uniform vec3 attenuation[MAXLIGHTS];

out vec4 vFragColor;

void main(void){

	vec3 unitToCamera = normalize(toCamera);

	vec2 coords = (clipSpace.xy/clipSpace.w)/2+0.5f;

	float floorDistance = 2.0 * near * far / (far + near - (2.0 * texture(depthTex, coords).x - 1.0) * (far - near));
	float waterDistance = 2.0 * near * far / (far + near - (2.0 * gl_FragCoord.z - 1.0) * (far - near));
	float waterDepth = (floorDistance - waterDistance) * unitToCamera.y;
	float waterDepthFactor = clamp(waterDepth/15,0,1);

	float t = fract(time * 30);

	vec2 distortedCoords = texture(dudvTex, vec2(tex.x + t, tex.y)).rg * 0.1f;
	distortedCoords = tex + vec2(distortedCoords.x, distortedCoords.y + t);
	vec2 distortion = (texture(dudvTex, distortedCoords).rg * 2 - 1) * waveStrength * waterDepthFactor;
	vec3 normal = texture(normalTex, distortedCoords).rgb;

	normal = vec3(normal.x * 2 - 1, normal.z * 3, normal.y * 2 - 1);
	vec3 unitNormal = normalize(normal);
	
	vec2 dcoords = coords + distortion;
	dcoords = clamp(dcoords, 0.001,0.999);

	vec4 refraction = texture(refractionTex, dcoords);
	vec4 reflection = texture(reflectionTex, vec2(dcoords.x, -dcoords.y));

	vec3 finalDiffuse = vec3(0);
	vec3 finalSpecular = vec3(0);

	vec4 color = mix(reflection, refraction, clamp(dot(normalize(toCamera), unitNormal),0,1));
	color = mix(color, vec4(0,0.3,0.5,1),0.2);

	float reflectivity = 1;
	float shineDamper = 10;

	float attenuationFactor = 1;

	for(int i = 0; i < pointLightNumber + dirLightNumber && i < MAXLIGHTS; i++){

		vec3 unitToLight = normalize(toLight[i]);
		vec3 lightColor;
		if(i >= pointLightNumber){
			lightColor = dirLightColor[i-pointLightNumber];
			attenuationFactor = 1;
			}
		else{
			lightColor = pointLightColor[i];
			float f = length(toLight[i]);
			attenuationFactor = attenuation[i].x + attenuation[i].y * f + attenuation[i].z * f * f;
			}

		//pp lighting
		float brightness = dot(unitNormal, unitToLight);
		finalDiffuse += brightness * lightColor / attenuationFactor;

		//specular lighting
		vec3 reflectedLightDirection = reflect( -unitToLight , unitNormal);
		float specular = max(dot(reflectedLightDirection, unitToCamera), 0);
		float dampedSpecular = pow(specular, shineDamper);
		finalSpecular += dampedSpecular * reflectivity * lightColor / attenuationFactor;
	}
	//ambient light
	finalDiffuse = max(finalDiffuse, 0.08f);

	vFragColor = vec4(finalSpecular * waterDepthFactor, 1) + color - vec4(1 - finalDiffuse, 1) / 10f;
	vFragColor.a = waterDepthFactor;

	vec4 skyColor = texture( skyTex,(clipSpace.xy/clipSpace.w)/2+0.5f);
	float distance = length(positionToCamera.xyz);
	vFragColor = mix(skyColor, vFragColor, clamp(1.5/(1+exp( 20/far*(distance - far / 3))),0,1));
}