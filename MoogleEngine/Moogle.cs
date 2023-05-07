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

        // Devolver un objeto SearchResult que contiene los resultados de la búsqueda.
        return new SearchResult(items2);
    }
}
