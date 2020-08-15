using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace mimij
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Escriba el código");
            //ignorar espacios en blanco, saltos de línea, y tabs
            var frase = Console.ReadLine();
            var delimitadores = new char[]{ ' ', '\t', '\n' };
            var Split = frase.Split(delimitadores);
            var resultL = new List<string>();
            for(int i = 0; i < Split.Length; i++)
            {
                if (Split[i] != string.Empty)
                {
                    resultL.Add(Split[i]);
                }
            }
            //empezar el proceso de validación por lexema
            var process = new Validation();
            foreach(string lexema in resultL)
            {
                process.validar(lexema);
            }
        }
    }
}
