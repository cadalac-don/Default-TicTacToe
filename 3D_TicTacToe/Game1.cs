using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using CommonTicTacToe;

//using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.Diagnostics;
using static _3D_TicTacToe.Grid;

namespace _3D_TicTacToe
{

    public class Game1 : Game
    {
        public static GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            //set the GraphicsDeviceManager's fullscreen property
            _graphics.IsFullScreen = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

        }

        // List of models to draw
        System.Collections.ArrayList Model_list = new System.Collections.ArrayList();

        // Grid object contains 3x3 array of grid cells
        Grid grid = new Grid();


        // The current model to change, of the List
        //int current = 0;

        // Used to detect change in trigger being pressed
        Boolean gp1TriggerState = false;
        Boolean gp2TriggerState = false;

        // The aspect ratio determines how to scale 3d to 2d projection.
        float aspectRatio;

        // these objects hold the current selection of the gamepad
        GamePadSelectionStatus GP1, GP2;


        protected override void LoadContent()
        {
            Debug.WriteLine("BasicCameraSample LoadContent");

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            grid.loadModels(Content);

            GP1 = new GamePadSelectionStatus();
            GP2 = new GamePadSelectionStatus();


            aspectRatio = (float)_graphics.GraphicsDevice.Viewport.Width /
                            (float)_graphics.GraphicsDevice.Viewport.Height;

            // model needs graphics device to muck with color
            CommonTicTacToe.myModel.setupGraphics(_graphics);

        }

        // state of keyboard to detect key hit
        Boolean lastTimeWasNotSpace = false;
        Boolean lastTimeWasNotUp = false;
        Boolean lastTimeWasNotDown = false;
        Boolean lastTimeWasNotRight = false;
        Boolean lastTimeWasNotLeft = false;
        Boolean lastTimeWasNotPageUp = false;
        Boolean lastTimeWasNotPageDown = false;

        // Mouse
        public static bool LeftClicked = false;
        private static MouseState ms = new MouseState(), oms;
        bool CameraMoving = false;
        int CameraMovingCount = 0;

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            // Mouse state
            oms = ms;
            ms = Mouse.GetState();
            LeftClicked = ms.LeftButton != ButtonState.Pressed && oms.LeftButton == ButtonState.Pressed;
            int mX = ms.X;
            int mY = ms.Y;

            if (LeftClicked) {
                CameraMovingCount = 200; // toggle camera moving on left click
            }

            if (CameraMovingCount > 0)
            {
                cameraPosition.X = 300 + mX;
                cameraPosition.Y = 300 + mY;
                CameraMovingCount = CameraMovingCount - 1;
            }

            // Keyboared state
            KeyboardState k = Keyboard.GetState();

            if (k.IsKeyDown(Keys.E))
                this.Exit();                                      // E = Exit Game

            // cameraPosition
            if (k.IsKeyDown(Keys.D1))
            {
                cameraPosition = new Vector3(-300.0f, 450.0f, 450.0f);
            }
            if (k.IsKeyDown(Keys.D2))
            {
                cameraPosition = new Vector3(-200.0f, 450.0f, 450.0f);
            }
            if (k.IsKeyDown(Keys.D3))
            {
                cameraPosition = new Vector3(-100.0f, 450.0f, 450.0f);
            }
            if (k.IsKeyDown(Keys.D4))
            {
                cameraPosition = new Vector3(0.0f, -450.0f, 450.0f);
            }



            // process player play
            if ((k.IsKeyDown(Keys.Up)) && lastTimeWasNotUp)
            {
                grid.currentGP1.SelectX = ((grid.currentGP1.SelectX + 1) % 3);
            }
            // process player play
            if ((k.IsKeyDown(Keys.Down)) && lastTimeWasNotDown)
            {
                grid.currentGP1.SelectX = Math.Abs((grid.currentGP1.SelectX - 1) % 3);
            }

