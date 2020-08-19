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
        static public int identificar(string lexema)
        {
            var caracteres = "\0\u0001\u0002\u0003\u0004\u0005\u0006\a\b\t\n\v\f\r\u000e\u000f\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001a\u001b\u001c\u001d\u001e\u001f !#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{\\|}~\u007f\u0080\u0081\u0082\u0083\u0084\u0085\u0086\u0087\u0088\u0089\u008a\u008b\u008c\u008d\u008e\u008f\u0090\u0091\u0092\u0093\u0094\u0095\u0096\u0097\u0098\u0099\u009a\u009b\u009c\u009d\u009e\u009f ¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿ";
            var listER = new List<string>()
            {
                //PALABRA RESERVADA
                "^void$|^int$|^double$|^boolean$|^string$|^class$|^const$|^interface$|^null$|^this$|^extends$|^implements$|^for$|^while$|^if$|^else$|^return$|^break$|^New$|^System$|^out$|^println$",
                //BOOLEAN
                "^true$|^false$",
                //ENTERO
                "^[0-9]+$",
                //HEXADECIMAL
                "^0[X|x][0-9A-Fa-f]+$",
                //POSIBLE HEXADECIMAL
                "^0[X|x]$",
                //DOUBLE
                "^[0-9]+\\.[0-9]*([E|e][+|-]?[0-9]{1,2})?$",
                //POSIBLE DOUBLE
                "^[0-9]+\\.[0-9]*[E|e][+|-]?",
                //OPERADORES
                "^\\+$|^\\-$|^\\/$|^\\*$|^\\%$|^\\<$|^\\<=$|^\\>$|^\\>\\=$|^\\=$|^\\=\\=$|^\\!=$|^\\&\\&$|^\\|\\|$|^\\!$|^\\;$|^\\,$|^\\.$|^\\[\\]$|^\\[$|^\\]|^\\(\\)$|^\\{$|^\\}$|^\\{\\}$|^\\($|^\\)$",
                //IDENTIFICADOR
                "^([A-z]){1,31}$|^[A-z]([A-z0-9]){2,31}$|^\\$[A-z0-9]{2,31}$",
                //STRING
                $"^\"[{caracteres}]*\"$",
                //POSIBLE STRING
                $"^\"[{caracteres}]*$",
                //comentario //
                $"^\\/\\/[{caracteres}]*$",
                //comentario/*
                $"^\\/\\*[{caracteres}]\\*\\/$",
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
        public static void fileWriting(string txtName, string name, int line, int columnFirst, int columnLast, int tipo)
        {
            var typesTokens = new List<string>()
            {
                "T_",
                "Boolean",
                "Entero",
                "Hexadecimal",
                "Double",
                "Operador",
                "Identificador",
                "String",
                "Comentario",
                "Comentario"
            };
            var path = Directory.GetCurrentDirectory();
            var escritura = 
                tipo == -1 ?
                $"*** Error line {line}. *** Unrecognized char: '{name}'" :
                $"{name}\t\tline {line} cols {columnFirst+1}-{columnLast+1} is {typesTokens[tipo]}";
            using (var writer = new StreamWriter(Path.Combine(path, $"{txtName}.out"), append: true))
            {
                writer.WriteLine(escritura);
            }
        }
    }
}
