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
        public static Stack<string> tipo = new Stack<string>();
        public static Stack<string> Stmt = new Stack<string>();
        public static string tipoS;
        public static int i = 0;
        static int reducction = 0;
        static bool error = false;
        static bool aceptar = false;
        static int lActual = 1;
        public static string path;
        public static int cantError = 0;
        public static string txtName;
        public static void Parse(string Path)
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
            if (cantError != 0)
            {
                Console.WriteLine("SU CADENA POSEE ERRORES SEMÁNTICOS");
            }
            else
            {
                Console.WriteLine("CADENA ACEPTADA: NO POSEE ERRORES SEMÁNTICOS");
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
                    epsilon = true;
                }
                if (actions.Contains('/'))
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
                        Pila.Pop();
                        Auxiliar.Add(Simbolo.Pop());
                    }
                    var tokenAux = new Token(simbol.First(), 0, 0, 0, string.Empty, string.Empty, string.Empty);
                    Simbolo.Push(tokenAux);
                    Validar(actions);
                    Auxiliar = new List<Token>();
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
            switch (redu)
            {
                case "r3":
                    MétodosParaEncontrar(1,1);
                    break;
                case "r4":
                   
                    break;
                case "r6":
                    MétodosParaEncontrar(1,1);
                    break;
                case "r9":
                    tipo.Push("int");
                    break;
                case "r10":
                    tipo.Push("double");
                    break;
                case "r11":
                    tipo.Push("boolean");
                    break;
                case "r12":
                    tipo.Push("string");
                    break;
                case "r14":
                    tipo.Push("ident");
                    break;
                case "r15":
                    tipo.Push(tipo.Pop()+"[]");
                    break;
                case "r16":
                    MétodosParaEncontrar(1,2);
                    break;
                case "r17":
                    MétodosParaEncontrar(1,0);
                    break;
                case "r39":
                    MétodosParaEncontrar(1,1);
                    break;
                case "r40":
                    MétodosParaEncontrar(1,1);
                    break;
                case "r82":
                    MétodosParaEncontrar(2,0);
                    break;
            }
        }
        private static void MétodosParaEncontrar(int numero, int numero2)
        {
            switch(numero)
            {
                case 1:
                    var type = tipo.Pop();
                    var tok = Auxiliar[numero2];
                    var Igual = TablaSimbolos.Find(X => X.name == tok.name);
                    if (Igual == null)
                    {
                        tok.tipoAS = type;
                        TablaSimbolos.Add(tok);
                    }
                    else
                    {
                        cantError++;
                        IndicarError(1, tok);
                    }
                    break;     
                case 2:
                    //esto es para el stmt de Expr de H -> ident
                    tok = Auxiliar[numero2];
                    Igual = TablaSimbolos.Find(X => X.name == tok.name);
                    if (Igual == null)
                    {
                        cantError++;
                        IndicarError(1, tok);
                    }
                    break;
            }
        }
        private static void IndicarError(int valor, Token tok)
        {
            switch(valor)
            {
                case 1:
                    Console.WriteLine($"**ERROR** EL TOKEN: {tok.name} LOCALIZADO EN LA LINEA: {tok.line} Y COLUMNAS: {tok.columnFirst} - {tok.columnEnd} YA SE ENCUENTRA DECLARADO");
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
