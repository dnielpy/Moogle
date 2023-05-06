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
    public static Dictionary<string, double> Cargar(string query){
        LoadFIles.txtData objeto1 = new LoadFIles.txtData(@"c:\Users\dnielpy\Documents\Code\GitHub\Moogle!\Moogle\MoogleEngine\Database");
        objeto1.GetAllData();
    
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
        Dictionary<string, double> Results = Cargar(query);

        List<SearchItem> items = new List<SearchItem>();
        foreach (KeyValuePair<string, double> item in Results)
        {
            items.Add(new SearchItem(item.Key, DocumentvsSnipet[item.Key], (float)item.Value));
        }
        
        SearchItem[] items2 = items.ToArray();

        System.Console.WriteLine("🔋--- Busqueda Realizada Con Exito ---🔋");
        return new SearchResult(items2);
    }
}
