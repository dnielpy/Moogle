//dotnet watch run --project MoogleServer
using LoadFIles;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace MoogleEngine;
public static class Moogle
{
    static Dictionary<string, string> DocumentvsSnipet = new Dictionary<string, string>();
    static LoadFIles.txtData objeto1; // Variable estática para almacenar la instancia de txtData
    
    // Este método carga los datos de la base de datos
    public static Dictionary<double, string> Cargar(string query){
        // Limpiar el diccionario DocumentvsSnipet.
        DocumentvsSnipet.Clear();

        // Si la instancia de txtData no ha sido creada, la creamos.
        if (objeto1 == null)
        {
            // Obtener la ruta de la carpeta Content.
            string tempath = Path.Combine(Directory.GetCurrentDirectory());
            string mypath = tempath.Replace("MoogleServer", "");
            mypath += @"Content";

            // Crear una instancia de txtData y cargar los datos de la base de datos.
            objeto1 = new LoadFIles.txtData(mypath);
            objeto1.GetAllData();
        }
    
        // Crear una instancia de tfidf y calcular el TFIDF de los documentos.
        TF_IDF.tfidf objeto2 = new TF_IDF.tfidf(objeto1.NamesvsWords, objeto1.NamesvsUnrepeatedWords, objeto1.Names);
        objeto2.TFIDF();

        // Tratar la consulta del usuario y calcular el TFIDF de las palabras de la consulta.
        objeto2.QueryTreatment(query);
        objeto2.QueryTFIDF();

        // Calcular la similitud coseno entre los documentos y la consulta.
        objeto2.CosSimilitude();

        // Agregar los snippets de los documentos a DocumentvsSnipet.
        foreach (var snipet in objeto2.Snippet)
        {
            if (DocumentvsSnipet.ContainsKey(snipet.Key))
            {
                break;
            }
            else
            {
                DocumentvsSnipet.Add(snipet.Key, snipet.Value);            
            }
        }
        // Devolver un diccionario que contiene los resultados de la búsqueda.
        return objeto2.Results;
    }

    // Realiza una búsqueda en la base de datos y devuelve los resultados en forma de objeto SearchResult.
    public static SearchResult Query(string query) {
        // Cargar los resultados de la búsqueda en un diccionario.
        Dictionary<double, string> Results = Cargar(query);
        // Ordenar los resultados por score.
        var ResultadosOrdenados = Results.OrderByDescending(x => x.Value);      

        // Crear una lista de objetos SearchItem para almacenar los resultados.
        List<SearchItem> items = new List<SearchItem>();

        // Iterar a través de los resultados ordenados y agregarlos a la lista de objetos SearchItem.
        foreach (KeyValuePair<double, string> item in ResultadosOrdenados)
        {
            items.Add(new SearchItem(item.Value, DocumentvsSnipet[item.Value], (float)item.Key));
        }
        // Convertir la lista de objetos SearchItem en un arreglo de objetos SearchItem.
        SearchItem[] items2 = items.ToArray();

        System.Console.WriteLine("🔋--- Busqueda Realizada Con Exito ---🔋");

        static string Suggestion(string query){
            string suggestion = "";

            //Implementar la distancia de Levenshtein para sugerir una palabra entre las palabras de la consulta y las palabras de los documentos.
            static int LevenshteinDistance(string s1, string s2)
            {
                int m = s1.Length;
                int n = s2.Length;
                int[,] dp = new int[m + 1, n + 1];

                for (int i = 0; i <= m; i++)
                    dp[i, 0] = i;

                for (int j = 0; j <= n; j++)
                    dp[0, j] = j;

                for (int i = 1; i <= m; i++)
                {
                    for (int j = 1; j <= n; j++)
                    {
                        if (s1[i - 1] == s2[j - 1])
                            dp[i, j] = dp[i - 1, j - 1];
                        else
                            dp[i, j] = 1 + Math.Min(dp[i - 1, j], Math.Min(dp[i, j - 1], dp[i - 1, j - 1]));
                    }
                }
                return dp[m, n];
            }
            
            int[] Distancias = new int [objeto1.Words.Length];
            for (int i = 0; i < objeto1.Words.Length; i++)
            {
                Distancias[i] = LevenshteinDistance(query, objeto1.Words[i]);
            }
            int min = Distancias.Min();
            int index = Array.IndexOf(Distancias, min);
            suggestion = objeto1.Words[index];
            
            return suggestion;
        }
        string suggestion = Suggestion(query);

        if (query == suggestion)
        {   
            SearchItem[] items4 = items2;
            return new SearchResult(items2);
        }
        else
        {
        // Devolver un objeto SearchResult que contiene los resultados de la búsqueda.
        return new SearchResult(items2, Suggestion(query));            
        }
    }
}
