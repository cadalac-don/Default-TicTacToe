using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using CommonTicTacToe;

namespace _3D_TicTacToe
{
    internal class Grid
    {


        // Enumeration defines the type of meshes in the grid spots
        public enum gridVal { X = 0, O = 1, center = 2, dot = 3 };

        // list of player turns, first X, then O
        // currentTurn indexes into turn So, the current 
        // turn is turn[currentTurn], then inc and mod currentTurn
        public gridVal[] turn = { gridVal.X, gridVal.O };
        gridVal whoseTurn;

        // Keep track of X's and O's, fill this matrix with gridVal
        public gridVal[,,] grid = new gridVal[3, 3, 3];

        // track who won (dot means no winner)
        public gridVal WhoWon;


        // each and any spot my be selected
        public Boolean[,,] selection = new Boolean[3, 3, 3];

        // current "cursor" in 3D
        //        public int[] current = new int[3];
        public struct Current
        {
            private int selectX;  // init grid selection
            private int selectY;
            private int selectZ;

            public int SelectX { get => selectX; set => selectX = value; }
            public int SelectY { get => selectY; set => selectY = value; }
            public int SelectZ { get => selectZ; set => selectZ = value; }
        };
        public Current currentGP1;
        public Current currentGP2;


        public Grid()
        {

            // initialize each grid position to a dot
            this.setAllGridPos(gridVal.dot);
            setAllNotSelected();
            //currentGP1 =  // no defalt for structs...always zero 
            //currentGP2 =


            // X starts
            whoseTurn = gridVal.X;
        }


        // Flyweight models
        private myModel[] m = new myModel[4];
        internal void loadModels(ContentManager content)
        {
            // Flyweight pattern: create one of each mesh, and reuse
            // these 4 meshes in the grid.  
            // load each mesh one at a time, 4 types of mesh objects
            // All mesh objects displayed, will render from one of these 4
            m[(int)gridVal.center] = new myModel(content.Load<Model>("Models\\Z"), Vector3.Zero, Vector3.Zero, Color.Yellow);
            m[(int)gridVal.X] = new myModel(content.Load<Model>("Models\\X"), Vector3.Zero, Vector3.Zero, Color.Purple);
            m[(int)gridVal.O] = new myModel(content.Load<Model>("Models\\O"), Vector3.Zero, Vector3.Zero, Color.White);
            m[(int)gridVal.dot] = new myModel(content.Load<Model>("Models\\Z"), Vector3.Zero, Vector3.Zero, Color.Brown);
        }

        public void toggleCellValue(int x, int y, int z)
        {
            // toggle current location
            selection[x, y, z] = !(selection[x, y, z]);

            // a cell was selected....process that choice
            int eVal = (int)grid[x, y, z];  // the int value X, O, .. at this cell
            int max_eVal = Enum.GetNames(typeof(gridVal)).Length;
            eVal = (eVal + 1) % max_eVal;  // 4 types of cell enum
            grid[x, y, z] = (gridVal)(eVal);  // grid location is an enum of possible (x,0,dot...)
        }

        public void toggleCellValue(Current gp1_current)
        {

            // location of the grid cell
            int x = gp1_current.SelectX;
            int y = gp1_current.SelectY;
            int z = gp1_current.SelectZ;

            // toggle current location
            selection[x, y, z] = !(selection[x, y, z]);

            // a cell was selected....process that choice
            int eVal = (int)grid[x, y, z];  // the int value X, O, .. at this cell
            int max_eVal = Enum.GetNames(typeof(gridVal)).Length;
            eVal = (eVal + 1) % max_eVal;  // 4 types of cell enum
            grid[x, y, z] = (gridVal)(eVal);  // grid location is an enum of possible (x,0,dot...)
        }

        /// <summary>
        /// Set the x,y,z grid postion to this value v.
        /// </summary>
        public void setGridPos(gridVal v, int i, int j, int k)
        {
            // The position in grid is same as display position 
            grid[i, j, k] = v;
        }


        /// <summary>
        /// Set all grid postions to this value, and the center to the
        /// center value.
        /// </summary>
        public void setAllGridPos(gridVal v)
        {

            // Set each mesh spot to that given as input
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        setGridPos(v, i, j, k);
                    }
                }
            }

            //// Make the center position look different, not a fair spot
            setGridPos(gridVal.center, 1, 1, 1);
        }

        public void setAllNotSelected()
        {

            // Set each mesh spot to that given as input
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        selection[i, j, k] = false;
                    }
                }
            }

        }


        // not all winning sequences are coded in yet
        public gridVal GetWhoWon()
        {
            // turn is enum of X, O
            foreach (var player in turn)
            {
                for (int i = 0; i < grid.GetLength(0); i++)
                {
                    // check horizontal
                    if ((grid[i, 0, 0] == player) && (grid[i, 1, 0] == player) && (grid[i, 2, 0] == player))
                    {
                        return  player;
                    }

                    // check vertical
                    if ((grid[0, i, 0] == player) && (grid[1, i, 0] == player) && (grid[2, i, 0] == player))
                    {
                        return player;
                    }
                }

                // check crossways
                if ((grid[0, 0, 0] == player) && (grid[1, 1, 0] == player) && (grid[2, 2, 0] == player))
                {
                    return player;
                }

                // check crossways
                if ((grid[2, 0, 0] == player) && (grid[1, 1, 0] == player) && (grid[0, 2, 0] == player))
                {
                    return player;
                }
            }

            // no winner yet 
            return gridVal.dot;
        }

        public void Draw(Vector3 cameraPosition, float aspectRatio, Vector3 cameraTarget, Vector3 cameraUpDirection)
        {

            // Set each mesh spot to that given as input
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        // draw cell item

                        // won becomes true if this is a winner player
                        // if no winner, won is true when this is a dot
                        // again, a grid class should take care of this
                        bool isThereaWinner = ((WhoWon == gridVal.X) || (WhoWon == gridVal.O));

                        Vector3 location;
                        // x,y,z location is a function of i,j,k grid location
                        // math should be inside a grid class
                        // these locations are a function of the grid
                        location.X = i * 100 - 50.0f;
                        location.Y = j * 100 - 50.0f;
                        location.Z = k * 100 - 50f;

                        // Draw the correct mesh on the screen in this location
                        // grid holds the x,y,z grid of enumerations, convert that 
                        // to int from the enumeration to get corresponding model
                        gridVal hereGridVal = grid[i, j, k];  // x,o, whatever
                        myModel here = m[(int)grid[i, j, k]];  // <== magic happens
                        here.pos = location;
                        //bool selected = selection[i, j, k];

                        // Default color
                        here.color = Color.PaleGreen;

                        // color of X, O
                        if (hereGridVal == gridVal.X) { here.color = Color.BlueViolet; }
                        if (hereGridVal == gridVal.O) { here.color = Color.DarkOrange; }

                    
                        // current location of player 1
                        if ((i == currentGP1.SelectX) && (j == currentGP1.SelectY) && (k == currentGP1.SelectZ))
                        {
                            // this is current location, special color
                            here.setColor(Color.WhiteSmoke);
                        }

                        if (isThereaWinner && (hereGridVal == WhoWon)) { 
                            here.setColor(Color.Navy);
                        }

                        //// current location of player 2
                        //if ((i == currentGP1.SelectX) && (j == currentGP1.SelectY) && (k == currentGP1.SelectZ))
                        //{
                        //    // this is current location, special color
                        //    here.setColor(Color.Cyan);
                        //}

                        here.draw(cameraPosition, aspectRatio, cameraTarget, cameraUpDirection);

                    }
                }
            }

        }

    }
}
