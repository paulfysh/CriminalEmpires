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
    //TODO: add a bunch of text options to display when random events occur. 
    class RandomGameEvents
    {
        Player playerObject;
        public static Random randomNumberGenerater = new Random(); //only want one random number generator, here makes the most sense to keep it. 

        // this variable keeps track of how often something randomly good happens by incrementing one, and something randomly bad by decrementing one to try and help bring the mean towards it being even.
        int karma = 0;
        int eventsInARow = 0;

        public RandomGameEvents(Player playerIn)
        {
            playerObject = playerIn;
            //randomNumberGenerater = new Random();
        }

        public int checkForPoliceInvolvement()
        {
            //start simply, get the heat value, roll a d100 and if the police heat value is less than the result, you need to bribe them. 
            int heat = playerObject.getPoliceHeat();

            if (heat >= 250)
            {
                //want to make sure its never 100% certain that you will get fined. 
                heat = 230;
            }

            if (randomNumberGenerater.Next(1, 250) < heat)
            {
                //want to use the real value for heat as the bribe value though, so it sucks more the worse you are. 
                return (playerObject.getPoliceHeat() * 30);
            }
            
            return 0;
        }

        public string checkForEnforcerIssues()
        {
            ArrayList npcs = playerObject.getNPCs();
            int enforcerValue = ((NPC)npcs[(int)Player.NPCType.muscle]).getValue();

            int otherValues = 0;
            int bestValue = 0;
            int bestNPC = 0;
            int loopCount = 0;

            foreach (NPC currNpc in npcs)
            {
                if (currNpc.getValue() > bestValue)
                {
                    bestNPC = loopCount;
                }
                otherValues += currNpc.getValue();
                loopCount++;
            }

            otherValues -= enforcerValue;

            //player does not have enough enforcers and has at least some value of recruits, to give a startup chance. 
            if ((otherValues > 20) && ((otherValues / 4) > enforcerValue))
            {
                //roll the dice and use the same logic as police 
                int enforcerHeat = (otherValues - enforcerValue); // get the difference between the two values. 

                if (randomNumberGenerater.Next(1, 100) < enforcerHeat)
                {
                    //kill 1 of the best members of best assets the player has
                    string recruitsKilled = ((NPC)playerObject.getNPCs()[bestNPC]).killRecruit(1, 4);
                    playerObject.handlePoliceHeat();
                    playerObject.modifyCityControl(-3);

                    return "You should get more muscle, rival gangs killed: " + recruitsKilled + " and we lost control of 3% of the city";
                    
                }
            }

            return "";
        }

        public string generateRandomEvent()
        {
            string result = "";
            //check that we want a random event, not every day should have a random event. Say 1/4 should give a event. 
            if (randomNumberGenerater.Next(1, 100) > 75 + eventsInARow)
            {
                //we take a random between 1 and 100 and if its <=(50+karma) its bad if its > its good.  
                if (randomNumberGenerater.Next(1, 100) > 50 + karma)
                {
                    result = generateGoodEvent();
                    karma++; // make it more likey the next event will be bad
                }
                else
                {
                    result = generateBadEvent();
                    karma--; // make it more likely the next event will be good. 
                }

                eventsInARow++;
            }
            else
            {
                eventsInARow = 0;
            }

            return result;
        }

        private string generateBadEvent()
        {   
            int badEventToPerform = randomNumberGenerater.Next(1, 100);
            if(badEventToPerform <= 30)
            {
                 //lose 1 recruit (20)
                return modifyRecruits(false, 1);
            }
            else if(badEventToPerform <= 40)
            {
                //lose several recruits (20) // max 3
                //recruits defect from rivals (10) // max 3
                if (badEventToPerform <= 34)
                {
                    return modifyRecruits(false, 1);
                }
                else if (badEventToPerform <= 37)
                {
                    return modifyRecruits(false, 2);
                }
                else
                {
                    return modifyRecruits(false, 3);
                }
            }
            else if(badEventToPerform <= 50)
            {
                //lose all recruits of a certain type (10)
                //this is a hack for now, it will basically destroy all recruits below the selected value in modify recruits, its a pretty rough event.
                return modifyRecruits(false, 1000);
            }
            else if (badEventToPerform <= 90)
            {
                //police raid (cash) (40)
                return modifyCash(false);
            }
            else
            {
                //gain some police heat
                return modifyPlayerHeat(true);
            }

            //maybe lose a recruiter but that seems overly harsh
        }

        private string generateGoodEvent()
        {
            int goodEventToPerform = randomNumberGenerater.Next(1, 100);

            if (goodEventToPerform <= 20)
            {
                //gain a recruit (20)
                return modifyRecruits(true, 1);
            }
            else if (goodEventToPerform <= 30)
            {
                //recruits defect from rivals (10) // max 3
                if (goodEventToPerform <= 24)
                {
                    return modifyRecruits(true, 1);
                }
                else if(goodEventToPerform <= 27)
                {
                    return modifyRecruits(true, 2);
                }
                else
                {
                    return modifyRecruits(true, 3);
                }
            }
            else if (goodEventToPerform <= 70)
            {
                //extra cash from x (40)
                return modifyCash(true);
            }
            else if (goodEventToPerform <= 90)
            {
                //lose some police heat (20)
                return modifyPlayerHeat(false);
            }
            else
            {
                //extra control (10)
                playerObject.modifyCityControl(5);
                return "Two rival gangs duked it out and we managed to capitalise, we gained control of 5% of the city";
            }
        }

        private string modifyRecruits(bool increase, int quantity)
        {
            string result = "";
            if (increase)
            {
                //find which recruiters are valid choices to increment.
                //we only want to increment hookers, muscle, drugs or guns
                ArrayList validIncrements = new ArrayList();

                if (((NPC)playerObject.getNPCs()[(int)Player.NPCType.drug]).isAvaliable)
                {
                    validIncrements.Add(Player.NPCType.drug);
                }
                if (((NPC)playerObject.getNPCs()[(int)Player.NPCType.guns]).isAvaliable)
                {
                    validIncrements.Add(Player.NPCType.guns);
                }
                if (((NPC)playerObject.getNPCs()[(int)Player.NPCType.hookers]).isAvaliable)
                {
                    validIncrements.Add(Player.NPCType.hookers);
                }
                if (((NPC)playerObject.getNPCs()[(int)Player.NPCType.muscle]).isAvaliable)
                {
                    validIncrements.Add(Player.NPCType.muscle);
                }

                if(validIncrements.Count > 0)
                {
                    int selection = randomNumberGenerater.Next(0, validIncrements.Count);
                    if (quantity == 1)
                    {
                        result = "Good news, a member of a rival gang has defected to us: ";
                    }
                    else
                    {
                        result = "Good news, seveal members of a rival gang have defected to us: ";
                    }
                    
                    result += ((NPC)playerObject.getNPCs()[(int)validIncrements[selection]]).addRecruit(quantity, randomNumberGenerater.Next(1, 5)); //I think the max is basically 0 indexed
                }
            }
            else
            {
                //figure out which one to kill off, based on which are active and which have a value.
                ArrayList validDecrements = new ArrayList();

                if (((NPC)playerObject.getNPCs()[(int)Player.NPCType.drug]).isAvaliable
                    && ((NPC)playerObject.getNPCs()[(int)Player.NPCType.drug]).getValue() > 0)
                {
                    validDecrements.Add(Player.NPCType.drug);
                }
                if (((NPC)playerObject.getNPCs()[(int)Player.NPCType.guns]).isAvaliable
                    && ((NPC)playerObject.getNPCs()[(int)Player.NPCType.guns]).getValue() > 0)
                {
                    validDecrements.Add(Player.NPCType.guns);
                }
                if (((NPC)playerObject.getNPCs()[(int)Player.NPCType.hookers]).isAvaliable
                    && ((NPC)playerObject.getNPCs()[(int)Player.NPCType.hookers]).getValue() > 0)
                {
                    validDecrements.Add(Player.NPCType.hookers);
                }
                if (((NPC)playerObject.getNPCs()[(int)Player.NPCType.muscle]).isAvaliable
                    && ((NPC)playerObject.getNPCs()[(int)Player.NPCType.muscle]).getValue() > 0)
                {
                    validDecrements.Add(Player.NPCType.muscle);
                }

                if (validDecrements.Count > 0)
                {
                    int selection = randomNumberGenerater.Next(0, validDecrements.Count);

                    if (quantity == 1)
                    {
                        result = "Awful news, a member of our gang defected to a rival: ";
                    }
                    else
                    {
                        result = "Awful news, seveal members of our gang have defected to a rival: ";
                    }
                    

                    //this can result in nobody being killed, if it hits on 2 and you only have values 3 and 4 nobody dies. Might want to look at that.
                    string killString = ((NPC)playerObject.getNPCs()[(int)validDecrements[selection]]).killRecruit(quantity, randomNumberGenerater.Next(1, 5));
                    if (killString != "")
                    {
                        result += killString;  //I think the max is basically 0 indexed
                    }
                    else
                    {
                        result = "";
                    }
                }
            }

            playerObject.handlePoliceHeat();
            return result;
        }

        private string modifyCash(bool increase)
        {
            string result = "";

            if (increase)
            {
                //increasing nets us a between 1 and 5% of our current cash
                result = "Great news, we made a bonus of $";
                float percentToGain = randomNumberGenerater.Next(1, 6);
                percentToGain /= 100;

                int valueToGain = (int)(playerObject.getMoney() * percentToGain);
                result += valueToGain;
                playerObject.spendMoney((valueToGain * -1));
            }
            else
            {
                //decreasing loses us between 10 and 20% of our cash.
                result = "Terrible news, we made $";
                float percentToLose = randomNumberGenerater.Next(10, 21);
                percentToLose /= 100;

                int valueToLose = (int)(playerObject.getMoney() * percentToLose);
                result += valueToLose;
                result += "less than expected";
                playerObject.spendMoney((valueToLose));
            }

            return result;
        }

        private string modifyPlayerHeat(bool increase)
        {
            string result = "";

            // gain or lose 5 hear with this one, not EVERYTHING needs to be random :P
            if (increase)
            {
                result = "Terrible news, we irked the police, now they have a hard on for us";
                playerObject.updatePlayerHeatModifier(5);
            }
            else
            {
                result = "Good news, the police are running a series of raids against our rivals, so we are receving less of their attention.";
                playerObject.updatePlayerHeatModifier(-5);
            }

            return result;
        }
    }
}
