using System;
using System.Linq;
using System.Collections.Generic;

namespace SongEvolutionModelLibrary
{
    public static class Ages{
        //Functions for getting the ages
        public static int[] InitialAgeDistribution (SimParams par){
            int[] AgeGroup;
            if(par.AgeDeath){
                float[] AgeRates = GetAgeRates(par);
                AgeGroup = GetAgeGroup(par, AgeRates);
            }else{
                List<int> AgeRange = Enumerable.Range(0,par.MaxAge+1).ToList();
                AgeGroup = par.RandomSampleEqualReplace(AgeRange, par.NumBirds); 
            }
            return(AgeGroup);
        }
        static float[] GetAgeRates (SimParams par){
            /*Get the fraction of the population in each age group
            The last element in the Survival Rates is omitted,
            because that is the "dead" slot; no birds in the
            population of live birds can be dead.
            */
            List<float> SurvivalRates = CalculateAllGenerations(par);
            float[] AgeRates = new float[SurvivalRates.Count-1];
            for(int i=0;i<AgeRates.Length;i++){
                AgeRates[i] = SurvivalRates[i]/par.NumBirds;
                AgeRates[i] = AgeRates[i]*(1/(1-(par.DeathThreshold/par.NumBirds)));
            }
            return(AgeRates);
        }
        static List<float> CalculateAllGenerations(SimParams par){
            //predict number of birds in each generation
            float N0 = par.DeathThreshold/(par.ChickSurvival*(float)Math.Pow(par.InitialSurvival, par.MaxAge));
            float N1 = N0*par.ChickSurvival;
            float[] nnext = new float[par.MaxAge];
            for(int i=0; i<par.MaxAge;i++){
                nnext[i] = N1*(float)Math.Pow(par.InitialSurvival,i+1);
            }
            List<float> AllNs = new List<float> {N0, N1};
            AllNs.AddRange(nnext);
            return(AllNs);
        }
        static int[] GetAgeGroup(SimParams par, float[] ageRates){
            //get the number of birds that are in each age group
            List<int> AgeGroup = new List<int> {};
            
            //Guaranteed ages
            int Birds = new int {};
            for(int i=0;i<=par.MaxAge;i++){
                Birds = (int)Math.Floor(par.NumBirds*ageRates[i]);
                AgeGroup.AddRange(Enumerable.Repeat(i,Birds).ToList());
            }

            //Get the chance ages if there are any
            int Remainder = par.NumBirds - AgeGroup.Count;
            List<int> RemainderAges = new List<int> {};
            if(Remainder > 0){
                float[] Chance = new float[Remainder];
                for(int i=0;i<Remainder;i++){Chance[i] = par.NextFloat();}
                
                //Chance to be at each possible age
                float[] CumulativeAgeProbability = new float[ageRates.Length+1];
                CumulativeAgeProbability[0] = 0;
                CumulativeAgeProbability[ageRates.Length] = 1;
                for(int i=0;i<ageRates.Length-1;i++){
                    CumulativeAgeProbability[i+1] = ageRates[i]+CumulativeAgeProbability[i];}
                
                //test whether Chance belongs to a given age group
                for(int i=0;i<=par.MaxAge;i++){
                    int AgeN = Chance.Count(x => (x >= CumulativeAgeProbability[i] && x < CumulativeAgeProbability[i+1]));
                    if(AgeN == 1){
                        RemainderAges.Add(i);
                    }else if(AgeN > 1){
                        RemainderAges.AddRange(Enumerable.Repeat(i,AgeN));
                    }
                }
                AgeGroup.AddRange(RemainderAges);
            }

            //Rearrange the age groups randomly
            List<int> Rearrange = Enumerable.Range(0,par.NumBirds).ToList();
            int[] NewIndex = par.RandomSampleEqualNoReplace(Rearrange, par.NumBirds);
            //AgeGroup = AgeGroup.OrderBy(x => par.Rand.Next()).ToList();  for generating array with rand numbers
            int[] ScrambledAgeGroup = AgeGroup.ToArray();
            Array.Sort(NewIndex, ScrambledAgeGroup);
            return(ScrambledAgeGroup);
        }        
    }
}