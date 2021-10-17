using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;


namespace Template
{
    //mesh hierarchy
    class SceneGraph
    {
        MyApplication App;
        public SceneGraph(MyApplication app) 
        {
            App = app;
        }

        public void renderGraph(Matrix4 camera, Node node, Matrix4 parentMatrix)
        {
            Matrix4 objectMatrix = node.modelMatrix;  //To world Space
            Matrix4 transform = objectMatrix * camera;
            Vector3 cameraLocation = camera.ExtractTranslation();
            if (node is SceneNode)
            {
                if (App.useRenderTarget)
                {
                    // enable render target
                    App.target.Bind();

                    // render scene to render target
                    node.mesh.Render(node.shader, transform, objectMatrix, node.texture, cameraLocation, node.Diffuse, node.Specular, node.Shininess);

                    // render quad
                    App.target.Unbind();
                    App.quad.Render(App.postproc, App.target.GetTextureID());
                }
                else
                {
                    // render scene directly to the screen
                    node.mesh.Render(node.shader, transform, objectMatrix, node.texture, cameraLocation, node.Diffuse, node.Specular, node.Shininess);
                    
                }
            }
            
            foreach (Node childNode in node.children)
            {
                renderGraph(camera, childNode, objectMatrix);
            }
        }
    }
}
