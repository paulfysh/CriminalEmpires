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
    class Player
    {
        Vector2 location;
        Vector2 nextLocation;
        int speed;
        Texture2D playerTexture;
        int money;
        ArrayList NPCs = new ArrayList();

        bool jobSet;
        jobBoard.job currentJob;

        int cityControl;


        int policeHeat;
        int playerHeatModifier;

        int lastMoneyChange;


        Color[,] terrainColors; //= TextureTo2DArray(map);
        Color[,] playerColors; //= TextureTo2DArray(playerTexture);

        public enum Direction
        {
            up = 0,
            down,
            left,
            right
        };

        public enum NPCType
        {
            drug = 0,
            guns,
            muscle,
            hookers,
            specialists,
            jobs
        };

        public Player(Texture2D tex, Texture2D map)
        {
            location.X = 450;
            location.Y = 800;
            nextLocation = location;
            money = 2000;
            speed = 5;
            cityControl = 10;

            lastMoneyChange = 0;

            policeHeat = 0;

            jobSet = false;

            playerTexture = tex;

            terrainColors = Helpers.TextureTo2DArray(map);
            playerColors = Helpers.TextureTo2DArray(playerTexture);

            //add my NPCs in order listed in the enum so we can do some hacky stuff and use that to access them when we need them specifically. 
            NPCs.Add(new DrugRecruiter());
            NPCs.Add(new GunRecruiter());
            NPCs.Add(new MuscleRecruiter());
            NPCs.Add(new HookerRecruiter());
            NPCs.Add(new SpecialstRecruiter());
            NPCs.Add(new jobBoard());
            NPCs.Add(new recruiter(NPCs));
        }

        public void modifyCityControl(int modificationValue)
        {
            cityControl += modificationValue;
        }

        public Vector2 getLocation()
        {
            return location;
        }

        public int getMoney()
        {
            return money;
        }

        public ArrayList getNPCs() 
        {
            return NPCs;
        }

        public int getPoliceHeat()
        {
            return policeHeat;
        }

        public int getCityControl()
        {
            return cityControl;
        }

        public void assignJob(jobBoard.job task)
        {
            currentJob = task;
            jobSet = true;
        }



        public Vector2 GetMove(Direction dir)
        {
            //need some collision detection - make sure we are not colliding with a non transparant part of the map. 
            nextLocation = location;

            switch (dir)
            {
                case Direction.up:
                    nextLocation.Y -= speed;
                    break;
                case Direction.down:
                    nextLocation.Y += speed;
                    break;
                case Direction.left:
                    nextLocation.X -= speed;
                    break;
                case Direction.right:
                    nextLocation.X += speed;
                    break;
            }

            return nextLocation;
        }

        //I dont like this get move commit move model, but frankly my code base is a bit nasty so, Ill do that for now and tidy it if I get chance. 
        public void DoMove(Map terrain)
        {
            if (!terrainCollision(nextLocation, terrain))
            {
                location = nextLocation;
            }
        }

        public void CancelMove()
        {
            nextLocation = location;
        }

        private bool terrainCollision(Vector2 nextLocation, Map terrain)
        {
            Matrix playerMat = Matrix.CreateTranslation(nextLocation.X, nextLocation.Y, 0);
            Matrix terrainMat = Matrix.CreateTranslation((terrain.getDisplayRectangle().X * -1), (terrain.getDisplayRectangle().Y * -1), 0) * Matrix.CreateTranslation(terrain.getDisplayRectangle().X, terrain.getDisplayRectangle().Y, 0);


            Vector2 collisionPoint = Helpers.TexturesCollide(playerColors, playerMat, terrainColors, terrainMat);

            if (collisionPoint.X != -1)
            {
                return true;
            }
            
            return false;
        }

        //this entire system is a mess and I hate the specific location access for the values, but for now it will do, need to refactor. 
        public bool unlockNPC(NPCType type)
        {
            NPC currNPC = (NPC)NPCs[(int)type];

            int npcCost = currNPC.getCost();
            if (spendMoney(npcCost))
            {
                //money -= npcCost;
                currNPC.isAvaliable = true;
                return true;
            }
            return false;
        }

        /*private bool haveEnoughMoney(int checkValue)
        {
            if(money >= checkValue)
            {
                return true;
            }
            return false;
        }*/

        public bool spendMoney(int spendValue)
        {
            if (money >= spendValue)
            {
                money -= spendValue;
                return true;
            }
            return false;
        }

        public bool handleDailyIncome()
        {
            //if this puts us in the negative we lose, so need to return false for game over.
            int startMoney = money;

            float pimpMod = ((SpecialstRecruiter)NPCs[(int)NPCType.specialists]).getPimpModifier();
            pimpMod /= 100; //its a percentage boost.
            foreach (NPC currNPC in NPCs)
            {
                if(currNPC.getName == "Hookers")
                {
                    int value = currNPC.getDailyMoneyChange();
                    money += (value + (int)(pimpMod * value));
                }
                else
                {
                    money += currNPC.getDailyMoneyChange();
                }
            }

            lastMoneyChange = (money - startMoney);

            if (money < 0)
            {
                return false;
            }
            return true;
        }

        public int getLastMoneyChange()
        {
            return lastMoneyChange;
        }

        public void handlePoliceHeat()
        {
            policeHeat = 0;

            foreach (NPC currNPC in NPCs)
            {
                policeHeat += currNPC.getValue() * currNPC.getHeatModifier();
            }
            policeHeat += playerHeatModifier;

            policeHeat += (((SpecialstRecruiter)NPCs[(int)NPCType.specialists]).getBriberyModifier());
        }

        public recruiter getRecruiter
        {
            get { return (recruiter)NPCs[NPCs.Count - 1]; }
        }

        public void updatePlayerHeatModifier(int value)
        {
            playerHeatModifier += value;
            handlePoliceHeat();
        }

        public string doJob()
        {
            string result = "";
            if (jobSet)
            {
                jobSet = false;

                int bestMuscleValue = (((MuscleRecruiter)NPCs[(int)NPCType.muscle]).getBestMuscleValueForNumber(currentJob.muscleRequired));

                if (bestMuscleValue > currentJob.difficulty)
                {
                    //job passed
                    money += currentJob.moneyValue;
                    cityControl += currentJob.controlValue;

                    result = "Your men accomplished their mission, your money changed by $" + currentJob.moneyValue + " you gained control of " + currentJob.controlValue + "% of the city";

                    //if I have time, might want to kill some men depending on the result. 

                }
                else
                {
                    //job failed
                    //kill all the muscle involved.
                    result = "Your men failed to accomplish their mission, you lost the following resource: ";
                    result += (((MuscleRecruiter)NPCs[(int)NPCType.muscle]).killRecruit(currentJob.muscleRequired, 4));

                    //reduce control
                    result += " You lost control of " + currentJob.controlValue + "% of the city";
                    cityControl -= currentJob.controlValue;
                }

                 money -= (((SpecialstRecruiter)NPCs[(int)NPCType.specialists]).getCostOfMission(currentJob.hitmenRequired));

                ((jobBoard)NPCs[(int)NPCType.jobs]).reset();
            }

            return result;
        }

        public bool haveRequiredNumberOfMuscle(int numberOfMuscle)
        {
            if (((MuscleRecruiter)NPCs[(int)NPCType.muscle]).getMuscleCount() >= numberOfMuscle)
            {
                return true;
            }
            return false;
        }

        public bool haveRequiredNumberOfHitmen(int numberOfHitmen)
        {
            if (((SpecialstRecruiter)NPCs[(int)NPCType.specialists]).getHitmanCount() >= numberOfHitmen)
            {
                return true;
            }
            return false;
        }

        public bool haveRequiredNumberOfBodyguards(int numberOfBodyguards)
        {
            if (((SpecialstRecruiter)NPCs[(int)NPCType.specialists]).getBodyguardCount() >= numberOfBodyguards)
            {
                return true;
            }
            return false;
        }
    }
}
