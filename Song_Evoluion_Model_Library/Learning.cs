using System;
using System.Linq;
using System.Collections.Generic;
//using System.Collections;

namespace SongEvolutionModelLibrary
{
    public static class Learning{
        /*Functions for decideing how birds learn and
        allowing them to do so*/

        //Main process
        public static List<int> CoreLearningProcess(SimParams par, Population pop,
        int learner, List<int> learnerSong, List<int> tutorSong){
            //Copy Tutor Syllables
            List<float> Rolls = new List<float> {};
            float AccRoll;
            for(int i=0;i<tutorSong.Count;i++){
                AccRoll = par.NextFloat();
                if(AccRoll < pop.Accuracy[learner]){//Correct
                    learnerSong.Add(tutorSong[i]);
                }else{Rolls.Add(AccRoll);}
            }


            //Innovation
            if(Rolls.Count>0){
                int Innovation = 0;
                float InventThresh = 1-((1-pop.Accuracy[learner])*pop.ChanceInvent[learner]);
                for(int i=0;i<Rolls.Count;i++){
                    if(Rolls[i] >= InventThresh){
                    Innovation += 1;
                    }
                }
                if(Innovation > 0){
                    int[] NewSyls;
                    HashSet<int> AvailableSyllables = new HashSet<int>(par.AllSyls);
                    AvailableSyllables.ExceptWith(Enumerable.Concat(learnerSong,tutorSong));
                    if(Innovation >= AvailableSyllables.Count){//Not enough syls for sampling
                        NewSyls = AvailableSyllables.ToArray();
                    }else{// Enough syls for sampling
                        NewSyls=par.RandomSampleEqualNoReplace(AvailableSyllables.ToList(),Innovation);
                    }
                        learnerSong.AddRange(NewSyls);
                }//Otherwise no innovation occured
            }//Otherwise no mistakes were made
            return(learnerSong);    
        }

        //Top Level Organizers
        public static List<int> VerticalLearning(SimParams par, Population pop,
        int fatherInd, int chickInd){
            List<int> NewSong = new List<int>{};
            if(pop.LearningThreshold[chickInd] < par.VerticalLearningCutOff){
                return(NewSong);    
            }
            List<int> FatherSong = ListeningTest(par, pop, new int[] {fatherInd}, par.FatherListeningThreshold)[0];
            List<int> ChickSong = CoreLearningProcess(par, pop, chickInd, NewSong, FatherSong);
            return(ChickSong);
        }

        public static Population ObliqueLearning(SimParams par, Population pop,
        int[] vacant, List<int> notVacant){
            //Get Learners and set up for each learning style                                                        
            List<int> Learners = GetLearners(par, pop, notVacant);
            List<int>[] AddSyls;
            List<int>[] LoseSyls;
            if(par.Consensus){
                List<int>[] Tutors = ChooseMultipleTutors(par, pop, Learners,
                                                    notVacant, par.NumTutorConsensusStrategy);
                var Results = ConsensusLearning(par, pop, Tutors, Learners);
                AddSyls = Results.AddSyls;
                LoseSyls = Results.ConsensusSongs;
            }else{//Add&|Forget Learning
                int[] Tutors = ChooseTutors(par, pop, Learners, notVacant);
                AddSyls = ListeningTest(par, pop, Tutors, par.ListeningThreshold);
                LoseSyls = AddSyls;
            }

            //Add and/or remove syllables as needed and update song-related traits
            if(par.Add){
               AddSyllables (par, pop, Learners, AddSyls);
            }
            if(par.Forget){
               ForgetSyllables (par, pop, Learners, LoseSyls);
            }
            UpdateSongTraits(par, pop, Learners);         
            return(pop);
        }

