using System;
using System.Linq;
using System.Collections.Generic;

namespace SongEvolutionModelLibrary
{
    public static class Locations{
                //Functions for getting the directions
        public static List<List<int>>[] FinalDirections(SimParams par){
            //Get the birds that are local and x-steps away
            List<List<int>> FirstStep = FirstStepDirections(par);
            List<List<int>> CurrentStep = FirstStep;
            if(par.Steps > 1){
                for(int i=1;i<par.Steps;i++){
                    CurrentStep = NextStepDirections(CurrentStep, FirstStep);
                }
            }
            List<List<int>>[] AllSteps = new List<List<int>>[par.Rows-(par.Steps+1)];
            AllSteps[0] = CurrentStep;
            
            for(int i=1;i<AllSteps.Length;i++){
                AllSteps[i] = NextStepDirections(AllSteps[i-1], FirstStep);
            }
            return(AllSteps);
        }
        private static List<List<int>> FirstStepDirections(SimParams par){
            //Get the local birds one step away
            int R = par.Rows;
            int C = par.Cols;
            List<List<int>> CellDirections = new List<List<int>>();

            //Upper left down to lower left           
            CellDirections.Add(new List<int>() {1, R, R+1});
            for(int i=1;i<=R-2;i++){
                CellDirections.Add(new List<int>
                                    {(i+1), (i-1), (i+R),(i+R+1), (i+R-1)});
            }
            CellDirections.Add(new List<int>() {R-2, 2*R-2, 2*R-1});
            
            //Centers
            for(int i=1;i<=C-2;i++){
                CellDirections.Add(new List<int>
                                {(i-1)*R, (i-1)*R+1, i*R+1, (i+1)*R, (i+1)*R+1});
                for(int j=0;j<=R-3;j++){
                    CellDirections.Add(new List<int> 
                                {(i-1)*R+j, (i-1)*R+1+j, (i-1)*R+2+j,
                                (i)*R+j, (i)*R+2+j,
                                (i+1)*R+j, (i+1)*R+1+j, (i+1)*R+2+j});
                }
                CellDirections.Add(new List<int>
                                {i*R-1, i*R-2, (i+1)*R-2, (i+2)*R-1, (i+2)*R-2});
            }
            //Upper right down to lower right
            CellDirections.Add(new List<int>() {R*(C-1)+1, R*(C-2), R*(C-2)+1});
            for(int i=0;i<=R-3;i++){
                CellDirections.Add(new List<int>
                                    {(R-1)*C+i, (R-1)*C+i+2,
                                    (R-2)*C+i, (R-2)*C+i+1, (R-2)*C+i+2});
            }
            CellDirections.Add(new List<int>() {R*C-2, R*C-R-1, R*C-R-2});
            return(CellDirections);
        }
        private static List<List<int>> NextStepDirections(List<List<int>> currentStep,
                                            List<List<int>> firstStep){
            //Get birds that are one territory further away
            List<List<int>> NextStep = new List<List<int>> {};
            for(int i=0;i<currentStep.Count;i++){
                List<int> Temp = new List<int> {};
                for(int j=0;j< currentStep[i].Count;j++){
                    Temp.AddRange(firstStep[currentStep[i][j]]);
                }
                Temp = Temp.Distinct().ToList();
                Temp.Remove(i);
                NextStep.Add(Temp);
            }
            //List<List<int>> lol = new List<List<int>> {};
            return(NextStep);
        }
        
        //Getting local and global birds
        public static int[] GetLocalBirds(SimParams par, Population pop,
        int target, HashSet<int> unavailable, int numBirds=1, float[] probs = null){
            //pick Tutors locally
            int[] Birds = new int[numBirds];
            int[] UsableInd;
            UsableInd = LocalSearch(par, pop, target, unavailable, numBirds).ToArray();

            //return the right number of birds
            if(UsableInd.Length == numBirds){//only numTutors choice(s)
                return(UsableInd);
            }else{//Multiple choices
                if(probs != null){//unequal probabilities
                    int[] Index;
                    float[] Prob = new float[UsableInd.Length];
                    for(int j=0;j<UsableInd.Length;j++){
                        Prob[j] = probs[UsableInd[j]];
                    }
                    Index = par.RandomSampleUnequal(Prob,numBirds,false);
                    for(int j=0; j<numBirds;j++){
                        Birds[j] = UsableInd[Index[j]];
                    }
                    return(Birds);
                }
                if(numBirds == 1){//can use the faster with replace
                    Birds[0] = UsableInd[par.RandomSampleEqualReplace(UsableInd.ToList(),1)[0]];
                    return(Birds);
                }//need the slower without replace
                Birds = par.RandomSampleEqualNoReplace(UsableInd.ToList(),numBirds);
                return(Birds);                
            }
        }
        private static List<int> LocalSearch(SimParams par, Population pop,
        int territory, HashSet<int> unavaliable, int needed=1){
            //Get birds that are accaptable and local to a given territory
            List<int> CurrentIndex;
            for(int i=0;i<pop.Local.Length;i++){
                CurrentIndex = pop.Local[i][territory].ToList();
                //Remove unacceptable birds
                for(int j=CurrentIndex.Count-1;j>=0;j--){
                    if(unavaliable.Contains(CurrentIndex[j])){
                        CurrentIndex.RemoveAt(j);
                    }
                }
                if(CurrentIndex.Count >= needed){
                    return(CurrentIndex);
                }
            }
            throw new ArgumentOutOfRangeException("All males are dead or songless; Game Over.");
        }
        public static int[] GetGlobalBirds(SimParams par, Population pop,
        int learner, List<int> potentialBirds, int numBirds=1){
                int[] Birds;
                List<int> PotentialBirds = potentialBirds.ToList();
                PotentialBirds.Remove(learner);
                if(numBirds == 1){
                    Birds = par.RandomSampleEqualReplace(PotentialBirds,numBirds);
                }else{
                    Birds = par.RandomSampleEqualNoReplace(PotentialBirds,numBirds);
                }
                return(Birds);                    
        }

    }
}