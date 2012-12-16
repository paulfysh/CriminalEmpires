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
    //This will be how you increase your control of the city, also does not really work like the other NPCs so might showa flaw in the hierarchy. 
    class jobBoard : NPC
    {
        public struct job
        {
            public int moneyValue;
            public int difficulty;
            public int controlValue;
            public int muscleRequired;
            public int hitmenRequired;
            public int bodyguardsRequired;
            public string missionText;

            public job(int cashValue, int requiredPassMark, int controlGained, int numMuscleNeeded, int numHitmenNeeded, int numBodyguardsNeeded, string text)
            {
                moneyValue = cashValue;
                difficulty = requiredPassMark; //we will add up the muscle sent on the mission value to see if this is successful.
                controlValue = controlGained;
                muscleRequired = numMuscleNeeded;
                hitmenRequired = numHitmenNeeded;
                bodyguardsRequired = numBodyguardsNeeded;
                missionText = text;
            }
        }

        ArrayList allPossibleJobs = new ArrayList();
        ArrayList selectedJobs = new ArrayList();
        bool displayingJobs = true;
        job displayedJob;
        string originalMenuText = "";

        public jobBoard()
        {
            name = "Job Seeker";
            Position = new Vector2(580, 800);
            isAvaliable = false;
            cost = 10000;

            menuText = "Theres always something needs doing\nAsk me about a job for more details";
            originalMenuText = menuText;


            generateJobList();
            selectJobsToDisplay();
            updateMenuOptions();

        }

        //a unique case, the player needs to reset this one because it works differently to the others. 
        // for example it generates new jobs to show each time and it can only select one per day.
        public void reset()
        {
            menuText = originalMenuText;
            selectJobsToDisplay();
            updateMenuOptions();
        }

        private void generateJobList()
        {
            //easy jobs
            allPossibleJobs.Add(new job(-1000, 5, 3, 2, 0, 0, "One of our rivals has pissed off a local street gang, if we give them some of our guns they might do some damage before they get wiped out"));
            allPossibleJobs.Add(new job(-500, 3, 2, 2, 0, 0, "A brothel has been cleared out in nearby terretory, if we can move in, we can expand a little"));
            allPossibleJobs.Add(new job(0, 2, 2, 1, 1, 0, "One of your lieutenants has been spotted shooting up, we dont allow our men to get high..."));
            allPossibleJobs.Add(new job(200, 2, 1, 2, 0, 1, "Its a little quiet, we had better send some patrols out."));
            allPossibleJobs.Add(new job(400, 4, 2, 3, 0, 1, "We have received word that someone put a hit out on you, time for a display of force."));

            //medium jobs
            allPossibleJobs.Add(new job(1000, 13, 5, 10, 0, 0, "We have located a rival arms storage area, raiding it will give us the edge in the arms trade."));
            allPossibleJobs.Add(new job(500, 8, 4, 3, 0, 0, "A rival has opened a drug den on our turf, we have to show them this will not stand."));
            allPossibleJobs.Add(new job(800, 15, 4, 6, 0, 0, "A police convoy is on its way to dispose of some guns, intercept them and we can paint the town red"));
            allPossibleJobs.Add(new job(600, 8, 3, 3, 1, 0, "Rival dealers are working our turf, we should kill as many as we can to send a message"));
            allPossibleJobs.Add(new job(900, 6, 4, 2, 0, 1, "Some rival muscle is getting roudy in one of our bars, get out there and kick them out"));

            //hard jobs
            allPossibleJobs.Add(new job(500, 18, 5, 5, 1, 0, "One of our rival cappos is on our turf, we need to get a group together to deal with him while we have the chance"));
            allPossibleJobs.Add(new job(2000, 18, 10, 5, 0, 2, "The cartel is flying in from out of town and have asked us to provide security for their meeting"));
            allPossibleJobs.Add(new job(1500, 16, 5, 4, 1, 1, "The chief of police is getting greedy, its time for an election"));
            allPossibleJobs.Add(new job(3000, 16, 8, 4, 2, 0, "A local official is running for office on a campaign against organised crime, make sure he does not win..."));
            allPossibleJobs.Add(new job(5000, 35, 15, 10, 0, 0, "Our nearest rival is weak, its time we stormed their terretory and put them out of our misery"));
        }

        private void selectJobsToDisplay()
        {
            selectedJobs.Clear();
            int firstJob = RandomGameEvents.randomNumberGenerater.Next(1, allPossibleJobs.Count);
            int secondJob = firstJob;
            int thirdJob = secondJob;
            while(secondJob ==  firstJob)
            {
                secondJob = RandomGameEvents.randomNumberGenerater.Next(1, allPossibleJobs.Count);
            }
            while (thirdJob == firstJob || thirdJob == secondJob)
            {
                thirdJob = RandomGameEvents.randomNumberGenerater.Next(1, allPossibleJobs.Count);
            }
            
            selectedJobs.Add(allPossibleJobs[firstJob]);
            selectedJobs.Add(allPossibleJobs[secondJob]);
            selectedJobs.Add(allPossibleJobs[thirdJob]);
        }



        override protected void updateMenuOptions()
        {
            menuOptions.Clear();

            foreach (job currJob in selectedJobs)
            {
                menuOptions.Add(currJob.missionText);
            }
           
            menuOptions.Add("Ok");
        }

        override public bool processSelectedItem(string selection, Player playerObject)
        {
            if (displayingJobs)
            {
                displayingJobs = false;

                foreach(job currJob in selectedJobs)
                {
                    if(currJob.missionText == selection)
                    {
                        menuText = selection + "\n";
                        menuText += "Mission reward: $" + currJob.moneyValue + "\n";
                        menuText += "Control reward: " + currJob.controlValue + "\n";
                        menuText += "Number of required Muscle: " + currJob.muscleRequired + "\n";
                        menuText += "Number of required bodyguards: " + currJob.bodyguardsRequired + "\n";
                        menuText += "Number of required hitmen: " + currJob.hitmenRequired + "\n";


                        menuOptions.Clear();
                        menuOptions.Add("Accept");
                        menuOptions.Add("Decline");

                        displayedJob = currJob;
                        break;
                    }
                }

                return false;
            }
            else
            {
                if (selection == "Accept")
                {
                    //need some way of adding this job to a queue back on game. Also only want one job accepted at a time, also need to check that the job is doable. 
                    if (playerObject.haveRequiredNumberOfBodyguards(displayedJob.bodyguardsRequired)
                        && playerObject.haveRequiredNumberOfHitmen(displayedJob.hitmenRequired)
                        && playerObject.haveRequiredNumberOfMuscle(displayedJob.muscleRequired))
                    {
                        playerObject.assignJob(displayedJob);
                        menuText = "Sorry Boss, we can only support one job at a time, come back tomorrow";
                        menuOptions.Clear();
                        menuOptions.Add("Ok");

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    displayingJobs = true;
                    menuText = originalMenuText;
                    updateMenuOptions();
                    return false;
                }
            }
        }


    }
}
