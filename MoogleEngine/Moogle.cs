//dotnet watch run --project MoogleServer
using LoadFIles;
namespace MoogleEngine;
public static class Moogle
{
    public static string[] Cargar(string query){
        string[] Parametros = new string[2];
        LoadFIles.txtData objeto1 = new LoadFIles.txtData(@"c:\Users\dnielpy\Documents\Code\Projects\Moogle\Moogle!\moogle\MoogleEngine\Database");
        objeto1.GetAllData();

        TF_IDF.tfidf objeto2 = new TF_IDF.tfidf(objeto1.NamesvsWords, objeto1.NamesvsUnrepeatedWords, objeto1.Names);
        objeto2.TFIDF();
        objeto2.QueryTreatment(query);
        objeto2.QueryTFIDF();
        objeto2.CosSimilitude();

        return Parametros;
    }
    public static SearchResult Query(string query) {
        // Modifique este método para responder a la búsqueda
        string[] Parametros = Cargar(query);
        SearchItem[] items = new SearchItem[1] {
            new SearchItem(Parametros[0], Parametros[1], 0.9f),
        };

        return new SearchResult(items, query);
    }
}
