﻿using System;
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
        public static Stack<Token> variables = new Stack<Token>();
        public static int i = 0;
        public static bool retorno = false;
        static int reducction = 0;
        static bool error = false;
        static bool aceptar = false;
        public static string path;
        public static int cantError = 0;

        public static void Parse(string Path)
        {
            path = Path;
            //pila
            var Pila = new Stack<int>();
            Pila.Push(0);
            ambitoBase.Push(0);
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
            if (!error || redu == "r41")
            {
                switch (redu)
                {
                    case "r3":
                        MétodosParaEncontrar(1, 1);
                        break;
                    case "r4":
                        MétodosParaEncontrar(3, 4);
                        MetodosParaAmbitos(2);
                        if (retorno)
                        {
                            IndicarError(7, TablaSimbolos.Find(x => x.name == Auxiliar[4].name && x.subnivel == ambitoBase.First().ToString()), "");
                        }
                        retorno = false;
                        break;
                    case "r5":
                        MétodosParaEncontrar(3, 5);
                        MetodosParaAmbitos(2);
                        if (!retorno)
                        {
                            IndicarError(8, TablaSimbolos.Find(x => x.name == Auxiliar[4].name && x.subnivel == ambitoBase.First().ToString()), "");
                        }
                        retorno = false;
                        break;
                    case "r6":
                        MétodosParaEncontrar(1, 1);
                        break;
                    case "r7":
                        MétodosParaEncontrar(3, 7);
                        MetodosParaAmbitos(2);
                        break;
                    case "r8":
                        MétodosParaEncontrar(3, 8);
                        MetodosParaAmbitos(5);
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
                        tipo.Push(tipo.Pop() + "[]");
                        break;
                    case "r16":
                        MetodosParaAmbitos(1);
                        MétodosParaEncontrar(1, 2);
                        break;
                    case "r17":
                        MetodosParaAmbitos(1);
                        MétodosParaEncontrar(1, 0);
                        break;
                    case "r18":
                        MetodosParaAmbitos(3);
                        break;
                    case "r19":
                        MetodosParaAmbitos(3);
                        break;
                    case "r26":
                        MétodosParaEncontrar(1, 1);
                        break;
                    case "r27":
                        MétodosParaEncontrar(3, 27);
                        MetodosParaAmbitos(2);
                        if (retorno)
                        {
                            IndicarError(7, TablaSimbolos.Find(x => x.name == Auxiliar[4].name && x.subnivel == ambitoBase.First().ToString()), "");
                        }
                        retorno = false;
                        break;
                    case "r28":
                        MétodosParaEncontrar(3, 28);
                        MetodosParaAmbitos(2);
                        if (!retorno)
                        {
                            IndicarError(8, TablaSimbolos.Find(x => x.name == Auxiliar[4].name &&x.subnivel == ambitoBase.First().ToString()), "");
                        }
                        retorno = false;
                        break;
                    case "r29":
                        MétodosParaEncontrar(1, 1);
                        break;
                    case "r30":
                        MetodosParaAmbitos(4);
                        break;
                    case "r31":
                        MetodosParaAmbitos(4);
                        break;
                    case "r32":
                        MétodosParaEncontrar(3, 32);
                        MetodosParaAmbitos(2);
                        break;
                    case "33":
                        MétodosParaEncontrar(3, 33);
                        MetodosParaAmbitos(2);
                        break;
                    case "r39":
                        MétodosParaEncontrar(1, 1);
                        break;
                    case "r40":
                        MétodosParaEncontrar(1, 1);
                        break;
                    case "r41":
                        variables = new Stack<Token>();
                        error = false;
                        break;
                    case "r48":
                        retorno = true;
                        break;
                    case "r59":
                        MétodosParaEncontrar(59, 0);
                        break;
                    case "r62":
                        MétodosParaEncontrar(62, 0);
                        break;
                    case "r64":
                        MétodosParaEncontrar(64, 0);
                        break;
                    case "r66":
                        MétodosParaEncontrar(66, 0);
                        break;
                    case "r67":
                        MétodosParaEncontrar(67, 0);
                        break;
                    case "r71":
                        MétodosParaEncontrar(71, 0);
                        break;
                    case "r72":
                        MétodosParaEncontrar(72, 0);
                        break;
                    case "r74":
                        MétodosParaEncontrar(74, 0);
                        break;
                    case "r75":
                        MétodosParaEncontrar(75, 0);
                        break;
                    case "r82":
                        var Igual = TablaSimbolos.Find(x => x.name == Auxiliar[0].name&& x.subnivel == ambitoBase.First().ToString());
                        if(Igual!=null)
                        {
                            Auxiliar[0].valor = Igual.valor;
                        }
                        variables.Push(Auxiliar[0]);
                        break;
                    case "r83":
                        Auxiliar[0].valor = "new";
                        variables.Push(Auxiliar[0]);
                        break;
                    case "r84":
                        Auxiliar[0].valor = Auxiliar[0].name;
                        variables.Push(Auxiliar[0]);
                        break;
                    case "r85":
                        Auxiliar[0].valor = Auxiliar[0].name;
                        variables.Push(Auxiliar[0]);
                        break;
                    case "r86":
                        Auxiliar[0].valor = Auxiliar[0].name;
                        variables.Push(Auxiliar[0]);
                        break;
                    case "r87":
                        Auxiliar[0].valor = Auxiliar[0].name;
                        variables.Push(Auxiliar[0]);
                        break;
                    case "r88":
                        Auxiliar[0].valor = Auxiliar[0].name;
                        variables.Push(Auxiliar[0]);
                        break;
                }
            }
        }
        private static int ambito = 0;
        private static Stack<int> ambitoBase = new Stack<int>();
        private static bool aumento = false;
        private static bool aumentoSpecial = false;
        private static Dictionary<int, string> nombreAmbitos = new Dictionary<int, string>{ {0, "root" } };
        private static void MetodosParaAmbitos(int caso)
        {
            switch (caso)
            {
                case 1:
                    if (!aumento)
                    {
                        aumento = true;
                        ambito++;
                        ambitoBase.Push(ambito);
                    }
                    break;
                case 2:
                    nombreAmbitos.Add(ambitoBase.Pop(), Auxiliar[Auxiliar.Count - 2].name);
                    aumento = false;
                    break;
                case 3:
                    ambito++;
                    ambitoBase.Push(ambito);
                    break;
                case 4:
                    if (!aumentoSpecial)
                    {
                        aumentoSpecial = true;
                        ambito++;
                        ambitoBase.Push(ambito);
                    }
                    break;
                case 5:
                    nombreAmbitos.Add(ambitoBase.Pop(), Auxiliar[Auxiliar.Count - 2].name);
                    aumentoSpecial = false;
                    break;
                default:
                    break;
            }
        }
        private static void MétodosParaEncontrar(int numero, int numero2)
        {
            double valorAux = 0;
            double valorAux2 = 0;
            var valorAux3 = false;
            var valorAux4 = false;
            double resultado = 0;
            var resultado2 = false;
            var token2 = new Token();
            var valor = string.Empty;
            switch (numero)
            {
                case 1:
                    var type = tipo.Pop();
                    var tok = Auxiliar[numero2];
                    var Igual = TablaSimbolos.Find(X => X.name == tok.name && X.subnivel == tok.subnivel);
                    if (Igual == null)
                    {
                        tok.tipoAS = type;
                        tok.subnivel = ambitoBase.Peek().ToString();
                        TablaSimbolos.Add(tok);
                    }
                    else
                    {
                        cantError++;
                        IndicarError(1, tok, "");
                    }
                    break;
                case 3:
                    var t = Auxiliar[Auxiliar.Count - 2];
                    if (numero2 == 4 || numero2 == 27)
                    {
                        t.tipoAS = "Process";
                    }
                    else if (numero2 == 5 || numero2 == 28)
                    {
                        t.tipoAS = "Function";
                    }
                    else if (numero2 == 7)
                    {
                        t.tipoAS = "Class";
                    }
                    else if (numero2 == 32)
                    {
                        t.tipoAS = "IProcess";
                    }
                    else if (numero2 == 33)
                    {
                        t.tipoAS = "Ifunction";
                    }
                    else if (numero2 == 8)
                    {
                        t.tipoAS = "Interface";
                    }
                    t.subnivel = ambitoBase.Peek().ToString();
                    TablaSimbolos.Add(t);
                    break;
                case 59:
                    tok = Auxiliar[2];
                    var tok1 = variables.Pop();
                    Igual = TablaSimbolos.Find(X => X.name == tok.name && X.subnivel == ambitoBase.First().ToString());
                    if (Igual != null)
                    {
                        tok = Igual;
                        if (tok1.tipo == 6)
                        {
                            var Igual2 = TablaSimbolos.Find(x => x.name == tok1.name);
                            if (Igual2 != null)
                            {
                                if (Igual.tipoAS == Igual2.tipoAS || (Igual.tipoAS == "double" && Igual2.tipoAS == "int"))
                                {
                                    TablaSimbolos.Remove(Igual);
                                    Igual.valor = tok1.valor;
                                    TablaSimbolos.Add(Igual);
                                }
                                else
                                {
                                    cantError++;
                                    IndicarError(4, tok, "");
                                    error = true;
                                }
                            }
                            else
                            {
                                cantError++;
                                IndicarError(4, tok1, "");
                                error = true;
                            }
                        }
                        else if (tok1.tipo == 1 && tok.tipoAS == "boolean")
                        {
                            TablaSimbolos.Remove(Igual);
                            Igual.valor = tok1.valor;
                            TablaSimbolos.Add(Igual);
                        }
                        else if (tok1.tipo == 2 && tok.tipoAS == "int")
                        {
                            TablaSimbolos.Remove(Igual);
                            Igual.valor = tok1.valor;
                            TablaSimbolos.Add(Igual);
                        }
                        else if ((tok1.tipo == 3 || tok1.tipo == 4 || tok1.tipo == 2) && tok.tipoAS == "double")
                        {
                            TablaSimbolos.Remove(Igual);
                            Igual.valor = tok1.valor;
                            TablaSimbolos.Add(Igual);
                        }
                        else if (tok1.tipo == 7 && tok.tipoAS == "string")
                        {
                            TablaSimbolos.Remove(Igual);
                            Igual.valor = tok1.valor;
                            TablaSimbolos.Add(Igual);
                        }
                        else if (tok1.valor == "null")
                        {
                            TablaSimbolos.Remove(Igual);
                            Igual.valor = tok1.valor;
                            TablaSimbolos.Add(Igual);
                        }
                        else if (tok1.valor == "new")
                        {
                            TablaSimbolos.Remove(Igual);
                            Igual.valor = string.Empty;
                            TablaSimbolos.Add(Igual);
                        }
                        else
                        {
                            cantError++;
                            IndicarError(6, tok, "=");
                            error = true;
                        }
                    }
                    else
                    {
                        cantError++;
                        IndicarError(4, tok, "");
                        error = true;
                    }
                    break;
                case 62:
                    Console.WriteLine("hola");
                    break;
                case 64:
                    tok = variables.Pop();
                    tok1 = variables.Pop();
                    var token1 = tok;
                    if (tok1.tipo != 6 && tok.tipo != 6)
                    {
                        if (tok.tipo == 1)
                        {
                            if (tok1.tipo == 1)
                            {
                                valorAux3 = (tok.valor == "true") ? true : false;
                                valorAux4 = (tok1.valor == "true") ? true : false;
                                resultado2 = (valorAux3 || valorAux4);
                                token1 = tok1;
                                token1.valor = (resultado2) ? "true" : "false";
                                token1.name = (resultado2) ? "true" : "false";
                                token1.tipo = 1;
                                variables.Push(token1);
                            }
                            else
                            {
                                cantError++;
                                IndicarError(2, tok1, "||");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(2, tok, "||");
                            error = true;
                        }
                    }
                    else if (tok1.tipo != 6 && tok.tipo == 6)
                    {
                        if (tok1.tipo == 1)
                        {
                            Igual = TablaSimbolos.Find(X => X.name == tok.name && X.subnivel == ambitoBase.First().ToString());
                            if (Igual != null)
                            {
                                if (Igual.tipoAS == "boolean")
                                {
                                    valorAux3 = (tok.valor == "true") ? true : false;
                                    valorAux4 = (tok1.valor == "true") ? true : false;
                                    resultado2 = (valorAux3 || valorAux4);
                                    token1 = tok1;
                                    token1.valor = (resultado2) ? "true" : "false";
                                    token1.name = (resultado2) ? "true" : "false";
                                    token1.tipo = 1;
                                    variables.Push(token1);
                                }
                                else
                                {
                                    cantError++;
                                    IndicarError(3, tok, "||");
                                    error = true;
                                }
                            }
                            else
                            {
                                cantError++;
                                IndicarError(4, tok, "||");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(2, tok1, "||");
                            error = true;
                        }
                    }
                    else if (tok1.tipo == 6 && tok.tipo != 6)
                    {
                        if (tok.tipo == 1)
                        {
                            Igual = TablaSimbolos.Find(X => X.name == tok1.name && X.subnivel == ambitoBase.First().ToString());
                            if (Igual != null)
                            {
                                if (Igual.tipoAS == "boolean")
                                {
                                    valorAux3 = (tok.valor == "true") ? true : false;
                                    valorAux4 = (tok1.valor == "true") ? true : false;
                                    resultado2 = (valorAux3 || valorAux4);
                                    token1 = tok1;
                                    token1.valor = (resultado2) ? "true" : "false";
                                    token1.name = (resultado2) ? "true" : "false";
                                    token1.tipo = 1;
                                    variables.Push(token1);
                                }
                                else
                                {
                                    cantError++;
                                    IndicarError(3, tok1, "||");
                                    error = true;
                                }
                            }
                            else
                            {
                                cantError++;
                                IndicarError(4, tok1, "||");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(2, tok, "||");
                            error = true;
                        }
                    }
                    else if (tok1.tipo == 6 && tok.tipo == 6)
                    {
                        Igual = TablaSimbolos.Find(X => X.name == tok.name && X.subnivel == ambitoBase.First().ToString());
                        if (Igual != null)
                        {
                            if (Igual.tipoAS == "boolean")
                            {
                                Igual = TablaSimbolos.Find(X => X.name == tok1.name && X.subnivel == ambitoBase.First().ToString());
                                if (Igual != null)
                                {
                                    if (Igual.tipoAS == "boolean")
                                    {
                                        valorAux3 = (tok.valor == "true") ? true : false;
                                        valorAux4 = (tok1.valor == "true") ? true : false;
                                        resultado2 = (valorAux3 || valorAux4);
                                        token1 = tok1;
                                        token1.valor = (resultado2) ? "true" : "false";
                                        token1.name = (resultado2) ? "true" : "false";
                                        token1.tipo = 1;
                                        variables.Push(token1);
                                    }
                                    else
                                    {
                                        cantError++;
                                        IndicarError(3, tok1, "||");
                                        error = true;
                                    }
                                }
                                else
                                {
                                    cantError++;
                                    IndicarError(4, tok1, "||");
                                    error = true;
                                }
                            }
                            else
                            {
                                cantError++;
                                IndicarError(3, tok, "||");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(4, tok, "||");
                            error = true;
                        }
                    }
                    break;
                case 66:
                    tok = variables.Pop();
                    tok1 = variables.Pop();
                    token1 = tok;
                    if (tok1.tipo != 6 && tok.tipo != 6)
                    {
                        if (tok.tipo == 2 || tok.tipo == 3 || tok.tipo == 4)
                        {
                            if (tok1.tipo == 2 || tok1.tipo == 3 || tok1.tipo == 4)
                            {
                                valorAux = (tok.valor != "") ? Convert.ToDouble(tok.valor) : 0;
                                valorAux2 = (tok1.valor != "") ? Convert.ToDouble(tok1.valor) : 0;
                                resultado2 = (valorAux2 > valorAux);
                                token1 = tok1;
                                token1.valor = (resultado2) ? "true" : "false";
                                token1.name = (resultado2) ? "true" : "false";
                                token1.tipo = 1;
                                variables.Push(token1);
                            }
                            else
                            {
                                cantError++;
                                IndicarError(2, tok1, ">");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(2, tok, ">");
                            error = true;
                        }
                    }
                    else if (tok1.tipo != 6 && tok.tipo == 6)
                    {
                        if (tok1.tipo == 2 || tok1.tipo == 3 || tok1.tipo == 4)
                        {
                            Igual = TablaSimbolos.Find(X => X.name == tok.name && X.subnivel == ambitoBase.First().ToString());
                            if (Igual != null)
                            {
                                if (Igual.tipoAS == "int" || Igual.tipoAS == "double")
                                {
                                    valorAux = (tok.valor != "") ? Convert.ToDouble(tok.valor) : 0;
                                    valorAux2 = (tok1.valor != "") ? Convert.ToDouble(tok1.valor) : 0;
                                    resultado2 = (valorAux2 > valorAux);
                                    token1 = tok1;
                                    token1.valor = (resultado2) ? "true" : "false";
                                    token1.name = (resultado2) ? "true" : "false";
                                    token1.tipo = 1;
                                    variables.Push(token1);
                                }
                                else
                                {
                                    cantError++;
                                    IndicarError(3, tok, ">");
                                    error = true;
                                }
                            }
                            else
                            {
                                cantError++;
                                IndicarError(4, tok, ">");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(2, tok1, ">");
                            error = true;
                        }
                    }
                    else if (tok1.tipo == 6 && tok.tipo != 6)
                    {
                        if (tok.tipo == 2 || tok.tipo == 3 || tok.tipo == 4)
                        {
                            Igual = TablaSimbolos.Find(X => X.name == tok1.name && X.subnivel == ambitoBase.First().ToString());
                            if (Igual != null)
                            {
                                if (Igual.tipoAS == "int" || Igual.tipoAS == "double")
                                {
                                    valorAux = (tok.valor != "") ? Convert.ToDouble(tok.valor) : 0;
                                    valorAux2 = (tok1.valor != "") ? Convert.ToDouble(tok1.valor) : 0;
                                    resultado2 = (valorAux2 > valorAux);
                                    token1 = tok1;
                                    token1.valor = (resultado2) ? "true" : "false";
                                    token1.name = (resultado2) ? "true" : "false";
                                    token1.tipo = 1;
                                    variables.Push(token1);
                                }
                                else
                                {
                                    cantError++;
                                    IndicarError(3, tok1, ">");
                                    error = true;
                                }
                            }
                            else
                            {
                                cantError++;
                                IndicarError(4, tok1, ">");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(2, tok, ">");
                            error = true;
                        }
                    }
                    else if (tok1.tipo == 6 && tok.tipo == 6)
                    {
                        Igual = TablaSimbolos.Find(X => X.name == tok.name && X.subnivel == ambitoBase.First().ToString());
                        if (Igual != null)
                        {
                            if (Igual.tipoAS == "int" || Igual.tipoAS == "double")
                            {
                                Igual = TablaSimbolos.Find(X => X.name == tok1.name && X.subnivel == ambitoBase.First().ToString());
                                if (Igual != null)
                                {
                                    if (Igual.tipoAS == "int" || Igual.tipoAS == "double")
                                    {
                                        valorAux = (tok.valor != "") ? Convert.ToDouble(tok.valor) : 0;
                                        valorAux2 = (tok1.valor != "") ? Convert.ToDouble(tok1.valor) : 0;
                                        resultado2 = (valorAux2 > valorAux);
                                        token1 = tok1;
                                        token1.valor = (resultado2) ? "true" : "false";
                                        token1.name = (resultado2) ? "true" : "false";
                                        token1.tipo = 1;
                                        variables.Push(token1);
                                    }
                                    else
                                    {
                                        cantError++;
                                        IndicarError(3, tok1, ">");
                                        error = true;
                                    }
                                }
                                else
                                {
                                    cantError++;
                                    IndicarError(4, tok1, ">");
                                    error = true;
                                }
                            }
                            else
                            {
                                cantError++;
                                IndicarError(3, tok, ">");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(4, tok, ">");
                            error = true;
                        }
                    }
                    break;
                case 67:
                    tok = variables.Pop();
                    tok1 = variables.Pop();
                    token1 = tok;
                    if(tok1.tipo != 6 && tok.tipo != 6)
                    {
                        if (tok.tipo == 2 || tok.tipo == 3 || tok.tipo == 4)
                        {
                            if (tok1.tipo == 2 || tok1.tipo == 3 || tok1.tipo == 4)
                            {
                                valorAux = (tok.valor != "") ? Convert.ToDouble(tok.valor) : 0;
                                valorAux2 = (tok1.valor != "") ? Convert.ToDouble(tok1.valor) : 0;
                                resultado2 = (valorAux2 >= valorAux);
                                token1 = tok1;
                                token1.valor = (resultado2)? "true":"false";
                                token1.name = (resultado2) ? "true" : "false";
                                token1.tipo = 1;
                                variables.Push(token1);
                            }
                            else
                            {
                                cantError++;
                                IndicarError(2, tok1, ">=");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(2, tok, ">=");
                            error = true;
                        }
                    }
                    else if (tok1.tipo != 6 && tok.tipo == 6)
                    {
                        if (tok1.tipo == 2 || tok1.tipo == 3 || tok1.tipo == 4)
                        {
                            Igual = TablaSimbolos.Find(X => X.name == tok.name && X.subnivel == ambitoBase.First().ToString());
                            if (Igual != null)
                            {
                                if (Igual.tipoAS == "int" || Igual.tipoAS == "double")
                                {
                                    valorAux = (tok.valor != "") ? Convert.ToDouble(tok.valor) : 0;
                                    valorAux2 = (tok1.valor != "") ? Convert.ToDouble(tok1.valor) : 0;
                                    resultado2 = (valorAux2 >= valorAux);
                                    token1 = tok1;
                                    token1.valor = (resultado2) ? "true" : "false";
                                    token1.name = (resultado2) ? "true" : "false";
                                    token1.tipo = 1;
                                    variables.Push(token1);
                                }
                                else
                                {
                                    cantError++;
                                    IndicarError(3, tok, ">=");
                                    error = true;
                                }
                            }
                            else
                            {
                                cantError++;
                                IndicarError(4, tok, ">=");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(2, tok1, ">=");
                            error = true;
                        }
                    }
                    else if (tok1.tipo == 6 && tok.tipo != 6)
                    {
                        if (tok.tipo == 2 || tok.tipo == 3 || tok.tipo == 4)
                        {
                            Igual = TablaSimbolos.Find(X => X.name == tok1.name && X.subnivel == ambitoBase.First().ToString());
                            if (Igual != null)
                            {
                                if (Igual.tipoAS == "int" || Igual.tipoAS == "double")
                                {
                                    valorAux = (tok.valor != "") ? Convert.ToDouble(tok.valor) : 0;
                                    valorAux2 = (tok1.valor != "") ? Convert.ToDouble(tok1.valor) : 0;
                                    resultado2 = (valorAux2 >= valorAux);
                                    token1 = tok1;
                                    token1.valor = (resultado2) ? "true" : "false";
                                    token1.name = (resultado2) ? "true" : "false";
                                    token1.tipo = 1;
                                    variables.Push(token1);
                                }
                                else
                                {
                                    cantError++;
                                    IndicarError(3, tok1, ">=");
                                    error = true;
                                }
                            }
                            else
                            {
                                cantError++;
                                IndicarError(4, tok1, ">=");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(2, tok, ">=");
                            error = true;
                        }
                    }
                    else if (tok1.tipo == 6 && tok.tipo == 6)
                    {
                        Igual = TablaSimbolos.Find(X => X.name == tok.name && X.subnivel == ambitoBase.First().ToString());
                        if (Igual != null)
                        {
                            if (Igual.tipoAS == "int" || Igual.tipoAS == "double")
                            {
                                Igual = TablaSimbolos.Find(X => X.name == tok1.name && X.subnivel == ambitoBase.First().ToString());
                                if (Igual != null)
                                {
                                    if (Igual.tipoAS == "int" || Igual.tipoAS == "double")
                                    {
                                        valorAux = (tok.valor != "") ? Convert.ToDouble(tok.valor) : 0;
                                        valorAux2 = (tok1.valor != "") ? Convert.ToDouble(tok1.valor) : 0;
                                        resultado2 = (valorAux2 >= valorAux);
                                        token1 = tok1;
                                        token1.valor = (resultado2) ? "true" : "false";
                                        token1.name = (resultado2) ? "true" : "false";
                                        token1.tipo = 1;
                                        variables.Push(token1);
                                    }
                                    else
                                    {
                                        cantError++;
                                        IndicarError(3, tok1, ">=");
                                        error = true;
                                    }
                                }
                                else
                                {
                                    cantError++;
                                    IndicarError(4, tok1, ">=");
                                    error = true;
                                }
                            }
                            else
                            {
                                cantError++;
                                IndicarError(3, tok, ">=");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(4, tok, ">=");
                            error = true;
                        }
                    }
                    break;
                case 69:
                    tok = variables.Pop();
                    tok1 = variables.Pop();
                    token1 = tok;
                    if (tok1.tipo != 6 && tok.tipo != 6)
                    {
                        if (tok.tipo == 2 || tok.tipo == 3 || tok.tipo == 4)
                        {
                            if (tok1.tipo == 2 || tok1.tipo == 3 || tok1.tipo == 4)
                            {
                                valorAux = (tok.valor != "") ? Convert.ToDouble(tok.valor) : 0;
                                valorAux2 = (tok1.valor != "") ? Convert.ToDouble(tok1.valor) : 0;
                                resultado = (valorAux2 - valorAux);
                                token1 = tok1;
                                token1.valor = resultado.ToString();
                                token1.name = resultado.ToString();
                                token1.tipo = 2;
                                variables.Push(token1);
                            }
                            else
                            {
                                cantError++;
                                IndicarError(2, tok1, "-");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(2, tok, "-");
                            error = true;
                        }
                    }
                    else if (tok1.tipo != 6 && tok.tipo == 6)
                    {
                        if (tok1.tipo == 2 || tok1.tipo == 3 || tok1.tipo == 4)
                        {
                            Igual = TablaSimbolos.Find(X => X.name == tok.name && X.subnivel == ambitoBase.First().ToString());
                            if (Igual != null)
                            {
                                if (Igual.tipoAS == "int" || Igual.tipoAS == "double")
                                {
                                    valorAux = (tok.valor != "") ? Convert.ToDouble(tok.valor) : 0;
                                    valorAux2 = (tok1.valor != "") ? Convert.ToDouble(tok1.valor) : 0;
                                    resultado = (valorAux2 - valorAux);
                                    token1 = tok1;
                                    token1.valor = resultado.ToString();
                                    token1.name = resultado.ToString();
                                    token1.tipo = 2;
                                    variables.Push(token1);
                                }
                                else
                                {
                                    cantError++;
                                    IndicarError(3, tok, "-");
                                    error = true;
                                }
                            }
                            else
                            {
                                cantError++;
                                IndicarError(4, tok, "-");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(2, tok1, "-");
                            error = true;
                        }
                    }
                    else if (tok1.tipo == 6 && tok.tipo != 6)
                    {
                        if (tok.tipo == 2 || tok.tipo == 3 || tok.tipo == 4)
                        {
                            Igual = TablaSimbolos.Find(X => X.name == tok1.name && X.subnivel == ambitoBase.First().ToString());
                            if (Igual != null)
                            {
                                if (Igual.tipoAS == "int" || Igual.tipoAS == "double")
                                {
                                    valorAux = (tok.valor != "") ? Convert.ToDouble(tok.valor) : 0;
                                    valorAux2 = (tok1.valor != "") ? Convert.ToDouble(tok1.valor) : 0;
                                    resultado = (valorAux2 - valorAux);
                                    token1 = tok1;
                                    token1.valor = resultado.ToString();
                                    token1.name = resultado.ToString();
                                    token1.tipo = 2;
                                    variables.Push(token1);
                                }
                                else
                                {
                                    cantError++;
                                    IndicarError(3, tok1, "-");
                                    error = true;
                                }
                            }
                            else
                            {
                                cantError++;
                                IndicarError(4, tok1, "-");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(2, tok, "-");
                            error = true;
                        }
                    }
                    else if (tok1.tipo == 6 && tok.tipo == 6)
                    {
                        Igual = TablaSimbolos.Find(X => X.name == tok.name && X.subnivel == ambitoBase.First().ToString());
                        if (Igual != null)
                        {
                            if (Igual.tipoAS == "int" || Igual.tipoAS == "double")
                            {
                                Igual = TablaSimbolos.Find(X => X.name == tok1.name && X.subnivel == ambitoBase.First().ToString());
                                if (Igual != null)
                                {
                                    if (Igual.tipoAS == "int" || Igual.tipoAS == "double")
                                    {
                                        valorAux = (tok.valor != "") ? Convert.ToDouble(tok.valor) : 0;
                                        valorAux2 = (tok1.valor != "") ? Convert.ToDouble(tok1.valor) : 0;
                                        resultado = (valorAux2 - valorAux);
                                        token1 = tok1;
                                        token1.valor = resultado.ToString();
                                        token1.name = resultado.ToString();
                                        token1.tipo = 2;
                                        variables.Push(token1);
                                    }
                                    else
                                    {
                                        cantError++;
                                        IndicarError(3, tok1, "-");
                                        error = true;
                                    }
                                }
                                else
                                {
                                    cantError++;
                                    IndicarError(4, tok1, "-");
                                    error = true;
                                }
                            }
                            else
                            {
                                cantError++;
                                IndicarError(3, tok, "-");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(4, tok, "-");
                            error = true;
                        }
                    }
                    break;
                case 71:
                    tok = variables.Pop();
                    tok1 = variables.Pop();
                    token1 = tok;
                    if (tok1.tipo != 6 && tok.tipo != 6)
                    {
                        if (tok.tipo == 2 || tok.tipo == 3 || tok.tipo == 4)
                        {
                            if (tok1.tipo == 2 || tok1.tipo == 3 || tok1.tipo == 4)
                            {
                                valorAux = (tok.valor != "") ? Convert.ToDouble(tok.valor) : 0;
                                if (valorAux != 0)
                                {
                                    valorAux2 = (tok1.valor != "") ? Convert.ToDouble(tok1.valor) : 0;
                                    resultado = (valorAux2 / valorAux);
                                    token1 = tok1;
                                    token1.valor = resultado.ToString();
                                    token1.name = resultado.ToString();
                                    token1.tipo = 2;
                                    variables.Push(token1);
                                }
                                else
                                {
                                    cantError++;
                                    IndicarError(5, tok, "/");
                                    error = true;
                                }
                            }
                            else
                            {
                                cantError++;
                                IndicarError(2, tok1, "/");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(2, tok, "/");
                            error = true;
                        }
                    }
                    else if (tok1.tipo != 6 && tok.tipo == 6)
                    {
                        if (tok1.tipo == 2 || tok1.tipo == 3 || tok1.tipo == 4)
                        {
                            Igual = TablaSimbolos.Find(X => X.name == tok.name && X.subnivel == ambitoBase.First().ToString());
                            if (Igual != null)
                            {
                                if (Igual.tipoAS == "int" || Igual.tipoAS == "double")
                                {
                                    valorAux = (tok.valor != "") ? Convert.ToDouble(tok.valor) : 0;
                                    if (valorAux != 0)
                                    {
                                        valorAux2 = (tok1.valor != "") ? Convert.ToDouble(tok1.valor) : 0;
                                        resultado = (valorAux2 / valorAux);
                                        token1 = tok1;
                                        token1.valor = resultado.ToString();
                                        token1.name = resultado.ToString();
                                        token1.tipo = 2;
                                        variables.Push(token1);
                                    }
                                    else
                                    {
                                        cantError++;
                                        IndicarError(5, tok, "/");
                                        error = true;
                                    }
                                }
                                else
                                {
                                    cantError++;
                                    IndicarError(3, tok, "/");
                                    error = true;
                                }
                            }
                            else
                            {
                                cantError++;
                                IndicarError(4, tok, "/");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(2, tok1, "/");
                            error = true;
                        }
                    }
                    else if (tok1.tipo == 6 && tok.tipo != 6)
                    {
                        if (tok.tipo == 2 || tok.tipo == 3 || tok.tipo == 4)
                        {
                            Igual = TablaSimbolos.Find(X => X.name == tok1.name && X.subnivel == ambitoBase.First().ToString());
                            if (Igual != null)
                            {
                                if (Igual.tipoAS == "int" || Igual.tipoAS == "double")
                                {
                                    valorAux = (tok.valor != "") ? Convert.ToDouble(tok.valor) : 0;
                                    if (valorAux != 0)
                                    {
                                        valorAux2 = (tok1.valor != "") ? Convert.ToDouble(tok1.valor) : 0;
                                        resultado = (valorAux2 / valorAux);
                                        token1 = tok1;
                                        token1.valor = resultado.ToString();
                                        token1.name = resultado.ToString();
                                        token1.tipo = 2;
                                        variables.Push(token1);
                                    }
                                    else
                                    {
                                        cantError++;
                                        IndicarError(5, tok, "/");
                                        error = true;
                                    }
                                }
                                else
                                {
                                    cantError++;
                                    IndicarError(3, tok1, "/");
                                    error = true;
                                }
                            }
                            else
                            {
                                cantError++;
                                IndicarError(4, tok1, "/");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(2, tok, "/");
                            error = true;
                        }
                    }
                    else if (tok1.tipo == 6 && tok.tipo == 6)
                    {
                        Igual = TablaSimbolos.Find(X => X.name == tok.name && X.subnivel == ambitoBase.First().ToString());
                        if (Igual != null)
                        {
                            if (Igual.tipoAS == "int" || Igual.tipoAS == "double")
                            {
                                Igual = TablaSimbolos.Find(X => X.name == tok1.name && X.subnivel == ambitoBase.First().ToString());
                                if (Igual != null)
                                {
                                    if (Igual.tipoAS == "int" || Igual.tipoAS == "double")
                                    {
                                        valorAux = (tok.valor != "") ? Convert.ToDouble(tok.valor) : 0;
                                        if (valorAux != 0)
                                        {
                                            valorAux2 = (tok1.valor != "") ? Convert.ToDouble(tok1.valor) : 0;
                                            resultado = (valorAux2 / valorAux);
                                            token1 = tok1;
                                            token1.valor = resultado.ToString();
                                            token1.name = resultado.ToString();
                                            token1.tipo = 2;
                                            variables.Push(token1);
                                        }
                                        else
                                        {
                                            cantError++;
                                            IndicarError(5, tok, "/");
                                            error = true;
                                        }
                                    }
                                    else
                                    {
                                        cantError++;
                                        IndicarError(3, tok1, "/");
                                        error = true;
                                    }
                                }
                                else
                                {
                                    cantError++;
                                    IndicarError(4, tok1, "/");
                                    error = true;
                                }
                            }
                            else
                            {
                                cantError++;
                                IndicarError(3, tok, "/");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(4, tok, "/");
                            error = true;
                        }
                    }
                    break;
                case 72:
                    tok = variables.Pop();
                    tok1 = variables.Pop();
                    token1 = tok;
                    if (tok1.tipo != 6 && tok.tipo != 6)
                    {
                        if (tok.tipo == 2 || tok.tipo == 3 || tok.tipo == 4)
                        {
                            if (tok1.tipo == 2 || tok1.tipo == 3 || tok1.tipo == 4)
                            {
                                valorAux = (tok.valor != "") ? Convert.ToDouble(tok.valor) : 0;
                                if (valorAux != 0)
                                {
                                    valorAux2 = (tok1.valor != "") ? Convert.ToDouble(tok1.valor) : 0;
                                    resultado = (valorAux2 % valorAux);
                                    token1 = tok1;
                                    token1.valor = resultado.ToString();
                                    token1.name = resultado.ToString();
                                    token1.tipo = 2;
                                    variables.Push(token1);
                                }
                                else
                                {
                                    cantError++;
                                    IndicarError(5, tok, "%");
                                    error = true;
                                }
                            }
                            else
                            {
                                cantError++;
                                IndicarError(2, tok1, "%");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(2, tok, "%");
                            error = true;
                        }
                    }
                    else if (tok1.tipo != 6 && tok.tipo == 6)
                    {
                        if (tok1.tipo == 2 || tok1.tipo == 3 || tok1.tipo == 4)
                        {
                            Igual = TablaSimbolos.Find(X => X.name == tok.name && X.subnivel == ambitoBase.First().ToString());
                            if(Igual != null)
                            {
                                if (Igual.tipoAS =="int"|| Igual.tipoAS == "double")
                                {
                                    valorAux = (tok.valor != "") ? Convert.ToDouble(tok.valor) : 0;
                                    if (valorAux != 0)
                                    {
                                        valorAux2 = (tok1.valor != "") ? Convert.ToDouble(tok1.valor) : 0;
                                        resultado = (valorAux2 % valorAux);
                                        token1 = tok1;
                                        token1.valor = resultado.ToString();
                                        token1.name = resultado.ToString();
                                        token1.tipo = 2;
                                        variables.Push(token1);
                                    }
                                    else
                                    {
                                        cantError++;
                                        IndicarError(5, tok, "%");
                                        error = true;
                                    }
                                }
                                else
                                {
                                    cantError++;
                                    IndicarError(3, tok, "%");
                                    error = true;
                                }
                            }
                            else
                            {
                                cantError++;
                                IndicarError(4, tok, "%");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(2, tok1, "%");
                            error = true;
                        }
                    }
                    else if (tok1.tipo == 6 && tok.tipo != 6)
                    {
                        if (tok.tipo == 2 || tok.tipo == 3 || tok.tipo == 4)
                        {
                            Igual = TablaSimbolos.Find(X => X.name == tok1.name && X.subnivel == ambitoBase.First().ToString());
                            if (Igual != null)
                            {
                                if (Igual.tipoAS == "int" || Igual.tipoAS == "double")
                                {
                                    valorAux = (tok.valor != "") ? Convert.ToDouble(tok.valor) : 0;
                                    if (valorAux != 0)
                                    {
                                        valorAux2 = (tok1.valor != "") ? Convert.ToDouble(tok1.valor) : 0;
                                        resultado = (valorAux2 % valorAux);
                                        token1 = tok1;
                                        token1.valor = resultado.ToString();
                                        token1.name = resultado.ToString();
                                        token1.tipo = 2;
                                        variables.Push(token1);
                                    }
                                    else
                                    {
                                        cantError++;
                                        IndicarError(5, tok, "%");
                                        error = true;
                                    }
                                }
                                else
                                {
                                    cantError++;
                                    IndicarError(3, tok1, "%");
                                    error = true;
                                }
                            }
                            else
                            {
                                cantError++;
                                IndicarError(4, tok1, "%");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(2, tok, "%");
                            error = true;
                        }
                    }
                    else if (tok1.tipo == 6 && tok.tipo == 6)
                    {
                        Igual = TablaSimbolos.Find(X => X.name == tok.name && X.subnivel == ambitoBase.First().ToString());
                        if (Igual != null)
                        {
                            if (Igual.tipoAS == "int" || Igual.tipoAS == "double")
                            {
                                Igual = TablaSimbolos.Find(X => X.name == tok1.name && X.subnivel == ambitoBase.First().ToString());
                                if (Igual != null)
                                {
                                    if (Igual.tipoAS == "int" || Igual.tipoAS == "double")
                                    {
                                        valorAux = (tok.valor != "") ? Convert.ToDouble(tok.valor) : 0;
                                        if(valorAux != 0)
                                        {
                                            valorAux2 = (tok1.valor != "") ? Convert.ToDouble(tok1.valor) : 0;
                                            resultado = (valorAux2 % valorAux);
                                            token1 = tok1;
                                            token1.valor = resultado.ToString();
                                            token1.name = resultado.ToString();
                                            token1.tipo = 2;
                                            variables.Push(token1);
                                        }
                                        else
                                        {
                                            cantError++;
                                            IndicarError(5, tok, "%");
                                            error = true;
                                        }
                                    }
                                    else
                                    {
                                        cantError++;
                                        IndicarError(3, tok1, "%");
                                        error = true;
                                    }
                                }
                                else
                                {
                                    cantError++;
                                    IndicarError(4, tok1, "%");
                                    error = true;
                                }
                            }
                            else
                            {
                                cantError++;
                                IndicarError(3, tok, "%");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(4, tok, "%");
                            error = true;
                        }
                    }
                    break;
                case 74:
                    valor = string.Empty;
                    valorAux = 0;
                    tok = variables.Pop();
                    token1 = tok;
                    if (tok.tipo != 6)
                    {
                        if (tok.tipo == 2 || tok.tipo == 3|| tok.tipo == 4)
                        {
                            valorAux = (tok.valor != "")?Convert.ToDouble(tok.valor):0;
                            valorAux *= -1; 
                            token1 = tok;
                            token1.valor = valorAux.ToString();
                            variables.Push(token1);
                        }
                        else
                        {
                            cantError++;
                            IndicarError(2, tok,"-");
                            error = true;
                        }
                    }
                    else
                    {
                        Igual = TablaSimbolos.Find(X => X.name == tok.name && X.subnivel == ambitoBase.First().ToString());
                        if (Igual != null)
                        {
                            if (Igual.tipoAS == "int"|| Igual.tipoAS == "double")
                            {
                                valorAux = (tok.valor != "") ? Convert.ToDouble(tok.valor) : 0;
                                valorAux *= -1;
                                token1 = tok;
                                token1.valor = valorAux.ToString();
                                variables.Push(token1);
                            }
                            else
                            {
                                cantError++;
                                IndicarError(3, tok,"-");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(4, tok,"-");
                            error = true;
                        }
                    }
                    break;
                case 75:
                    valor = string.Empty;
                    tok=variables.Pop();
                    token1 = tok;
                    if (tok.tipo != 6)
                    {
                        if (tok.valor == "true" || tok.valor == "false")
                        {
                            valor = (tok.valor == "true") ? "false" : "true";
                            token1 = tok;
                            token1.valor = valor;
                            variables.Push(token1);
                        }
                        else
                        {
                            cantError++;
                            IndicarError(2,tok,"!");
                            error = true;
                        }
                    }
                    else
                    {
                        Igual = TablaSimbolos.Find(X => X.name == tok.name && X.subnivel == ambitoBase.First().ToString());
                        if (Igual != null)
                        {
                            if (Igual.tipoAS == "boolean")
                            {
                                valor = (tok.valor == "") ? "true" : (tok.valor == "false") ? "true" : "false";
                                token1 = tok;
                                token1.valor = valor;
                                variables.Push(token1);
                            }
                            else
                            {
                                cantError++;
                                IndicarError(3, tok,"!");
                                error = true;
                            }
                        }
                        else
                        {
                            cantError++;
                            IndicarError(4, tok,"!");
                            error = true;
                        }
                    }
                    break;
            }
        }
        private static void IndicarError(int num, Token tok, string toke)
        {
            switch(num)
            {
                case 1:
                    Console.WriteLine($"**ERROR** EL TOKEN: {tok.name} LOCALIZADO EN LA LINEA: {tok.line} Y COLUMNAS: {tok.columnFirst} - {tok.columnEnd} YA SE ENCUENTRA DECLARADO");
                    break;
                case 2:
                    Console.WriteLine($"**ERROR** EL TOKEN: {tok.name} LOCALIZADO EN LA LINEA: {tok.line} Y COLUMNAS: {tok.columnFirst} - {tok.columnEnd} NO POSEE EL TIPO INDICADO PARA SER USADO POR EL TOKEN {toke}");
                    break;
                case 3:
                    Console.WriteLine($"**ERROR** EL TOKEN: {tok.name} LOCALIZADO EN LA LINEA: {tok.line} Y COLUMNAS: {tok.columnFirst} - {tok.columnEnd} IDENTIFICADOR NO POSEE EL TIPO ADECUADO PARA SER UTILIZADO POR EL TOKEN {toke}");
                    break;
                case 4:
                    Console.WriteLine($"**ERROR** EL TOKEN: {tok.name} LOCALIZADO EN LA LINEA: {tok.line} Y COLUMNAS: {tok.columnFirst} - {tok.columnEnd} NO SE ENCUENTRA DECLARADO");
                    break;
                case 5:
                    Console.WriteLine($"**ERROR** EL TOKEN: {tok.name} LOCALIZADO EN LA LINEA: {tok.line} Y COLUMNAS: {tok.columnFirst} - {tok.columnEnd} ESTA TRATANDO DE REALIZAR UNA DIVISIÓN ENTRE 0");
                    break;
                case 6:
                    Console.WriteLine($"**ERROR** EL TOKEN: {tok.name} LOCALIZADO EN LA LINEA: {tok.line} Y COLUMNAS: {tok.columnFirst} - {tok.columnEnd} NO SE LE PUEDE REALIZAR DICHA ASIGNACIÓN");
                    break;
                case 7:
                    Console.WriteLine($"**ERROR** EL TOKEN: {tok.name} LOCALIZADO EN LA LINEA: {tok.line} Y COLUMNAS: {tok.columnFirst} - {tok.columnEnd} EL PROCEDIMIENTO POSEE UNA SENTENCIA RETURN INVALIDA");
                    break;
                case 8:
                    Console.WriteLine($"**ERROR** EL TOKEN: {tok.name} LOCALIZADO EN LA LINEA: {tok.line} Y COLUMNAS: {tok.columnFirst} - {tok.columnEnd} LA FUNCION NO POSEE UNA SENTENCIA RETURN");
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
