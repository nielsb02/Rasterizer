using System;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.ES20;

namespace Template
{
    internal class MyApplication
    {
        // member variables
        public Surface screen;                  // background surface for printing etc.

        private const float PI = 3.1415926535f;         // PI
        private float a = 0, tick = 0;                            // teapot rotation angle
        private Stopwatch timer;                        // timer for measuring frame duration
        private Shader shader;                          // shader to use for rendering
        public Shader postproc;                        // shader to use for post processing
        private Texture wood;                           // texture to use for rendering
        public RenderTarget target;                    // intermediate render target
        public ScreenQuad quad;                        // screen filling quad for post processing
        private SceneGraph graph;
        public bool useRenderTarget = false;
        private float angle90degrees = PI / 2;
        private float dist;
        private SceneNode teapotNode, floorNode, airplane1;
        private WorldNode world;
        private Matrix4 Tcamera;
        private Vector3 cameraR;
        private Vector3 cameraP;

        // initialize
        public void Init()
        {
            // initialize stopwatch
            timer = new Stopwatch();
            timer.Reset();
            timer.Start();

            // create shaders
            shader = new Shader("../../shaders/vs.glsl", "../../shaders/fs.glsl");
            postproc = new Shader("../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl");
            // load a texture
            wood = new Texture("../../assets/wood.jpg");
            Texture brick = new Texture("../../assets/wallTextures/base.png");
            Texture mountain = new Texture("../../assets/grassTexture.jpg");
            Mesh Wall = new Mesh("../../assets/wall.obj");
            // create the render target
            target = new RenderTarget(screen.width, screen.height);
            quad = new ScreenQuad();

            // load scene
            graph = new SceneGraph(this);
            world = new WorldNode();

            SceneNode grass = new SceneNode(world, new Mesh("../../assets/floor.obj"), mountain, shader, 8.0f, Matrix4.Identity, Matrix4.CreateTranslation(0.0f, 10.0f, 0.0f), 1.0f, 0.7f, 900);
            SceneNode road = new SceneNode(grass, new Mesh("../../assets/untitled.obj"), new Texture("../../assets/roadText.jpg"), shader, 0.5f, Matrix4.CreateRotationY(angle90degrees), Matrix4.CreateTranslation(20.0f, -14.0f, 0.0f), 1.0f, 0.3f, 600);
            SceneNode hangar = new SceneNode(grass, new Mesh("../../assets/hangarObj.obj"), new Texture("../../assets/whiteSteel.jpg"), shader, 0.12f, Matrix4.Identity, Matrix4.CreateTranslation(-20.0f, -2.5f, 10.0f), 1.0f, 0.2f, 750);
            SceneNode airplane2 = new SceneNode(road, new Mesh("../../assets/airplane.obj"), wood, shader, 0.005f, Matrix4.CreateRotationZ(angle90degrees), Matrix4.CreateTranslation(-20.0f, -4.4f, 0.0f), 1.0f, 0.7f, 100);
            airplane1 = new SceneNode(road, new Mesh("../../assets/piper_pa18.obj"),
                new Texture("../../assets/piper_diffuse.jpg"), shader, 0.5f, Matrix4.CreateRotationY(angle90degrees),
                Matrix4.CreateTranslation(17.5f, 0.0f, 20f), 1.0f, 0.6f, 750);
            SceneNode controltower = new SceneNode(grass, new Mesh("../../assets/controlTower.obj"), new Texture("../../assets/concrete.png"), shader, 2f, Matrix4.Identity, Matrix4.CreateTranslation(30f, -25f, -40f), 1.0f, 0.4f, 400);
            SceneNode helicopter = new SceneNode(grass, new Mesh("../../assets/helicopter.obj"), new Texture("../../assets/piper_diffuse.jpg"), shader, 0.02f, Matrix4.CreateRotationY(angle90degrees), Matrix4.CreateTranslation(40f, 10f, 10f), 1.0f, 0.4f, 400);
            cameraP = new Vector3(new Vector3(15.0f, -5.0f, -50.0f));
            cameraR = new Vector3(0, 0, 0);
            Tcamera = Matrix4.CreateTranslation(cameraP);
            dist = 0.0f;
        }

        // tick for background surface
        public void Tick()
        {
            screen.Clear(0);
        }

        // tick for OpenGL rendering code
        public void RenderGL()
        {
            // measure frame duration
            float frameDuration = timer.ElapsedMilliseconds;
            timer.Reset();
            timer.Start();

            // prepare matrix for vertex shader
            Matrix4 Tview = Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);

            Vector3 tempLocation = airplane1.translationMatrix.ExtractTranslation();
            if (tick < 100)
            {
                airplane1.translationMatrix = Matrix4.CreateTranslation(tempLocation.X, tempLocation.Y,
                    (float)(tempLocation.Z - tick / 100));
                airplane1.setModelMatrix();
            }
            else if (tempLocation.Y < 50)
            {
                airplane1.translationMatrix = Matrix4.CreateTranslation(tempLocation.X, tempLocation.Y + tick / 300,
                    (float)(tempLocation.Z - tick / 100));
                airplane1.setModelMatrix();
            }
            else if (tempLocation.Y > 50 && dist == 0.0f)
            {
                airplane1.translationMatrix = Matrix4.CreateTranslation(tempLocation.X, tempLocation.Y + 1.0f,
                    (float)(tempLocation.Z - tick / 100));
                airplane1.setModelMatrix();
                dist = (float)Math.Sqrt(tempLocation.Z * tempLocation.Z + tempLocation.X * tempLocation.X);
                a = (float)Math.Acos(tempLocation.Z / dist);
            }
            else
            {
                airplane1.translationMatrix = Matrix4.CreateTranslation((dist * (float)Math.Sin(a)), tempLocation.Y,
                    (dist * (float)Math.Cos(a)));
                airplane1.setModelMatrix();
            }

            // creates the cameraTranslation matrix, by first rotating (on its own axis) and afterwards translating its location.
            Matrix4 Rx = Matrix4.CreateRotationX(cameraR.X * angle90degrees);
            Matrix4 Ry = Matrix4.CreateRotationY(cameraR.Y * angle90degrees);
            Matrix4 Rz = Matrix4.CreateRotationZ(cameraR.Z * angle90degrees);
            Matrix4 Translation = Matrix4.CreateTranslation(cameraP);
            Tcamera = Rx * Ry * Rz * Translation;

            foreach (Node child in world.children)
            {
                graph.renderGraph(Tcamera * Tview, child, Matrix4.Identity);
            }

            // update rotation
            tick++;
            a += 0.001f * frameDuration;
            if (a > 2 * PI) a -= 2 * PI;
        }

        public void moveCam(float x, float y, float z, float angle)
        {
            Vector3 temp = new Vector3(x, y, z);
            if (angle != 0.0f)
            {
                temp *= angle;
                cameraR += temp;
            }
            else
            {
                cameraP += temp;
            }
        }
    }
}