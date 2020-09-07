using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System;

namespace mimij
{
    public class Validation
    {
        public static int error = 0;
        public static string txtName = string.Empty;
        static public int identificar(string lexema)
        {
            var caracteres = "\0\u0001\u0002\u0003\u0004\u0005\u0006\a\b\t\n\v\f\r\u000e\u000f\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001a\u001b\u001c\u001d\u001e\u001f !#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ\\[\\]\\^_`abcdefghijklmnopqrstuvwxyz{\\|}~\u007f\u0080\u0081\u0082\u0083\u0084\u0085\u0086\u0087\u0088\u0089\u008a\u008b\u008c\u008d\u008e\u008f\u0090\u0091\u0092\u0093\u0094\u0095\u0096\u0097\u0098\u0099\u009a\u009b\u009c\u009d\u009e\u009f ¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿ";
            var listER = new List<string>()
            {
                //PALABRA RESERVADA
                "^void$|^int$|^double$|^boolean$|^string$|^class$|^const$|^interface$|^null$|^this$|^extends$|^implements$|^for$|^while$|^if$|^else$|^return$|^break$|^New$|^System$|^out$|^println$|^Print$",
                //BOOLEAN
                "^true$|^false$",
                //ENTERO
                "^[0-9]+$",
                //HEXADECIMAL
                "^0[X|x][0-9A-Fa-f]+$",
                //DOUBLE
                "^[0-9]+\\.[0-9]*([E|e][+|-]?[0-9]+)?$",
                //OPERADORES
                "^\\+$|^\\-$|^\\/$|^\\*$|^\\%$|^\\<$|^\\<=$|^\\>$|^\\>\\=$|^\\=$|^\\=\\=$|^\\!=$|^\\&\\&$|^\\|\\|$|^\\!$|^\\;$|^\\,$|^\\.$|^\\[\\]$|^\\[$|^\\]|^\\(\\)$|^\\{$|^\\}$|^\\{\\}$|^\\($|^\\)$",
                //IDENTIFICADOR
                "^[A-z|$]([A-z0-9$])*$",
                //STRING
                $"^\"[{caracteres}|\\\\]*\"$",
                //comentario //
                $"^\\/\\/[{caracteres}]*$",
                //comentario/*
                $"^\\/\\*$",
                //POSIBLE HEXADECIMAL
                "^0[X|x]$",
                 //POSIBLE DOUBLE     
                "^[0-9]+\\.[0-9]*[E|e][+|-]?$",
                //POSIBLE STRING
                $"^\"[{caracteres}|\\\\]*$",
                //Comentario sin emparejar
                "^\\*\\/$"
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
        public static int validation(int line, string frase, ref string lexema, int start, ref int i, ref bool comentado)
        {
            var auxTipo = identificar(frase.Substring(start, i - start - 1));
            switch (auxTipo)
            {
                case -1:
                    //CARACTER INVALIDO
                    Validation.fileWriting(lexema, line, start, auxTipo);
                    i++;
                    break;
                case 10:
                    //NUMERO HEXADECIMAL INCOMPLETO
                    lexema = frase.Substring(start, i - start - 1);
                    Validation.fileWriting(lexema, line, start, auxTipo);
                    break;
                case 11:
                    //DOUBLE INCOMPLETO
                    if(lexema.Last() == '+' || lexema.Last() == '-')
                    {
                        i -= 2;
                    }
                    else if (lexema.Last() == 'E' || lexema.Last() == 'e')
                    {
                        i--;
                    }
                    auxTipo = identificar(frase.Substring(start, i - start - 1));
                    lexema = frase.Substring(start, i - start - 1);
                    Validation.fileWriting(lexema, line, start, auxTipo);
                    break;
                case 12:
                    //STRING INCOMPLETO
                    lexema = frase.Substring(start, i - start - 1);
                    Validation.fileWriting(lexema, line, start, auxTipo);
                    break;
                case 13:
                    lexema = frase.Substring(start, i - start - 1);
                    Validation.fileWriting(lexema, line, start, auxTipo);
                    break;
                case 9:
                    comentado = true;
                    break;
                case 8:break;
                default:
                    lexema = frase.Substring(start, i - start - 1);
                        Validation.fileWriting(lexema, line, start, auxTipo);
                    lexema = string.Empty;
                    break;
            }
            i--;
            return i;
        }
        public static void fileWriting(string name, int line, int columnFirst, int tipo)
        {
            var typesTokens = new List<string>()
            {
                $"T_{name}",
                "T_BoolConstant",
                $"T_IntConstant (value = {name})",
                $"T_HexaConstant (value = {name})",
                $"T_DoubleConstant (value = {name})",
                $"T_Operador = '{name}'",
                "T_Identificador",
                $"T_StringConstant = (value = {name})"
            };
            var path = Directory.GetCurrentDirectory();
            var escritura =
                (tipo == -1 || tipo == 10 | tipo == 11) ?
                $"*** Error line {line}. *** Unrecognized: '{name}'" : (tipo == 12) ?
                $"*** Error line {line}. *** Incomplete string: " + '"' : (tipo == 9) ?
                $"*** Error line {line} *** EOF:" : (tipo == 7 && name.Contains('\0')) ?
                $"*** Error line {line}. *** Invalid char :" + '\0':(tipo == 13)?
                $"*** Error line {line}. *** Unrecognized close : {name}": (tipo == 6 && name.Length>31)?
                $"*** Error line {line}. *** Identifier too long : {name}"
                : $"{name}\t\tline {line} cols {columnFirst + 1}-{name.Length + columnFirst} is {typesTokens[tipo]}";
            if(tipo==-1||(tipo<14&&tipo>7))
            {
                Console.WriteLine(escritura);
                error++;
            }
            else
            {
                SintaxisDescRecur.orden.Add(new Token(name, line, columnFirst, tipo));
            }
            using (var writer = new StreamWriter(Path.Combine(path, $"{txtName}.out"), append: true))
            {
                writer.WriteLine(escritura);
            }
        }
    }
}
