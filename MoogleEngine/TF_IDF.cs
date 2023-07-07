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
        public Dictionary<double, string> Results = new Dictionary<double, string>();
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
            Dictionary<double, string> Results;
            Dictionary<string, string> Snippet;
        }
    // Este método cuenta la cantidad de veces que una palabra aparece en un arreglo de palabras    
    public double Counter(string word, string[] words){
              return words.Count(w => w == word);
    }

    // Este método cuenta la cantidad de documentos en los que aparece una palabra
    public double WhereExist(string word, Dictionary<string, string[]> NombrevsPalabras, string[] Names){
        double DocumentosDodeExiste = 0;
        // Se recorre el arreglo de nombres de documentos Names
        for (int i = 0; i < Names.Length; i++)
        {
            // Se obtiene el arreglo de palabras del documento actual
            string[] Documento = NombrevsPalabras[Names[i]];
            // Se recorre el arreglo de palabras del documento actual
            for (int x = 0; x < Documento.Length; x++)
            {
                // Si la palabra actual es igual a la palabra buscada, se incrementa el contador de documentos donde aparece la palabra
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
        //System.Console.WriteLine("\nCalculando TF en los siguientes Documentos:");
            // Crear un diccionario para almacenar el número de ocurrencias de cada palabra en el documento
            Dictionary<string, double> wordCounts = new Dictionary<string, double>();
            // Inicializar la longitud del documento
            int documentLength = 0;

            // Calcular el número de ocurrencias de cada palabra en el documento y la longitud del documento
            foreach (string[] words in this.NameVSWorsd.Values)
            {
                // Incrementar la longitud del documento
                documentLength += words.Length;
                // Contar el número de ocurrencias de cada palabra
                foreach (string word in words)
                {
                    if (wordCounts.ContainsKey(word))
                    {
                        wordCounts[word]++;
                    }
                    else
                    {
                        wordCounts[word] = 1;
                    }
                }
            }

            // Calcular TF para cada palabra en cada documento
            foreach (string name in this.Names)
            {
                // Obtener las palabras del documento
                string[] words = this.NameVSWorsd[name];
                // Crear un diccionario para almacenar el TF de cada palabra en el documento
                Dictionary<string, double> nameVsTF = new Dictionary<string, double>();

                // Calcular el TF para cada palabra en el documento
                foreach (string word in this.NameVSUnrepeatedWords[name])
                {
                    // Obtener el número de ocurrencias de la palabra
                    double count = wordCounts[word];
                    // Calcular el TF
                    double tf = count / documentLength;
                    // Almacenar el TF en el diccionario
                    nameVsTF[word] = tf;
                }
                // Imprimir el nombre del documento
        //System.Console.WriteLine(name);
                // Almacenar el diccionario de TF para el documento
                this.Name_Words_TF.Add(name, nameVsTF); 
            }  
        }
        public void IDF(){
            // Imprimir mensaje de inicio
        //System.Console.WriteLine("\nCalculando IDF en los siguientes Documentos:");
            // Crear un diccionario para almacenar el número de documentos que contienen cada palabra
            Dictionary<string, int> wordCounts = new Dictionary<string, int>();
            // Crear un diccionario para almacenar el IDF de cada palabra
            Dictionary<string, double> wordIDFs = new Dictionary<string, double>();
            // Obtener el número total de documentos
            double totalDocuments = this.Names.Length;

            // Calcular el número de documentos que contienen cada palabra
            foreach (string[] words in this.NameVSWorsd.Values)
            {
                // Crear un conjunto de palabras únicas en el documento
                HashSet<string> uniqueWords = new HashSet<string>(words);
                // Contar el número de documentos que contienen cada palabra
                foreach (string word in uniqueWords)
                {
                    if (wordCounts.ContainsKey(word))
                    {
                        wordCounts[word]++;
                    }
                    else
                    {
                        wordCounts[word] = 1;
                    }
                }
            }

            // Calcular IDF para cada palabra
            foreach (string word in wordCounts.Keys)
            {
                // Calcular IDF
                double idf = Math.Log(totalDocuments / wordCounts[word]);
                // Almacenar IDF en el diccionario
                wordIDFs[word] = idf;
            }

            // Calcular IDF para cada palabra en cada documento
            foreach (string name in this.Names)
            {
                // Obtener las palabras del documento
                string[] words = this.NameVSWorsd[name];
                // Crear un diccionario para almacenar el IDF de cada palabra en el documento
                Dictionary<string, double> nameVsIDF = new Dictionary<string, double>();

                // Calcular IDF para cada palabra en el documento
                foreach (string word in this.NameVSUnrepeatedWords[name])
                {
                    // Obtener IDF de la palabra
                    double idf = wordIDFs[word];
                    // Almacenar IDF en el diccionario
                    nameVsIDF[word] = idf;
                }

                // Imprimir el nombre del documento
        //System.Console.WriteLine(name);
                // Almacenar el diccionario de IDF para el documento
                this.Name_Words_IDF.Add(name, nameVsIDF);
            }
        }

        public Dictionary<string, Dictionary<string, double>> MultTFIDF(){
            // Imprimir mensaje de inicio
        //System.Console.WriteLine("\nMultiplicando valores TFIDF en los siguientes documentos:");
            // Iterar sobre cada documento
            for (int i = 0; i < this.Names.Length; i++)
            {
                // Obtener las palabras del documento y los diccionarios de TF e IDF
                string[] PalabrasTemp = this.NameVSWorsd[this.Names[i]];
                string[] PalabrasNoRepetidasTemp = this.NameVSUnrepeatedWords[this.Names[i]];
                Dictionary<string, double> WordsvsTF = this.Name_Words_TF[Names[i]];
                Dictionary<string, double> WordsvsIDF = this.Name_Words_IDF[Names[i]];      
                // Crear un diccionario para almacenar los valores TFIDF de cada palabra en el documento
                Dictionary<string, double> WordsvsTFIDF = new Dictionary<string, double>();
                
                // Calcular el valor TFIDF para cada palabra en el documento
                for (int x = 0; x < PalabrasNoRepetidasTemp.Length; x++)
                {
                    // Obtener el valor TF y IDF de la palabra
                    double tf = WordsvsTF[PalabrasNoRepetidasTemp[x]];
                    double idf = WordsvsIDF[PalabrasNoRepetidasTemp[x]];
                    // Calcular el valor TFIDF
                    double tfidf = tf*idf;
                    // Almacenar el valor TFIDF en el diccionario
                    WordsvsTFIDF.Add(PalabrasNoRepetidasTemp[x], tfidf);
                }
                // Almacenar el diccionario de TFIDF para el documento
                this.Name_Words_TFIDF.Add(this.Names[i], WordsvsTFIDF);
                // Imprimir el nombre del documento
        //System.Console.WriteLine(this.Names[i]);
            }
            // Devolver el diccionario de TFIDF para todos los documentos
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
    //Función para calcular TF del query
    void QueryTF(){
        for (int i = 0; i < QueryWords.Length; i++)
        {
            //Si la palabra no está en el diccionario de TF del query, se agrega
            if (Query_TF.ContainsKey(QueryWords[i]) != true)
            {
                //Se calcula el TF de la palabra y se agrega al diccionario de TF del query
                Query_TF.Add(QueryWords[i], Counter(QueryWords[i], QueryWords)/QueryWords.Length);     
            }
        }
    }
    //Función para calcular IDF del query
    void QueryIDF(){
        double cantidad_total_documentos = this.Names.Length;
        for (int i = 0; i < QueryWords.Length; i++)
        {
            //Se calcula el IDF de la palabra y se agrega al diccionario de IDF del query
            double idf = Math.Log(cantidad_total_documentos+1/WhereExist(QueryWords[i], this.NameVSWorsd, this.Names)+1);
            if (Query_IDF.ContainsKey(QueryWords[i]) != true)
            {
                Query_IDF.Add(QueryWords[i], idf);
            }
        }
    }
    //Función para calcular TFIDF del query
    void QueryMultTFIDF(){
        for (int i = 0; i < QueryWords.Length; i++)
        {
            double tf = Query_TF[QueryWords[i]];
            double idf = Query_IDF[QueryWords[i]];
            double tfidf = tf*idf;
            //Se agrega la palabra al diccionario de TFIDF del query si la palabra existe en el query y en los documentos
            if (idf < 50)
            {
                if (Query_TFIDF.ContainsKey(QueryWords[i]) != true)
                {
                    Query_TFIDF.Add(QueryWords[i], tfidf);
                }   
            }
        }
    }
    //Se llaman las funciones para calcular TF, IDF y TFIDF del query
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
                            if (MultTFIDF.ContainsKey(queryword) == false)
                            {
                                MultTFIDF.Add(queryword,Name_Words_TFIDF[name][queryword] * Query_TFIDF[queryword]);
                            }
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
            /*    var items = from pair in CosSimilitud
                            orderby pair.Value descending
                            select pair;
            */
                foreach (var score in CosSimilitud.Keys)
                {
                    if (CosSimilitud[score] > 0)
                    {
                        if (!this.Results.ContainsKey(CosSimilitud[score]))
                        {
                        this.Results.Add(CosSimilitud[score], score);
                        }
                    }
                }
                

                //calcular el snipet con 4 palabras antes y 4 despues de la palabra clave
                foreach (var item in this.Results.Values)
                {
                    // Obtener las palabras y palabras no repetidas del elemento actual
                    string[] words = this.NameVSWorsd[item];
                    string[] unrepeatedwords = this.NameVSUnrepeatedWords[item];

                    // Crear un arreglo de 8 elementos para almacenar el snipet
                    string[] snipet = new string[8];

                    // Buscar la posición de la palabra clave en el arreglo de palabras
                    int index = 0;
                    for (int i = 0; i < words.Length; i++)
                    {
                        if (words[i] == QueryWords[0])
                        {
                            index = i;
                            break;
                        }
                    }
    
                    // Si la posición es menor a 4, tomar las primeras 8 palabras del arreglo               
                    if (index - 4 < 0)
                    {
                        for (int i = 0; i < words.Length; i++)
                        {
                            snipet[i] = words[i];
                        }
                    }
                    // Si la posición es mayor a la longitud del arreglo menos 4, tomar las últimas 8 palabras del arreglo
                    else if (index + 4 > words.Length)
                    {
                        for (int i = words.Length - 8; i < words.Length; i++)
                        {
                            snipet[i] = words[i];
                        }
                    }
                    // Si la posición está en el rango adecuado, tomar las 4 palabras antes y 4 palabras después de la palabra clave
                    else
                    {
                        int contador = 0;
                        for (int i = index - 4; i < index + 4; i++)
                        {
                            snipet[contador] = words[i];
                            contador++;
                        }
                    }

                    // Unir el arreglo de snipet en una sola cadena de texto
                    string snipetvar = string.Join(" ", snipet);

                    // Agregar el snipet al diccionario Snippet con el identificador del elemento
                    this.Snippet.Add(item, snipetvar);
            }
        }
        Similitude();
        }
        public void TFIDF(){
            // Iniciar un cronómetro para medir el tiempo de ejecución
            Stopwatch crono = new Stopwatch();
            crono.Start();
            TF();
            IDF();
            MultTFIDF();
            // Detener el cronómetro y calcular el tiempo de ejecución en segundos
            crono.Stop();
            float time = crono.ElapsedMilliseconds / 1000;

            // Imprimir un mensaje indicando que el cálculo de TFIDF se ha completado con éxito y el tiempo de ejecución
            System.Console.WriteLine($"\nIFIDF Calculado Con Exito  - {time}/s ✅");
        }
    }
}