        //Learning Accessory functions
        private static ConsensusResults ConsensusLearning(SimParams par, Population pop,
        List<int>[] tutors, List<int> learners){
            //Create a consensus based on songs from several males
            List<int>[] ConsensusSongs = new List<int>[learners.Count];
            List<int>[] AddSyls = new List<int>[learners.Count];
            List<int>[] AllSongs;
            List<int> CollapsedSongs;
            float Conform;
            for(int i=0;i<learners.Count;i++){
                ConsensusSongs[i] = new List<int>{};
                AddSyls[i] = new List<int>{};
                AllSongs = ListeningTest(par, pop, tutors[i].ToArray(), par.ListeningThreshold);
                CollapsedSongs = AllSongs.SelectMany(x => x).ToList();
                ConsensusSongs[i] = CollapsedSongs.Distinct().ToList();
                for(int j=0;j<ConsensusSongs[i].Count();j++){
                    Conform = ConsensusCalc(par, ConsensusSongs[i][j], CollapsedSongs);
                    if(par.NextFloat() < Conform){
                        AddSyls[i].Add(ConsensusSongs[i][j]);
                    }
                }
            }
            ConsensusResults Returnable = new ConsensusResults(AddSyls, ConsensusSongs);
            return(Returnable);           
        }
        private static float ConsensusCalc(SimParams par, int syl, List<int> collapsedSongs){
            float Conform;
            if(par.ConsensusStrategy == "Conform"){
                Conform = collapsedSongs.Count(x => x==syl)/(float)par.NumTutorConsensusStrategy;
                Conform = (float)(Conform - Math.Sin(2*Math.PI*Conform)/(2*Math.PI));
            }else if(par.ConsensusStrategy == "AllNone"){
                Conform = collapsedSongs.Count(x => x==syl)/par.NumTutorConsensusStrategy;
            }else{
                Conform = collapsedSongs.Count(x => x==syl)/(float)par.NumTutorConsensusStrategy;
            }
            return(Conform);
        }
        private struct ConsensusResults{
            public List<int>[] AddSyls;
            public List<int>[] ConsensusSongs;
            public ConsensusResults (List<int>[] addSyls, List<int>[] consensusSongs){
                AddSyls = addSyls;
                ConsensusSongs = consensusSongs;
            }
        }
        private static List<int> GetLearners(SimParams par, Population pop, List<int> notVacant){
            List<int> Capable = TestLearningThreshold(par, pop, notVacant);
            Capable = CheckEncounter(par, Capable);
            if(Capable.Count == 0){Console.WriteLine("Warning: There were no learners.");}//throw new Exception("There were no learners.");}
            return(Capable);
        }
        private static List<int> TestLearningThreshold(SimParams par, Population pop,
        List<int> notVacant){
            /*Get birds with a threshold below the age keepign in mind
            the vertical learning threshold*/
            float ModifiedThresh;
            List<int> Learners = new List<int>{};
            for(int i=0;i<notVacant.Count;i++){
                ModifiedThresh = pop.LearningThreshold[notVacant[i]] >= 1?
                                pop.LearningThreshold[notVacant[i]]:
                                (pop.LearningThreshold[notVacant[i]]-par.VerticalLearningCutOff)/(1-par.VerticalLearningCutOff);
                if(pop.Age[notVacant[i]]+1 <= ModifiedThresh){//full learn to learn
                    Learners.Add(notVacant[i]);
                }else if(pop.Age[notVacant[i]] < ModifiedThresh){//Chance to learn during partical year
                    if(par.NextFloat() < ModifiedThresh-pop.Age[notVacant[i]]){
                        Learners.Add(notVacant[i]);
                    }
                } 
            }      
            return(Learners);
        }
        private static List<int> CheckEncounter(SimParams par, List<int> capable){
            for(int i=capable.Count-1;i>=0;i--){
                if(par.NextFloat() > par.EncounterSuccess){
                    capable.RemoveAt(i);
                }
            }
            return(capable);
        }
        private static Population AddSyllables (SimParams par, Population pop,
        List<int> learners, List<int>[] tutorSyls){
            //Learn heard Syllables
            List<int> Gain;
            for(int i=0;i<learners.Count;i++){
                Gain = tutorSyls[i].Except(pop.MaleSong[learners[i]]).ToList();
                pop.MaleSong[learners[i]] = CoreLearningProcess(par, pop, learners[i],
                                            pop.MaleSong[learners[i]],
                                            Gain);
            }
            return(pop);
        }
        private static Population ForgetSyllables (SimParams par, Population pop,
        List<int> learners, List<int>[] tutorSyls){
            //Forget unheard Syllables
            List<int> Lose;
             for(int i=0;i<learners.Count;i++){
                Lose = pop.MaleSong[learners[i]].Except(tutorSyls[i]).ToList();
                pop.MaleSong[learners[i]] = DropSyllables(par,pop.MaleSong[learners[i]],
                                                            Lose, pop.ChanceForget[i]);
            }
            return(pop);
        }
        private static Population UpdateSongTraits(SimParams par, Population pop, List<int> learners){
            for(int i=0;i<learners.Count;i++){
                pop.SyllableRepertoire[learners[i]] = pop.MaleSong[learners[i]].Count;
                if(par.MatchPreference != 0 || par.SaveMatch){
                    pop.Match[learners[i]] = Songs.GetMatch(par, pop.MaleSong[learners[i]], pop.FemaleSong[learners[i]]);
                }
            }
            return(pop);
        }


