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
        public static InvasionData InvasionLrnThrsh(SimParams par, float InvaderStat, int numInvaders=1, int burnIn=500){
            Population Pop = new Population(par);
            //Get to Equilibrium         
            for(int i=0;i<burnIn;i++){
                Pop = BirthDeathCycle.Step(par,Pop);
            }
            
            //Invade
            int[] InvaderIndex = par.randomSampleEqualNoReplace(Enumerable.Range(0,par.NumBirds).ToList(), numInvaders);
            for(int i=0;i<numInvaders;i++){
                Pop.Age[InvaderIndex[i]] = 1;
                Pop.LearningThreshold[InvaderIndex[i]] = InvaderStat;
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
                    Categories = Pop.LearningThreshold.Distinct().Count();
                }
            }
            InvasionData Results = new InvasionData(Counter, Pop.LearningThreshold.Average());
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
    }

}
