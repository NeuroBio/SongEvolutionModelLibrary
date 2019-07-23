using System;
using System.Linq;
using System.Collections.Generic;

namespace SongEvolutionModelLibrary
{
    public class Songs{
        public List<int>[] FemaleSongs;
        public List<int>[] MaleSongs;
        public float[] Match;

        //Constructor
        public Songs(SimParams par){
            Match = new float[par.NumBirds];
            MaleSongs = new List<int>[par.NumBirds];
            FemaleSongs = new List<int>[par.NumBirds];
            
            if(par.MatchPreference == 0 && !par.SaveMatch){
                for(int i=0;i<par.NumBirds;i++){
                MaleSongs[i] = GenerateNovelSong(par);
                }
            }else{//Matching context
                FemaleSongs = GenerateFemaleSongs(par);
                if(par.MaleDialects=="Same"){
                    for(int i=0;i<MaleSongs.Length;i++){
                        MaleSongs[i] = FemaleSongs[i].ToList();
                    }
                }else{
                    for(int i=0;i<par.NumBirds;i++){
                        MaleSongs[i] = GenerateNovelSong(par);
                    }
                    if(par.MaleDialects=="Similar" && par.NumDialects > 1){
                        MaleSongs = EstablishDialects(par, MaleSongs);
                    }
                }
                if(par.ChooseMate && par.MaleDialects != "Same"){
                    var Assign = AssignFemale(par, MaleSongs, FemaleSongs);                
                    List<int>[] NewMaleSong = new List<int>[par.NumBirds];
                    List<int>[] NewFemaleSong = new List<int>[par.NumBirds];
                    for(int i=0;i<par.NumBirds;i++){
                        NewMaleSong[i] = MaleSongs[Assign.MaleOrder[i]];
                        NewFemaleSong[i] = FemaleSongs[Assign.FemaleOrder[i]];
                    }
                    MaleSongs = NewMaleSong;
                    FemaleSongs = NewFemaleSong;
                    Match = Assign.Match;
                }else{
                    for(int i=0;i<par.NumBirds;i++){
                        Match[i] = Songs.GetMatch(par, MaleSongs[i], FemaleSongs[i]);
                    }
                }
            }
        }
        //Functions for getting the basic songs and setting up dialects
        private static List<int>[] GenerateFemaleSongs(SimParams par){
            List<int>[] FSongs = new List<int>[par.NumBirds]; 
            if(par.MatchUniform){
                FSongs[0] = GenerateNovelSong(par);
                for(int i=1;i<par.NumBirds;i++){
                    FSongs[i] = FSongs[0].ToList(); 
                }
            }else{
                for(int i=0;i<par.NumBirds;i++){
                    FSongs[i] = GenerateNovelSong(par);
                }
            }
            if(par.NumDialects > 1){
                FSongs = EstablishDialects(par, FSongs);
            }
            return(FSongs);
        }
        private static List<int> GenerateNovelSong(SimParams par){
            //Test whether each syllable is learned
            List<int> Song = new List<int>();
            while(Song.Count == 0){
                for(int i=0; i<par.SongCore.Count;i++){
                    if(par.NextFloat() < par.SongCore[i]){
                        Song.Add(i);}
                }
            }
            return(Song);
        }
        private static List<int>[] EstablishDialects(SimParams par, List<int>[] fSongs){
            List<int> Facts = PrimeFactorization(par.NumDialects);

            //get the dimentions of the submatricies for each dialect
            Facts.Reverse();
            int[] Divisors;
            if(Facts.Count() == 1){
                Divisors = new int[] {1,Facts[0]};
            }else{
                int First = Facts[0];
                int Second = Facts[Facts.Count()-1];
                if(Facts.Count>2){
                    for(int i=1;i<(Facts.Count()-1);i++){
                        if(Facts[i]*First < Facts[i]*Second){
                            First = Facts[i]*First;
                        }else{
                            Second = Facts[i]*Second;
                        }
                    }
                }
                if(First < Second){
                    Divisors = new int[] {First, Second};
                }else{Divisors = new int[] {Second,First};}
            }
            if(par.Rows < par.Cols){
                Divisors.Reverse();
            }
            int SplitRow = par.Rows/Divisors[0];
            int SplitCol = par.Cols/Divisors[1];
            int CoreLength = par.InitialSyllableRepertoireSize +
                                2*(int)Math.Ceiling(par.InitialSyllableRepertoireSize*par.PercentSyllableOverhang);
            int Shift;
            //Get the index for each dialect sub matrix
            int k = 1;
            for(int i=0;i<Divisors[0];i++){
                for(int j=0;j<Divisors[1];j++){
                    if(i==0 && j==0){continue;}
                    List<int> RowSet = Enumerable.Range(SplitRow*i,SplitRow).ToList();
                    List<int> ColSet = Enumerable.Range(SplitCol*j, SplitCol).ToList();
                    for(int l=0;l<ColSet.Count();l++){
                        ColSet[l] = ColSet[l]*par.Rows;
                    }
                    List<int> Index = new List<int> {};
                    for(int l=0;l<ColSet.Count();l++){
                        for(int m=0;m<RowSet.Count();m++){
                            Index.Add(ColSet[l]+RowSet[m]);
                        }
                    }
                    //Use Index to shift songs
                    Shift = CoreLength*k;
                    for(int l=0;l<Index.Count();l++){
                        for(int m=0;m<fSongs[Index[l]].Count;m++){
                            fSongs[Index[l]][m] += Shift;
                        }
                    }
                    k = k+1;
                }
            }
            return(fSongs);
        }
        private static List<int> PrimeFactorization(int birds){
            List<int> PrimeFactors= new List<int> {};
            for (int b=2;birds>1; b++){
                if (birds%b == 0){

                    int x = 0;
                    while (birds%b == 0){
                        birds/=b;
                        x++;
                    }
                    PrimeFactors.AddRange(Enumerable.Repeat(b,x).ToList());
                    for(int i=0;i<PrimeFactors.Count();i++){
                    }
                }
            }
            return(PrimeFactors);
        }

