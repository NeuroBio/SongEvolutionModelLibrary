using System;
using MathNet.Numerics;
using System.Collections.Generic;
//using System.Collections;
using System.Linq;
using System.IO;
using System.Globalization;

namespace SongEvolutionModelLibrary{
    public class SimParams{
        public int Rows, Cols, NumBirds, Steps, InitialSyllableRepertoireSize,
                    MaxSyllableRepertoireSize, MaxAge, NumTutorConsensusStrategy,
                    NumTutorOverLearn, NumDialects, SimStep, NumSim, Seed;
        public float PercentSyllableOverhang, InitialAccuracy, InheritedAccuracyNoise,
                    MaxAccuracy, MinAccuracy, InitialLearningThreshold,
                    InheritedLearningThresholdNoise, MaxLearningThreshold, MinLearningThreshold,
                    InitialChancetoInvent, InheritedChancetoInventNoise, MaxChancetoInvent,
                    MinChancetoInvent,InitialChancetoForget, InheritedChancetoForgetNoise,
                    MaxChancetoForget, MinChancetoForget,
                    ListeningThreshold, EncounterSuccess, LearningPenalty, PercentDeath,
                    DeathThreshold, ChickSurvival, InitialSurvival, RepertoireSizePreference,
                    MatchPreferenece, NoisePreference, VerticalLearningCutOff;
        public string MaleDialects;
        public bool OverLearn, FemaleEvolution, SaveMatch, SaveAccuracy,
                    SaveLearningThreshold, SaveChancetoInvent, SaveChancetoForget,
                    SaveNames, SaveAge, MatchUniform, ObliqueLearning,LogScale,
                    ConsensusStrategy, Add, Forget, ChooseMate,
                    LocalBreeding, LocalTutor, AgeDeath, SaveMSong, SaveFSong;
        public HashSet<int> AllSyls;
        public List<float> SongCore;
        public Random Rand;
        //not implemented in main code
        //public float MatchScale;

        
         //Constructor
        public SimParams(int rows = 20, int cols = 20, int steps = 1,
        int initialSyllableRepertoireSize = 5, float percentSyllableOverhang = .2f,
        int maxSyllableRepertoireSize = 500, float initialAccuracy = .7f,
        float inheritedAccuracyNoise = .15f, int maxAccuracy = 1, int minAccuracy = 0,
        int maxAge = 20, float initialLearningThreshold = 2f, bool saveFSong = false,
        float inheritedLearningThresholdNoise = .25f, float maxLearningThreshold = default(float),
        float minLearningThreshold = 0f, float initialChancetoInvent = 3f,
        float inheritedChancetoInventNoise = 0f, float maxChancetoInvent=100f,
        float minChancetoInvent = 1f, float initialChancetoForget = .2f,
        float inheritedChancetoForgetNoise = 0f, float listeningThreshold = 7f,
        float encounterSuccess = .95f, float learningPenalty = .5f, float deathThreshold = 1,
        bool ageDeath = true, float percentDeath = .1f, bool saveMSong = false,
        float chickSurvival = .3f, bool localBreeding = true, bool localTutor = true,
        string learningStrategy = "Add", int numTutorConsensusStrategy = 4,
        bool overLearn = false, int numTutorOverLearn = 4, bool logScale =true,
        float repertoireSizePreference = 1f, float matchPreferenece = 0f,
        int numDialects = 1, string maleDialects = "None", bool femaleEvolution = false,
        bool? saveMatch = null, bool? saveAccuracy  = null, bool matchUniform = true,
        bool? saveLearningThreshold  = null, bool? saveChancetoInvent  = null,
        bool? saveChancetoForget  = null, bool saveNames  = false, bool chooseMate = false,
        bool saveAge  = false, int numSim = 1000, int seed = default(int),
        float minChancetoForget = 0f, float maxChancetoForget = 1f, 
        bool obliqueLearning = true, float verticalLearningCutOff=.25f, bool reload = false,
        string path = default(string)){    
            if(reload){
                //Load in params, string split and assign
                string[] Params = File.ReadAllText(path).Split('\n');
                for(int i=0;i<Params.Length-1;i++){
                    Params[i] = Params[i].Split("=")[1];
                }
                    Rows=System.Convert.ToInt32(Params[0]);
                    Cols=System.Convert.ToInt32(Params[1]);
                    NumBirds=System.Convert.ToInt32(Params[2]);
                    Steps=System.Convert.ToInt32(Params[3]);
                    InitialSyllableRepertoireSize=System.Convert.ToInt32(Params[4]);
                    PercentSyllableOverhang=float.Parse(Params[5], CultureInfo.InvariantCulture);
                    MaxSyllableRepertoireSize=System.Convert.ToInt32(Params[6]);
                    InitialAccuracy=float.Parse(Params[7], CultureInfo.InvariantCulture);
                    InheritedAccuracyNoise=float.Parse(Params[8], CultureInfo.InvariantCulture);
                    MinAccuracy=float.Parse(Params[9], CultureInfo.InvariantCulture);
                    MaxAccuracy=float.Parse(Params[10], CultureInfo.InvariantCulture);
                    MaxAge=System.Convert.ToInt32(Params[11]);
                    InitialLearningThreshold=float.Parse(Params[12], CultureInfo.InvariantCulture);
                    InheritedLearningThresholdNoise=float.Parse(Params[13], CultureInfo.InvariantCulture);
                    MinLearningThreshold=float.Parse(Params[14], CultureInfo.InvariantCulture);
                    MaxLearningThreshold=float.Parse(Params[15], CultureInfo.InvariantCulture);
                    InitialChancetoInvent=float.Parse(Params[16], CultureInfo.InvariantCulture);
                    InheritedChancetoInventNoise=float.Parse(Params[17], CultureInfo.InvariantCulture);
                    MinChancetoInvent=float.Parse(Params[18], CultureInfo.InvariantCulture);
                    MaxChancetoInvent=float.Parse(Params[19], CultureInfo.InvariantCulture);
                    InitialChancetoForget=float.Parse(Params[20], CultureInfo.InvariantCulture);
                    InheritedChancetoForgetNoise=float.Parse(Params[21], CultureInfo.InvariantCulture);
                    MinChancetoForget=float.Parse(Params[22], CultureInfo.InvariantCulture);
                    MaxChancetoForget=float.Parse(Params[23], CultureInfo.InvariantCulture);
                    ListeningThreshold=float.Parse(Params[24], CultureInfo.InvariantCulture);
                    EncounterSuccess=float.Parse(Params[25], CultureInfo.InvariantCulture);
                    LearningPenalty=float.Parse(Params[26], CultureInfo.InvariantCulture);
                    AgeDeath=System.Convert.ToBoolean(Params[27]);
                    PercentDeath=float.Parse(Params[28], CultureInfo.InvariantCulture);
                    DeathThreshold=float.Parse(Params[29], CultureInfo.InvariantCulture);
                    ChickSurvival=float.Parse(Params[30], CultureInfo.InvariantCulture);
                    if(Params[31] != "NA\r"){
                        InitialSurvival=float.Parse(Params[31], CultureInfo.InvariantCulture);
                    }
                    LocalBreeding=System.Convert.ToBoolean(Params[32]);
                    LocalTutor=System.Convert.ToBoolean(Params[33]);
                    ConsensusStrategy=System.Convert.ToBoolean(Params[34]);                                     
                    Add=System.Convert.ToBoolean(Params[35]);                  
                    Forget=System.Convert.ToBoolean(Params[36]);                  
                    NumTutorConsensusStrategy=System.Convert.ToInt32(Params[37]);
                    OverLearn=System.Convert.ToBoolean(Params[38]);
                    NumTutorOverLearn=System.Convert.ToInt32(Params[39]);
                    ObliqueLearning=System.Convert.ToBoolean(Params[40]);
                    VerticalLearningCutOff=float.Parse(Params[41], CultureInfo.InvariantCulture);
                    RepertoireSizePreference=float.Parse(Params[42], CultureInfo.InvariantCulture);
                    LogScale=System.Convert.ToBoolean(Params[43]);
                    MatchPreferenece=float.Parse(Params[44], CultureInfo.InvariantCulture);
                    NoisePreference=float.Parse(Params[45], CultureInfo.InvariantCulture);
                    MatchUniform=System.Convert.ToBoolean(Params[46]);
                    //MatchScale=System.Convert.ToInt32(Params[47]);
                    NumDialects=System.Convert.ToInt32(Params[48]);
                    MaleDialects=Params[49].Replace("\r", "");
                    FemaleEvolution=System.Convert.ToBoolean(Params[50]);
                    ChooseMate=System.Convert.ToBoolean(Params[51]);
                    SaveMatch=System.Convert.ToBoolean(Params[52]);
                    SaveAccuracy=System.Convert.ToBoolean(Params[53]);
                    SaveLearningThreshold=System.Convert.ToBoolean(Params[54]);
                    SaveChancetoInvent=System.Convert.ToBoolean(Params[55]);
                    SaveChancetoForget=System.Convert.ToBoolean(Params[56]);
                    SaveNames=System.Convert.ToBoolean(Params[57]);
                    SaveAge=System.Convert.ToBoolean(Params[58]);
                    SaveMSong=System.Convert.ToBoolean(Params[59]);
                    SaveFSong=System.Convert.ToBoolean(Params[60]);
                    //SimStep=System.Convert.ToInt32(Params[61]);
                    NumSim=System.Convert.ToInt32(Params[62]);
                    Seed=System.Convert.ToInt32(Params[63]);

            }else{
                Rows = rows; Cols = cols; NumBirds = rows*cols; Steps = steps;
                InitialSyllableRepertoireSize = initialSyllableRepertoireSize;
                PercentSyllableOverhang = percentSyllableOverhang;
                MaxSyllableRepertoireSize = maxSyllableRepertoireSize;
                InitialAccuracy = initialAccuracy; InheritedAccuracyNoise = inheritedAccuracyNoise;
                MaxAccuracy = maxAccuracy; MinAccuracy = minAccuracy; MaxAge = maxAge;
                InitialLearningThreshold = initialLearningThreshold;
                InheritedLearningThresholdNoise = inheritedLearningThresholdNoise;

                if(maxLearningThreshold == default(float)){
                   MaxLearningThreshold = MaxAge; 
                } else {MaxLearningThreshold = maxLearningThreshold;}

                MinLearningThreshold = minLearningThreshold;
                InitialChancetoInvent = initialChancetoInvent;
                InheritedChancetoInventNoise = inheritedChancetoInventNoise;
                MaxChancetoInvent = maxChancetoInvent; MinChancetoInvent = minChancetoInvent;
                InitialChancetoForget = initialChancetoForget;
                InheritedChancetoForgetNoise = inheritedChancetoForgetNoise;
                ListeningThreshold = listeningThreshold; EncounterSuccess= encounterSuccess;
                LearningPenalty = learningPenalty; DeathThreshold = deathThreshold;
                AgeDeath = ageDeath; PercentDeath=percentDeath;
                ChickSurvival = chickSurvival; MatchUniform = matchUniform;
                VerticalLearningCutOff=verticalLearningCutOff;
                MinChancetoForget =  minChancetoForget; SaveMSong = saveMSong;
                MaxChancetoForget = maxChancetoForget; SaveFSong = saveFSong;
                
                InitialSurvival = CalculateProportion(NumBirds, DeathThreshold, 
                                                       ChickSurvival, MaxAge);

                LocalBreeding = localBreeding; LocalTutor= localTutor;
                if(learningStrategy == "Consensus"){
                    ConsensusStrategy = true;
                    Add = true;
                    Forget =true;
                }else{
                    if(learningStrategy == "Add" || learningStrategy == "AddForget"){
                        Add = true;
                    }
                    if(learningStrategy == "Forget" || learningStrategy == "AddForget"){
                        Forget =true;
                    }
                }
                NumTutorConsensusStrategy = numTutorConsensusStrategy;
                OverLearn = overLearn; NumTutorOverLearn = numTutorOverLearn;
                RepertoireSizePreference = repertoireSizePreference;
                MatchPreferenece = matchPreferenece; ChooseMate = chooseMate;
                NoisePreference = 1-(RepertoireSizePreference + MatchPreferenece);
                NumDialects = numDialects; MaleDialects = maleDialects;
                FemaleEvolution = femaleEvolution; LogScale = logScale;
                SaveMatch = TestRequirement(saveMatch, matchPreferenece, femaleEvolution);
                SaveAccuracy=TestRequirement(saveAccuracy, InheritedAccuracyNoise);
                SaveLearningThreshold = TestRequirement(saveLearningThreshold, InheritedLearningThresholdNoise);
                SaveChancetoInvent = TestRequirement(saveChancetoInvent, InheritedChancetoInventNoise);
                SaveChancetoForget = TestRequirement(saveChancetoForget, InheritedChancetoForgetNoise);
                SaveNames = saveNames; SaveAge = saveAge; SimStep = 1; ObliqueLearning= obliqueLearning;
                NumSim = numSim; Seed=seed;
            }
            AllSyls = new HashSet<int>(Enumerable.Range(0,MaxSyllableRepertoireSize));
            SongCore = MakeCore(PercentSyllableOverhang, InitialSyllableRepertoireSize);
            if(Seed == 0){Rand = new Random();
            }else{Rand = new Random(Seed);}
        }
        //Song
        private List<float> MakeCore(float PercentSyllableOverhang, int InitialSyllableRepertoireSize){
            //create the probability vector to learn syllables
            List<float> SongCore = Enumerable.Repeat(.9f, InitialSyllableRepertoireSize).ToList();
            int Overhang = Convert.ToInt32(PercentSyllableOverhang * InitialSyllableRepertoireSize);
            SongCore.AddRange(Enumerable.Repeat(.1f, Overhang).ToList());
            SongCore.AddRange(Enumerable.Repeat(.01f, Overhang).ToList());
            return(SongCore);
        }
        //Death/Survival
        float CalculateProportion(int numBirds, float deathThreshold,
        float chickSurvival, int maxAge){
            //calulates the proportion of birds that survive each generation
            double[] coefficients = new double[maxAge+1];
            coefficients[0] = deathThreshold-numBirds;
            for(int i = 1; i < coefficients.Length - 1; i++){
                coefficients[i] = (double)deathThreshold;
            }
            coefficients[coefficients.Length-1] = (deathThreshold/chickSurvival)+deathThreshold;

            Func<double, double> f = x => polynomial(coefficients, x);
            double root = MathNet.Numerics.FindRoots.OfFunction(f, 1.0, 2, 0.001, 1000);
            if (root < 0){
                throw new Exception("Couldn't find a root!!!");
            }
            root = 1.0/root;
            return (float)root;
        }
        private static double polynomial(double[] coefficients, double in_x){
            //solves polynomials for survival curves
            double value = coefficients[0];
            int length = coefficients.Length - 1;
            double x = in_x;
            for (int i = 1; i <= length; i++){
                value += x * coefficients[i];
                x *= in_x;
            }
            return value;
        }
        bool TestRequirement(bool? Test, float Dependancy1 = 0f, bool Dependancy2 = false){
            if(Test == null){
                if(Dependancy1 == 0 && !Dependancy2){return(false);}
                return(true);
            }else{return(Convert.ToBoolean(Test));}
        }

