using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace LoadFIles{
class txtData
{
    public txtData(string path){    // =>  'Bob The Constructor'
        MyPath = path;        //Should have this value => "/home/dnielpy/Documentos/Code/Projects/Moogle/MoogleTest/Database";
        AllFilesPaths = Directory.GetFiles(this.MyPath);  // => This add all the paths of ALL file on the directory
        Paths = new string[AllFilesPaths.Length];  // => This have only paths of txt files
        Texts = new string[Paths.Length];  // => This contains all Texts of .txt files
        Names = new string[Paths.Length];
        string[] Words; 
        string[] UnrepeatedWords;
    }
    private string MyPath{get;set;}
    private string TextVar{get;set;}
    public string[] AllFilesPaths{get;set;}
    public string[] Paths{get;set;}
    public string[] Texts{get;set;}
    public string[] Names{get;set;}
    public string[] Words{get;set;}
    public string[] UnrepeatedWords{get;set;}
    public Dictionary<string, string[]> NamesvsWords = new Dictionary<string, string[]>();
    public Dictionary<string, string[]> NamesvsUnrepeatedWords = new Dictionary<string, string[]>();

    private String[] GetPaths(){
    //Devuelve la ruta de todos los archivos de texto en 'this.AllFilesPaths'
    for (int i = 0; i < this.AllFilesPaths.Length; i++)
    {
        // Comprueba si la ruta del archivo de este índice contiene ".txt"
        if (this.AllFilesPaths[i].Contains(".txt"))
        {
            // Si lo hace, agrega la ruta a la matriz de rutas
            this.Paths[i] = this.AllFilesPaths[i];
        }
    }
    // Devuelve la matriz de rutas actualizada
    return this.Paths;
    }

    // Este método obtiene los nombres de los archivos a partir de sus rutas
    private String[] GetNames(){
    // Iteramos a través de la matriz de rutas
    for (int i = 0; i < this.Paths.Length; i++)
    {
        // Verificamos que la ruta de este índice no sea nula
        if (this.Paths[i] != null)
        {
            // Obtenemos la ruta actual del directorio de trabajo
            string tempath = Path.Combine(Directory.GetCurrentDirectory());
            string mypath = tempath.Replace("MoogleServer", "");
            mypath += @"Content";
            
            // Reemplazamos la sección de la ruta que describe el nombre del archivo con una cadena vacía
            this.Names[i] = this.Paths[i].Replace(mypath, "");
            
            // Eliminamos la extensión de archivo ".txt"
            this.Names[i] = this.Names[i].Replace(".txt", "");
            this.Names[i] = this.Names[i].Replace(@"\", "");

        }
    }
    // Devolvemos la matriz de nombres actualizada
    return this.Names;
    }

    private String[] GetTexts(){     // Toma this.Paths y devuelve todos los Textos en los archivos .txt
        for (int i = 0; i < this.Paths.Length; i++)
        {
            if (this.Paths[i] != null)      //  => El método 'ReadAllText' no puede recibir parámetros nulos. Por lo tanto, para solucionar esto, tengo que agregar esta condición para excluir los elementos en el array Paths que contienen elementos nulos.
            {
                this.Texts[i] = File.ReadAllText(this.Paths[i]); //  => Agrega a la matriz 'Texts' todo el texto de los archivos .txt.           
                this.Texts[i] = this.Texts[i].ToLower();
            }
        }
        return this.Texts;
    }

    // Este método toma el arreglo de textos this.Texts y devuelve todas las palabras de estos textos
    private String[] GetWords(){     
        // Se define un arreglo de caracteres que se utilizarán como delimitadores para separar las palabras
        char[] delimeters = new char[] {' ', '.', ',', ';', ':', '!', '?', '—', '_', '"'};
        string TextVar = "";
        // Se recorre el arreglo de textos this.Texts
        for (int i = 0; i < this.Texts.Length; i++)
        {   
            TextVar = this.Texts[i];
            // Si el texto y el nombre no son nulos
            if (TextVar != null && this.Names[i] != null)
            {
                // Se agrega al diccionario NamesvsWords el nombre y las palabras del texto separadas por los delimitadores
                NamesvsWords.Add(this.Names[i], TextVar.Split(delimeters, StringSplitOptions.RemoveEmptyEntries));                        
                // Se agrega al diccionario NamesvsUnrepeatedWords el nombre y las palabras del texto sin repeticiones
                NamesvsUnrepeatedWords.Add(this.Names[i], NamesvsWords[this.Names[i]].Distinct().ToArray());
            }
        }    
        TextVar = "";
        // Se recorre el arreglo de textos this.Texts
        for (int i = 0; i < this.Texts.Length; i++)
        {
            // Se concatenan los textos en una sola variable
            TextVar += this.Texts[i];
        }  
        // Se separan las palabras de la variable TextVar utilizando los delimitadores definidos anteriormente
        Words =  TextVar.Split(delimeters, StringSplitOptions.RemoveEmptyEntries);
        return this.Words;
    }

    // Este método elimina las palabras duplicadas del arreglo de palabras Words
    private String[] CleanWords(){       
        // Se utiliza el método Distinct() para obtener las palabras sin repeticiones y se convierte el resultado en un arreglo
        this.UnrepeatedWords = Words.Distinct().ToArray();
        return this.UnrepeatedWords;
    }
    
    // Este método obtiene todos los datos necesarios para el funcionamiento de Moogle
    public string[] GetAllData(){
        // Se crea un objeto Stopwatch para medir el tiempo de ejecución del método
        Stopwatch crono = new Stopwatch();
        crono.Start();
        // Se imprimen mensajes en la consola para indicar el progreso de la carga de datos
        System.Console.WriteLine("🔋--- Inicializando Moogle ---🔋");
        System.Console.WriteLine("Cargando Datos...0% ⏳");
        // Se llaman a los métodos GetPaths, GetTexts, GetNames, GetWords y CleanWords para obtener los datos necesarios
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
        // Se detiene el cronómetro y se calcula el tiempo de ejecución del método
        crono.Stop();
        float time = crono.ElapsedMilliseconds / 1000;
        // Se imprime un mensaje en la consola para indicar que los datos se han cargado con éxito
        System.Console.WriteLine($"Datos cargados con exito  - {time}/s ✅");
        // Se devuelve el arreglo de textos this.Texts
        return this.Texts;
        }
    }
}

