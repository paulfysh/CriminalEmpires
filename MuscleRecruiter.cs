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
    class MuscleRecruiter : NPC
    {
        public MuscleRecruiter()
        {
            name = "Muscle";
            Position = new Vector2(100, 10);
            isAvaliable = false;
            cost = 400;

            heatModifier = 1; //muscle just protect your assets with regard to random events so dont antagonise the police beyond their existance. 

            menuText = "Some geezer giving you trouble boss?\nRecruit - Initial Cost - Daily value - currently recruited";

            setupMuscleTypes();

            updateMenuOptions();
        }

        private void setupMuscleTypes()
        {
            recruitTypes.Add(new recruit("Skinhead", 40, -15));
            recruitTypes.Add(new recruit("Thug", 100, -40));
            recruitTypes.Add(new recruit("Henchman", 200, -80));
            recruitTypes.Add(new recruit("Enforcer", 400, -160));
        }

        public int getMuscleCount()
        {
            int muscleCount = 0;
            foreach (recruit currRecruit in recruitTypes)
            {
                muscleCount += currRecruit.currentNumber;
            }
            return muscleCount;
        }

        public int getBestMuscleValueForNumber(int numberOfMuscle)
        {
            int value = 0;
            int currentMuscleAdded = 0;

            for (int i = recruitTypes.Count; i > 0 ; i--)
            {
                int currentRecruitNumber = ((recruit)recruitTypes[i - 1]).currentNumber;

                if (currentRecruitNumber > (numberOfMuscle - currentMuscleAdded))
                {
                    value += ((numberOfMuscle - currentMuscleAdded) * i);
                    currentMuscleAdded += (numberOfMuscle - currentMuscleAdded);
                    break;
                }
                else
                {
                    value += (currentRecruitNumber * i);
                    currentMuscleAdded += currentRecruitNumber;
                }
            }

            return value;

        }

    }
}
