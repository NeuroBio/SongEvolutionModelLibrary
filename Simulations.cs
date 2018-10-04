using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Diagnostics;
//using System.Threading;
//using MathNet.Numerics;

namespace SongEvolutionModelLibrary
{  
    public class Simulations
    {
        public static WriteData Basic(SimParams par){
            Population Pop = new Population(par);
            WriteData SimData = new WriteData();
            SimData.Write(par, Pop);
            //Simulation and saving data         
            for(int i=0;i<par.NumSim;i++){
                Pop = BirthDeathCycle.Step(par,Pop);
                SimData.Write(par, Pop);
            }
            return(SimData);
        }
        public static WriteData Interval(SimParams par, int Frequency=200){
            Population Pop = new Population(par);
            WriteData SimData = new WriteData();
            SimData.Write(par, Pop);

            //Simulation and saving data         
            for(int i=0;i<par.NumSim;i++){
                Pop = BirthDeathCycle.Step(par,Pop);
                if(i%Frequency == 0){
                    SimData.Write(par, Pop);
                }
            }
            return(SimData);
        }
    }



}
