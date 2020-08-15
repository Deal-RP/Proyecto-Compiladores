using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
namespace mimij
{
    public class Validation
    {
        public Validation()
        {

        }

        public bool validar(string lexema)
        {
            var valido = false;
            string pattern;
            //expresión regular con Regex
            for (int i = 0; i < 5; i++)
            {
                switch (i)
                {
                    case 0:
                        //para Palabras reservadas
                        pattern = "^void$|^int$|^double$|^boolean$|^string$|^class$|^const$|^interface$|^null$|^this$|^extends$|^implements$|^for$|^while$|^if$|^else$|^return$|^break$|^New$|^System$|^out$|^println$";
                        valido = Regex.IsMatch(lexema, pattern);
                        Console.WriteLine(valido);
                        Console.ReadLine();
                        //mandar a escribir al archivo de aquí de una vez o vos que pensas, dime??
                        break;
                    case 1:
                        //constantes booleanas
                        pattern = "^true$|^false$";
                        valido = Regex.IsMatch(lexema, pattern);
                        Console.WriteLine(valido);
                        Console.ReadLine();
                        break;
                    case 2:
                        //para numeros
                        pattern = "^[0-9]+$|^0[X|x][0-9A-F]+$|^[0-9]+.[0-9]+$|^[0-9]+.([0-9]+)?[E|e]\\+[0-9]{1,2}$";
                        valido = Regex.IsMatch(lexema, pattern);
                        Console.WriteLine(valido);
                        Console.ReadLine();
                        break;
                    case 3:
                        pattern = "^\\+$|^\\-$|^\\/$|^\\*$|^\\%$|^\\<$|^\\<=$|^\\>$|^\\>\\=$|^\\=$|^\\=\\=$|^\\!=$|^\\&\\&$|^\\|\\|$|^\\!$|^\\;$|^\\,$|^\\.$|^\\[\\]$|^\\[$|^\\]|^\\(\\)$|^\\{$|^\\}$|^\\{\\}$|^\\($|^\\)$";
                        valido = Regex.IsMatch(lexema, pattern);
                        Console.WriteLine(valido);
                        Console.ReadLine();
                        break;
                    case 4:
                        // para nombre de variables (Identificadores)
                        pattern = "^([A-z]){1,31}$|^[A-z]([A-z0-9]){2,31}$|^\\$[A-z0-9]{2,31}$";
                        valido = Regex.IsMatch(lexema, pattern);
                        Console.WriteLine(valido);
                        Console.ReadLine();
                        break;
                }
                if (valido)
                {
                    break;
                }
            }
            return valido;
        }
    }
}
