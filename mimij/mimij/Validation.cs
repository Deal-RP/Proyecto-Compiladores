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
        //static Tuple<string, int, int, int, string> tablaOut;
        static public int validar(string lexema)
        {
            var listER = new List<string>()
            {
                //PALABRA RESERVADA
                "^void$|^int$|^double$|^boolean$|^string$|^class$|^const$|^interface$|^null$|^this$|^extends$|^implements$|^for$|^while$|^if$|^else$|^return$|^break$|^New$|^System$|^out$|^println$",
                //BOOLEAN
                "^true$|^false$",
                //NUMBER
                "^[0-9]+$|^0[X|x][0-9A-F]+$|^[0-9]+.[0-9]+$|^[0-9]+.([0-9]+)?[E|e]\\+[0-9]{1,2}$",
                //OPERADORES
                "^\\+$|^\\-$|^\\/$|^\\*$|^\\%$|^\\<$|^\\<=$|^\\>$|^\\>\\=$|^\\=$|^\\=\\=$|^\\!=$|^\\&\\&$|^\\|\\|$|^\\!$|^\\;$|^\\,$|^\\.$|^\\[\\]$|^\\[$|^\\]|^\\(\\)$|^\\{$|^\\}$|^\\{\\}$|^\\($|^\\)$",
                //IDENTIFICADOR
                "^([A-z]){1,31}$|^[A-z]([A-z0-9]){2,31}$|^\\$[A-z0-9]{2,31}$",
                //STRING
                "^\"[A-z0-9/*+-]*$"
            };

            var cont = 0;
            foreach (var ER in listER)
            {
                if (Regex.IsMatch(lexema, ER))
                {
                    return cont;
                }
                cont++;
            }
            return -1;
        }
        public static void fileWriting(string name, int line, int columnFirst, int columnLast, int tipo)
        {
            var typesTokens = new List<string>()
            {
                "T_",
                "Boolean",
                "Number",
                "Operador",
                "Identificador",
                "String"
            };
            var path = Directory.GetCurrentDirectory();
            var tupla = Tuple.Create(name, line, columnFirst + 1, columnLast + 1, typesTokens[tipo]);
            var escritura = 
                tipo == -1 ?
                $"*** Error line {line}. *** Unrecognized char: '{name}'" :
                $"{tupla.Item1}\t\tline {tupla.Item2} cols {tupla.Item3}-{tupla.Item4} is {tupla.Item5}";
            
            using (var writer = new StreamWriter(path + "\\Tokens.out", append: true))
            {
                writer.WriteLine(escritura);
            }
        }
    }
}