        //Matching functions and assigning females
        public static float GetMatch(SimParams par, List<int> maleSong,
        List<int> femaleSong){
            //Calculate the match
            IEnumerable<int> Compare = femaleSong.Intersect(maleSong);
            float FSyls = femaleSong.Count;
            float Missing = FSyls-Compare.Count();
            float Extra = Math.Max(maleSong.Count-FSyls,0);
            float Mistakes = ((Extra+Missing)/FSyls);
            float Match = Math.Max(1f-Mistakes,0);
            return(Match);
        }
        public static Population ChooseMates(SimParams par, Population pop){
           //All females can pick a new mate
            var Assign = AssignFemale(par, pop.MaleSong, pop.FemaleSong);                
            List<int>[] NewMaleSong = new List<int>[par.NumBirds];
            List<int>[] NewFemaleSong = new List<int>[par.NumBirds];
            for(int i=0;i<par.NumBirds;i++){
                NewMaleSong[i] = pop.MaleSong[Assign.MaleOrder[i]];
                NewFemaleSong[i] = pop.FemaleSong[Assign.FemaleOrder[i]];
            }
            pop.MaleSong = NewMaleSong;
            pop.FemaleSong = NewFemaleSong;
            pop.Match = Assign.Match;
            return(pop);
        }
        private static AssignResults AssignFemale(SimParams par, List<int>[] maleSong,
        List<int>[] femaleSong){
            int[] Order = new int[femaleSong.Count()];
            float[] Match = new float[femaleSong.Count()];
            List<int> Available = Enumerable.Range(0,par.NumBirds).ToList();
            float[] probability = Enumerable.Repeat(0f,femaleSong.Count()).ToArray();
            int[] ScrambleFemales = par.RandomSampleUnequal(probability, femaleSong.Count(),false);
            for(int i=0;i<femaleSong.Count()-1;i++){
                List<float> TempMatches = new List<float> {};
                for(int j=0;j<Available.Count();j++){
                    TempMatches.Add(GetMatch(par, maleSong[Available[j]], femaleSong[ScrambleFemales[i]]));
                }
                List<float> ModifyMatches = TempMatches;
                List<int> WrongSong = Enumerable.Range(0, TempMatches.Count()).Where(x => TempMatches[x] == 0f).ToList();
                for(int j=0;j<WrongSong.Count();j++){
                    ModifyMatches[WrongSong[j]] = 0.001f;
                }
                float[] TempProbability = ModifyMatches.ToArray();
                int[] Chosen = par.RandomSampleUnequal(TempProbability, 1,true);
                Order[i] = Available[Chosen[0]];
                Match[i] = TempMatches[Chosen[0]];
                Available.RemoveAt(Chosen[0]);
            }
            //Assign remaining info
            Match[femaleSong.Length-1] = GetMatch(par, maleSong[Available[0]], femaleSong[ScrambleFemales[femaleSong.Length-1]]);
            Order[femaleSong.Length-1] = Available[0];
            AssignResults Results = new AssignResults(Match, Order, ScrambleFemales);
            return(Results);
        }
        private struct AssignResults{
            public float[] Match;
            public int[] MaleOrder;
            public int[] FemaleOrder;
            public AssignResults(float[] match, int[] maleOrder, int[] femaleOrder){
                Match=match;
                MaleOrder=maleOrder;
                FemaleOrder=femaleOrder;
            }

        }
    }
}