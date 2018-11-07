using System;
using System.Linq;
using System.Collections.Generic;

namespace SongEvolutionModelLibrary
{
    public static class BirthDeathCycle{
        /*Main simulation processes: removal of bids
        and genertaion of news ones*/
        public static Population Step(SimParams par, Population pop){
            int[] Vacant;
            List<int> NotVacant = Enumerable.Range(0,par.NumBirds).ToList();
            //Chose birds to die, mark ones still alive
            if(par.AgeDeath){
                Vacant = AgeDeath(par, pop);
            }else{Vacant = RandomDeath(par, pop);}
            for(int i=0;i<Vacant.Length;i++){
                NotVacant.Remove(Vacant[i]);
            }
            
            //Allow for oblique learning and age up
            if(par.ObliqueLearning){
                Learning.ObliqueLearning(par, pop, Vacant, NotVacant);
            }
            for(int i=0;i<NotVacant.Count;i++){
                pop.Age[NotVacant[i]] += 1;
            }

            //Determine fathers and generate chicks
            int[] FatherInd = ChooseMaleFathers(par, pop, Vacant, NotVacant);
            for(int i=0;i<FatherInd.Length;i++){
                pop.ReplaceBird(par, FatherInd[i], Vacant[i]);
            }

            //allow for chicks to overlearn, update match and sylrep
            if(par.OverLearn == true){
                Learning.OverLearn(par, pop, Vacant, NotVacant);
            }

            /*Allow for the female song to evolve and/or
            let them pick new mates if old mate deceased*/
            if(par.FemaleEvolution){
                int[] FemaleFathers = ChooseFemaleFathers(par, pop,
                                            Vacant, NotVacant, FatherInd);
                for(int i=0;i<Vacant.Length;i++){
                    pop.FemaleSong[Vacant[i]] = pop.MaleSong[FatherInd[i]];
                }
            }
            if(par.ChooseMate){
                Songs.ChooseMates(par, pop);
            }
                      
            //update survival probability
            if(par.AgeDeath){
                UpdateDeathProbabilities(par, pop, FatherInd);
            }
            return(pop);
        }

        //Death
        private static int[] AgeDeath(SimParams par, Population pop){
            //Send birds to the circus based on retirement age
            List<int> DeadIndex = new List<int> {};
            float[] PickChance = LearningPenalty(par, pop.LearningThreshold);
            int[] AgePool;
            float Dead;
            int MustDie;
            float ChanceDie;
            int[] Chosen;
            int[] ChosenInd;
            float[] PickChanceSubSet;

            for(int i=0;i<par.MaxAge;i++){
                //which birds are age i
                AgePool = Enumerable.Range(0, pop.Age.Length)
                                    .Where(x => pop.Age[x] == i).ToArray();
                if(AgePool.Length == 0){//No bids of a given age
                    continue;
                }
                Dead = AgePool.Length*(1-pop.SurvivalChance[i]);
                MustDie = (int)Math.Floor(Dead);
                ChanceDie = Dead%1;
                if(par.NextFloat() < ChanceDie){
                    MustDie += 1;
                }
                if(MustDie == 0){
                    continue;
                }
                Chosen = new int[MustDie];
                if(MustDie >= AgePool.Length){
                    Chosen = AgePool;
                }else{
                    PickChanceSubSet = new float[AgePool.Length];
                    for(int j=0;j<AgePool.Length;j++){
                        PickChanceSubSet[j] = PickChance[AgePool[j]];
                    }
                    ChosenInd = par.randomSampleUnequal(PickChanceSubSet, MustDie, false);
                    for(int j=0;j<ChosenInd.Length;j++){
                        Chosen[j] = AgePool[ChosenInd[j]];
                    }
                }
                DeadIndex.AddRange(Chosen);
            }
            DeadIndex.AddRange(Enumerable.Range(0, pop.Age.Length).Where(x => pop.Age[x] == par.MaxAge).ToArray());//birds of max age must die            
            
            /*double[] GetIndexer = new double[DeadIndex.Count]; 
            for(int i=0;i<DeadIndex.Count;i++){
                GetIndexer[i] = PickChance[DeadIndex[i]];
            }
            Console.WriteLine(PickChance.Average());
            //Console.WriteLine(GetIndexer.Average());*/
            return(DeadIndex.ToArray());
        }
        private static int[] RandomDeath(SimParams par, Population pop){
            float[] PickChance = LearningPenalty(par, pop.LearningThreshold);
            int numLostBirds = (int)Math.Floor(par.NumBirds*par.PercentDeath);
            int[] LostBirds = par.randomSampleUnequal(PickChance, numLostBirds,false);
            return(LostBirds);
        }
        private static float[] LearningPenalty(SimParams par, float[] learnThreshold){
            float Base = par.LearningPenalty/(par.MaxAge-1);
            float[] PickChance = new float[par.NumBirds];
            for(int i=0;i<par.NumBirds;i++){
                PickChance[i] = Base*(learnThreshold[i]-1)+1;  
            }
                List<int> Cheaters = Enumerable.Range(0, PickChance.Length).Where(x => PickChance[x] < 1).ToList();
                for(int i=0;i<Cheaters.Count;i++){  
                    PickChance[Cheaters[i]] = 1;                 
                }
            return(PickChance);
        }

