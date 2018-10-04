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
    }



}
