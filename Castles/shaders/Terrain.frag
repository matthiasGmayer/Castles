#version 430
const int MAXLIGHTS = 12;
in vec3 normal;
in vec3 toLight[MAXLIGHTS];
in vec3 toCamera;
in vec2 uv;
in float visibility;
in vec4 clipSpace;
in float steepness;

out vec4 vFragColor;

uniform vec3 pointLightColor[MAXLIGHTS];
uniform vec3 dirLightColor[MAXLIGHTS];
uniform vec3 attenuation[MAXLIGHTS];
uniform int pointLightNumber;
uniform int dirLightNumber;
uniform bool grassNormalMapping = false;
uniform bool stoneNormalMapping = false;
uniform bool grassSpecularMapping = false;
uniform bool stoneSpecularMapping = false;
const float reflectivity = 1;
const float shineDamper = 10;
layout(binding = 0)uniform sampler2D grassTex;
layout(binding = 1)uniform sampler2D grassNormalTex;
layout(binding = 2)uniform sampler2D grassSpecularTex;
layout(binding = 5)uniform sampler2D stoneTex;
layout(binding = 6)uniform sampler2D stoneNormalTex;
layout(binding = 7)uniform sampler2D stoneSpecularTex;
layout(binding = 10)uniform sampler2D skyTex;

void main(void){


	vec2 coords = uv * 50;
	float grassFactor = pow(steepness, 40);
	float stoneFactor = 1 - grassFactor;
	vec4 color = grassFactor * texture(grassTex, coords) + stoneFactor * texture(stoneTex, coords);
	vec4 specular = vec4(0);
	if(grassSpecularMapping)
		specular += grassFactor * texture(grassSpecularTex, coords);
	if(stoneSpecularMapping)
		specular += stoneFactor * texture(stoneSpecularTex, coords);


	vec3 finalDiffuse = vec3(0);
	vec3 finalSpecular = vec3(0);
	vec3 unitToCamera = normalize(toCamera);
	vec3 unitNormal = vec3(0);
	if(grassSpecularMapping)
		specular += grassFactor * texture(grassSpecularTex, coords);
	if(stoneSpecularMapping)
		specular += stoneFactor * texture(stoneSpecularTex, coords);
	if(!stoneSpecularMapping && !grassSpecularMapping)
		unitNormal = normalize(normal);



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
		vec3 reflectedLightDirection = reflect( -unitToLight , unitNormal);
		float specular = max(dot(reflectedLightDirection, unitToCamera), 0);
		float dampedSpecular = pow(specular, shineDamper);
		finalSpecular += dampedSpecular * reflectivity * lightColor / attenuationFactor;
	}
	//ambient light
	if((grassSpecularMapping && grassFactor > 0.5) || (stoneSpecularMapping && stoneFactor > 0.5)){
		//finalSpecular *= specular.x;
		//finalDiffuse += sqrt(spec.g - 0.5) + 0.5;
		if(specular.y > 0.5)
		finalDiffuse = vec3(1);
	}
	finalDiffuse = clamp(finalDiffuse, 0.08, 1.);
	vec4 finalLight = vec4(finalDiffuse + finalSpecular, 1);
	vFragColor = finalLight * color;

	vec4 skyColor = texture( skyTex,(clipSpace.xy/clipSpace.w)/2+0.5f);
	vFragColor = mix(skyColor, vFragColor, visibility);
}