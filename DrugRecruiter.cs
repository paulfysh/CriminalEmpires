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
    class DrugRecruiter : NPC
    {
        public DrugRecruiter()
        {
            name = "Drug Dealers";
            Position = new Vector2(430, 10);
            isAvaliable = false;
            cost = 400;

            heatModifier = 3; // police hate drugs. 

            menuText = "High, low, any way you want to go. \nRecruit - Initial Cost - Daily value - currently recruited";

            setupDealerTypes();

            updateMenuOptions();
        }

        private void setupDealerTypes()
        {
            recruitTypes.Add(new recruit("Junkie", 50, 60));
            recruitTypes.Add(new recruit("Pusher", 100, 120));
            recruitTypes.Add(new recruit("Trafficker", 200, 240));
            recruitTypes.Add(new recruit("Distributer", 500, 400));
        }
    }
}
