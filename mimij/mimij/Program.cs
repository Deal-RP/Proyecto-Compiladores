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
                            var tipo = Validation.validar(lexema);
                            if (tipo == -1)
                            {
                                if (frase[i - 1] == '\t' || frase[i - 1] == ' ')
                                {
                                    if (lexema.Length > 1)
                                    {
                                        lexema = frase.Substring(start, i - start - 1);
                                    }
                                    lexema = string.Empty;
                                    start = i;
                                }
                                else
                                {
                                    if (lexema.Length > 0)
                                    {
                                        //if hexadecimales
                                        if (lexema.Last() == 'x' || lexema.Last() == 'X')
                                        {
                                            while (tipo != -1 && i < frase.Length)
                                            {
                                                lexema += frase[i];
                                                tipo = Validation.validar((lexema));
                                                if (tipo != -1) i++;
                                            }
                                        }
                                        //double con exponente
                                        else if (lexema.Last() == 'e' || lexema.Last() == 'E')
                                        {
                                            var ER = "^[0-9]+\\.[0-9]*[E|e]$";
                                            if (Regex.IsMatch(lexema, ER))
                                            {
                                                while (tipo != -1 && i < frase.Length)
                                                {
                                                    lexema += frase[i];
                                                    tipo = Validation.validar(lexema);
                                                    lexema = (tipo != -1) ? lexema : lexema.Substring(0, lexema.Length - 1);
                                                    i = (tipo != -1) ? i + 1 : i;
                                                }
                                            }
                                        }
                                        // tercero para strings
                                        else if (lexema.Last() == '"')
                                        {
                                            var LAux = string.Empty;
                                            LAux += lexema.Last();
                                            lexema = lexema.Substring(0, lexema.Length - 1);
                                            //mandar a escribir el lexema.
                                            do
                                            {
                                                LAux += frase[i];
                                                i++;
                                            } while (LAux.Last() != '"' && i < frase.Length);
                                            tipo = Validation.validar(LAux);
                                            if (tipo==-1)
                                            {
                                                //string sin cerrar
                                            }
                                            else
                                            {
                                                //mandar a escribir el resto
                                            }
                                        }
                                        //cuarto para comentarios //
                                        else if (lexema.Last() == '/' && frase[i] == '/')
                                        {
                                            //Almacenar el último valor
                                            var LAux = string.Empty;
                                            LAux += lexema.Last();
                                            lexema = lexema.Substring(0, lexema.Length - 1);
                                            //mandar a escribir el lexema.
                                            while (i < frase.Length)
                                            {
                                                LAux += frase[i];
                                                i++;
                                            }
                                            //mandar a escribir LAux
                                        }
                                        //quinto para comentario con *
                                        else if((lexema.Last() == '/'&&frase[i]=='*')/*||(lexema.Last()=='*'&&lexema[lexema.Length-2]=='/')*/)
                                        {
                                            var LAux = string.Empty;
                                            LAux += lexema.Last();
                                            lexema = lexema.Substring(0, lexema.Length - 1);
                                            //mandar a escribir el lexema.
                                        }
                                        //Validation.fileWriting(txtName, lexema, countLinea, start, i, tipo);
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
