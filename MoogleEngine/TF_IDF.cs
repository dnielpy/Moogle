using System.Collections.Generic;
using System;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace TF_IDF
{
    class tfidf{
        //Diccionarios del tfidf normal    
        public Dictionary<string, Dictionary<string, double>> Name_Words_TF = new Dictionary<string, Dictionary<string, double>>();
        public Dictionary<string, Dictionary<string, double>> Name_Words_IDF = new Dictionary<string, Dictionary<string, double>>();
        public Dictionary<string, Dictionary<string, double>> Name_Words_TFIDF = new Dictionary<string, Dictionary<string, double>>();
        public string[] Names{get;set;}
        public Dictionary<string, string[]> NameVSWorsd = new Dictionary<string, string[]>();
        public Dictionary<string, string[]> NameVSUnrepeatedWords = new Dictionary<string, string[]>();
        
        //Diccionarios del query-tfidf
        public Dictionary<string, double> Query_TF = new Dictionary<string, double>();
        public Dictionary<string, double> Query_IDF = new Dictionary<string, double>();
        public Dictionary<string, double> Query_TFIDF = new Dictionary<string, double>();
        public string Query {get;set; }
        public string[] QueryWords {get;set;}

        //Diccionarios de la similitud
        public Dictionary<string, double> Documents_Similitude = new Dictionary<string, double>();
        public Dictionary<string, double> Documents_SimilitudeCos = new Dictionary<string, double>();

        //Resultados
        public Dictionary<string, double> Results = new Dictionary<string, double>();
        public Dictionary<string, string> Snippet = new Dictionary<string, string>();

        public tfidf(Dictionary<string, string[]> namevswords, Dictionary<string, string[]> namevsunrepeatedwords, string[] names){
            //Diccionarios del tfidf normal
            Dictionary<string, Dictionary<string, double>> Name_Words_TF;
            Dictionary<string, Dictionary<string, double>> Name_Words_IDF;
            Dictionary<string, Dictionary<string, double>> Name_Words_TFIDF;
            
            //Diccionarios del query-tfidf
            Dictionary<string, double> Query_TF;
            Dictionary<string, double> Query_IDF;
            Dictionary<string, double> Query_TFIDF;
            NameVSWorsd = namevswords;
            NameVSUnrepeatedWords = namevsunrepeatedwords;
            Names = names;
            
            //Diccionarios de la similitud
            Dictionary<string, double> Documents_Similitude;
            Dictionary<string, double> Documents_SimilitudeCos;

            //Results
            Dictionary<string, double> Results;
            Dictionary<string, string> Snippet;
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
        public double WhereExist(string word, Dictionary<string, string[]> NombrevsPalabras, string[] Names){
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
            return this.Name_Words_TFIDF;
        }   

       


         
        //A partir de aqui es el trabajo con el query
        public string[] QueryTreatment(string Query){
            //quitar signos de puntuacion a UserQuery
            Query = Query.Replace(",", "");
            //Poner todo userquery en minuscula
            Query = Query.ToLower();
            //separar UserQuery en palabras e indexarlo en QureryWords
            QueryWords = Query.Split(" ");
            
            return QueryWords;
        }
        public void QueryTFIDF(){
            void QueryTF(){
                for (int i = 0; i < QueryWords.Length; i++)
                {
                    if (Query_TF.ContainsKey(QueryWords[i]) != true)
                    {
                        Query_TF.Add(QueryWords[i], Counter(QueryWords[i], QueryWords)/QueryWords.Length);     
                    }
                }
            }
            void QueryIDF(){
                double cantidad_total_documentos = this.Names.Length;
                for (int i = 0; i < QueryWords.Length; i++)
                {
                    double idf = Math.Log(cantidad_total_documentos+1/WhereExist(QueryWords[i], this.NameVSWorsd, this.Names)+1);
                    if (Query_IDF.ContainsKey(QueryWords[i]) != true)
                    {
                        Query_IDF.Add(QueryWords[i], idf);
                    }
                }
            }
            void QueryMultTFIDF(){
                for (int i = 0; i < QueryWords.Length; i++)
                {
                    double tf = Query_TF[QueryWords[i]];
                    double idf = Query_IDF[QueryWords[i]];
                    double tfidf = tf*idf;
                    //hacer un condicional y solo agregar las palabras al diccionario del query si la palabra existe en el query y en los documentos
                    if (idf < 50)
                    {
                        if (Query_TFIDF.ContainsKey(QueryWords[i]) != true)
                        {
                            Query_TFIDF.Add(QueryWords[i], tfidf);
                        }   
                    }
                }
            }
            QueryTF();
            QueryIDF();
            QueryMultTFIDF();
        }
        public void CosSimilitude(){
            void Similitude(){
                //Sacar los tfidf que tienen las palabras del query en la bd en caso de que existan
                Dictionary<string, double> QueryWordsOnBD = new Dictionary<string, double>();
                Dictionary<string, double> Similitud = new Dictionary<string, double>();
                Dictionary<string, double> CosSimilitud = new Dictionary<string, double>();

                foreach (var name in this.Names)
                {
                    Dictionary<string, double> MultTFIDF = new Dictionary<string, double>();
                    foreach (var queryword in QueryWords)
                    {
                        if (Name_Words_TFIDF[name].ContainsKey(queryword))
                        {
                            MultTFIDF.Add(queryword,Name_Words_TFIDF[name][queryword] * Query_TFIDF[queryword]);
                        }
                    }
                    double sum = 0;
                    foreach (var item in MultTFIDF.Values)
                    {
                        sum += item;
                    }
                    Similitud.Add(name, sum);
                }
            //Calcular la similitud del coseno
                foreach (var name in this.Names)
                {
                    double sum = 0;
                    double sum1 = 0;
                    double sum2 = 0;
                    foreach (var item in Name_Words_TFIDF[name].Values)
                    {
                        sum1 += Math.Pow(item, 2);
                    }
                    foreach (var item in Query_TFIDF.Values)
                    {
                        sum2 += Math.Pow(item, 2);
                    }
                    sum = Math.Sqrt(sum1) * Math.Sqrt(sum2);
                    CosSimilitud.Add(name, sum);
                }
                foreach (var name in this.Names)
                {
                    CosSimilitud[name] = Similitud[name] / CosSimilitud[name];
                }
                //Ordenar los valores de mayor a menor
                var items = from pair in CosSimilitud
                            orderby pair.Value descending
                            select pair;
                foreach (var score in CosSimilitud.Keys)
                {
                    if (CosSimilitud[score] > 0)
                    {
                        this.Results.Add(score, CosSimilitud[score]);
                    }
                }

                //calcular el snipet con 4 palabras antes y 4 despues de la palabra clave
                foreach (var item in this.Results.Keys)
                {
                    string[] words = this.NameVSWorsd[item];
                    string[] unrepeatedwords = this.NameVSUnrepeatedWords[item];
                    string[] snipet = new string[8];
                    int index = 0;
                    for (int i = 0; i < words.Length; i++)
                    {
                        if (words[i] == QueryWords[0])
                        {
                            index = i;
                            break;
                        }
                    }
                    if (index - 4 < 0)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            snipet[i] = words[i];
                        }
                    }
                    else if (index + 4 > words.Length)
                    {
                        for (int i = words.Length - 8; i < words.Length; i++)
                        {
                            snipet[i] = words[i];
                        }
                    }
                    else
                    {
                        int contador = 0;
                        for (int i = index - 4; i < index + 4; i++)
                        {
                            snipet[contador] = words[i];
                            contador++;
                        }
                    }
                    string snipetvar = string.Join(" ", snipet);
                    this.Snippet.Add(item, snipetvar);
                }
        }
        Similitude();
        }
        public void TFIDF(){
            Stopwatch crono = new Stopwatch();
            crono.Start();
            TF();
            IDF();
            MultTFIDF();
            crono.Stop();
            float time = crono.ElapsedMilliseconds / 1000;
            System.Console.WriteLine($"\nIFIDF Calculado Con Exito  - {time}/s ✅");
        }
    }
}