using System;
using System.IO;

namespace mimij
{
    class Program
    {
        static void Main(string[] args)
        {
            SintaxisAscSLR.load();
            var countLinea = 0;
            Console.WriteLine("Arrastre el archivo de entrada a consola");
            var path = Console.ReadLine().Trim('"');
            Validation.txtName = Path.GetFileNameWithoutExtension(path);
            if (File.Exists(Validation.txtName + ".out"))
            {
                File.Delete(Validation.txtName + ".out");
            }
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
                        var specialCase = false;
                        while (start != frase.Length && i < frase.Length + 1)
                        {
                            lexema = frase.Substring(start, i - start);
                            var tipo = Validation.identificar(lexema);
                            if (!specialCase && tipo == 0)
                            {
                                if (lexema == "int" && frase.Length >= i + 6 && frase.Substring(start, i - start + 6) == "interface")
                                {
                                    i += 6; 
                                    lexema = frase.Substring(start, i - start);
                                }
                                else if (lexema == "System" && frase.Length >= i + 12 && frase.Substring(start, i - start + 12) == "System.out.println")
                                {
                                    i += 12;
                                    lexema = frase.Substring(start, i - start);
                                }
                                else if (lexema == "System")
                                {
                                    specialCase = true;
                                }
                                if (!specialCase)
                                {
                                    Validation.fileWriting(lexema, countLinea, start, tipo);
                                    lexema = string.Empty;
                                    start = i;
                                    specialCase = false;
                                }
                            }
                            else if (lexema == " "|| lexema == "\t")
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
                                start = Validation.validation(countLinea, frase, ref lexema, start, ref i, ref comentado);
                            }
                            i++;
                        }
                        if (!comentado && lexema.Length != 0)
                        {
                            Validation.validation(countLinea, frase, ref lexema, start, ref i, ref comentado);
                            lexema = string.Empty;
                        }
                    }
                }
                if (comentado)
                {
                    //ERROR COMENTARIO DE PARRAFO NO CERRADO
                    Validation.fileWriting("EOF", countLinea, 0, 9);
                }
            }
            if (Validation.error == 0)
            {
                Console.WriteLine("Archivo Leido Exitosamente!!!!!.");
                if (SintaxisAscSLR.Parse())
                {
                    ASemantico.Parse();
                }
            }
            Console.ReadKey();
        }
    }
}
