using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;

namespace mimij
{
    class SintaxisAscSLR
    {
        public static Queue<Token> Tokens = new Queue<Token>();
        public static Dictionary<int, Dictionary<string, string>> Tabla = new Dictionary<int, Dictionary<string, string>>();
        public static Dictionary<int, Dictionary<string, int>> producciones = new Dictionary<int,Dictionary<string, int>>();
        static int reducction = 0;
        static bool error = false;
        static bool aceptar = false;
        static int lActual = 1;
        static int cantError = 0;
        static string newPath = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.IndexOf("\\bin"));
        static void loadTabla()
        {
            using (var sr = new StreamReader(Path.Combine(newPath, "AnalisiSintactico", "Tabla.txt")))
            {
                var cont = 0;
                var columnBase = new List<string>();
                var line = sr.ReadLine();
                var split = line.Split('_');
                //Numero de columnas
                for (int i = 1; i < 77; i++)
                {
                    columnBase.Add(split[i]);
                }
                while ((line = sr.ReadLine()) != null)
                {
                    var dicFila = new Dictionary<string, string>();
                    split = line.Split('_');
                    //Numero de columnas
                    for (int i = 1; i < 77; i++)
                    {
                        var accion = split[i] == " " ? string.Empty : split[i];
                        dicFila.Add(columnBase[i - 1], accion);
                    }
                    Tabla.Add(cont, dicFila);
                    cont++;
                }
            }
        }
        static void loadGramatica()
        {
            using (var file = new StreamReader(Path.Combine(newPath, "AnalisiSintactico", "Gramatica.txt")))
            {
                var cont = 0;
                var line = string.Empty;
                while ((line = file.ReadLine()) != null)
                {
                    var produc = line.Split(new[] { " -> " }, StringSplitOptions.None);
                    var cantProduc = produc[1].Split(new char[] { ' ' , '\t'});
                    producciones.Add(cont, new Dictionary<string, int> { { produc[0], cantProduc.Length } });
                    cont++;
                }
            }
        }
        public static void load()
        {
            loadGramatica();
            loadTabla();
        }
        public static bool Parse()
        {
            //pila
            var Pila = new Stack<int>();
            Pila.Push(0);
            //simbolo
            var Simbolo = new Stack<Token>();
            //Entrada
            var tokenAux = new Token("$", 0, 0, 0, string.Empty,string.Empty, string.Empty, false, 0);
            Tokens.Enqueue(tokenAux);
            var pos = 0;
            while (Tokens.Count() != 0)
            {
                pos = Action(ref Pila, ref Simbolo, ref Tokens, Tabla[pos]);
                
                if (pos == 1 && aceptar)
                {
                    break;
                }
                if (!error)
                {
                    lActual = Tokens.First().line;
                }
                if (pos == 0 && error)
                {
                    //resetear la pila
                    Pila = new Stack<int>();
                    Simbolo = new Stack<Token>();
                    Pila.Push(0);
                    reducction = 0;
                    //evaluar donde se encontrará
                    while (lActual == Tokens.First().line)
                    {
                        if (Tokens.First().name == "$")
                        {
                            break;
                        }
                        Tokens.Dequeue();
                    }
                    if (Tokens.First().name == "$")
                    {
                        break;
                    }
                    //indicar que volverá a iniciar de 0
                    error = false;
                    lActual = Tokens.First().line;
                }
            }
            if (cantError > 0)
            {
                Console.WriteLine("Error: La cadena no es aceptada");
                return false;
            }
            else
            {
                Console.WriteLine("Cadena aceptada");
                return true;
            }
        }
        private static int Action(ref Stack<int> Pila, ref Stack<Token> Simbolo, ref Queue<Token> Tokens, Dictionary<string, string> Estado)
        {
            var actions = string.Empty;
            if (reducction == 0)
            {
                int estadoA;
                bool epsilon = false;
                var valor = (Estado.ContainsKey(Tokens.First().name) && EsTerminal(Tokens.First().name)) ? Tokens.First().name : esUnTerminalDiferente();
                actions = Estado[valor];
                if(actions ==string.Empty)
                {
                    actions = Estado["e"];
                    if(actions == string.Empty)
                    {
                        Console.WriteLine("El token {0} localizado en la linea {1} es incorrecto", Tokens.First().name, Tokens.First().line);
                        error = true;
                        cantError++;
                        return 0;
                    }
                    epsilon = true;
                }
                if(actions.Contains('/'))
                {
                    var actionsAux = actions.Split('/');
                    var t1 = Tokens.ElementAt(0);
                    var t2 = Tokens.ElementAt(1);
                    var t3 = Tokens.ElementAt(2);
                    actions = (t2.tipo == 6 && t3.name == "(") ? actionsAux[0] : actionsAux[1];
                    actions = actions.Trim(' ');
                }
                if (actions[0] == 's')
                {
                    estadoA = Convert.ToInt32(actions.Substring(1));
                    Pila.Push(estadoA);
                    Simbolo.Push(Tokens.First());
                    if (!epsilon)
                    {
                        Tokens.Dequeue();
                    }
                    return estadoA;
                }
                else if (actions[0] == 'r')
                {
                    estadoA = Convert.ToInt32(actions.Substring(1));
                    var producction = producciones[estadoA];
                    var simbol = producction.Keys;
                    var cantidadRemover = producction[simbol.First()];
                    for (int i = 0; i < cantidadRemover; i++)
                    {
                        Pila.Pop();
                        Simbolo.Pop();
                    }
                    var tokenAux = new Token(simbol.First(), 0, 0, 0, string.Empty,string.Empty, string.Empty, false, 0);
                    Simbolo.Push(tokenAux);
                    reducction = 1;
                    return Pila.First();
                }
                else if (actions == "acc")
                {
                    aceptar = true;
                    return 1;
                }
            }
            else
            {
                actions = Estado[Simbolo.First().name];
                if (actions != string.Empty)
                {
                    var value = Convert.ToInt32(actions);
                    Pila.Push(value);
                    reducction = 0;
                    return value;
                }
                else
                {
                    Console.WriteLine($"La linea {lActual} no posee la estructura correcta");
                    error = true;
                    return 0;
                }
            }
            return 0;
        }
        private static string esUnTerminalDiferente()
        {
            switch(Tokens.First().tipo)
            {
                case 1:
                    return "booleanConstant";
                case 2:
                    return "intConstant";
                case 3:
                    return "intConstant";
                case 4:
                    return "doubleConstant";
                case 6:
                    return "ident";
                case 7:
                    return "stringConstant";
                default: return "e";
            }
        }
        private static bool EsTerminal(string Token)
        {
            switch(Token)
            {
                case "Program":
                    return false;
                case "Decl":
                    return false;
                case "TYPEX":
                    return false;
                case "Type":
                    return false;
                case "Formals":
                    return false;
                case "Extends":
                    return false;
                case "Implements":
                    return false;
                case "Ident":
                    return false;
                case "FieldX":
                    return false;
                case "Field":
                    return false;
                case "PrototypeS":
                    return false;
                case "Prototype":
                    return false;
                case "StmtBlock":
                    return false;
                case "VariableDecl":
                    return false;
                case "ConstDecl":
                    return false;
                case "StmtX":
                    return false;
                case "Stmt":
                    return false;
                case "ElseStmt":
                    return false;
                case "ExprX":
                    return false;
                case "Expr":
                    return false;
                case "A":
                    return false;
                case "AX":
                    return false;
                case "B":
                    return false;
                case "C":
                    return false;
                case "D":
                    return false;
                case "E":
                    return false;
                case "F":
                    return false;
                case "G":
                    return false;
                case "H":
                    return false;
                default:
                    return true;
            }
        }
    }
}
