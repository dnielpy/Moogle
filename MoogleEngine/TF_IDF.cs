using System.Collections.Generic;
using System;

namespace TF_IDF
{
    class tfidf{
        public Dictionary<string, Dictionary<string, double>> Name_Words_TF = new Dictionary<string, Dictionary<string, double>>();
        public Dictionary<string, Dictionary<string, double>> Name_Words_IDF = new Dictionary<string, Dictionary<string, double>>();
        public Dictionary<string, Dictionary<string, double>> Name_Words_TFIDF = new Dictionary<string, Dictionary<string, double>>();

        public string[] Names{get;set;}
        public Dictionary<string, string[]> NameVSWorsd = new Dictionary<string, string[]>();
        public Dictionary<string, string[]> NameVSUnrepeatedWords = new Dictionary<string, string[]>();

        public tfidf(Dictionary<string, string[]> namevswords, Dictionary<string, string[]> namevsunrepeatedwords, string[] names, string Query){
            Dictionary<string, Dictionary<string, double>> Name_Words_TF;
            Dictionary<string, Dictionary<string, double>> Name_Words_IDF;
            Dictionary<string, Dictionary<string, double>> Name_Words_TFIDF;

            NameVSWorsd = namevswords;
            NameVSUnrepeatedWords = namevsunrepeatedwords;

            Names = names;
        }

        public double Counter(string word, string[] words){
            double contador = 0;
            for (int i = 0; i < words.Length; i++)
            {
                if (word == words[i])
                {
                    contador++;
                }
            }
            return contador;
        }
        
        public double WhereExist(string word, Dictionary<string, string[]>NombrevsPalabras, string[] Names){
        double DocumentosDodeExiste = 0;
        for (int i = 0; i < Names.Length; i++)
        {
            string[] Documento = NombrevsPalabras[Names[i]];
            for (int x = 0; x < Documento.Length; x++)
            {
                if (Documento[x] == word)
                {
                    DocumentosDodeExiste++;
                    break;
                }
            }
        }
        return DocumentosDodeExiste;
       }

        public void TF(){ 
            System.Console.WriteLine("\nCalculando TF en los siguientes Documentos:");
            for (int i = 0; i < this.Names.Length; i++)
            {
                string[] PalabrasTemp = this.NameVSWorsd[this.Names[i]];
                string[] PalabrasNoRepetidasTemp = this.NameVSUnrepeatedWords[this.Names[i]];
                Dictionary<string, double> NamevsTF = new Dictionary<string, double>();

                //Tengo que tener lo mismo de arriba pero NamevsUnrepeatedWords
                for (int x = 0; x < PalabrasNoRepetidasTemp.Length; x++)
                {
                    NamevsTF.Add(PalabrasNoRepetidasTemp[x], Counter(PalabrasNoRepetidasTemp[x], PalabrasTemp)/PalabrasTemp.Length);            
                }
                System.Console.WriteLine(this.Names[i]);
                this.Name_Words_TF.Add(this.Names[i], NamevsTF);
            }  
        }
        public void IDF(){
            System.Console.WriteLine("\nCalculando IDF en los siguientes Documentos:");
            double cantidad_total_documentos = this.Names.Length;
            for (int i = 0; i < this.Names.Length; i++)
            {
                string[] PalabrasTemp = this.NameVSWorsd[this.Names[i]];
                string[] PalabrasNoRepetidasTemp = this.NameVSUnrepeatedWords[this.Names[i]];
                Dictionary<string, double> NamevsIDF = new Dictionary<string, double>();

                //Tengo que tener lo mismo de arriba pero NamevsUnrepeatedWords
                for (int x = 0; x < PalabrasNoRepetidasTemp.Length; x++)
                {
                    double idf = Math.Log(cantidad_total_documentos/WhereExist(PalabrasNoRepetidasTemp[x], this.NameVSWorsd, this.Names));

                    NamevsIDF.Add(PalabrasNoRepetidasTemp[x], idf);            
                }
                System.Console.WriteLine(this.Names[i]);
                this.Name_Words_IDF.Add(this.Names[i], NamevsIDF);
            }
        
        }
    
        public Dictionary<string, Dictionary<string, double>> MultTFIDF(){
            System.Console.WriteLine("\nMultiplicando valores TFIDF en los siguientes documentos:");
            for (int i = 0; i < this.Names.Length; i++)
            {
                //Diccionarios a iterar
                string[] PalabrasTemp = this.NameVSWorsd[this.Names[i]];
                string[] PalabrasNoRepetidasTemp = this.NameVSUnrepeatedWords[this.Names[i]];
                Dictionary<string, double> WordsvsTF = this.Name_Words_TF[Names[i]];
                Dictionary<string, double> WordsvsIDF = this.Name_Words_IDF[Names[i]];      
                //Diccionario a indexar
                Dictionary<string, double> WordsvsTFIDF = new Dictionary<string, double>();
                
                for (int x = 0; x < PalabrasNoRepetidasTemp.Length; x++)
                {
                    double tf = WordsvsTF[PalabrasNoRepetidasTemp[x]];
                    double idf = WordsvsIDF[PalabrasNoRepetidasTemp[x]];
                    double tfidf = tf*idf;
                    
                    WordsvsTFIDF.Add(PalabrasNoRepetidasTemp[x], tfidf);
                }
                this.Name_Words_TFIDF.Add(this.Names[i], WordsvsTFIDF);
                System.Console.WriteLine(this.Names[i]);
            }
            System.Console.WriteLine("\n-- TFIDF Calculado con exito --");
            return this.Name_Words_TFIDF;
        }
        public void TFIDF(){
            TF();
            IDF();
            MultTFIDF();
        }
    }
}