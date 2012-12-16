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
    class HookerRecruiter : NPC
    {
        public HookerRecruiter()
        {
            name = "Hookers";
            Position = new Vector2(100, 300);
            isAvaliable = false;
            cost = 500;

            heatModifier = 1; //police love the hookers. 

            menuText = "Like some of the ladies do you?\nRecruit - Initial Cost - Daily value - currently recruited";

            setupHookerTypes();

            updateMenuOptions();
        }

        private void setupHookerTypes()
        {
            recruitTypes.Add(new recruit("Street walker", 100, 30));
            recruitTypes.Add(new recruit("Brothel girl", 150, 40));
            recruitTypes.Add(new recruit("Dominatrix", 350, 90));
            recruitTypes.Add(new recruit("Escort", 1000, 400));
        }
    }
}
