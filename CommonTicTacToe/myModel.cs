using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace CommonTicTacToe
{
    public class myModel
    {
        // Contain the mesh, position and rotation for this object
        public Model m;
        public Vector3 pos;
        public Vector3 vel;
        public Vector3 rot;
        public Vector3 rot_vel;
        public Color color;

        static GraphicsDeviceManager _graphics;

        // Constructor, set up the shape
        public myModel(Model m)
        {
            this.m = m;
            pos = Vector3.Zero;
            vel = Vector3.Zero;
            rot = Vector3.Zero;
            color = Color.Blue;
        }


        // Constructor, set up the shape
        public myModel(Model m, Vector3 pos, Vector3 vel, Color color)
        {
            this.m = m;
            this.pos = pos;
            this.vel = vel;
            this.rot = Vector3.Zero;
            this.color = color;
        }

        // Constructor, set up the shape
        public myModel(Model m, Color color)
        {
            this.m = m;
            pos = Vector3.Zero;
            vel = Vector3.Zero;
            rot = Vector3.Zero;
            this.color = color;
        }

        public static void setupGraphics(GraphicsDeviceManager _graphics)
        {
            // one and only instance of graphics screen
            myModel._graphics = _graphics;
        }

        public void setColor(Color color)
        {
            this.color = color;
        }

        Random random = new Random();
        public void SetRND_Velocity()
        {
            // make a ranom vector at set it to the model velocity
            Vector3 rnd = new Vector3();
            rnd.X = (float)random.NextDouble() * 2;
            rnd.Y = (float)random.NextDouble() * 2;
            rnd.Z = (float)random.NextDouble() * 2;
            vel = rnd;

            // make a ranom vector at set it to the model velocity
            rnd = new Vector3();
            rnd.X = (float)random.NextDouble() / 10;
            rnd.Y = (float)random.NextDouble() / 10;
            rnd.Z = (float)random.NextDouble() / 10;
            rot_vel = rnd;

        }

        public void Move()
        {
            // max on all sides
            float max = 200;

            // 3D movement
            pos = pos + vel;

            if (Math.Abs(pos.X) > max)
            {
                vel.X = -vel.X;
            }
            if (Math.Abs(pos.Y) > max)
            {
                vel.Y = -vel.Y;
            }
            if (Math.Abs(pos.Z) > max)
            {
                vel.Z = -vel.Z;
            }

            rot = rot + rot_vel;

        }

        // https://stackoverflow.com/questions/32429219/procedurally-generating-a-texture2d-in-xna-monogame
        // but the delegate in stackoverflow isn't needed ... just pass in the paint color
        // Set the Color of the Model
        public static Texture2D CreateTexture(int width, int height, Color paint)
        {
            //initialize a texture
            Texture2D texture = new Texture2D(_graphics.GraphicsDevice, width, height);

            //the array holds the color for each pixel in the texture
            Color[] data = new Color[width * height];
            for (int pixel = 0; pixel < data.Count(); pixel++)
            {
                //the function applies the color according to the specified pixel
                data[pixel] = paint;
            }

            //set the color
            texture.SetData(data);

            return texture;
        }


        // default won to false
        public void draw(Vector3 cameraPosition, float aspectRatio, Vector3 cameraTarget, Vector3 cameraUpDirection, bool won = false)
        {

            // Settings of each iteration through the models
            Model Models = m;
            Vector3 modelPosition = this.pos;
            Vector3 modelRotation = rot;

            Texture2D x = m.Meshes[0].MeshParts[0].Effect.Parameters["Texture"].GetValueTexture2D();

            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[Models.Bones.Count];
            Models.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in Models.Meshes)
            {
                // This is where the mesh orientation is set, as well as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index]
                        * Matrix.CreateRotationY(modelRotation.Y)
                        * Matrix.CreateRotationX(modelRotation.X)
                        * Matrix.CreateTranslation(modelPosition);
                    effect.View = Matrix.CreateLookAt(cameraPosition, cameraTarget, cameraUpDirection);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
                        aspectRatio, 1.0f, 10000.0f);

                    // need texture enabled to change color
                    effect.TextureEnabled = true; // sorta blue-ish
                    effect.Texture = CreateTexture(64, 64, color);

                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }

        }
    }
}
