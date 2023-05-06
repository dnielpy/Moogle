using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace LoadFIles{
class txtData
{
    public txtData(string path){    // =>  'Bob The Constructor'
        Path = path;        //Should have this value => "/home/dnielpy/Documentos/Code/Projects/Moogle/MoogleTest/Database";
        AllFilesPaths = Directory.GetFiles(this.Path);  // => This add all the paths of ALL file on the directory
        Paths = new string[AllFilesPaths.Length];  // => This have only paths of txt files
        Texts = new string[Paths.Length];  // => This contains all Texts of .txt files
        Names = new string[Paths.Length];
        string[] Words; 
        string[] UnrepeatedWords;
        Dictionary<string, int> TokenizedWords;
    }
    
    private string Path{get;set;}
    private string TextVar{get;set;}
    public string[] AllFilesPaths{get;set;}
    public string[] Paths{get;set;}
    public string[] Texts{get;set;}
    public string[] Names{get;set;}
    public string[] Words{get;set;}
    public string[] UnrepeatedWords{get;set;}
    public Dictionary<string, int> TokenizedWords = new Dictionary<string, int>();
    public Dictionary<string, string[]> NamesvsWords = new Dictionary<string, string[]>();
    public Dictionary<string, string[]> NamesvsUnrepeatedWords = new Dictionary<string, string[]>();

    private String[] GetPaths(){      //Take 'this.Path' and return all the paths of txt files          
        //Only returns paths with '.txt' at the end of them
        for (int i = 0; i < this.AllFilesPaths.Length; i++)
        {
            if (this.AllFilesPaths[i].Contains(".txt"))
            {
                this.Paths[i] = this.AllFilesPaths[i];  //Eliminar los primeros 2 valores que son null
            }
        }
        return this.Paths;
    }
    private String[] GetNames(){
        for (int i = 0; i < this.Paths.Length; i++)
        {
            if (this.Paths[i] != null)
            {
                this.Names[i] = this.Paths[i].Replace(@"c:\Users\dnielpy\Documents\Code\GitHub\Moogle!\Moogle\MoogleEngine\Database", "");
            }
        }
        return this.Names;
    }
    private String[] GetTexts(){     //Take this.Paths and return all Texts on the .txt files
        for (int i = 0; i < this.Paths.Length; i++)
        {
            if (this.Paths[i] != null)      //  => The Method 'ReadAllText' can't recive null parameters. So, to fix this i have to add this conditional for exclude elements on Paths array that contains null elements
            {
                this.Texts[i] = File.ReadAllText(this.Paths[i]); //  => Add to array 'Texts' all text off .txt files contains.           
                this.Texts[i] = this.Texts[i].ToLower();
            }
        }
        return this.Texts;
    }
    private String[] GetWords(){     //Take this.Texts and return all this.Words of this texts        
        //Hacer este codigo pero para cada elementos de this.Text y almacenarlo en un diccionario
        char[] delimeters = new char[] {' ', '.', ',', ';', ':', '!', '?',};
        string TextVar = "";
        for (int i = 0; i < this.Texts.Length; i++)
        {   
            TextVar = this.Texts[i];
            if (TextVar != null && this.Names[i] != null)
            {
                NamesvsWords.Add(this.Names[i], TextVar.Split(delimeters, StringSplitOptions.RemoveEmptyEntries));                        
                NamesvsUnrepeatedWords.Add(this.Names[i], NamesvsWords[this.Names[i]].Distinct().ToArray());
            }
        }    
        TextVar = "";
        for (int i = 0; i < this.Texts.Length; i++)
        {
            TextVar += this.Texts[i];
        }  
        Words =  TextVar.Split(delimeters, StringSplitOptions.RemoveEmptyEntries);
        return this.Words;
    }
    private String[] CleanWords(){       //This clean duplicated words
        this.UnrepeatedWords = Words.Distinct().ToArray();
        return this.UnrepeatedWords;
    }
    
    public string[] GetAllData(){
        Stopwatch crono = new Stopwatch();
        crono.Start();
        System.Console.WriteLine("🔋--- Inicializando Moogle ---🔋");
        System.Console.WriteLine("Cargando Datos...0% ⏳");
        GetPaths();
        System.Console.WriteLine("Cargando Datos...10% ⏳");
        GetTexts();
        System.Console.WriteLine("Cargando Datos...20% ⏳");
        GetNames();
        System.Console.WriteLine("Cargando Datos...30% ⏳");
        GetWords();
        System.Console.WriteLine("Cargando Datos...40% ⏳");
        CleanWords();
        System.Console.WriteLine("Cargando Datos...100% ⏳");
        crono.Stop();
        float time = crono.ElapsedMilliseconds / 1000;
        System.Console.WriteLine($"Datos cargados con exito  - {time}/s ✅");
        return this.Texts;
        }
    }
}

