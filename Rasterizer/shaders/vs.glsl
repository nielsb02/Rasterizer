  #version 330
 
// shader input
in vec2 vUV;			// vertex uv coordinate
in vec3 vNormal;		// untransformed vertex normal in model space
in vec3 vPosition;		// untransformed vertex position

// shader output
out vec4 normal;		// transformed vertex normal
out vec4 worldPos;
out vec2 uv;				

uniform mat4 transform; // should be model space to world space, bc thats where lightPosition will be?
uniform mat4 toWorld;
 
// vertex shader
void main()
{
	// transform vertex using supplied matrix
	gl_Position = transform * vec4(vPosition, 1.0);

	// forward normal and uv coordinate; will be interpolated over triangle
	worldPos = toWorld * vec4( vPosition, 1.0f );
	normal = toWorld * vec4( vNormal, 0.0f );
	uv = vUV;
}