        //Sampling
        public float NextFloat(){
            //for random number generation throughout the simmulation
            return (float)Rand.NextDouble();
        }
        public int NextN(int N){
            //for random number generation throughout the simmulation
            return Rand.Next(N);
        }
        public int[] randomSampleEqualNoReplace(List<int> list, int k){
            //warning, index will get changed by this code!
            if(k > list.Count){
                throw new ArgumentOutOfRangeException("Cannot sample indexs that do not exist.");
            }
            int[] Chosen = new int[k];
            int Drawn;          
            /*int Original;
            int Max = list.Count;
            for(int i=0;i<k;i++){
                Drawn = (int)Math.Floor(this.NextN(Max))+i;
                Original = list[i];
                Chosen[i] = list[Drawn];
                list[Drawn] = Original;
                Max -= 1;
            }
            return(Chosen);*/
            for(int i=0;i<k;i++){
                Drawn = this.NextN(list.Count);
                Chosen[i] = list[Drawn];
                list.RemoveAt(Drawn);
            }
            return(Chosen);
        }
        public int[] randomSampleEqualReplace(List<int> list, int k){
            int[] Chosen = new int[k];
            for(int i=0;i<k;i++){
                Chosen[i] = this.NextN(list.Count);
            }
            return(Chosen);
        }
        /*public int[] randomSampleUnequalReplace(float[] probs, int n){
            int[] Chosen = new int[n];
            float TotalWeight = probs.Sum();
            float Drawn;
            int Right;
            int Left; 
            int Halfway = (int)Math.Ceiling((decimal)probs.Length/2);
            int Loc;

            probs[0] = probs[0]/TotalWeight;
            for(int i=1;i<probs.Length;i++){
                probs[i] = probs[i]/TotalWeight+probs[i-1];
            }

            for(int i=0;i<n;i++){
                Drawn = NextFloat();
                Loc = Halfway;
                Right = probs.Length;
                Left = 0;
                //Implement binary search algorithm
                while(Chosen[i] == 0){
                    if(Drawn <= probs[Loc]){
                        if(Loc == 0){
                            Chosen[i] = Loc;
                            break;  
                        }else if(Drawn > probs[Loc-1]){
                            Chosen[i] = Loc;       
                        }else{
                            Right = Loc-1;
                            Loc = (int)Math.Ceiling((decimal)(Right-Left)/2)+Left;

                        }
                    }else{
                        Left = Loc+1;
                        Loc = (int)Math.Floor((decimal)(Right-Left)/2)+Left;
                    }
                }
            }
            return(Chosen);
        }*/
        public int[] randomSampleUnequal(float[] probs, int n, bool withReplacement = false){
            float[] tree = new float[2 * probs.Length - 1];
            int[] choices = new int[n];
            for (int i = 0; i < probs.Length; i++){
                int j = tree.Length - probs.Length + i;
                float p = probs[i];
                backPropValue(ref tree, j, p);
            }
            for (int i = 0; i < n; i++){
                int j = 0, left = 1, right = 2;
                float r = this.NextFloat() * tree[0];
                float sum = 0;
                while (left < tree.Length){
                if (r < (sum + tree[left])){
                    j = left;
                } else {
                    j = right;
                    sum += tree[left];
                }

                left = 2 * j + 1;
                right = left + 1;
                }
                choices[i] = j - (tree.Length - probs.Length);
                if (!withReplacement){
                backPropValue(ref tree, j, -tree[j]);
                }
            }
            return choices;
        }
        static void backPropValue(ref float[] tree, int j, float v){
            tree[j] += v;
            do{
                if (j % 2 == 0) j = (j - 2) / 2;
                else j = (j - 1) / 2;
                tree[j] += v;
            }
            while (j > 0);
            return;       
        }
    }
}
