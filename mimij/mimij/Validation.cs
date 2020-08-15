using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.IO;

namespace mimij
{
    public class Validation
    {
        //Nombre, linea, columnaInicial, columnaFinal, atributo
       static Tuple<string, int, int, int, string> tablaOut = new Tuple<string, int, int, int, string>("hola", 1, 2, 3, "null");
       static public bool validar(string lexema)
       {
            //expresión regular con Regex
            var palabraReservada = "^void$|^int$|^double$|^boolean$|^string$|^class$|^const$|^interface$|^null$|^this$|^extends$|^implements$|^for$|^while$|^if$|^else$|^return$|^break$|^New$|^System$|^out$|^println$";
            var boolean = "^true$|^false$";
            var number = "^[0-9]+$|^0[X|x][0-9A-F]+$|^[0-9]+.[0-9]+$|^[0-9]+.([0-9]+)?[E|e]\\+[0-9]{1,2}$";
            var operadores = "^\\+$|^\\-$|^\\/$|^\\*$|^\\%$|^\\<$|^\\<=$|^\\>$|^\\>\\=$|^\\=$|^\\=\\=$|^\\!=$|^\\&\\&$|^\\|\\|$|^\\!$|^\\;$|^\\,$|^\\.$|^\\[\\]$|^\\[$|^\\]|^\\(\\)$|^\\{$|^\\}$|^\\{\\}$|^\\($|^\\)$";
            var identificador = "^([A-z]){1,31}$|^[A-z]([A-z0-9]){2,31}$|^\\$[A-z0-9]{2,31}$";
            var cadena = "^\"[A-z0-9/*+-]*$";
            if (Regex.IsMatch(lexema, palabraReservada))
            {
                return true;
            }
            else if (Regex.IsMatch(lexema, boolean))
            {
                return true;
            }
            else if (Regex.IsMatch(lexema, number))
            {
                return true;
            }
            else if (Regex.IsMatch(lexema, operadores))
            {
                return true;
            }
            else if (Regex.IsMatch(lexema, identificador))
            {
                return true;
            }
            else
            {
                //esto hay que validarlo (lo hago cuando regrese)
                return false;
            }
        }
        public static void fileWriting(string path, string name, int line, int columnFirst, int columnLast, string categoriaLexema)
        {
            var tupla = Tuple.Create(name,line,columnFirst,columnLast,categoriaLexema);
            using (var writer = new StreamWriter(path + "\\Tokens.out", append: true))
            {
                    writer.Write(tupla.Item1);
                    writer.Write("\t");
                    writer.Write("line ");
                    writer.Write(tupla.Item2 + " ");
                    writer.Write(tupla.Item3 + "-");
                    writer.Write(tupla.Item4 + " is ");
                    writer.Write(tupla.Item5);
                    writer.Write("\n");
            }
            
        }
    }
}
