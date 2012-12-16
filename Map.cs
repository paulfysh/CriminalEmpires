using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CriminalEmpires
{
    class Map
    {
        Rectangle currentlyDisplayedRectangle;
        int displayWidth;
        int displayHeight;
        Texture2D mapTexture;
        bool playerPositionCentered;

        public Map(Texture2D tex, int screenWidth, int screenHeight, Vector2 playerPosition)
        {
            playerPositionCentered = true;
            displayWidth = screenWidth;
            displayHeight = screenHeight;
            //currentlyDisplayedRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            mapTexture = tex;
            currentlyDisplayedRectangle = new Rectangle(0, 0, tex.Width, tex.Height);

            updateDisplayRegion(playerPosition);
        }

        public void updateDisplayRegion(Vector2 playerPosition)
        {
            //we want the player to be in the middle, so the displayed X should be player x - 1/2 screen width. Same for y
            int xBoundary = ((int)playerPosition.X - (displayWidth / 2));
            int yBoundary = ((int)playerPosition.Y - (displayHeight / 2));
            playerPositionCentered = true;

            if(xBoundary < 0)
            {
                xBoundary = 0;
                playerPositionCentered = false;
            }

            if(yBoundary < 0)
            {
                yBoundary = 0;
                playerPositionCentered = false;
            }

            if(xBoundary + displayWidth > mapTexture.Width)
            {
                xBoundary = (mapTexture.Width - displayWidth);
                playerPositionCentered = false;
            }

            if(yBoundary + displayHeight > mapTexture.Height)
            {
                yBoundary = (mapTexture.Height - displayHeight);
                playerPositionCentered = false;
            }

            currentlyDisplayedRectangle.X = xBoundary;
            currentlyDisplayedRectangle.Y = yBoundary;

            //Note this can cause the player character to be drawn in an incorrect position, need to sort that.
        }

        public Rectangle getDisplayRectangle()
        {
            return currentlyDisplayedRectangle;
        }

        public bool playerIsCentered()
        {
            return playerPositionCentered;
        }
    }
}