            if ((k.IsKeyDown(Keys.Right)) && lastTimeWasNotRight)
            {
                grid.currentGP1.SelectY = ((grid.currentGP1.SelectY + 1) % 3);
            }
            if ((k.IsKeyDown(Keys.Left)) && lastTimeWasNotLeft)
            {
                grid.currentGP1.SelectY = Math.Abs((grid.currentGP1.SelectY - 1) % 3);
            }

            if ((k.IsKeyDown(Keys.PageUp)) && lastTimeWasNotPageUp)
            {
                grid.currentGP1.SelectZ = ((grid.currentGP1.SelectZ + 1) % 3);
            }
            if ((k.IsKeyDown(Keys.PageDown)) && lastTimeWasNotPageDown)
            {
                grid.currentGP1.SelectZ = Math.Abs((grid.currentGP1.SelectZ - 1) % 3);
            }

            // use keyboard space to toggle player 1
            if (k.IsKeyDown(Keys.Space) && lastTimeWasNotSpace)
            {
                Debug.WriteLine(" current[0]=" + grid.currentGP1.SelectX +
                    " current[1]=" + grid.currentGP1.SelectY +
                    " current[2]=" + grid.currentGP1.SelectZ);

                grid.toggleCellValue(grid.currentGP1);  // select current cell
            }

            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                // get gamepad states
                GamePadState gp1 = GamePad.GetState(PlayerIndex.One);
                Grid.Current gp1_current = GP1.getGamePadCurrent(GamePad.GetState(PlayerIndex.One));

                //Debug.WriteLine("gp=" + GamePad.GetState(PlayerIndex.One).Triggers.Right);
                // player 1 Change current grid location
                if ((gp1TriggerState == false) && (GamePad.GetState(PlayerIndex.One).Triggers.Right == 1))
                {
                    grid.toggleCellValue(gp1_current);  // player 1 = game pad 1
                }
                gp1TriggerState = (GamePad.GetState(PlayerIndex.One).Triggers.Right == 1);
            }

            if (GamePad.GetState(PlayerIndex.Two).IsConnected)
            {
                // get gamepad states
                GamePadState gp2 = GamePad.GetState(PlayerIndex.Two);
                Grid.Current gp2_current = GP2.getGamePadCurrent(GamePad.GetState(PlayerIndex.Two));

                // player 2 Change current grid location
                if ((gp2TriggerState == false) && (GamePad.GetState(PlayerIndex.Two).Triggers.Right == 1))
                {
                    grid.toggleCellValue(gp2_current);  // player 1 = game pad 1
                }
                gp2TriggerState = (GamePad.GetState(PlayerIndex.Two).Triggers.Right == 1);
            }

            // determine winner
            grid.WhoWon = grid.GetWhoWon();
            if ((grid.WhoWon == gridVal.X) || (grid.WhoWon == gridVal.X))
            {
                Debug.WriteLine(" Winner=" + grid.WhoWon);
            }

            // Set flags for last keystroke
            lastTimeWasNotUp = !(k.IsKeyDown(Keys.Up));
            lastTimeWasNotRight = !(k.IsKeyDown(Keys.Right));
            lastTimeWasNotPageUp = !(k.IsKeyDown(Keys.PageUp));
            lastTimeWasNotSpace = !(k.IsKeyDown(Keys.Space));
            lastTimeWasNotDown = !(k.IsKeyDown(Keys.Down));
            lastTimeWasNotLeft = !(k.IsKeyDown(Keys.Left));
            lastTimeWasNotPageDown = !(k.IsKeyDown(Keys.PageDown));

            base.Update(gameTime);
        }

        // Set the position of the camera in world space, for our view matrix.
        Vector3 cameraPosition = new Vector3(-200.0f, 450.0f, 450.0f);
        Vector3 cameraTarget = Vector3.Zero;
        Vector3 cameraUpDirection = Vector3.Up;


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            grid.Draw(cameraPosition, aspectRatio, cameraTarget, cameraUpDirection);

            base.Draw(gameTime);
        }
    }
}
