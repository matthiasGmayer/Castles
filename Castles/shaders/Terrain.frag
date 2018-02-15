#version 430
const int MAXLIGHTS = 12;
in vec3 normal;
in vec3 toLight[MAXLIGHTS];
in vec3 toCamera;
in vec2 uv;
in float visibility;
in vec4 clipSpace;

out vec4 vFragColor;

uniform vec3 pointLightColor[MAXLIGHTS];
uniform vec3 dirLightColor[MAXLIGHTS];
uniform vec3 attenuation[MAXLIGHTS];
uniform int pointLightNumber;
uniform int dirLightNumber;
//uniform float reflectivity;
//uniform float shineDamper;
uniform sampler2D grassTex;
uniform sampler2D stoneTex;
uniform sampler2D skyTex;

void main(void){

	vec3 unitNormal = normalize(normal);

	vec4 grass = texture(grassTex, uv * 50);
	vec4 stone = texture(stoneTex, uv * 50);

	float factor = pow(unitNormal.y, 40);
	vec4 color = factor * grass + (1 - factor) * stone ;

	vec3 finalDiffuse = vec3(0);
	//vec3 finalSpecular = vec3(0);
	vec3 unitToCamera = normalize(toCamera);

	float attenuationFactor = 1;

	for(int i = 0; i < pointLightNumber + dirLightNumber && i < MAXLIGHTS; i++){

		vec3 unitToLight = normalize(toLight[i]);
		vec3 lightColor;
		if(i >= pointLightNumber){
			lightColor = dirLightColor[i - pointLightNumber];
			attenuationFactor = 1;
			}
		else{
			lightColor = pointLightColor[i];
			float f = length(toLight[i]);
			attenuationFactor = attenuation[i].x + attenuation[i].y * f + attenuation[i].z * f * f;
			}
		//pp lighting
		float brightness = dot(unitNormal, unitToLight);
		brightness = clamp(brightness, 0, 1);
		finalDiffuse += brightness * lightColor / attenuationFactor;

		//specular lighting   TERRAIN DOES AT THE TIME NOT NEED THIS
		//vec3 reflectedLightDirection = reflect( -unitToLight , unitNormal);
		//float specular = max(dot(reflectedLightDirection, unitToCamera), 0);
		//float dampedSpecular = pow(specular, shineDamper);
		//finalSpecular += dampedSpecular * reflectivity * lightColor / attenuationFactor;
	}
	//ambient light
	finalDiffuse = max(finalDiffuse, 0.08f);
	vec4 finalLight = vec4(finalDiffuse 
	//+ finalSpecular
	, 1);
	vFragColor = finalLight * color;

	vec4 skyColor = texture( skyTex,(clipSpace.xy/clipSpace.w)/2+0.5f);
	vFragColor = mix(skyColor, vFragColor, visibility);
}