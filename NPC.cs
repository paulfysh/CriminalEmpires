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
    class NPC
    {
        protected Vector2 Position;
        protected bool avaliable;
        protected string initialText;
        protected string menuText;
        protected ArrayList menuOptions = new ArrayList();
        protected bool spokenTo = false;
        protected int cost;
        protected string name;
        protected int heatModifier = 1;

        bool visible = false;

        protected ArrayList recruitTypes = new ArrayList();

        protected struct recruit
        {
            public recruit(string recruitName, int cost, int income)
            {
                name = recruitName;
                initialCost = cost;
                dailyIncome = income;
                currentNumber = 0;
            }
            public string name;
            public int initialCost;
            public int dailyIncome;
            public int currentNumber;
        }

        virtual protected void updateMenuOptions()
        {
            menuOptions.Clear();

            foreach (recruit currRecruit in recruitTypes)
            {
                menuOptions.Add(currRecruit.name + " - $" + currRecruit.initialCost + " - $" + currRecruit.dailyIncome + " - " + currRecruit.currentNumber);
            }

            menuOptions.Add("Ok");
        }

        public Vector2 getLocation()
        {
            return Position;
        }

        public int getHeatModifier()
        {
            return heatModifier;
        }

        //used for random events, quite simple in general it just counts the number of recruits and multiplies them by their position, so a higher level drug dealer counts more than a lower level one.
        public int getValue()
        {
            int value = 0;
            int multiplyer = 1;
 
            foreach (recruit currRecruit in recruitTypes)
            {
                value += (currRecruit.currentNumber * multiplyer);
                multiplyer++;
            }

            return value;
        }

        public bool isAvaliable
        {
            get { return avaliable; }
            set { avaliable = value; }
            
        }

        public int getCost()
        {
            return cost;
        }

        public string getName
        {
            get { return name; }
        }

        public String getMenuText()
        {
            if (!spokenTo && (initialText != "" && initialText != null))
            {
                spokenTo = true;
                return initialText;
            }
            else
            {
                return menuText;
            }
        }

        public ArrayList getMenuOptions()
        {
            if (!spokenTo && (initialText != "" && initialText != null))
            {
                return new ArrayList();
            }
            else
            {
                return menuOptions;
            }
        }

        public bool isVisible
        {
            get { return visible; }
            set { visible = value; }
        }

        virtual public bool processSelectedItem(string selection, Player playerObject)
        {
            string Name = selection.Substring(0, selection.IndexOf("-") - 1);
            int i = 0;

            foreach (recruit currRecruit in recruitTypes)
            {
                if (currRecruit.name == Name)
                {
                    if (playerObject.spendMoney(currRecruit.initialCost))
                    {
                        recruit incrementDealer = (recruit)recruitTypes[i];
                        incrementDealer.currentNumber++;
                        recruitTypes[i] = incrementDealer;
                        updateMenuOptions();
                        return true;
                    }
                }
                i++;
            }

            return false;
        }

        virtual public int getDailyMoneyChange()
        {
            int currentValue = 0;

            foreach (recruit currRecruit in recruitTypes)
            {
                currentValue += (currRecruit.dailyIncome * currRecruit.currentNumber);
            }

            return currentValue;
        }

        public string killRecruit(int numberToKill, int valueToKill)
        {
            string killed = "";
            if (recruitTypes.Count >= valueToKill && valueToKill >= 1)
            {
                recruit recruitToKill = (recruit)recruitTypes[valueToKill - 1];
                if(recruitToKill.currentNumber >= numberToKill)
                {
                    killed = numberToKill + " " + recruitToKill.name;
                    recruitToKill.currentNumber -= numberToKill;
                    recruitTypes[valueToKill - 1] = recruitToKill;
                }
                else
                {
                    //if we have not killed enough, recurse. 
                    if (recruitToKill.currentNumber > 0)
                    {
                        int valueKilled = numberToKill - recruitToKill.currentNumber;
                        recruitToKill.currentNumber = 0;

                        if (valueKilled > 0)
                        {
                            killed += valueKilled + " " + recruitToKill.name;
                        }
                    }
                    killed += killRecruit(numberToKill, valueToKill - 1);
                }
                recruitTypes[valueToKill - 1] = recruitToKill;
            }

            updateMenuOptions();

            return killed;
        }

        public string addRecruit(int numberToAdd, int recruitToAdd)
        {
            string result = "";
            if (recruitTypes.Count >= recruitToAdd && recruitToAdd >= 1)
            {
                recruit modificationRecruit = (recruit)recruitTypes[recruitToAdd - 1];
                modificationRecruit.currentNumber += numberToAdd;
                recruitTypes[recruitToAdd - 1] = modificationRecruit;

                result = numberToAdd + " " + modificationRecruit.name;
                updateMenuOptions();
            }
            return result;
        }
    }
}