        //Getting tutors, the original runs faster, but cannot get more than one tutor
        private static int[] ChooseTutors(SimParams par, Population pop,
        List<int> learners, List<int> notVacant){
            //Picks tutos that are alive, not chicks, and not songless,
            //Males can tutor multiple learners
            
            /*remove chicks and songless birds + any Misc, mark all excluded
            birds as unavailable*/
            HashSet<int> PotentialTutorsTemp = new HashSet<int>(notVacant.Where(x => pop.Age[x] > 0));
            PotentialTutorsTemp.ExceptWith(PotentialTutorsTemp.Where(x => pop.SyllableRepertoire[x] == 0).ToArray());

            //pick Tutors
            int[] Tutors = new int[learners.Count];
            if(par.LocalTutor){
                //Set up unavailable for local testing
                HashSet<int> Unavailable = new HashSet<int>(Enumerable.Range(0, par.NumBirds));
                Unavailable.ExceptWith(PotentialTutorsTemp);
                for(int i=0;i<learners.Count;i++){
                    if(par.SocialCues){
                        Tutors[i] = Locations.GetLocalBirds(par, pop, learners[i], Unavailable, 1, pop.Bred)[0];
                    }else{
                        Tutors[i] = Locations.GetLocalBirds(par, pop, learners[i], Unavailable)[0];
                    }
                }
            }else{//drawn individually so no learner is his own tutor
                List<int> PotentialTutors;
                for(int i=0;i<learners.Count;i++){
                    PotentialTutors = PotentialTutorsTemp.ToList();
                    PotentialTutors.Remove(learners[i]);

                    if(par.SocialCues){
                        float[] TutorProbs = new float[PotentialTutors.Count];
                        for(int j=0;j<TutorProbs.Length;j++){
                            TutorProbs[j] = pop.Bred[PotentialTutors[j]]; 
                        }
                        Tutors[i] = PotentialTutors[par.RandomSampleUnequal(TutorProbs, 1)[0]];
                    }else{
                        Tutors[i] = PotentialTutors[par.RandomSampleEqualReplace(PotentialTutors, 1)[0]];
                    }
                }
            }
            return(Tutors);
        }
        private static List<int>[] ChooseMultipleTutors(SimParams par, Population pop,
        List<int> learners, List<int> notVacant, int numTutors){
            /*remove chicks and songless birds + any Misc, mark all excluded
            birds as unavailable*/
            HashSet<int> PotentialTutorsTemp = new HashSet<int>(notVacant.Where(x => pop.Age[x] > 0));
            PotentialTutorsTemp.ExceptWith(PotentialTutorsTemp.Where(x => pop.SyllableRepertoire[x] == 0).ToArray());
            List<int>[] Tutors = new List<int>[learners.Count];
            
            if(par.LocalTutor){
                HashSet<int> Unavailable = new HashSet<int>(Enumerable.Range(0, par.NumBirds));
                Unavailable.ExceptWith(PotentialTutorsTemp);
                for(int i=0;i<learners.Count;i++){
                    if(par.SocialCues){
                        Tutors[i] = Locations.GetLocalBirds(par, pop, learners[i], Unavailable, numTutors, pop.Bred).ToList();
                    }else{
                        Tutors[i] = Locations.GetLocalBirds(par, pop, learners[i], Unavailable, numTutors).ToList();
                    }
                }
            }else{
                List<int> PotentialTutors;
                for(int i=0;i<learners.Count;i++){
                    PotentialTutors = PotentialTutorsTemp.ToList();
                    PotentialTutors.Remove(learners[i]);
                    if(par.SocialCues){
                        float[] TutorProbs = new float[PotentialTutors.Count];
                        for(int j=0;j<TutorProbs.Length;j++){
                            TutorProbs[j] = pop.Bred[PotentialTutors[j]];
                        }
                        int[] ChosenIndex = par.RandomSampleUnequal(TutorProbs, numTutors);
                        for(int j=0;j<numTutors;j++){
                            ChosenIndex[j] = PotentialTutors[ChosenIndex[j]];
                        }
                        Tutors[i] = ChosenIndex.ToList();
                    }else{
                        Tutors[i] = par.RandomSampleEqualNoReplace(PotentialTutors, numTutors).ToList();
                    }
                }
            }
            return(Tutors);
        }

