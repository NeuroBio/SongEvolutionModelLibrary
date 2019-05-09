using System;
using System.Linq;
using System.Collections.Generic;
//using System.Collections;
//using MathNet.Numerics;
using MathNet.Numerics.Distributions;
//using System.Text;
//using System.IO;


namespace SongEvolutionModelLibrary
{
    public class Population{
        public int[] Age,SyllableRepertoire;
        public List<List<int>>[] Local;
        public string[] Name,FatherName;
        public float[] Accuracy,LearningThreshold,
                    ChanceInvent,ChanceForget, Match;
        public List<int>[] MaleSong,FemaleSong;
        public List<float> SurvivalChance;public float SurvivalStore;

        //Constructor
        public Population(SimParams par) {        
            
            //Get variables dependant on population size (Age, Survival Stats, and Locality)
            Age = Ages.InitialAgeDistribution(par);
            if(par.AgeDeath){
                SurvivalChance = new List<float> {par.ChickSurvival};
                SurvivalChance.AddRange(Enumerable.Repeat(par.InitialSurvival, par.MaxAge));
                SurvivalStore = SurvivalChance[1];
            }
            if(par.LocalBreeding || par.LocalTutor){
               Local = Locations.FinalDirections(par);
            }

            //Get Songs and song data
            Songs SongData = new Songs(par);
            MaleSong = SongData.MaleSongs;
            FemaleSong = SongData.FemaleSongs;
            Match = SongData.Match;
            SyllableRepertoire = new int[par.NumBirds];
            for(int i=0;i<par.NumBirds;i++){
                SyllableRepertoire[i] = MaleSong[i].Count;
            }


            if(par.SaveNames){
                FatherName = Enumerable.Repeat("NA",par.NumBirds).ToArray();
                Name = new string[par.NumBirds];
                for(int i=0;i<par.NumBirds;i++){
                    Name[i] = System.Guid.NewGuid().ToString();
                }
            }

            //Set up distributions for noisy inheritence as needed and fill out arrays
            Accuracy = InitialDistributions(par, par.InheritedAccuracyNoise, par.InitialAccuracy,
                                            par.MaxAccuracy, par.MinAccuracy);    
            LearningThreshold = InitialDistributions(par, par.InheritedLearningThresholdNoise, par.InitialLearningThreshold,
                                                        par.MaxLearningThreshold, par.MinLearningThreshold);    
            ChanceInvent = InitialDistributions(par, par.InheritedChancetoInventNoise, par.InitialChancetoInvent,
                                                    par.MaxChancetoInvent, par.MinChancetoInvent);    
            ChanceForget = InitialDistributions(par, par.InheritedChancetoForgetNoise, par.InitialChancetoForget,
                                                    par.MaxChancetoForget, par.MinChancetoForget);   
 
        }
        //Functions for birth
        private float[] InitialDistributions(SimParams par, float noise, float initial, float max, float min){
            float[]  DistArray = new float[par.NumBirds];
            if(noise != 0){
            float Max = Math.Min(initial+noise, max);
            float Min = Math.Max(initial-noise, min);
                BetaScaled Dist = BetaScaled.PERT(Min, Max, initial, par.Rand);
                for(int i=0;i<par.NumBirds;i++){
                    DistArray[i] = (float)Dist.Sample();
                }
            }else{
                DistArray = Enumerable.Repeat(initial, par.NumBirds).ToArray();                
            }
            return(DistArray);
        }
        public void ReplaceBird(SimParams par, int father, int territory){
            //manditory
            Age[territory] = 0;
            /*Each set either takes the fathers value or calculates a new values based on a
            Distribution about the father's value.*/ 
            ChanceForget[territory] = par.InheritedChancetoForgetNoise!=0?
                (float)Inheritance(par.MinChancetoForget,par.MaxChancetoForget,
                ChanceForget[father],par.InheritedChancetoForgetNoise, par.Rand)
                :ChanceForget[father];
            ChanceInvent[territory] = par.InheritedChancetoInventNoise!=0?
                (float)Inheritance(par.MinChancetoInvent, par.MaxChancetoInvent, ChanceInvent[father], par.InheritedChancetoInventNoise,par.Rand)
                :ChanceInvent[father];
            LearningThreshold[territory] = par.InheritedLearningThresholdNoise!=0?
                (float)Inheritance(par.MinLearningThreshold, par.MaxLearningThreshold, LearningThreshold[father], par.InheritedLearningThresholdNoise,par.Rand)
                :LearningThreshold[father];
            Accuracy[territory] = par.InheritedAccuracyNoise!=0?
                (float)Inheritance(par.MinAccuracy, par.MaxAccuracy, Accuracy[father], par.InheritedAccuracyNoise, par.Rand)
                :Accuracy[father];
            if(par.VerticalLearning){
                MaleSong[territory] = Learning.VerticalLearning(par, this, father, territory);
            }else{
                MaleSong[territory]=new List<int>{};
            }
            SyllableRepertoire[territory] = MaleSong[territory].Count;

            //Optional
            if(par.SaveNames){
                Name[territory] = System.Guid.NewGuid().ToString();
                FatherName[territory] = Name[father];
            }
            if(par.SaveMatch){
                Match[territory] = Songs.GetMatch(par, MaleSong[territory], FemaleSong[territory]);
            }
        }
        private float Inheritance(double min, double max, double mode,
        double noise, Random rand){
            //Get noise around inheritance using triangular ditribution
            double Max = Math.Min(mode+noise, max);
            double Min = Math.Max(mode-noise, min);
            BetaScaled Distribution = BetaScaled.PERT(Min, Max, mode, rand);
            return((float)Distribution.Sample());
        }
        

