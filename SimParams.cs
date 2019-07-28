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
                    NumTutorOverLearn, NumDialects, SimStep, MinLearnedSyllables, NumSim, Seed;
        public float PercentSyllableOverhang, InitialAccuracy, InheritedAccuracyNoise,
                    MaxAccuracy, MinAccuracy, InitialLearningThreshold,
                    InheritedLearningThresholdNoise, MaxLearningThreshold, MinLearningThreshold,
                    InitialChancetoInvent, InheritedChancetoInventNoise, MaxChancetoInvent,
                    MinChancetoInvent, InitialChancetoForget, InheritedChancetoForgetNoise,
                    MaxChancetoForget, MinChancetoForget,
                    ListeningThreshold, FatherListeningThreshold,
                    EncounterSuccess, LearningPenalty, PercentDeath,

                    DeathThreshold, ChickSurvival, InitialSurvival,
                    RepertoireSizePreference, MatchPreference,
                    FrequencyPreference, SocialPreference, NoisePreference,
                    InheritedPreferenceNoise,
                    VerticalLearningCutOff,
                    SocialBred, SocialNotBred;
        public string MaleDialects, ConsensusStrategy, MatchStrategy;
        public bool OverLearn, FemaleEvolution, SaveMatch, SaveAccuracy,
                    SaveLearningThreshold, SaveChancetoInvent, SaveChancetoForget,
                    SaveNames, SaveAge, MatchUniform, ObliqueLearning,
                    VerticalLearning, LogScale, SocialCues,
                    ChooseMate, Consensus=false, Add=false, Forget=false,
                    RarePrefered,
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
        float minLearningThreshold = 0f, float initialChancetoInvent = .1f,
        float inheritedChancetoInventNoise = 0f, float maxChancetoInvent=1f,
        float minChancetoInvent = 0f, float initialChancetoForget = .2f,
        float inheritedChancetoForgetNoise = 0f, float listeningThreshold = 7f,
        float fatherListeningThreshold = .999f,  int minLearnedSyl = 7,
        float encounterSuccess = .95f, float learningPenalty = .75f, float deathThreshold = 1,
        bool ageDeath = true, float percentDeath = .1f, bool saveMSong = false,
        float chickSurvival = .3f, bool localBreeding = false, bool localTutor = false,
        string learningStrategy = "Add", string consensusStrategy = "Conform",
        string matchStrategy="Match", int numTutorConsensusStrategy = 8,
        bool overLearn = false, int numTutorOverLearn = 3, bool logScale =true,
        float repertoireSizePreference = 1f, float matchPreference = 0f,
        float frequencyPreference = 0f, bool rarePrefered = true,
        float socialPreference = 0f, float inheritedPreferenceNoise = 0f,
        int numDialects = 1, string maleDialects = "None", bool femaleEvolution = false,
        bool? saveMatch = null, bool? saveAccuracy  = null, bool matchUniform = true,
        bool? saveLearningThreshold  = null, bool? saveChancetoInvent  = null,
        bool? saveChancetoForget  = null, bool saveNames  = false, bool chooseMate = false,
        bool saveAge  = false, int numSim = 1000, int seed = default(int),
        float minChancetoForget = 0f, float maxChancetoForget = 1f, 
        bool obliqueLearning = true, bool verticalLearning = true,
        bool socialCues = false, float socialBred = .9f, float socialNotBred = .1f,
        float verticalLearningCutOff=.25f, bool reload = false,
        string path = default(string), bool errorCheck = true){    
            if(reload){
                //Load in params, string split and assign
                string[] Params = File.ReadAllText(path).Split('\n');
                for(int i=0;i<Params.Length-1;i++){
                    Params[i] = Params[i].Split('=')[1];
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
                    FatherListeningThreshold=float.Parse(Params[25], CultureInfo.InvariantCulture);
                    MinLearnedSyllables=System.Convert.ToInt32(Params[26]);
                    EncounterSuccess=float.Parse(Params[27], CultureInfo.InvariantCulture);
                    LearningPenalty=float.Parse(Params[28], CultureInfo.InvariantCulture);
                    AgeDeath=System.Convert.ToBoolean(Params[29]);
                    PercentDeath=float.Parse(Params[30], CultureInfo.InvariantCulture);
                    DeathThreshold=float.Parse(Params[31], CultureInfo.InvariantCulture);
                    ChickSurvival=float.Parse(Params[32], CultureInfo.InvariantCulture);
                    if(Params[33] != "NA\r"){
                        InitialSurvival=float.Parse(Params[33], CultureInfo.InvariantCulture);
                    }
                    LocalBreeding=System.Convert.ToBoolean(Params[34]);
                    LocalTutor=System.Convert.ToBoolean(Params[35]);
                    Consensus=System.Convert.ToBoolean(Params[36]);
                    ConsensusStrategy=Params[37].Replace("\r", "");                                     
                    Add=System.Convert.ToBoolean(Params[38]);                  
                    Forget=System.Convert.ToBoolean(Params[39]);                  
                    NumTutorConsensusStrategy=System.Convert.ToInt32(Params[40]);
                    OverLearn=System.Convert.ToBoolean(Params[41]);
                    NumTutorOverLearn=System.Convert.ToInt32(Params[42]);
                    ObliqueLearning=System.Convert.ToBoolean(Params[43]);
                    VerticalLearning=System.Convert.ToBoolean(Params[44]);
                    VerticalLearningCutOff=float.Parse(Params[45], CultureInfo.InvariantCulture);
                    RepertoireSizePreference=float.Parse(Params[46], CultureInfo.InvariantCulture);
                    LogScale=System.Convert.ToBoolean(Params[47]);
                    MatchPreference=float.Parse(Params[48], CultureInfo.InvariantCulture);
                    MatchStrategy=Params[49].Replace("\r", "");
                    FrequencyPreference=float.Parse(Params[50], CultureInfo.InvariantCulture);
                    RarePrefered=System.Convert.ToBoolean(Params[51]);
                    SocialPreference=float.Parse(Params[52], CultureInfo.InvariantCulture);
                    InheritedPreferenceNoise=float.Parse(Params[53], CultureInfo.InvariantCulture);
                    NoisePreference=float.Parse(Params[54], CultureInfo.InvariantCulture);
                    MatchUniform=System.Convert.ToBoolean(Params[55]);
                    NumDialects=System.Convert.ToInt32(Params[56]);
                    MaleDialects=Params[57].Replace("\r", "");
                    FemaleEvolution=System.Convert.ToBoolean(Params[58]);
                    ChooseMate=System.Convert.ToBoolean(Params[59]);
                    SocialCues=System.Convert.ToBoolean(Params[60]);
                    SocialBred=float.Parse(Params[61], CultureInfo.InvariantCulture);
                    SocialNotBred=float.Parse(Params[62], CultureInfo.InvariantCulture);
                    SaveMatch=System.Convert.ToBoolean(Params[63]);
                    SaveAccuracy=System.Convert.ToBoolean(Params[64]);
                    SaveLearningThreshold=System.Convert.ToBoolean(Params[65]);
                    SaveChancetoInvent=System.Convert.ToBoolean(Params[66]);
                    SaveChancetoForget=System.Convert.ToBoolean(Params[67]);
                    SaveNames=System.Convert.ToBoolean(Params[68]);
                    SaveAge=System.Convert.ToBoolean(Params[69]);
                    SaveMSong=System.Convert.ToBoolean(Params[70]);
                    SaveFSong=System.Convert.ToBoolean(Params[71]);
                    //SimStep=System.Convert.ToInt32(Params[72]);
                    NumSim=System.Convert.ToInt32(Params[73]);
                    Seed=Params[74]=="NA\r"?
                                0:System.Convert.ToInt32(Params[74]);
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
                ListeningThreshold = listeningThreshold;
                FatherListeningThreshold = fatherListeningThreshold;
                EncounterSuccess = encounterSuccess; MatchStrategy = matchStrategy;
                LearningPenalty = learningPenalty; DeathThreshold = deathThreshold;
                AgeDeath = ageDeath; PercentDeath=percentDeath;
                ChickSurvival = chickSurvival; MatchUniform = matchUniform;
                VerticalLearningCutOff=verticalLearningCutOff;
                MinChancetoForget =  minChancetoForget; SaveMSong = saveMSong;
                MaxChancetoForget = maxChancetoForget; SaveFSong = saveFSong;
                
                InitialSurvival = CalculateProportion(NumBirds, DeathThreshold, 
                                                       ChickSurvival, MaxAge);

                LocalBreeding = localBreeding; LocalTutor= localTutor;
                ConsensusStrategy = consensusStrategy;
                if(learningStrategy == "Consensus"){
                    Consensus = true;
                    Add = true;
                    Forget =true;
                }else{
                    if(learningStrategy == "Add" || learningStrategy == "AddForget"){
                        Add = true;
                    }
                    if(learningStrategy == "Forget" || learningStrategy == "AddForget"){
                        Forget =true;
                    }
                    if(Add==false && Forget==false){
                        throw new System.ArgumentException("learningStrategy must be either: Add, AddForget, Forget, or Consensus");
                    }
                }
                SocialCues = socialCues;
                SocialBred = socialBred;
                SocialNotBred = socialNotBred;
                NumTutorConsensusStrategy = numTutorConsensusStrategy;
                OverLearn = overLearn; NumTutorOverLearn = numTutorOverLearn;
                RepertoireSizePreference = repertoireSizePreference;
                MatchPreference = matchPreference; ChooseMate = chooseMate;
                FrequencyPreference = frequencyPreference; RarePrefered = rarePrefered;
                SocialPreference = socialPreference; InheritedPreferenceNoise = inheritedPreferenceNoise;
                NoisePreference = 1-(RepertoireSizePreference + MatchPreference + FrequencyPreference + SocialPreference);
                NumDialects = numDialects; MaleDialects = maleDialects;
                FemaleEvolution = femaleEvolution; LogScale = logScale;
                SaveMatch = TestRequirement(saveMatch, MatchPreference, femaleEvolution);
                SaveAccuracy = TestRequirement(saveAccuracy, InheritedAccuracyNoise);
                SaveLearningThreshold = TestRequirement(saveLearningThreshold, InheritedLearningThresholdNoise);
                SaveChancetoInvent = TestRequirement(saveChancetoInvent, InheritedChancetoInventNoise);
                SaveChancetoForget = TestRequirement(saveChancetoForget, InheritedChancetoForgetNoise);
                SaveNames = saveNames; SaveAge = saveAge; SimStep = 1; ObliqueLearning=obliqueLearning;
                VerticalLearning = verticalLearning;   MinLearnedSyllables = minLearnedSyl;             
                NumSim = numSim; Seed=seed;
            }
            AllSyls = new HashSet<int>(Enumerable.Range(0,MaxSyllableRepertoireSize));
            SongCore = MakeCore(PercentSyllableOverhang, InitialSyllableRepertoireSize);
            if(Seed == 0){Rand = new Random();
            }else{Rand = new Random(Seed);}
            if(errorCheck){
                ErrorCheck();
            }
        }
        //Song
        private List<float> MakeCore(float percentSyllableOverhang, int initialSyllableRepertoireSize){
            //create the probability vector to learn syllables
            List<float> SongCore = Enumerable.Repeat(.9f, initialSyllableRepertoireSize).ToList();
            int Overhang = Convert.ToInt32(percentSyllableOverhang * initialSyllableRepertoireSize);
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

            Func<double, double> f = x => Polynomial(coefficients, x);
            double root = MathNet.Numerics.FindRoots.OfFunction(f, 1.0, 2, 0.001, 1000);
            if (root < 0){
                throw new Exception("Couldn't find a root!!!");
            }
            root = 1.0/root;
            return (float)root;
        }
        private static double Polynomial(double[] coefficients, double in_x){
            //creates polynomials for survival curves
            double value = coefficients[0];
            int length = coefficients.Length - 1;
            double x = in_x;
            for (int i = 1; i <= length; i++){
                value += x * coefficients[i];
                x *= in_x;
            }
            return value;
        }
        bool TestRequirement(bool? test, float dependancy1 = 0f, bool dependancy2 = false){
            if(test == null){
                if(dependancy1 == 0 && !dependancy2){return(false);}
                return(true);
            }else{return(Convert.ToBoolean(test));}
        }
        private void ErrorCheck(){
            //Arguments outside of intended range
            CheckMin(Rows, "Rows",3);
            CheckMin(Cols, "Cols",3);
            CheckMin(Steps, "Steps",1);
            CheckMin(NumSim, "NumSims",1);
            CheckMin(SocialBred, "SocialBred",.01f);CheckMax(SocialBred, "SocialBred",.99f);
            CheckMin(SocialNotBred, "SocialNotBred",.01f);CheckMax(SocialNotBred, "SocialNotBred",.99f);
            CheckMin(InitialSyllableRepertoireSize, "InitialSyllableRepertoireSize",1f);
            CheckMin(MaxSyllableRepertoireSize, "MaxSyllableRepertoireSize",1f);
            CheckMin(PercentSyllableOverhang, "PercentSyllableOverhang",0f);
            CheckMin(EncounterSuccess,"EncounterSuccess");CheckMax(EncounterSuccess,"EncounterSuccess");
            CheckMin(MinLearnedSyllables,"MinLearnedSyllables", 1);CheckMax(MinLearnedSyllables,"MinLearnedSyllables",MaxSyllableRepertoireSize);
            CheckMin(LearningPenalty,"LearningPenalty");
            CheckMin(NumTutorOverLearn,"NumTutorOverLearn",1f);
            CheckMin(NumTutorConsensusStrategy,"NumTutorConsensusStrategy",2f);
            CheckMin(VerticalLearningCutOff,"VerticalLearningCutOff");CheckMax(VerticalLearningCutOff,"VerticalLearningCutOff");
            CheckMin(PercentDeath,"PercentDeath",.01f);CheckMax(PercentDeath,"PercentDeath",.9f);
            CheckMin(DeathThreshold,"DeathThreshold",.0001f);CheckMax(DeathThreshold,"DeathThreshold",.2f*NumBirds);
            CheckMin(ChickSurvival,"ChickSurvival",.1f);CheckMax(ChickSurvival,"ChickSurvival");
            CheckMin(InheritedPreferenceNoise,"InheritedPreferenceNoise",0f);CheckMax(InheritedPreferenceNoise,"InheritedPreferenceNoise", .5f);

            CheckTrait(InitialAccuracy, InheritedAccuracyNoise, MinAccuracy, MaxAccuracy, "Accuracy");
            CheckTrait(InitialLearningThreshold, InheritedLearningThresholdNoise, MinLearningThreshold, MaxLearningThreshold, "Learning Threshold", MaxAge);
            CheckTrait(InitialChancetoInvent, InheritedChancetoInventNoise, MinChancetoInvent, MaxChancetoInvent, "Chance Invent");
            CheckTrait(InitialChancetoForget, InheritedChancetoForgetNoise, MinChancetoForget, MaxChancetoForget, "Chance Forget");
            
            //Complex Errors
            if(RepertoireSizePreference + MatchPreference + FrequencyPreference + SocialPreference > 1){
                throw new System.ArgumentException("RepertoireSizePreference + MatchPreference + FrequencyPreference + SocialPreference cannot exceed 1.");
            }
            if(InitialSyllableRepertoireSize*(1+2*PercentSyllableOverhang) > MaxSyllableRepertoireSize){
                throw new System.ArgumentException("InitialSYllableRepertoireSize*(1+2*PercentSyllableOverhang) cannot be greater than MaxSyllableRepertoireSize.");
            }            
            if((ListeningThreshold%1 != 0 && ListeningThreshold > 1) ||
                    (ListeningThreshold > MaxSyllableRepertoireSize)  ||
                    ListeningThreshold < 0){
                throw new System.ArgumentException(string.Format("ListeningThreshold must either be an integer from 1 to {0}, or a fraction representing a precentage.  If .999 or greater is typed, it is converted to 100%.", MaxSyllableRepertoireSize));
            }
            if((FatherListeningThreshold%1 != 0 && FatherListeningThreshold > 1) ||
                    (FatherListeningThreshold > MaxSyllableRepertoireSize)  ||
                    FatherListeningThreshold < 0){
                throw new System.ArgumentException(string.Format("FatherListeningThreshold must either be an integer from 1 to {0}, or a fraction representing a precentage.  If .999 or greater is typed, it is converted to 100%.", MaxSyllableRepertoireSize));
            }          
            if(NumDialects < 1 ||
                NumDialects >= NumBirds ||
                NumBirds%NumDialects != 0||
                NumDialects > MaxSyllableRepertoireSize/(InitialSyllableRepertoireSize*(1+2*PercentSyllableOverhang))){
                    throw new System.ArgumentException("Dialects must meet the following criterion: 1) Must be an integer of 1 or greater. 2) Cannot be larger than the number of birds. 3) Must be a factor of the number of birds. 4) Must be less than or equal to MaxSylRepSize/(InitialSylRepSize*(1+2*PrcntSylOverhang)).");
            }
            if(MaleDialects != "None" && MaleDialects != "Similar" && MaleDialects != "Same"){
                throw new System.ArgumentException("MaleDialects must be None, Similar, or Same.");
            }
            if(ConsensusStrategy != "Conform" && ConsensusStrategy != "AllNone" && ConsensusStrategy != "Percentage"){
                throw new System.ArgumentException("ConsensusStrategy must be Conform, AllNone, or Percentage.");
            }
            if(MatchStrategy != "Match" && MatchStrategy != "Presence"){
                throw new System.ArgumentException("MatchStrategyStrategy must be Match or Presence.");
            }
            if(MatchPreference == 0  && SaveMatch == false && SaveFSong == true){
                throw new System.ArgumentException("Cannot save female song unless it is generated. It is not generated unless 1) MatchPrefer > 0, 2) FemaleEvolve == TRUE,or 3) SaveMatch == TRUE.");
            }

            //Warnings
            if(FemaleEvolution==true && MatchPreference == 0){
                Console.WriteLine("Warning: FemaleEvolve implimented only when females have a match preference > 0.");
            }
            if(MatchPreference == 0 && !SaveMatch && MaleDialects != "None"){
                Console.WriteLine("Warning: MaleDialects only implemented when match preference > 0 or SaveMatch is manually set to true.");
            }
            if(DeathThreshold < 1){
                Console.WriteLine("Warning: Small DeathThresholds decrease the chances that any birds will survive the selection process long enough to reach the MaxAge.");
            }
            if(Steps >= Math.Max(Rows, Cols)-1 && (LocalBreeding || LocalTutor)){
                Console.WriteLine("Warning: Steps set to a value that makes Local equivalent to Global.  LocalBreeding and LocalTutor set to false.");
                LocalBreeding = false;
                LocalTutor = false;
            }
            if(EncounterSuccess==0){
                Console.WriteLine("Warning: EncounterSuccess set to zero, so not oblique learning can occur.");
            }
            

        }
        public void CheckMin(float trait, string traitName, float min=0f){
            if(trait < min){
                throw new System.ArgumentException(string.Format("{0} cannot be less than {1}.", traitName,min));
            }
        }
        public void CheckMax(float trait, string traitName, float max=1f){
            if(trait > max){
                throw new System.ArgumentException(string.Format("{0} cannot be More than {1}.", traitName,max));
            }
        }
        private void CheckTrait(float initial, float noise, float min, float max, string name, float absMax=1f){
            if(min >= max){
                throw new System.ArgumentException(string.Format("The min for {0} must be less than the max.", name));
            }
            if(noise > (max-min)/2){
                throw new System.ArgumentException(string.Format("The noise for {0} is too large.", name));
            }
            if(noise < 0){
                throw new System.ArgumentException(string.Format("The noise for {0} cannot be less than 0", name));
            }
            if(min < 0){
                throw new System.ArgumentException(string.Format("The min {0} cannot be less than 0.", name));
            }
            if(max > absMax){
                throw new System.ArgumentException(string.Format("The max {0} cannot be greater than {1}.", name, absMax));
            }
            if(initial > max || initial < min){
                throw new System.ArgumentException(string.Format("The initial value for {0} must be with within the range of its min and max.", name));
            }
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
        public int[] RandomSampleEqualNoReplace(List<int> list, int k){
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
        public int[] RandomSampleEqualReplace(List<int> list, int k){
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
        public int[] RandomSampleUnequal(float[] probs, int n, bool withReplacement = false){
            float[] tree = new float[2 * probs.Length - 1];
            int[] choices = new int[n];
            for (int i = 0; i < probs.Length; i++){
                int j = tree.Length - probs.Length + i;
                float p = probs[i];
                BackPropValue(ref tree, j, p);
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
                BackPropValue(ref tree, j, -tree[j]);
                }
            }
            return choices;
        }
        static void BackPropValue(ref float[] tree, int j, float v){
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
