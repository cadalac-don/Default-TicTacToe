using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace _3D_TicTacToe
{
    internal class GamePadSelectionStatus
    {

        // current selection of this gamepad
        Grid.Current localCurrent = new Grid.Current();

        // allow push of current from source other than gamepad
        public void setGPcurrent(int x, int y, int z) {
            localCurrent.SelectX = x;
            localCurrent.SelectY = y;
            localCurrent.SelectZ = z;
        }

        public Grid.Current getGamePadCurrent(in GamePadState gpState)
        {

            // Get rotation from thumbstick
            float thumbLeftX = gpState.ThumbSticks.Left.X;
            float thumbLeftY = gpState.ThumbSticks.Left.Y;
            float thumbRightX = gpState.ThumbSticks.Right.X;
            float thumbRightY = gpState.ThumbSticks.Right.Y;

            // get DPad state
            ButtonState dPadUp = gpState.DPad.Up;
            ButtonState dPadDown = gpState.DPad.Down;

            if (thumbRightX < -0.1f) { localCurrent.SelectZ = 0; }
            else if (thumbRightX < +0.1f) { localCurrent.SelectZ = 1; }
            else { localCurrent.SelectZ = 2; }

            if (thumbRightY < -0.1f) { localCurrent.SelectY = 0; }
            else if (thumbRightY < +0.1f) { localCurrent.SelectY = 1; }
            else { localCurrent.SelectY = 2; }

            //if (thumbLeftX < -0.1f) { selectZ = 0; }
            //else if (thumbLeftX < +0.1f) { selectZ = 1; }
            //else { selectZ = 2; }

            if (dPadUp == ButtonState.Pressed) { localCurrent.SelectX = 0; }
            else if (dPadDown == ButtonState.Pressed) { localCurrent.SelectX = 2; }
            else {  localCurrent.SelectX = 1; }

            // current x,y,z is returned, but also kept local
            return localCurrent;
        }

    }
}
