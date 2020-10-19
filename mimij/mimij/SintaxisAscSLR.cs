using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mimij
{
    class SintaxisAscSLR
    {
        public static List<Token> Tokens = new List<Token>();
        public static Dictionary<int, Dictionary<string, string>> Tabla = new Dictionary<int, Dictionary<string, string>>();
        public static void tableCreation()
        {
            for (int i = 0; i < 9; i++)
            {
                var diccionario = valueAsignation(i);
                Tabla.Add(i, diccionario);
            }
        }
        static Dictionary<string, string> valueAsignation(int estado)
        {
            var diccionario = new Dictionary<string, string>();
            switch (estado)
            {
                case 0:
                    diccionario.Add("id", "d2");
                    diccionario.Add("S", "1");
                    diccionario.Add("V", "3");
                    /*diccionario.Add("ident", "d9");
                    diccionario.Add("static", "d5");
                    diccionario.Add("class", "d6");
                    diccionario.Add("interface", "d7");
                    diccionario.Add("int", "d11");
                    diccionario.Add("double", "d12");
                    diccionario.Add("boolean", "d13");
                    diccionario.Add("string", "d14");
                    diccionario.Add("void", "d10");
                    diccionario.Add("Program", "1");
                    diccionario.Add("Decl", "2");
                    diccionario.Add("TYPEX", "8");
                    diccionario.Add("TYPE", "3");
                    diccionario.Add("FDECL", "4");
                    */
                    break;
                case 1:
                    diccionario.Add("$", "Accept");
                    //diccionario.Add("$", "Accept");
                    break;
                case 2:
                    diccionario.Add(":=", "r3");
                    diccionario.Add("$", "r1,r3");
                    /*diccionario.Add("ident", "d9");
                    diccionario.Add("static", "d5");
                    diccionario.Add("class", "d6");
                    diccionario.Add("interface", "d7");
                    diccionario.Add("int", "d11");
                    diccionario.Add("double", "d12");
                    diccionario.Add("boolean", "d13");
                    diccionario.Add("string", "d14");
                    diccionario.Add("void", "d10");
                    diccionario.Add("$", "r2");
                    diccionario.Add("Program", "15");
                    diccionario.Add("Decl", "2");
                    diccionario.Add("TYPEX", "8");
                    diccionario.Add("TYPE", "3");
                    diccionario.Add("FDECL", "4");
                    */
                    break;
                case 3:
                    diccionario.Add(":=", "d4");
                    //diccionario.Add("ident", "d16,r16");
                    //diccionario.Add("[]", "d17");
                    break;
                case 4:
                    diccionario.Add("id", "d8");
                    diccionario.Add("num", "d7");
                    diccionario.Add("V", "6");
                    diccionario.Add("E", "5");
                    break;
                case 5:
                    diccionario.Add("$", "r2");
                    break;
                case 6:
                    diccionario.Add("$", "r4");
                    break;
                case 7:
                    diccionario.Add("$", "r5");
                    break;
                case 8:
                    diccionario.Add("$", "r3");
                    diccionario.Add(":=", "r3");
                    break;
            }
            return diccionario;
        }
        public static void Parse()
        {
            var tokenAux = new Token("$",0,0,0);
            Tokens.Add(tokenAux);
            var pos = 0;
            for (int i = 0; i < Tokens.Count(); i++)
            {
                Action(Tabla[pos], Tokens[i]);
            }
        }
        private static void Action(Dictionary<string, string> diccionario, Token token)
        {
            if(diccionario.ContainsKey(token.name))
            {
                var accion = diccionario[token.name];
            }
            else
            {

            }
        }
    }
}
