using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;



String jsonSecretos = System.Environment.GetEnvironmentVariable("json");
String prefijo = System.Environment.GetEnvironmentVariable("prefijo");
String sufijo = System.Environment.GetEnvironmentVariable("sufijo");
String ruta = System.Environment.GetEnvironmentVariable("path");
String validarTodos = System.Environment.GetEnvironmentVariable("path");

bool validarTodoBool = validarTodos.ToLower().Equals("true");

ReplaceTokens(jsonSecretos,
   prefijo, sufijo, ruta);


static void ReplaceTokens(string jsonSecretos, string prefijo, string sufijo, string ruta, bool validarTodos = false)
{
    TextWriter errorWriter = Console.Error;
    try
    {
        IDictionary<string, object> secretos = JsonConvert.DeserializeObject<IDictionary<string, object>>(jsonSecretos);
        Console.WriteLine("Encontrados {0} secretos para reemplazar:", secretos.Count());
        DirectoryInfo d = new DirectoryInfo(Directory.GetCurrentDirectory());
        FileInfo[] archivos = d.GetFiles(ruta);
        foreach (FileInfo archivo in archivos)
        {
            Console.WriteLine("Reemplazando en {0}", archivo.FullName);
            string texto = File.ReadAllText(archivo.FullName);

            IEnumerable<string> tagsOrigen = Regex.Matches(texto, "##(.*?)##").Cast<Match>().Select(m => m.Value.ToString());
            Console.WriteLine("\tSe encontraron {0} tags en el archivo {1}", tagsOrigen.Count(), archivo.FullName);
            foreach (var tagOrigen in tagsOrigen.GroupBy(x => x.ToString()))
            {

                string valorSecreto = secretos.FirstOrDefault(x => x.Key == tagOrigen.Key.ToString().Replace(prefijo, "").Replace(sufijo, "")).Key;
                if (valorSecreto != null)
                {
                    texto = texto.Replace(tagOrigen.Key.ToString(), valorSecreto.ToString());
                }
                else
                {
                    if (validarTodos)
                    {
                        errorWriter.WriteLine("\tTag {0} no encontrado", tagOrigen.Key.ToString());
                        Environment.Exit(1);
                    }
                    else
                    {
                        Console.WriteLine("\tTag {0} no encontrado", tagOrigen.Key.ToString());
                    }

                }
            }
            File.WriteAllText("reemplazo_" + archivo.Name, texto);
        }
    }
    catch (Exception e)
    {
        errorWriter.WriteLine("ERROR: {0}", e.Message);
        Environment.Exit(1);
    }



}



