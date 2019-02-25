using System;
//using System.Collections.Generic;
using System.Linq;
//using System.Diagnostics;
//using System.Threading;
//using MathNet.Numerics;

namespace SongEvolutionModelLibrary
{  
    public class Simulations
    {
        public static WriteData Basic(SimParams par, bool writeAll = true){
            Population Pop = new Population(par);
            WriteData SimData = new WriteData();
            SimData.Write(par, Pop, writeAll);
            //Simulation and saving data         
            for(int i=0;i<par.NumSim;i++){
                Pop = BirthDeathCycle.Step(par,Pop);
                SimData.Write(par, Pop, writeAll);
            }
            return(SimData);
        }
        public static WriteData Interval(SimParams par, int Frequency=200, bool writeAll = true){
            Population Pop = new Population(par);
            WriteData SimData = new WriteData();
            SimData.Write(par, Pop, writeAll);
            //Simulation and saving data         
            for(int i=0;i<par.NumSim;i++){
                Pop = BirthDeathCycle.Step(par,Pop);
                if((i+1)%Frequency == 0){
                    SimData.Write(par, Pop, writeAll);
                }
            }
            return(SimData);
        }

        public static InvasionData Invasion(SimParams par, string type, float invaderStat, int numInvaders=1, int burnIn=500){
            CheckStatValue(par, type, invaderStat);
            Population Pop = new Population(par);

            //Get to Equilibrium         
            for(int i=0;i<burnIn;i++){
                Pop = BirthDeathCycle.Step(par,Pop);
            }
            
            //Invade
            int[] InvaderIndex = par.RandomSampleEqualNoReplace(Enumerable.Range(0,par.NumBirds).ToList(), numInvaders);
            for(int i=0;i<numInvaders;i++){
                CreateInvader(Pop, type, InvaderIndex[i], invaderStat);
                //Pop.Age[InvaderIndex[i]] = 1;
                //Pop.LearningThreshold[InvaderIndex[i]] = invaderStat;
            }

            //Postinvasion run
            int Counter = 1;
            int Categories = 2;
            while(Categories != 1){
                Counter += 1;
                Pop = BirthDeathCycle.Step(par,Pop);
                if(Counter == 400){
                    break;
                }else{
                    Categories = CountCategories(Pop, type);//Pop.LearningThreshold.Distinct().Count();
                }
            }
            InvasionData Results = new InvasionData(Counter, GetAverage(Pop, type));//Pop.LearningThreshold.Average());
            return(Results);
        }
        public struct InvasionData{
            public int Steps;
            public float TraitAve;
            public InvasionData(int steps = default(int), float traitAve=default(float)){
                Steps = steps;
                TraitAve = traitAve;
            }
        }

        private static void CheckStatValue(SimParams par, string type, float stat){
            if(type=="Learning"){
                if(stat > par.MaxLearningThreshold || stat < par.MinLearningThreshold){
                    throw new System.ArgumentException("Chosen invader stat was outside the boundaries for Learning Threshold.");  
                }
                if(par.InheritedLearningThresholdNoise != 0){
                    Console.WriteLine("Warning: Learning Inheritance noise not set to 0!");
                }
            }else if(type=="Accuracy"){
                if(stat > par.MaxAccuracy || stat < par.MinAccuracy){
                    throw new System.ArgumentException("Chosen invader stat was outside the boundaries for Accuracy.");    
                }
                if(par.InheritedAccuracyNoise != 0){
                    Console.WriteLine("Warning: Accuracy Inheritance noise not set to 0!");
                }
            }else if(type=="Forget"){
                if(stat > par.MaxChancetoForget || stat < par.MinChancetoForget){
                    throw new System.ArgumentException("Chosen invader stat was outside the boundaries for Chance to Forget.");    
                }
                if(par.InheritedChancetoForgetNoise != 0){
                    Console.WriteLine("Warning: Forget Inheritance noise not set to 0!");
                }
            }else if(type=="Invent"){
                if(stat > par.MaxChancetoInvent || stat < par.MinChancetoInvent){
                    throw new System.ArgumentException("Chosen invader stat was outside the boundaries for Chance to Invent.");    
                }
                if(par.InheritedChancetoInventNoise != 0){
                    Console.WriteLine("Warning: Invent Inheritance noise not set to 0!");
                }
            }else{
                throw new System.ArgumentException("Type can only take the following values:  Learning, Accuracy, Forget, or Invent.");
            }
        }
        private static Population CreateInvader(Population pop, string type, int index, float stat){
            pop.Age[index] = 1;
            if(type=="Learning"){
                pop.LearningThreshold[index] = stat;
            }else if(type=="Accuracy"){
                pop.Accuracy[index] = stat;
            }else if(type=="Forget"){
                pop.ChanceForget[index] = stat;
            }else{
                pop.ChanceInvent[index] = stat;
            }
            return pop;
        }

        private static int CountCategories(Population pop, string type){
            if(type=="Learning"){
                return pop.LearningThreshold.Distinct().Count();
            }else if(type=="Accuracy"){
                return pop.Accuracy.Distinct().Count();
            }else if(type=="Forget"){
                return pop.ChanceForget.Distinct().Count();
            }else{
                return pop.ChanceInvent.Distinct().Count();
            }
        }

        private static float GetAverage(Population pop, string type){
            if(type=="Learning"){
                return pop.LearningThreshold.Average();
            }else if(type=="Accuracy"){
                return pop.Accuracy.Average();
            }else if(type=="Forget"){
                return pop.ChanceForget.Average();
            }else{
                return pop.ChanceInvent.Average();
            }
        }
    }

}
