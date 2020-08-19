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

                            if (tipo == -1)
                            {
                                var auxTipo = Validation.identificar(frase.Substring(start, i - start - 1));

                                switch (auxTipo)
                                {
                                    case -1:
                                        //CARACTER INVALIDO
                                        break;
                                    case 2:
                                        if (frase[i - 1] == '\t' || frase[i - 1] == ' ' || Validation.identificar(frase[i - 1].ToString()) == 7)
                                        {
                                            lexema = frase.Substring(start, i - start - 1);
                                            if (frase[i - 1] != '\t' && frase[i - 1] != ' ') i--;
                                        }
                                        else
                                        {
                                            //ERROR
                                        }
                                        break;
                                    case 4:
                                        //NUMERO HEXADECIMAL INCOMPLETO
                                        break;
                                    case 6:
                                        //DOUBLE INCOMPLETO
                                        break;
                                    case 10:
                                        //STRING INCOMPLETO
                                        break;
                                    default:
                                        lexema = frase.Substring(start, i - start - 1);
                                        if (frase[i - 1] != '\t' && frase[i - 1] != ' ') i--;
                                        lexema = string.Empty;
                                        break;
                                }
                                start = i;
                                //Validation.fileWriting(txtName, lexema, countLinea, start, i, auxTipo);
                            }
                            i++;
                        }

                        if (lexema.Length != 0)
                        {
                            var tipo = Validation.identificar(lexema);
                            switch (tipo)
                            {
                                case -1:
                                    //CARACTER INVALIDO
                                    break;
                                case 4:
                                    //NUMERO HEXADECIMAL INCOMPLETO
                                    break;
                                case 6:
                                    //DOUBLE INCOMPLETO
                                    break;
                                case 10:
                                    //STRING INCOMPLETO
                                    break;
                                default:
                                    lexema = frase.Substring(start, i - start - 1);
                                    break;
                            }
                            //Validation.fileWriting(txtName, lexema, countLinea, frase.Length - lexema.Length, frase.Length, tipo);
                            lexema = string.Empty;
                        }
                    }
                }
            }
            Console.ReadKey();
        }
    }
}
