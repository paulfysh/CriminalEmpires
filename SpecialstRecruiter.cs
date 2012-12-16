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
    class SpecialstRecruiter : NPC
    {
        recruit pimp;
        recruit hitman;
        recruit bodyguard;
        recruit policeBriber;
        //specialists are to deal with jobs when those are implemented, as well as give boosts. 
        public SpecialstRecruiter()
        {
            name = "Specialists";
            Position = new Vector2(100, 590);
            isAvaliable = false;
            cost = 5000;

            heatModifier = 0; // heat is handled uniquely for these guys. 

            menuText = "I see you need some \"specialist\" help.";

            setupSpecialistTypes();
            updateMenuOptions();
        }

        private void setupSpecialistTypes()
        {
            //dont really fit the recruit model as they are all special cases. 
            pimp = new recruit("Pimp", 500, 3);
            hitman = new recruit("Hitman", 1000, -2000);
            bodyguard = new recruit("Bodyguard", 3000, -1000);
            policeBriber = new recruit("Police briber", 2000, -800);

            /*recruitTypes.Add(pimp);
            recruitTypes.Add(hitman);
            recruitTypes.Add(bodyguard);
            recruitTypes.Add(policeBriber);*/
        }

        override protected void updateMenuOptions()
        {
            menuOptions.Clear();

            menuOptions.Add(pimp.name + " - $" + pimp.initialCost + " - increases hooker income by %" + pimp.dailyIncome + " - " + pimp.currentNumber);
            menuOptions.Add(hitman.name + " - initial and daily cost $" + hitman.initialCost + " - per mission cost $" + hitman.dailyIncome + " - " + hitman.currentNumber);
            menuOptions.Add(bodyguard.name + " - $" + bodyguard.initialCost + " - $" + bodyguard.dailyIncome + " - " + bodyguard.currentNumber);
            menuOptions.Add(policeBriber.name + " - $" + policeBriber.initialCost + " - $" + policeBriber.dailyIncome + " - " + policeBriber.currentNumber);


            menuOptions.Add("Ok");
        }

        public override int getDailyMoneyChange()
        {
            //pimps need to be handled with hookers.
            int hitmanCost = (hitman.currentNumber * (hitman.initialCost * -1)); // need to handle mission cost seperately.
            int bodyGuardCost = (bodyguard.currentNumber * bodyguard.dailyIncome);
            int briberyCost = (policeBriber.currentNumber * policeBriber.dailyIncome);

            return (hitmanCost + bodyGuardCost + briberyCost);
        }

        public int getPimpModifier()
        {
            return pimp.currentNumber * pimp.dailyIncome;
        }

        public int getBodyguardCount()
        {
            return bodyguard.currentNumber;
        }

        public int getHitmanCount()
        {
            return hitman.currentNumber;
        }

        public int getCostOfMission(int hitmanCount)
        {
            return (hitman.dailyIncome * hitmanCount);
        }

        public int getBriberyModifier()
        {
            //each police briber reduces heat by 200, its a reduction so we want the result to be negative. 
            return ((policeBriber.currentNumber * 100) * -1);
        }

        override public bool processSelectedItem(string selection, Player playerObject)
        {
            string Name = selection.Substring(0, selection.IndexOf("-") - 1);

            if (Name == pimp.name)
            {
                if (playerObject.spendMoney(pimp.initialCost))
                {
                    pimp.currentNumber++;
                    updateMenuOptions();
                    return true;
                }
            }
            else if (Name == hitman.name)
            {
                if (playerObject.spendMoney(hitman.initialCost))
                {
                    hitman.currentNumber++;
                    updateMenuOptions();
                    return true;
                }
            }
            else if (Name == bodyguard.name)
            {
                if (playerObject.spendMoney(bodyguard.initialCost))
                {
                    bodyguard.currentNumber++;
                    updateMenuOptions();
                    return true;
                }
            }
            else if (Name == policeBriber.name)
            {
                if (playerObject.spendMoney(policeBriber.initialCost))
                {
                    policeBriber.currentNumber++;
                    updateMenuOptions();
                    return true;
                }
            }
            return false;
        }
    }
}
