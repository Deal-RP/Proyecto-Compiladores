using System;
using System.IO;

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
            if (File.Exists(txtName + ".out"))
            {
                File.Delete(txtName + ".out");
            }
            var error = 0;
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
                                Validation.fileWriting(txtName, lexema, countLinea, start, tipo,ref error);
                                lexema = string.Empty;
                                start = i;
                            }
                            else if (lexema == " ")
                            {
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
                                start = Validation.validation(txtName, countLinea, frase, ref lexema, start, ref i, ref comentado,ref error);
                            }
                            i++;
                        }
                        if (!comentado && lexema.Length != 0)
                        {
                            Validation.validation(txtName, countLinea, frase, ref lexema, start, ref i, ref comentado,ref error);
                            lexema = string.Empty;
                        }
                    }
                }
                if (comentado)
                {
                    //ERROR COMENTARIO DE PARRAFO NO CERRADO
                    Validation.fileWriting(txtName, "EOF", countLinea, 0, 9, ref error);
                }
            }
            if(error == 0)
            {
                Console.WriteLine("Archivo Leido Exitosamente!!!!!.");
            }
            Console.ReadKey();
        }
    }
}
