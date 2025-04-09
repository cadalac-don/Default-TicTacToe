using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using CommonTicTacToe;


//using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.Diagnostics;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace _2D_TicTacToe;


    /**
    * 3D: Place an Image on the Screen using XNA:
    * ===========================================
    * This sample:
    *      - Uses MonoGame programming...used to be xBox/XNA
    *      - Shows how 3D mesh shapes are placed on the screen
    *      - Too functional, should use more OOP
    *      - myModel class isn't bad, but grid should be a class with it's own draw()
    *      - note enumerations used to show meaning in variables
    *      
    *      
    **/

    public class Game1 : Game
    {
        public static GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }


        // Set the position of the camera in world space, for our view matrix.
        Vector3 cameraPosition = new Vector3(0.0f, 350.0f, 350.0f);


        // List of models to draw
        System.Collections.ArrayList Model_list = new System.Collections.ArrayList();


        // Used to detect the B button being pressed
        ButtonState lastUpdateState;
        ButtonState thisUpdateState;

        // The aspect ratio determines how to scale 3d to 2d projection.
        float aspectRatio;

        // Enumeration defines the type of meshes in the grid spots
        private enum gridVal { X = 0, O = 1, center = 2, dot = 3 };

        // list of player turns, first X, then O
        // currentTurn indexes into turn So, the current 
        // turn is turn[currentTurn], then inc and mod currentTurn
        private gridVal[] turn = { gridVal.X, gridVal.O };

        // Keep track of X's and O's, fill this matrix with gridVal
        private gridVal[,] grid = new gridVal[3, 3];

        // track who won (dot means no winner)
        private gridVal WhoWon;


        // each and any spot my be selected
        private Boolean[,] selection = new Boolean[3, 3];

        // current "cursor" in 2D
        private int[] current = new int[2];

        Boolean lastTimeWasNotSpace = false;
        Boolean lastTimeWasNotUp = false;
        Boolean lastTimeWasNotRight = false;

        /// <summary>
        /// Set the x,y,z grid postion to this value v.
        /// </summary>
        private void setGridPos(gridVal v, int x, int y)
        {
            // The position in grid is same as display position 
            grid[x, y] = v;
        }


        /// <summary>
        /// Set all grid postions to this value, and the center to the
        /// center value.
        /// </summary>
        private void setAllGridPos(gridVal v)
        {

            // Set each mesh spot to that given as input
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    setGridPos(v, i, j);
                }
            }

        }

        private void setAllNotSelected()
        {

            // Set each mesh spot to that given as input
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    selection[i, j] = false;
                }
            }

            //// Make the center position look different, not a fair spot
            //setGridPos(gridVal.center, 1, 1, 1);
        }

        private myModel[] m = new myModel[4];


        protected override void LoadContent()
        {
            Debug.WriteLine("BasicCameraSample LoadContent");

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Flyweight pattern: create one of each mesh, and reuse
            // these 4 meshes in the grid.  
            // load each mesh one at a time, 4 types of mesh objects
            // All mesh objects displayed, will render from one of these 4
            myModel.setupGraphics(_graphics);
            m[(int)gridVal.center] = new myModel(Content.Load<Model>("Models\\Z"), Vector3.Zero, Vector3.Zero, Color.Yellow);
            m[(int)gridVal.X] = new myModel(Content.Load<Model>("Models\\X"), Vector3.Zero, Vector3.Zero, Color.Yellow);
            m[(int)gridVal.O] = new myModel(Content.Load<Model>("Models\\O"), Vector3.Zero, Vector3.Zero, Color.Yellow);
            m[(int)gridVal.dot] = new myModel(Content.Load<Model>("Models\\dot"), Vector3.Zero, Vector3.Zero, Color.Yellow);

            // initialize each grid position to a dot
            this.setAllGridPos(gridVal.dot);
            setAllNotSelected();
            this.current = [0, 0];  // default current cursor

            aspectRatio = (float)_graphics.GraphicsDevice.Viewport.Width /
                            (float)_graphics.GraphicsDevice.Viewport.Height;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Get Keyboard Keystroke
            KeyboardState k = Keyboard.GetState();

            // Act on the keystroke
            if (k.IsKeyDown(Keys.E))
                this.Exit();                                      // E = Exit Game


            // cameraPosition
            if (k.IsKeyDown(Keys.D1))
            {
                cameraPosition = new Vector3(0.0f, 350.0f, 350.0f);
            }
            if (k.IsKeyDown(Keys.D2))
            {
                cameraPosition = new Vector3(50.0f, 450.0f, 450.0f);
            }
            if (k.IsKeyDown(Keys.D3))
            {
                cameraPosition = new Vector3(100.0f, 250.0f, 250.0f);
            }
            if (k.IsKeyDown(Keys.D4))
            {
                cameraPosition = new Vector3(0.0f, -350.0f, 250.0f);
            }




            if ((k.IsKeyDown(Keys.Up)) && lastTimeWasNotUp)
            {
                this.current[1] = ((this.current[1] + 1) % 3);
            }


            if ((k.IsKeyDown(Keys.Right)) && lastTimeWasNotRight)
            {
                this.current[0] = (this.current[0] + 1) % 3;
            }

            if (k.IsKeyDown(Keys.Space) && lastTimeWasNotSpace)
            {
                Debug.WriteLine(" current[0]=" + current[0] + " current[1]=" + current[1]);
                selection[this.current[0], this.current[1]] = !(selection[this.current[0], this.current[1]]);  // Spacebar...selects

                // a cell was selected....process that choice by looping this grid location thru choices
                int eVal = (int)grid[this.current[0], this.current[1]];  // the int value X, O, .. at this cell
                int max_eVal = Enum.GetNames(typeof(gridVal)).Length;
                eVal = (eVal + 1) % max_eVal;  // 4 types of cell 
                grid[this.current[0], this.current[1]] = (gridVal)(eVal);
            }

            lastTimeWasNotUp = !(k.IsKeyDown(Keys.Up));
            lastTimeWasNotRight = !(k.IsKeyDown(Keys.Right));
            lastTimeWasNotSpace = !(k.IsKeyDown(Keys.Space));

            WhoWon = getWhoWon(grid);

            base.Update(gameTime);
        }

        private gridVal getWhoWon(gridVal[,] grid)
        {
            // turn is enum of X, O
            foreach (var player in turn)
            {
                for (int i = 0; i < grid.GetLength(0); i++)
                {
                    // check horizontal
                    if ((grid[i, 0] == player) && (grid[i, 1] == player) && (grid[i, 2] == player))
                    {
                        return player;
                    }

                    // check vertical
                    if ((grid[0, i] == player) && (grid[1, i] == player) && (grid[2, i] == player))
                    {
                        return player;
                    }
                }

                // check crossways
                if ((grid[0, 0] == player) && (grid[1, 1] == player) && (grid[2, 2] == player))
                {
                    return player;
                }

                // check crossways
                if ((grid[2, 0] == player) && (grid[1, 1] == player) && (grid[0, 2] == player))
                {
                    return player;
                }
            }

            // no winner yet 
            return gridVal.dot;
        }

        Vector3 cameraTarget = Vector3.Zero;
        Vector3 cameraUpDirection = Vector3.Up;

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // this loop should be in a grid class
            // at this level we should just see grid.draw()

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {

                    // won becomes true if this is a winner player
                    // if no winner, won is true when this is a dot
                    // again, a grid class should take care of this
                    bool won = ((grid[i, j] != gridVal.dot) && (WhoWon == grid[i, j]));

                    Vector3 location;
                    // x,y,z location is a function of i,j,k grid location
                    // math here is BAD...math should be deep inside a grid class
                    // these locations are a function of the grid
                    location.X = i * 100 - 50.0f;
                    location.Y = j * 100 - 50.0f;
                    location.Z = 1.0f - 1.0f;

                    // Draw the correct mesh on the screen in this location
                    // grid holds the x,y grid of enumerations, convert that 
                    // to int from the enumeration to get corresponding model
                    myModel here = m[(int)grid[i, j]];  // <== which model from grid
                    here.pos = location;

                    // Default color
                    here.color = Color.PaleGreen;

                    // color of X, O
                    if (grid[i, j] == gridVal.X) { here.color = Color.BlueViolet; }
                    if (grid[i, j] == gridVal.O) { here.color = Color.DarkOrange; }

                    // current "cursor" location?
                    if ((i == current[0]) && (j == current[1]))
                    {
                        here.color = Color.Red;
                    }

                    here.draw(cameraPosition, aspectRatio, cameraTarget, cameraUpDirection, won);

                }
            }

            base.Draw(gameTime);
        }
    }
