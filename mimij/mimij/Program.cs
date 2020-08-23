using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
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
            var txtName = Path.GetFileNameWithoutExtension(path);
            var frase = string.Empty;
            using (var sr = new StreamReader(path))
            {
                var comentado = false;
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
                            var tipo = Validation.identificar(lexema);
                            if (tipo == 0)
                            {
                                Validation.fileWriting(txtName, lexema, countLinea, start, i, tipo);
                                lexema = string.Empty;
                                start = i;
                            }
                            else if (comentado && lexema.Length > 1 && lexema.Substring(lexema.Length - 2, 2) == "*/")
                            {
                                comentado = false;
                                lexema = string.Empty;
                            }
                            else if (!comentado && tipo == -1)
                            {
                                start = Validation.validation(txtName,countLinea,frase,ref lexema,start,ref i,ref comentado);
                            }
                            i++;
                        }
                        if (!comentado && lexema.Length != 0)
                        {
                            Validation.validation(txtName,countLinea,frase, ref lexema, start, ref i, ref comentado);
                            lexema = string.Empty;
                        }
                    }
                }
                if (comentado)
                {
                    //ERROR COMENTARIO DE PARRAFO NO CERRADO
                    Validation.fileWriting(txtName, "EOF", 0, 0, 0, 9);
                }
            }
            Console.ReadKey();
        }
    }
}
