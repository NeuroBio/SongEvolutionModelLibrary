using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;

namespace SongEvolutionModelLibrary
{
    public class WriteData{

        StringBuilder Age = new StringBuilder();
        StringBuilder SylRep = new StringBuilder();
        StringBuilder Match = new StringBuilder();
        StringBuilder Name = new StringBuilder();
        StringBuilder FatherName = new StringBuilder();
        StringBuilder Accuracy = new StringBuilder();
        StringBuilder LearningThreshold = new StringBuilder();
        StringBuilder ChanceInvent = new StringBuilder();
        StringBuilder ChanceForget = new StringBuilder();
        StringBuilder MaleSong = new StringBuilder();
        StringBuilder FemaleSong = new StringBuilder();


        public void Write(SimParams par, Population pop, bool All=true){
            //save the data for each step
            if(All){
                WriteAll(par, pop);
            }else{
                WriteAve(par, pop);
            }
        }
        private void WriteAll(SimParams par, Population pop){
            SylRep.AppendLine(string.Join(",", pop.SyllableRepertoire));
            if(par.SaveMatch){
                Match.AppendLine(string.Join(",", pop.Match));
            }
            if(par.SaveAge){
                Age.AppendLine(string.Join(",", pop.Age));
            }
            if(par.SaveNames){
                Name.AppendLine(pop.Name.ToString());
                FatherName.AppendLine(string.Join(",", pop.FatherName));
            }
            if(par.SaveLearningThreshold){
                LearningThreshold.AppendLine(string.Join(",", pop.LearningThreshold));
            }
            if(par.SaveAccuracy){
                Accuracy.AppendLine(string.Join(",", pop.Accuracy));
            }
            if(par.SaveChancetoForget){
                ChanceForget.AppendLine(string.Join(",", pop.ChanceForget));
            }
            if(par.SaveChancetoInvent){
                ChanceInvent.AppendLine(string.Join(",", pop.ChanceInvent));
            }
            if(par.SaveMSong){
                List<int> MSyls = pop.MaleSong.SelectMany(x => x).ToList();
                int[] SylCount = Enumerable.Repeat(0, par.MaxSyllableRepertoireSize).ToArray();
                for(int i=0; i<MSyls.Count;i++){
                    SylCount[MSyls[i]] += 1;
                }
                MaleSong.AppendLine(string.Join(",", SylCount));
            }
            if(par.SaveFSong){
                List<int> FSyls = pop.FemaleSong.SelectMany(x => x).ToList();
                int[] SylCount = Enumerable.Repeat(0, par.MaxSyllableRepertoireSize).ToArray();
                for(int i=0; i<FSyls.Count;i++){
                    SylCount[FSyls[i]] += 1;
                }
                FemaleSong.AppendLine(string.Join(",", SylCount));
            }
        }
        private void WriteAve(SimParams par, Population pop){
            SylRep.AppendLine(pop.SyllableRepertoire.Average().ToString());
            if(par.SaveMatch){
                Match.AppendLine(pop.Match.Average().ToString());
            }
            if(par.SaveAge){
                Age.AppendLine(pop.Age.Average().ToString());
            }
            if(par.SaveLearningThreshold){
                LearningThreshold.AppendLine(pop.LearningThreshold.Average().ToString());
            }
            if(par.SaveAccuracy){
                Accuracy.AppendLine(pop.Accuracy.Average().ToString());
            }
            if(par.SaveChancetoForget){
                ChanceForget.AppendLine(pop.ChanceForget.Average().ToString());
            }
            if(par.SaveChancetoInvent){
                ChanceInvent.AppendLine(pop.ChanceInvent.Average().ToString());
            }
            if(par.SaveMSong){
                List<int> MSyls = pop.MaleSong.SelectMany(x => x).ToList();
                int[] SylCount = Enumerable.Repeat(0, par.MaxSyllableRepertoireSize).ToArray();
                for(int i=0; i<MSyls.Count;i++){
                    SylCount[MSyls[i]] += 1;
                }
                MaleSong.AppendLine(string.Join(",", SylCount));
            }
            if(par.SaveFSong){
                List<int> FSyls = pop.FemaleSong.SelectMany(x => x).ToList();
                int[] SylCount = Enumerable.Repeat(0, par.MaxSyllableRepertoireSize).ToArray();
                for(int i=0; i<FSyls.Count;i++){
                    SylCount[FSyls[i]] += 1;
                }
                FemaleSong.AppendLine(string.Join(",", SylCount));
            }
        }
        public void Output(SimParams par, string filePath, string tag, bool writePar = true){
            //Make the final .csvs
            File.WriteAllText(filePath+"/"+tag+"SylRep.csv", SylRep.ToString(), Encoding.UTF8);
            if(par.SaveMatch){
                File.WriteAllText(filePath+"/"+tag+"Match.csv", Match.ToString(), Encoding.UTF8);
            }
            if(par.SaveAge){
                File.WriteAllText(filePath+"/"+tag+"Age.csv", Age.ToString(), Encoding.UTF8);
            }
            if(par.SaveNames){
                File.WriteAllText(filePath+"/"+tag+"Name.csv", Name.ToString(), Encoding.UTF8);            
                File.WriteAllText(filePath+"/"+tag+"FatherName.csv", FatherName.ToString(), Encoding.UTF8);            
            }
            if(par.SaveLearningThreshold){
                File.WriteAllText(filePath+"/"+tag+"LrnThrsh.csv", LearningThreshold.ToString(), Encoding.UTF8);
            }
            if(par.SaveAccuracy){
                File.WriteAllText(filePath+"/"+tag+"Acc.csv", Accuracy.ToString(), Encoding.UTF8);
            }
            if(par.SaveChancetoForget){
                File.WriteAllText(filePath+"/"+tag+"ChanFor.csv", ChanceForget.ToString(), Encoding.UTF8);
            }
            if(par.SaveChancetoInvent){
                File.WriteAllText(filePath+"/"+tag+"ChanInv.csv", ChanceInvent.ToString(), Encoding.UTF8);
            }
            if(par.SaveMSong){
                File.WriteAllText(filePath+"/"+tag+"MSong.csv", MaleSong.ToString(), Encoding.UTF8);
            }
            if(par.SaveFSong){
                File.WriteAllText(filePath+"/"+tag+"FSong.csv", FemaleSong.ToString(), Encoding.UTF8);
            }

            //Save parameters is desired
            if(writePar){
            WriteParams(par, filePath, tag);
            }
        }        
        private void WriteParams(SimParams par, string filePath, string tag){
            StringBuilder Par = new StringBuilder();
            Par.AppendLine($"R={par.Rows}");
            Par.AppendLine($"C={par.Cols}");
            Par.AppendLine($"numBirds={par.NumBirds}");
            Par.AppendLine($"Steps={par.Steps}");
            Par.AppendLine($"RSize0={par.InitialSyllableRepertoireSize}");
            Par.AppendLine($"PerROh={par.PercentSyllableOverhang}");
            Par.AppendLine($"MaxRSize={par.MaxSyllableRepertoireSize}");
            Par.AppendLine($"Acc0={par.InitialAccuracy}");
            Par.AppendLine($"IAccN={par.InheritedAccuracyNoise}");
            Par.AppendLine($"MinAcc={par.MinAccuracy}");
            Par.AppendLine($"MaxAcc={par.MaxAccuracy}");
            Par.AppendLine($"MAge={par.MaxAge}");
            Par.AppendLine($"LrnThrsh0={par.InitialLearningThreshold}");
            Par.AppendLine($"ILrnN={par.InheritedLearningThresholdNoise}");
            Par.AppendLine($"MinLrn={par.MinLearningThreshold}");
            Par.AppendLine($"MaxLrn={par.MaxLearningThreshold}");
            Par.AppendLine($"CtI0={par.InitialChancetoInvent}");
            Par.AppendLine($"ICtIN={par.InheritedChancetoInventNoise}");
            Par.AppendLine($"MinCtI={par.MinChancetoInvent}");
            Par.AppendLine($"MaxCtI={par.MaxChancetoInvent}");
            Par.AppendLine($"CtF0={par.InitialChancetoForget}");
            Par.AppendLine($"ICtFN={par.InheritedChancetoForgetNoise}");
            Par.AppendLine($"MinCtF={par.MinChancetoForget}");
            Par.AppendLine($"MaxCtF={par.MaxChancetoForget}");
            Par.AppendLine($"LisThrsh={par.ListeningThreshold}");
            Par.AppendLine($"FLisThrsh={par.FatherListeningThreshold}");
            Par.AppendLine($"MinLrnSyl={par.MinLearnedSyllables}");
            Par.AppendLine($"EnSuc={par.EncounterSuccess}");
            Par.AppendLine($"Lpen={par.LearningPenalty}");
            Par.AppendLine($"DStrat={par.AgeDeath}");
            Par.AppendLine($"PDead={par.PercentDeath}");
            Par.AppendLine($"DeadThrsh={par.DeathThreshold}");
            Par.AppendLine($"Pc={par.ChickSurvival}");
            Par.AppendLine($"InitProp={par.InitialSurvival}");
            Par.AppendLine($"ScopeB={par.LocalBreeding}");
            Par.AppendLine($"ScopeT={par.LocalTutor}");
            Par.AppendLine($"Consen={par.Consensus}");
            Par.AppendLine($"ConsenS={par.ConsensusStrategy}");
            Par.AppendLine($"Add={par.Add}");
            Par.AppendLine($"Forget={par.Forget}");
            Par.AppendLine($"ConNoTut={par.NumTutorConsensusStrategy}");
            Par.AppendLine($"OvrLrn={par.OverLearn}");
            Par.AppendLine($"OLNoTut={par.NumTutorOverLearn}");
            Par.AppendLine($"Obliq={par.ObliqueLearning}");
            Par.AppendLine($"Vert={par.VerticalLearning}");
            Par.AppendLine($"VertLrnCut={par.VerticalLearningCutOff}");
            Par.AppendLine($"RepPref={par.RepertoireSizePreference}");
            Par.AppendLine($"LogScl={par.LogScale}");
            Par.AppendLine($"MatPref={par.MatchPreference}");
            Par.AppendLine($"NoisePref={par.NoisePreference}");
            Par.AppendLine($"UniMat={par.MatchUniform}");
            Par.AppendLine($"MScl=1");
            Par.AppendLine($"Dial={par.NumDialects}");
            Par.AppendLine($"MDial={par.MaleDialects}");
            Par.AppendLine($"FEvo={par.FemaleEvolution}");
            Par.AppendLine($"ChoMate={par.ChooseMate}");
            Par.AppendLine($"SMat={par.SaveMatch}");
            Par.AppendLine($"SAcc={par.SaveAccuracy}");
            Par.AppendLine($"SLrn={par.SaveLearningThreshold}");
            Par.AppendLine($"SCtI={par.SaveChancetoInvent}");
            Par.AppendLine($"SCtF={par.SaveChancetoForget}");
            Par.AppendLine($"SNam={par.SaveNames}");
            Par.AppendLine($"SAge={par.SaveAge}");
            Par.AppendLine($"SMSng={par.SaveMSong}");
            Par.AppendLine($"SFSng={par.SaveFSong}");
            Par.AppendLine($"SimStep=1");
            Par.AppendLine($"nSim={par.NumSim}");
            Par.AppendLine($"Seed={par.Seed}");

            File.WriteAllText(filePath+"/"+tag+"Parameters.semp", Par.ToString());
        }      
        public void ConCat(SimParams par, WriteData newData){
            SylRep.Append(newData.SylRep);
            if(par.SaveMatch){
                Match.Append(newData.Match);
            }
            if(par.SaveAge){
                Age.Append(newData.Age);
            }
            if(par.SaveNames){
                Name.Append(newData.Name);
                FatherName.Append(newData.FatherName);
            }
            if(par.SaveLearningThreshold){
                LearningThreshold.Append(newData.LearningThreshold);
            }
            if(par.SaveAccuracy){
                Accuracy.Append(newData.Accuracy);
            }
            if(par.SaveChancetoForget){
                ChanceForget.Append(newData.ChanceForget);
            }
            if(par.SaveChancetoInvent){
                ChanceInvent.Append(newData.ChanceInvent);
            }
            if(par.SaveMSong){
                MaleSong.Append(newData.MaleSong);
            }
            if(par.SaveFSong){
                FemaleSong.Append(newData.FemaleSong);
            }
        }
    }
}