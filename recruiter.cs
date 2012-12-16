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
    class recruiter : NPC
    {
        ArrayList NPCList;
        public recruiter(ArrayList npcs)
        {
            NPCList = npcs;
            name = "Recruiter";

            initialText = "Hey there, welcome to criminal empires, you are here to start running this outfit, you need to start making money and eventually we need you to take over this city. Talk to me again when you are ready to start\nTo win you need to recruit specialists and a job seeker to allow you to take control of the city, but you will need a solid income before you can acomplish that, so start with some hookers, gun runners or drug dealers with some muscle to protect them.";
            menuText = "Hi boss, who sort of help do you need to start recruiting now?";

            updateMenuOptions(npcs);

            /*menuOptions.Add("Drug Dealers");
            menuOptions.Add("Gun Runners");
            menuOptions.Add("Muscle");
            menuOptions.Add("Specialists");
            menuOptions.Add("Hookers");
            menuOptions.Add("Job Seeker");*/
            //menuOptions.Add("Ok");

            Position = new Vector2(320, 800);
            isAvaliable = true;
        }

        public void updateMenuOptions(ArrayList npcs)
        {
            menuOptions.Clear();
            foreach (NPC currNPC in npcs)
            {
                if (!currNPC.isAvaliable)
                {
                    menuOptions.Add(currNPC.getName + " - $" + currNPC.getCost());
                }
            }

            menuOptions.Add("Ok");
        }

        public override bool processSelectedItem(string selection, Player playerObject)
        {
            string Name = selection.Substring(0, selection.IndexOf("-") - 1);
            bool success = false;

            if (Name == "Drug Dealers")
            {
                success = playerObject.unlockNPC(Player.NPCType.drug);
            }
            else if (Name == "Gun Runners")
            {
                success = playerObject.unlockNPC(Player.NPCType.guns);
            }
            else if (Name == "Muscle")
            {
                success = playerObject.unlockNPC(Player.NPCType.muscle);
            }
            else if (Name == "Specialists")
            {
                success = playerObject.unlockNPC(Player.NPCType.specialists);
            }
            else if (Name == "Hookers")
            {
                success = playerObject.unlockNPC(Player.NPCType.hookers);
            }
            else if (Name == "Job Seeker")
            {
                success = playerObject.unlockNPC(Player.NPCType.jobs);
            }

            updateMenuOptions(playerObject.getNPCs());

            return success;

        }
    }
}
