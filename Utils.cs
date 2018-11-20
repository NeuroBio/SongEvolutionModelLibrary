using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace SongEvolutionModelLibrary{
    public class Utils{
        public static string[] GetValidParams(string path){
            //Get the files in a directory, test if .semp
            //Return all .semps
            string[] PotentialParams = Directory.GetFiles(path);
            string[] File;
            List<string> FinalParams = new List<string>{};
            
            for(int i=0;i<PotentialParams.Length;i++){
                File = PotentialParams[i].Split('.');
                if(File[File.Length-1] == "semp"){
                    FinalParams.Add(PotentialParams[i]);
                }
            }
            return(FinalParams.ToArray());
        }
        public static string GetTag(string fileName){
            fileName = fileName.Replace("\\","/");
            string[] Tag = fileName.Split('/');
            Tag = Tag[Tag.Length-1].Split('.');
            return(Tag[0]);
        }
    }
}