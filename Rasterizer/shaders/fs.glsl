#version 330
 
// shader input
in vec2 uv;			// interpolated texture coordinates
in vec4 normal;			// interpolated normal
in vec4 worldPos;

uniform sampler2D pixels;	// texture sampler
uniform vec3 Phlong;
uniform vec3 Camera;

vec3 lightPos;
vec3 lightColor;

// shader output
out vec4 outputColor;

// fragment shader
void main()
{
    //hardcoded light
    lightPos = vec3( 0.0f, 25.0f, 9.5f);
    float ambientCoeff = 0.45f;
    lightColor = vec3( 1.1f, 1.0f, 0.8f );
    float strength = 500.5f;

    //shading calculation    
    vec3 lightDirection = lightPos - worldPos.xyz;
    vec3 viewDir = normalize(Camera - worldPos.xyz);
    vec3 materialColor = texture(pixels, uv).xyz;
    vec3 norm = normalize(normal.xyz);
    
    vec3 ambient = ambientCoeff * lightColor * materialColor;

    float diffCoef = Phlong.x;
    float specCoef = Phlong.y;
    float shini = Phlong.z;
    lightColor *= strength;

    float dist = length(lightDirection);
    float attenuation = 1.0f / (dist * dist);
    lightDirection = normalize(lightDirection);
    
    vec3 diffuse = attenuation * diffCoef * max(0.0, dot(lightDirection, norm)) * lightColor * materialColor;
     
    vec3 halfwayDir = normalize(lightDirection + viewDir);  
    vec3 reflectDir = reflect(-lightDirection, norm);
    //float spec = pow(max(dot(norm, halfwayDir), 0.0), shini);    //blin-phlong
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), shini);     //phlong
    vec3 specular =  specCoef * spec * lightColor * materialColor;

    vec3 result = (ambient + diffuse + specular);

    outputColor = vec4(result, 1);
}