        //Check/Diagnostic Functions
        public void Print(int birb) {
            //Quickly see what is in a bird;
            List<string> Inputs = new List<string> {};
            Inputs.Add(birb.ToString());
            Inputs.Add(Age[birb].ToString());
            Inputs.Add(Name == null?
                        " ": Name[birb].ToString());
            Inputs.Add(Match == null?
                        " ": Match[birb].ToString());
            Inputs.Add(SyllableRepertoire[birb].ToString());
            Inputs.Add(LearningThreshold == null?
                        " ": LearningThreshold[birb].ToString());
            Inputs.Add(Accuracy == null?
                        " ": Accuracy[birb].ToString());
            Inputs.Add(ChanceForget == null?
                        " ": ChanceForget[birb].ToString());
            Inputs.Add(ChanceInvent == null?
                        " ": ChanceInvent[birb].ToString());
            Console.WriteLine(String.Format("Bird {0}, Age: {1}, Guid: {2}, Match: {3}, SylRep: {4} "
            + "LrnThrsh: {5}, Acc: {6}, Forget: {7}, Invent: {8}", Inputs.ToArray()));
        }
        public void SongFragments(string sex = "Male"){
            if(sex == "Male"){
                for(int l=0;l<MaleSong.Length;l++){
                    for(int i=0;i<MaleSong[l].Count;i++){
                        Console.Write(MaleSong[l][i]);
                    }
                Console.WriteLine();
                }
            }else if(sex == "Female"){
                for(int l=0;l<FemaleSong.Length;l++){
                    for(int i=0;i<FemaleSong[l].Count;i++){
                        Console.Write(FemaleSong[l][i]);
                    }
                    Console.WriteLine();
                }
            }
        }
        public void UniqueSyllables(){
            List<int> CollapsedSongs;
            List<int> Syls;
            CollapsedSongs = MaleSong.SelectMany(x => x).ToList();
            Syls = CollapsedSongs.Distinct().ToList();
            Console.WriteLine("Male Syllables:");
            for(int i=0; i<Syls.Count;i++){
                Console.Write(" {0} ", Syls[i]);
            }
            CollapsedSongs = FemaleSong.SelectMany(x => x).ToList();
            Syls = CollapsedSongs.Distinct().ToList();
            Console.WriteLine();
            Console.WriteLine("Female Syllables:");
            for(int i=0; i<Syls.Count;i++){
                Console.Write(" {0} ", Syls[i]);
            }
            Console.WriteLine();
        }
    }
}
