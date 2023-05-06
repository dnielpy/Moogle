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
    public static Dictionary<double, string> Cargar(string query){
        DocumentvsSnipet.Clear();
        // Si la instancia de txtData no ha sido creada, la creamos
        if (objeto1 == null)
        {
            objeto1 = new LoadFIles.txtData(@"c:\Users\dnielpy\Documents\Code\GitHub\Moogle!\Moogle\MoogleEngine\Database");
            objeto1.GetAllData();
        }
    
        TF_IDF.tfidf objeto2 = new TF_IDF.tfidf(objeto1.NamesvsWords, objeto1.NamesvsUnrepeatedWords, objeto1.Names);
        objeto2.TFIDF();
        objeto2.QueryTreatment(query);
        objeto2.QueryTFIDF();
        objeto2.CosSimilitude();

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

        return objeto2.Results;
    }
    public static SearchResult Query(string query) {
        // Modifique este método para responder a la búsqueda
        Dictionary<double, string> Results = Cargar(query);
        var ResultadosOrdenados = Results.OrderByDescending(x => x.Value);      //Ordenas score

        List<SearchItem> items = new List<SearchItem>();

        foreach (KeyValuePair<double, string> item in ResultadosOrdenados)
        {
            items.Add(new SearchItem(item.Value, DocumentvsSnipet[item.Value], (float)item.Key));
        }
        
        SearchItem[] items2 = items.ToArray();


        System.Console.WriteLine("🔋--- Busqueda Realizada Con Exito ---🔋");
        return new SearchResult(items2);
    }
}
