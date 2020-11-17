using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mimij
{
    class ASemantico
    {
        public static List<Token> TablaSimbolos = new List<Token>();
        public static List<Token> Auxiliar = new List<Token>();
        public static List<Token> Tokens = new List<Token>();
        public static string tipoS;
        public static int i = 0;
        static int reducction = 0;
        static bool error = false;
        static bool aceptar = false;
        static int lActual = 1;
        public static string path;
        public static int cantError = 0;
        public static string txtName;
        public static bool Parse(string Path)
        {
            path = Path;
            //pila
            var Pila = new Stack<int>();
            Pila.Push(0);
            //simbolo
            var Simbolo = new Stack<Token>();
            //Entrada
            var tokenAux = new Token("$", 0, 0, 0, string.Empty, string.Empty, string.Empty);
            Tokens.Add(tokenAux);
            var pos = 0;
            while (Tokens.Count() != 0)
            {
                pos = Action(ref Pila, ref Simbolo, ref Tokens, SintaxisAscSLR.Tabla[pos]);

                if (pos == 1 && aceptar)
                {
                    break;
                }
                if (!error)
                {
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
        private static int Action(ref Stack<int> Pila, ref Stack<Token> Simbolo, ref List<Token> Tokens, Dictionary<string, string> Estado)
        {
            var actions = string.Empty;
            if (reducction == 0)
            {
                int estadoA;
                bool epsilon = false;
                var valor = (Estado.ContainsKey(Tokens.First().name) && EsTerminal(Tokens.First().name)) ? Tokens.First().name : esUnTerminalDiferente();
                actions = Estado[valor];
                if (actions == string.Empty)
                {
                    actions = Estado["e"];
                    if (actions == string.Empty)
                    {
                        Console.WriteLine("El token {0} localizado en la linea {1} es incorrecto", Tokens.First().name, Tokens.First().line);
                        error = true;
                        cantError++;
                        return 0;
                    }
                    epsilon = true;
                }
                if (actions[0] == 's')
                {
                    estadoA = Convert.ToInt32(actions.Substring(1));
                    Pila.Push(estadoA);
                    Simbolo.Push(Tokens.First());
                    if (!epsilon)
                    {
                        Tokens.Remove(Tokens[0]);
                    }
                    return estadoA;
                }
                else if (actions[0] == 'r')
                {
                    estadoA = Convert.ToInt32(actions.Substring(1));
                    var producction = SintaxisAscSLR.producciones[estadoA];
                    var simbol = producction.Keys;
                    var cantidadRemover = producction[simbol.First()];
                    for (int i = 0; i < cantidadRemover; i++)
                    {
                        //insertar cada token en la lista auxiliar para validar que no sea un token
                        Pila.Pop();
                        Auxiliar.Add(Simbolo.Pop());
                    }
                    //al momento de realizar una reducción validar que no exista dicho token en la lista en caso la reducción sea de una variable
                    Validar(actions);
                    

                    

                    var tokenAux = new Token(simbol.First(), 0, 0, 0, string.Empty, string.Empty, string.Empty);
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
            }
            return 0;
        }
        private static string esUnTerminalDiferente()
        {
            switch (Tokens.First().tipo)
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
            switch (Token)
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

        private static void Validar(string redu)
        {
            var encontrarIgual = new Token();
            var identificador = new Token();
            switch (redu)
            {
                case "r3":
                    //se realizó una declaración de variable

                    break;
                case "r6":

                    break;
                case "r9":
                    //verificar que lo que siga sea el dentificador
                    if(Tokens[0].tipo==6)
                    {
                        if(Tokens[1].name==";")
                        {
                            //verificar si existe ese token dentro de la tabla simbolos
                            identificador = Auxiliar.Find(x => x.tipo == 6);
                            encontrarIgual = TablaSimbolos.Find(x=> x.name == identificador.name);
                            if(encontrarIgual == null)
                            {
                                //no existe dicho identificador
                                //añadirlo a la tabla
                                TablaSimbolos.Add(Tokens[0]);
                            }
                            else
                            {
                                //indicar ese error y no añadirlo a la tabla
                            }
                        }
                        else
                        {

                        }
                    }
                    break;
                case "r10":
                    //verificar que lo que siga sea el dentificador
                    if (Tokens[0].tipo == 6)
                    {
                        if (Tokens[1].name == ";")
                        {
                            //allí si validar que exista el token
                        }
                    }
                    break;
                case "r11":
                    //verificar que lo que siga sea el dentificador
                    if (Tokens[0].tipo == 6)
                    {
                        if (Tokens[1].name == ";")
                        {
                            //allí si validar que exista el token
                        }
                    }
                    break;
                case "r12":
                    //verificar que lo que siga sea el dentificador
                    if (Tokens[0].tipo == 6)
                    {
                        if (Tokens[1].name == ";")
                        {
                            //allí si validar que exista el token
                        }
                    }
                    break;
                case "r14":
                    //verificar que lo que siga sea el dentificador
                    if (Tokens[0].tipo == 6)
                    {
                        if (Tokens[1].name == ";")
                        {
                            //allí si validar que exista el token
                        }
                        else
                        {

                        }
                    }
                    break;
            }
        }

        public void escrituraArchivo(string escritura)
        {
            using (var writer = new StreamWriter(Path.Combine(path, $"{Validation.txtName}_Semantico.out"), append: true))
            {
                writer.WriteLine(escritura);
            }
        }
    }
}
