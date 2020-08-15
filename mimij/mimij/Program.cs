using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mimij
{
    class Program
    {
        static void Main(string[] args)
        {
            var countLinea = 0;
            Console.WriteLine("Arrastre el archivo de entrada a consola");
            var path = Console.ReadLine().Trim('"');
            var frase = string.Empty;
            using (var sr = new StreamReader(path))
            {
                while ((frase = sr.ReadLine()) != null)
                {
                    countLinea++;
                    var countColumna = 0;
                    var lexema = string.Empty;
                    var aux = string.Empty;
                    foreach (char caracter in frase)
                    {
                        countColumna++;
                        lexema += caracter;
                        var error = Validation.validar(lexema);
                        if (!error)
                        {
                            var paths = Directory.GetCurrentDirectory();
                            Validation.fileWriting(paths, aux, countLinea, countColumna,countColumna, string.Empty);
                            lexema = string.Empty;
                            aux = string.Empty;
                        }
                        else
                        {
                            aux += caracter;
                        }
                    }
                }
            }
            Console.ReadKey();
        }
    }
}
