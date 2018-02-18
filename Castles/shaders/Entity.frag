#version 430
const int MAXLIGHTS = 12;
in vec3 normal;
in vec3 toLight[MAXLIGHTS];
in vec3 toCamera;
in vec2 uv;
in vec4 clipSpace;
in float visibility;
in vec3 tangent;

out vec4 vFragColor;

uniform vec3 pointLightColor[MAXLIGHTS];
uniform vec3 dirLightColor[MAXLIGHTS];
uniform vec3 attenuation[MAXLIGHTS];
uniform int pointLightNumber;
uniform int dirLightNumber;
uniform float reflectivity;
uniform float shineDamper;
layout(binding = 0)uniform sampler2D tex;
layout(binding = 1)uniform sampler2D normalMap;
layout(binding = 2)uniform sampler2D specularMap;
layout(binding = 3)uniform sampler2D skyTex;
uniform bool normalMapping;
uniform bool specularMapping;

void main(void){

	vec4 color = texture(tex, uv);
	if(color.a < 0.5)
		discard;

	vec3 finalDiffuse = vec3(0);
	vec3 finalSpecular = vec3(0);


	vec3 unitNormal;
	if(normalMapping)
	{
		unitNormal = normalize(2 * texture(normalMap, uv).rgb - 1);
	}
	else{
		unitNormal = normalize(normal);
	}

	vec3 unitToCamera = normalize(toCamera);

	float attenuationFactor = 1;

	for(int i = 0; i < pointLightNumber + dirLightNumber && i < MAXLIGHTS; i++){

		vec3 unitToLight = normalize(toLight[i]);
		vec3 lightColor;
		if(i >= pointLightNumber)
		{
			lightColor = dirLightColor[i-pointLightNumber];
			attenuationFactor = 1;
		}
		else
		{
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
	if(specularMapping){
		vec4 spec = texture(specularMap, uv);
		finalSpecular *= spec.r;
		//finalDiffuse += sqrt(spec.g - 0.5) + 0.5;
		if(spec.g > 0.5)
		finalDiffuse = vec3(1);
	}
	finalDiffuse = clamp(finalDiffuse, 0.08, 1.);
	vec4 finalLight = vec4(finalDiffuse + finalSpecular, 1);
	vFragColor = finalLight * color;

	vec4 skyColor = texture( skyTex,(clipSpace.xy/clipSpace.w)/2+0.5f);
	vFragColor = mix(skyColor, vFragColor, visibility);
}