        //Adult Learning-Specific Accessory functions
        private static List<int>[] ListeningTest(SimParams par, Population pop, int[] tutors, float lisThresh){
            /*Get syls and test whether Repsize larger than listening threshold,
            if so, randomly remove extra syllables.*/
            List<int>[] TutorSyls = new List<int>[tutors.Length];
            for(int i=0;i<tutors.Length;i++){
                TutorSyls[i] = pop.MaleSong[tutors[i]].ToList();
            }
            if(lisThresh >= .999f & lisThresh <1){
                return(TutorSyls);
            }
            if(lisThresh%1 == 0){//Absolute number of sylls learned
                int Thresh = (int)lisThresh;
                int Remove;
                for(int i=0;i<TutorSyls.Length;i++){
                    if(TutorSyls[i].Count>Thresh){
                        Remove = TutorSyls[i].Count-Thresh;
                        par.RandomSampleEqualNoReplace(TutorSyls[i], Remove);
                    }
                }
            }else{//percentage of sylls learned
                float Learnable;
                float Remove;
                for(int i=0;i<TutorSyls.Length;i++){
                    Learnable = (TutorSyls[i].Count-par.MinLearnedSyllables)*lisThresh + par.MinLearnedSyllables;
                    if(Learnable < TutorSyls[i].Count){
                        Learnable = par.NextFloat() < Learnable%1?
                                (float)Math.Ceiling(Learnable):(float)Math.Floor(Learnable);
                        Remove = TutorSyls[i].Count-Learnable;
                        if(Remove > 0){
                            par.RandomSampleEqualNoReplace(TutorSyls[i], (int)Remove);
                        }
                    }
                }
            }
            return(TutorSyls);
        }
        private static List<int> DropSyllables(SimParams par, List<int> song,
        List<int> droppableSyls, float chanceForget){
            //Remove syllables not heard from tutor based on ChanceForget
            for(int i = 0;i<droppableSyls.Count;i++){
                if(par.NextFloat() < chanceForget){
                    song.Remove(droppableSyls[i]);
                }
            }
            return(song);
        }

        //Misc     
        public static Population OverLearn(SimParams par, Population pop,
        int[] vacant, List<int> notVacant){
            //Get learners and tutors
            List<int> Learners = vacant.ToList();
            List<int>[] Tutors = ChooseMultipleTutors(par, pop, Learners, notVacant, par.NumTutorOverLearn);
            List<int>[] TutorSyls = new List<int>[Learners.Count];
            
            //Create combined song with all sylls
            List<int>[] AllSongs;
            List<int> CollapsedSongs;
            for(int i=0;i<Learners.Count;i++){
                AllSongs = ListeningTest(par, pop, Tutors[i].ToArray(), par.ListeningThreshold);
                CollapsedSongs = AllSongs.SelectMany(x => x).ToList();
                TutorSyls[i] = CollapsedSongs.Distinct().ToList();
            }

            //Add Syls and update song traits
            AddSyllables (par, pop, Learners, TutorSyls);
            UpdateSongTraits(par, pop, Learners);
            return(pop);
        }
    }
}