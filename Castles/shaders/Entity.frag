#version 430
const int MAXLIGHTS = 12;
in vec3 normal;
in vec3 toLight[MAXLIGHTS];
in vec3 toCamera;

out vec4 vFragColor;

uniform vec3 pointLightColor[MAXLIGHTS];
uniform vec3 dirLightColor[MAXLIGHTS];
uniform int pointLightNumber;
uniform int dirLightNumber;
uniform float reflectivity;
uniform float shineDamper;

void main(void){

	int MAXLIGHTS = 6;

	vec4 color = vec4(1,1,1,1);

	vec3 finalDiffuse = vec3(0,0,0);
	vec3 finalSpecular = vec3(0,0,0);

	vec3 unitNormal = normalize(normal);
	vec3 unitToCamera = normalize(toCamera);

	for(int i = 0; i < pointLightNumber + dirLightNumber && i < MAXLIGHTS; i++){

		vec3 unitToLight = normalize(toLight[i]);
		vec3 lightColor;
		if(i >= pointLightNumber){
			lightColor = dirLightColor[i-pointLightNumber];
			}
		else{
			lightColor = pointLightColor[i];
			}

		//pp lighting
		float brightness = dot(unitNormal, unitToLight);
		finalDiffuse += brightness * lightColor;

		//specular lighting
		vec3 reflectedLightDirection = reflect(-unitToLight , unitNormal);
		float specular = max(dot(reflectedLightDirection, unitToCamera),0);
		float dampedSpecular = pow(specular, shineDamper);
		finalSpecular += dampedSpecular * reflectivity * lightColor;
	}
	//ambient light
	finalDiffuse = max(finalDiffuse, 0.08f);
	vec4 finalLight = vec4(finalDiffuse + 0.001f*finalSpecular,1);
	vFragColor = finalLight * color;
}