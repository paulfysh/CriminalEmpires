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
    class GunRecruiter : NPC
    {
        public GunRecruiter()
        {
            name = "Gun Runners";
            Position = new Vector2(775, 10);
            isAvaliable = false;
            cost = 400;

            heatModifier = 2; // police are not fond of guns

            menuText = "I find a smoking barrel solves most of my problems\nRecruit - Initial Cost - Daily value - currently recruited";

            setupGunRunnerTypes();

            updateMenuOptions();
        }

        private void setupGunRunnerTypes()
        {
            recruitTypes.Add(new recruit("Pistols", 150, 40));
            recruitTypes.Add(new recruit("Shotguns", 200, 50));
            recruitTypes.Add(new recruit("Semi autos", 400, 160));
            recruitTypes.Add(new recruit("Assault Rifles", 800, 300));
        }
    }
}
