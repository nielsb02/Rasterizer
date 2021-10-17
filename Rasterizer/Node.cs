using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
    class Node
    {
        public List<SceneNode> children;
        public Matrix4 modelMatrix = Matrix4.Zero;
        public Node parent = null;
        public Mesh mesh;
        public Texture texture;
        public Shader shader;
        public Matrix4 translationMatrix;
        public Matrix4 scaleMatrix;
        public Matrix4 rotationMatrix;
        public float Specular;
        public float Diffuse;
        public int Shininess;

        public Node()
        {
            children = new List<SceneNode>();
        }
    }

    class WorldNode : Node
    {
    }

    class SceneNode : Node
    {    
        public SceneNode(Node _parent, Mesh _mesh, Texture _texture, Shader _shader, float _scale, Matrix4 _rotationMatrix, Matrix4 _translationMatrix, float diffuse, float spec, int shini)
        {        
            mesh = _mesh;
            texture = _texture;
            shader = _shader;
            Diffuse = diffuse;
            Specular = spec;
            Shininess = shini;

            if (_scale != 0.0)
                scaleMatrix = Matrix4.CreateScale(_scale);
            else
                scaleMatrix =  Matrix4.Identity;
            rotationMatrix = _rotationMatrix;
            translationMatrix = _translationMatrix;

            if (_parent != null)
            {
                parent = _parent;
                parent.children.Add(this);
            }
            setModelMatrix();
        }
        
        public void setModelMatrix()
        {
            modelMatrix = scaleMatrix * rotationMatrix * translationMatrix;
            if (parent != null && parent.modelMatrix != Matrix4.Zero)
            {
                modelMatrix = parent.modelMatrix * modelMatrix;
            }
        }

    }
}
