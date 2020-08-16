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
                    if (frase != "")
                    {
                        var start = 0;
                        var i = 1;
                        var lexema = string.Empty;
                        while (start != frase.Length && i < frase.Length + 1)
                        {
                            lexema = frase.Substring(start, i - start);
                            var tipo = Validation.validar(lexema);
                            if (tipo == -1)
                            {
                                if (frase[i - 1] == '\t' || frase[i - 1] == ' ')
                                {
                                    if (lexema.Length > 1)
                                    {
                                        lexema = frase.Substring(start, i - start - 1);
                                        //Validation.fileWriting(lexema, countLinea, start, i, tipo);
                                    }
                                    lexema = string.Empty;
                                    start = i;
                                }
                                else
                                {
                                    if (lexema.Length > 0)
                                    {
                                        tipo = Validation.validar(lexema.Substring(0, lexema.Length - 1));
                                        if (tipo != -1)
                                        {
                                            lexema = lexema.Substring(0, lexema.Length - 1);
                                            i--;
                                        }
                                        //Validation.fileWriting(lexema, countLinea, start, i, tipo);
                                        start = i;
                                        lexema = string.Empty;
                                    }
                                }
                            }
                            i++;
                        }
                        if (lexema.Length != 0)
                        {
                            var tipo = Validation.validar(lexema);
                            //Validation.fileWriting(lexema, countLinea, frase.Length - lexema.Length, frase.Length, tipo);
                            lexema = string.Empty;
                        }
                    }
                }
            }
            Console.ReadKey();
        }
    }
}
