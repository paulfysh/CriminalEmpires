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

using System.Collections;

namespace CriminalEmpires
{
    //This is used to display dialog options when you talk to NPCs which is basically the game. 
    class GameMenu
    {
        string text = "";
        ArrayList options = new ArrayList();
        bool displayed;
        int selectedItem;

        public GameMenu()
        {
            displayed = false;
            selectedItem = 0;
        }

        public string menuText
        {
            get { return text; }
            set { text = value; }
        }

        public ArrayList menuOptions
        {
            get 
            {
                if (options.Count > 0)
                {
                    return options;
                }
                else
                {
                    options = new ArrayList();
                    options.Add("Ok");
                    return options;
                }
            }
            set 
            {
                selectedItem = 0;
                options = value;
            }
        }

        public bool isDisplayed
        {
            get { return displayed; }
            set { displayed = value; }
        }

        public int selectedPosition
        {
            get { return selectedItem; }
        }

        public string GetSelection
        {
            get { return (string)options[selectedItem]; }
        }

        public void increaseSelection()
        {
            if (selectedItem < options.Count - 1)
            {
                selectedItem++;
            }
            else
            {
                selectedItem = 0;
            }
        }

        public void decreaseSelection()
        {
            if (selectedItem > 0)
            {
                selectedItem--;
            }
            else
            {
                selectedItem = (options.Count - 1);
            }
        }
    }
}
