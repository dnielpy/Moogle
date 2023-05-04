//dotnet watch run --project MoogleServer
using LoadFIles;
namespace MoogleEngine;
public static class Moogle
{
    public static void Cargar(string query){
        LoadFIles.txtData objeto1 = new LoadFIles.txtData(@"c:\Users\dnielpy\Documents\Code\Projects\Moogle\Moogle!\moogle\MoogleEngine\Database");
        objeto1.GetAllData();

        TF_IDF.tfidf objeto2 = new TF_IDF.tfidf(objeto1.NamesvsWords, objeto1.NamesvsUnrepeatedWords, objeto1.Names);
        objeto2.TFIDF();
        objeto2.QueryTreatment(query);
        objeto2.QueryTFIDF();
    }

    public static SearchResult Query(string query) {
        // Modifique este método para responder a la búsqueda
        Cargar(query);
        SearchItem[] items = new SearchItem[4] {
            new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.9f),
            new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.5f),
            new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.1f),
            new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.1f),
        };

        return new SearchResult(items, query);
    }
}
