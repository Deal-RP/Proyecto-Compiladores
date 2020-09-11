using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace mimij
{
    class SintaxisDescRecur
    {
        public static List<Token> orden = new List<Token>();
        static int pos, contReturn, contPrint, contExpr, contVariable;
        static string msg = string.Empty;
        static void getNewLine()
        {
            int firstLine = 0;
            var first = true;
            var encontrado = false;
            while (pos < orden.Count && !encontrado)
            {
                if (first)
                {
                    firstLine = orden[pos].line;
                    first = false;
                }
                else if (firstLine != orden[pos].line)
                {
                    encontrado = true;
                    pos--;
                }
                pos++;
            }
            if (first)
            {
                pos++;
            }
        }
        public static void AnalizadorS()
        {
            var cont = 0;
            var error = 0;
            pos = 0;
            do
            {
                if (ntDecl())
                {
                    cont++;
                }
                else
                {
                    Console.WriteLine(msg);
                    error++;
                    getNewLine();
                }
            } while (pos < orden.Count);

            if (cont == 0)
            {
                Console.WriteLine("Se esperaba por lo menos una declaracion");
            }
            else if (error == 0)
            {
                Console.WriteLine("No se encontraron errores sintacticos");
            }
        }
        static bool ntDecl()
        {
            return ntVariableDecl() || ntFunctionDecl();
        }
        static bool ntVariableDecl()
        {
            if (ntVariable())
            {
                var actual = orden[pos];
                if (actual.name == ";")
                {
                    pos++;
                    return true;
                }
                else
                {
                    msg = $"ERROR: SE ESPERABA UN ';' EN LA LINEA {actual.line}";
                }
            }
            return false;
        }
        static bool ntVariable()
        {
            if (ntType())
            {
                var actual = orden[pos];
                if (actual.tipo == 6)
                {
                    pos++;
                    return true;
                }
                else { msg = $"ERROR: SE ESPERABA UN IDENTIFICADOR EN LA LINEA:  {actual.line}"; }
            }
            return false;
        }
        static bool ntType()
        {
            var actual = orden[pos];
            if (actual.name == "int" || actual.name == "double" || actual.name == "boolean" || actual.name == "string" || actual.tipo == 6)
            {
                contVariable++;
                pos++;
                return ntDType();
            }
            msg = $"ERROR: FALTAN TIPO DE VARIABLE EN LA LINEA {actual.line}";
            return false;
        }
        static bool ntDType()
        {
            var actual = orden[pos];
            if (actual.name == "[]")
            {
                pos++;
                return ntDType();
            }
            else if (actual.name == "[")
            {
                pos++;
                actual = orden[pos];
                if (actual.name == "]")
                {
                    pos++;
                    return ntDType();
                }
                else { msg = $"ERROR: FALTAN ']' EN LA LINEA {actual.line}"; return false; }
            }
            return true;
        }
        static bool ntFunctionDecl()
        {
            var actual = orden[pos];
            if (ntType())
            {
                actual = orden[pos];
                if (actual.tipo == 6)
                {
                    pos++;
                    actual = orden[pos];
                    if (actual.name == "(")
                    {
                        pos++;
                        if (ntFormals())
                        {
                            actual = orden[pos];
                            if (actual.name == ")")
                            {
                                pos++;
                                return stmtCerradura();
                            }
                            else { msg = $"ERROR: FALTA ')' EN LA LINEA {actual.line}"; }
                        }
                    }
                    else if(actual.name =="()")
                    {
                        pos++;
                        return stmtCerradura();
                    }
                    else { msg = $"ERROR: '(' EN LA LINEA {actual.line}"; }
                }
                else { msg = $"ERROR: FALTA IDENTIFICADOR EN LA LINEA {actual.line}"; }
            }
            else if (actual.name == "void")
            {
                pos++;
                actual = orden[pos];
                if (actual.tipo == 6)
                {
                    pos++;
                    actual = orden[pos];
                    if (actual.name == "(")
                    {
                        pos++;
                        if (ntFormals())
                        {
                            actual = orden[pos];
                            if (actual.name == ")")
                            {
                                pos++;
                                return stmtCerradura();
                            }
                            else { msg = $"ERROR: FALTA ')' EN LA LINEA {actual.line}"; }
                        }
                    }
                    else if(actual.name == "()")
                    {
                        pos++;
                        return stmtCerradura();
                    }
                    else { msg = $"ERROR: '(' EN LA LINEA {actual.line}"; }
                }
                else { msg = $"ERROR: FALTA IDENTIFICADOR {actual.line}"; }
            }
            return false;
        }
        static bool stmtCerradura()
        {
            var cont = 0;
            var posibleError = false;
            while (pos < orden.Count && !posibleError)
            {
                if (ntStmt())
                {
                    cont++;
                }
                else
                {
                    if (contExpr > 0 || contPrint > 0 || contReturn > 0)
                    {
                        return false;
                    }
                    else
                    {
                        posibleError = true;
                    }
                }
            }
            return true;
        }
        static bool ntStmt()
        {
            contExpr = 0;
            contReturn = 0;
            if (!ntReturnStmt())
            {
                if (contReturn > 0 && contExpr > 0) { return false; }
                contExpr = 0;
                contPrint = 0;
                if (!ntPrintStmt())
                {
                    if (contPrint > 0 && contExpr > 0) { return false; }
                    contExpr = 0;
                    if (!ntExpr()) { return contExpr > 0 ? false : true; }
                    else
                    {
                        var actual = orden[pos];
                        if (actual.name == ";")
                        {
                            pos++;
                            return true;
                        }
                    }
                }
            }
            return true;
        }
        static bool ntFormals()
        {
            // Variable +
            var cont = 0;
            var posibleError = false;
            while (pos < orden.Count && !posibleError)
            {
                contVariable = 0;
                if (ntVariable())
                {
                    cont++;
                }
                else
                {
                    if (contVariable > 0)
                    {
                        return false;
                    }
                    else
                    {
                        posibleError = true;
                    }
                }
            }
            if (cont != 0)
            {
                var actual = orden[pos];
                if (actual.name == ",") { pos++; }
                else
                {
                    msg = "ERROR FALTA ','";
                    return false;
                }
                msg = $"Se esperaba por lo menos una variable en la linea {actual.line}";
                return false;
            }
            return true;
        }
        static bool ntReturnStmt()
        {
            var actual = orden[pos];
            if (actual.name == "return")
            {
                contReturn++;
                pos++;
                if (!ntExpr() && contExpr > 0) { return false; }
                return true;
            }
            return false;
        }
        static bool ntPrintStmt()
        {
            var actual = orden[pos];
            if (actual.name == "Print")
            {
                contPrint++;
                pos++;
                if (actual.name == "(")
                {
                    pos++;
                    // Expr +
                    var cont = 0;
                    var posibleError = false;
                    while (pos < orden.Count && !posibleError)
                    {
                        contExpr = 0;
                        if (ntExpr())
                        {
                            cont++;
                        }
                        else
                        {
                            if (contExpr > 0)
                            {
                                return false;
                            }
                            else
                            {
                                posibleError = true;
                            }
                        }
                    }
                    if (cont != 0)
                    {
                        actual = orden[pos];
                        if (actual.name == ",")
                        {
                            pos++;
                            actual = orden[pos];
                            if (actual.name == ")")
                            {
                                pos++;
                                if (actual.name == ";")
                                {
                                    pos++;
                                    return true;
                                }
                            }

                        }
                    }
                }
            }
            return false;
        }
        static bool ntExpr()
        {
            if (ntA())
            {
                if (ntDExpr())
                {
                    return true;
                }
            }
            return false;
        }
        static bool ntDExpr()
        {
            var actual = orden[pos];
            if (actual.name == ".")
            {
                pos++;
                actual = orden[pos];
                if (actual.tipo == 6)
                {
                    pos++;
                    if (ntX())
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            else if (actual.name == "[")
            {
                pos++;
                if (ntExpr())
                {
                    if (actual.name == "]")
                    {
                        pos++;
                        if (ntZ())
                        {
                            return true;
                        }
                    }
                    return false;
                }
                return false;
            }
            return true;
        }
        static bool ntX()
        {
            var actual = orden[pos];
            if (actual.name == "=")
            {
                pos++;
                if (ntA())
                {
                    if (ntDExpr())
                    {
                        return true;
                    }
                }
            }
            else if (ntDExpr())
            {
                return true;
            }
            return false;
        }
        static bool ntZ()
        {
            var actual = orden[pos];
            if (actual.name == "=")
            {
                pos++;
                if (ntA())
                {
                    if (ntDExpr())
                    {
                        return true;
                    }
                }
            }
            else if (actual.name == "[")
            {
                pos++;
                if (ntExpr())
                {
                    actual = orden[pos];
                    if (actual.name == "]")
                    {
                        pos++;
                        if (ntDExpr())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        static bool ntA()
        {
            if (ntB())
            {
                if (ntDA())
                {
                    return true;
                }
            }
            return false;
        }
        static bool ntDA()
        {
            var actual = orden[pos];
            if (actual.name == "||")
            {
                pos++;
                if (ntB())
                {
                    if (ntDA())
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return true;
        }
        static bool ntB()
        {
            if (ntC())
            {
                if (ntDB())
                {
                    return true;
                }
            }
            return false;
        }
        static bool ntDB()
        {
            var actual = orden[pos];
            if (actual.name == "&&")
            {
                pos++;
                if (ntC())
                {
                    if (ntDB())
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return true;
        }
        static bool ntC()
        {
            if (ntD())
            {
                if (ntDC())
                {
                    return true;
                }
            }
            return false;
        }
        static bool ntDC()
        {
            var actual = orden[pos];
            if (actual.name == "==" || actual.name == "!=")
            {
                pos++;
                if (ntD())
                {
                    if (ntDC())
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return true;
        }
        static bool ntD()
        {
            if (ntE())
            {
                if (ntDD())
                {
                    return true;
                }
            }
            return false;
        }
        static bool ntDD()
        {
            var actual = orden[pos];
            if (actual.name == "<" || actual.name == ">")
            {
                pos++;
                if (ntDDD())
                {
                    return true;
                }
                return false;
            }
            return true;
        }
        static bool ntDDD()
        {
            var actual = orden[pos];
            if (actual.name == "=")
            {
                pos++;
                if (ntE())
                {
                    if (ntDD())
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (ntE())
                {
                    if (ntDD())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        static bool ntE()
        {
            if (ntF())
            {
                if (ntDE())
                {
                    return true;
                }
            }
            return false;
        }
        static bool ntDE()
        {
            var actual = orden[pos];
            if (actual.name == "+" || actual.name == "-")
            {
                pos++;
                if (ntF())
                {
                    if (ntDE())
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return true;
        }
        static bool ntF()
        {
            if (ntG())
            {
                if (ntDF())
                {
                    return true;
                }
            }
            return false;
        }
        static bool ntDF()
        {
            var actual = orden[pos];
            if (actual.name == "*" || actual.name == "/" || actual.name == "%")
            {
                pos++;
                if (ntG())
                {
                    if (ntDF())
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return true;
        }
        static bool ntG()
        {
            var actual = orden[pos];
            if (actual.name == "-")
            {
                contExpr++;
                pos++;
                if (ntH())
                {
                    return true;
                }
            }
            else if (actual.name == "!")
            {
                contExpr++;
                pos++;
                if (ntH())
                {
                    return true;
                }
            }
            else if (ntH())
            {
                return true;
            }
            return false;
        }
        static bool ntH()
        {
            var actual = orden[pos];
            if (actual.name == "New")
            {
                contExpr++;
                pos++;
                actual = orden[pos];
                if (actual.name == "(")
                {
                    pos++;
                    actual = orden[pos];
                    if (actual.tipo == 6)
                    {
                        pos++;
                        actual = orden[pos];
                        if (actual.name == ")")
                        {
                            pos++;
                            return true;
                        }
                    }
                }
            }
            else if (ntConstant())
            {
                return true;
            }
            else if (actual.name == "this")
            {
                contExpr++;
                pos++;
                return true;
            }
            else if (actual.tipo == 6)
            {
                contExpr++;
                pos++;
                if (ntDH())
                {
                    return true;
                }
            }
            return false;
        }
        static bool ntDH()
        {
            var actual = orden[pos];
            if (actual.name == "=")
            {
                pos++;
                if (ntExpr())
                {
                    return true;
                }
                return false;
            }
            return true;
        }
        static bool ntConstant()
        {
            var actual = orden[pos];
            if (actual.tipo == 2 || actual.tipo == 3 || actual.tipo == 4 || actual.tipo == 7 || actual.name == "null")
            {
                contExpr++;
                pos++;
                return true;
            }
            return false;
        }
    }
}