        //Birth
        private static int[] ChooseMaleFathers(SimParams par, Population pop,
        int[] vacant, List<int> notVacant){       
            /*Picks fathers to sire chicks into vacancies,
            Uses only living males.
            It is likely for high quality males to sire multiple
            offspring in one sim step for both local and global scopes*/
            
            //Remove songless birds from notVacant, and mark all not in that list as unavailable
            HashSet<int> PotentialFathersTemp = notVacant.Where(x => pop.SyllableRepertoire[x] > 0).ToHashSet();
            HashSet<int> Unavailable = Enumerable.Range(0, par.NumBirds).ToHashSet();
            Unavailable.ExceptWith(PotentialFathersTemp);
            //Get the probabilities
            List<int> PotentialFathers = PotentialFathersTemp.ToList();
            float[] Probability = Enumerable.Repeat(0f, par.NumBirds).ToArray();
            float[] Probs = ReproductiveProbability(par,pop,PotentialFathers);
            for(int i=0;i<PotentialFathers.Count;i++){
                Probability[PotentialFathers[i]] = Probs[i];
            }


            //pick fathers
            int[] Fathers = new int[vacant.Length];
            if(par.LocalBreeding){
                for(int i=0;i<Fathers.Length;i++){
                   Fathers[i] = Locations.GetLocalBirds(par, pop, vacant[i],
                                                        Unavailable, probs: Probability)[0];
                }
            }else{               
                float[] PotenProbs = new float[PotentialFathers.Count];
                int[] FatherIndex;
                for(int i=0;i<PotenProbs.Length;i++){
                    PotenProbs[i] = Probability[PotentialFathers[i]];
                }
                FatherIndex = par.randomSampleUnequal(PotenProbs,vacant.Length,false);
                for(int i=0;i<Fathers.Length;i++){
                    Fathers[i] = PotentialFathers[FatherIndex[i]];
                }               
            }
            
            return(Fathers);
        }
        private static int[] ChooseFemaleFathers(SimParams par, Population pop,
        int[] vacant, List<int> notVacant, int[] maleFathers){       
            /*Picks fathers to sire chicks into vacancies,
            Uses only living males.
            It is likely for high quality males to sire multiple
            offspring in one sim step for both local and global scopes*/
            
            //Remove songless birds from notVacant, and mark all not in that list as unavailable
            HashSet<int> PotentialFathersTemp = notVacant.Where(x => pop.SyllableRepertoire[x] > 0).ToHashSet();
            HashSet<int> Unavailable = Enumerable.Range(0, par.NumBirds).ToHashSet();
            Unavailable.ExceptWith(PotentialFathersTemp);
            
            //Get the probabilities
            List<int> PotentialFathers = PotentialFathersTemp.ToList();
            float[] Probability = Enumerable.Repeat(0f, par.NumBirds).ToArray();
            float[] Probs = ReproductiveProbability(par,pop,PotentialFathers);
            for(int i=0;i<PotentialFathers.Count;i++){
                Probability[PotentialFathers[i]] = Probs[i];
            }

            //pick fathers
            int[] Fathers = new int[vacant.Length];
            HashSet<int> UnavailableNoMateFather;
            if(par.LocalBreeding){
                for(int i=0;i<Fathers.Length;i++){
                    UnavailableNoMateFather = Unavailable;
                    UnavailableNoMateFather.Remove(maleFathers[i]);
                    Fathers[i] = Locations.GetLocalBirds(par, pop, vacant[i],
                                                        UnavailableNoMateFather, probs: Probability)[0];
                }
            }else{
                float[] PotenProbs = new float[PotentialFathers.Count];
                for(int i=0;i<PotenProbs.Length;i++){
                    PotenProbs[i] = Probability[PotentialFathers[i]];
                }
                float[] PotenProbsTemp;
                //List<int> PotentialFathersNoMateFather;
                for(int i=0;i<vacant.Length;i++){
                    //PotentialFathersNoMateFather = PotentialFathers.ToList();
                    //PotentialFathersNoMateFather.Remove(MaleFathers[i]);
                    PotenProbsTemp = PotenProbs.ToArray();
                    PotenProbsTemp[maleFathers[i]] = 0;
                    /*for(int j=0;j<PotenProbs.Length;j++){
                        PotenProbs[j] = Probability[PotentialFathers[j]];
                    }*/
                    Fathers[i] = PotentialFathers[par.randomSampleUnequal(PotenProbsTemp,1,true)[0]];
                }
            }
            return(Fathers);
        }
        private static float[] ReproductiveProbability(SimParams par, Population pop,
        List<int> usableMales){
            //Get the bonus for each category, combine, and return
            //Choices <- PotentialFathers$Males[UsableInd,]
            float[] FullBonus = new float[usableMales.Count];
            //Noise
            float[] NoiseBonus = Enumerable.Repeat(par.NoisePreference, usableMales.Count).ToArray();
            //Rep
            float[] RepBonus = new float[usableMales.Count];
            if(par.RepertoireSizePreference != 0){
                float[] Rep = new float[usableMales.Count];
                if(par.LogScale){
                    for(int i=0;i<usableMales.Count;i++){
                        Rep[i] = (float)Math.Log(pop.SyllableRepertoire[usableMales[i]]);
                    } 
                }else{
                    for(int i=0;i<usableMales.Count;i++){
                        Rep[i] = pop.SyllableRepertoire[usableMales[i]];
                    }
                }
                float Worst = Rep.Min();
                float Best = Rep.Max();
                if(Worst == Best){
                  RepBonus = Enumerable.Repeat(par.RepertoireSizePreference, usableMales.Count).ToArray();  
                }else{
                    float Fraction = 1/(Best - Worst);
                    for(int i=0;i<usableMales.Count;i++){
                        RepBonus[i] = ((Rep[i] - Worst)*Fraction)*par.RepertoireSizePreference;
                    }

                }
            }else{RepBonus = Enumerable.Repeat(0f, usableMales.Count).ToArray();}
            
            //Match
            float[] MatBonus = new float[usableMales.Count];
            if(par.MatchPreferenece != 0){
                float Bonus;
                for(int i=0;i<usableMales.Count;i++){
                    Bonus = par.MatchPreferenece*pop.Match[usableMales[i]];
                    if(Bonus < .001){Bonus = .001f;}
                    MatBonus[i] = Bonus;
                }
            }else{MatBonus = Enumerable.Repeat(0f, usableMales.Count).ToArray();}

            //Merge
            for(int i=0;i<usableMales.Count;i++){
                FullBonus[i] = NoiseBonus[i] + RepBonus[i] + MatBonus[i]; 
            }
            return(FullBonus);
        }
        //Reset for next run        
        private static Population UpdateDeathProbabilities(SimParams par, Population pop, 
        int[] fatherInd){
            //get death probs for next step
            if(pop.SurvivalChance.Count>2){
                for(int i=par.MaxAge;i>1;i--){
                    pop.SurvivalChance[i] = pop.SurvivalChance[i-1]; 
                }
            }
            pop.SurvivalChance[1] = pop.SurvivalStore;
            pop.SurvivalStore = (float)Math.Pow(1/(par.ChickSurvival*fatherInd.Length),1/par.MaxAge); 
            return(pop);
        }
    }
}