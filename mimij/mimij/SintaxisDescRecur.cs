using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace mimij
{
    class SintaxisDescRecur
    {
        static int pos;
        static string msg = string.Empty;
        static int cProgram = 0;
        static int cFunction = 0;
        static int cFormals = 0;
        static int cDReturn = 0;
        static int cDPrint = 0;
        // Agregar if de que pos no sea mayor a cantidad de lista Tokens
        public static List<Token> orden = new List<Token>();
        public static bool ntProgram()
        {
            return ntDecl() && ntDProgram();
        }
        public static bool ntDProgram()
        {
            //Agregar validacion
            cProgram = 0;
            var bandera = ntProgram();
            if(!bandera &&cProgram >0)
            {
                return false;
            }
            return true;
            //return ntProgram();
        }
        public static bool ntDecl()
        {
            cProgram++;
            return ntVariableDecl() || ntFunctionDecl();
        }
        public static bool ntVariableDecl()
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
                    msg = "ERROR: SE ESPERABA UN ';'";
                }
            }
            return false;
        }
        public static bool ntVariable()
        {
            cFormals++;
            if (ntType())
            {
                var actual = orden[pos];
                if (actual.tipo == 6)
                {
                    pos++;
                    return true;
                }
                else { msg = "ERROR: SE ESPERABA UN IDENTIFICADOR"; }
            }
            return false;
        }
        public static bool ntType()
        {
            var actual = orden[pos];
            if (actual.name == "int" || actual.name == "double" || actual.name == "boolean" || actual.name == "string" || actual.tipo == 6)
            {
                //contProgram++;
                //Aqui va ese if al momento de sumar cada pos++
                pos++;
                return ntDType();
            }
            msg = "ERROR: FALTAN TIPO DE VARIABLE";
            return false;
        }
        public static bool ntDType()
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
                else { msg = "ERROR: FALTAN ']'"; return false; }
            }
            return true;
        }
        public static bool ntFunctionDecl()
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
                                return ntDFunctionDecl();
                            }
                            else { msg = "ERROR: FALTA ')'"; }
                        }
                    }
                    else { msg = "ERROR: '('"; }
                }
                else { msg = "ERROR: FALTA IDENTIFICADOR"; }
            }
            else if (actual.name == "void")
            {
                //contProgram++;
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
                                return ntDFunctionDecl();
                            }
                            else { msg = "ERROR: FALTA ')'"; }
                        }
                    }
                    else { msg = "ERROR: '('"; }
                }
                else { msg = "ERROR: FALTA IDENTIFICADOR"; }
            }
            return false;
        }
        public static bool ntDFunctionDecl()
        {
            cFunction = 0;
            var bandera = ntStmt();
            if(!bandera &&cFunction>0)
            { 
                return false;
            }
            return true;
        }
        public static bool ntFormals()
        {
            if (ntDFormals())
            {
                var actual = orden[pos];
                if (actual.name == ",") { pos++; }
                else
                {
                    msg = "ERROR FALTA ','";
                    return false;
                }
            }
            return true;
        }
        public static bool ntDFormals()
        {
            //Validacion
            cFormals = 0;
            var bandera = ntVariable();
            if (!bandera && cFormals != 0)
            {
                return false;
            }
            return true;
            //return ntVariable() && ntDVariable();
        }
        public static bool ntStmt()
        {
            cFunction++;
            if (ntReturnStmt()||ntPrintStmt())
            {
                pos++;
                return true;
            }
            else if(ntExpr())
            {
                pos++;
                var actual = orden[pos];
                if (actual.name ==";")
                {
                    return true;
                }
            }
            return false;
        }
        public static bool ntReturnStmt()
        {
            var actual = orden[pos];
            if (actual.name == "return")
            {
                pos++;
                if(ntDReturnStmt())
                {
                    return true;
                }
                return false;
            }
            return true;
        }
        public static bool ntDReturnStmt()
        {
            cDReturn = 0;
            var bandera = ntExpr();
            if(!bandera && cDReturn>0)
            {
                return false;
            }
            return true;
        }
        public static bool ntPrintStmt()
        {
            var actual = orden[pos];
            if(actual.name == "Print")
            {
                pos++;
                if(actual.name == "(")
                {
                    pos++;
                    if(ntDPrintStmt())
                    {
                        pos++;
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
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        public static bool ntDPrintStmt()
        {
            cDPrint = 0;
            var bandera = ntExpr();
            if(!bandera && cDPrint != 0)
            {
                return false;
            }
            return true;
        }
        public static bool ntExpr()
        {
            cDReturn++;
            cDPrint++;
            if (ntLValue())
            {
                pos++;
                var actual = orden[pos];
                if(actual.name == "=")
                {
                    pos++;
                    if(ntA())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool ntA()
        {
            if(ntB())
            {
                pos++;
                if(ntA())
                {
                    return true;
                }
            }
            return false;
        }
        public static bool ntDA()
        {
            var actual = orden[pos];
            if(actual.name == "||")
            {
                pos++;
                if(ntB())
                {
                    pos++;
                    if(ntDA())
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
        public static bool ntB()
        {
            if(ntC())
            {
                pos++;
                if(ntDB())
                {
                    return true;
                }
            }
            return false;
        }
        public static bool ntDB()
        {
            var actual = orden[pos];
            if(actual.name== "&&")
            {
                pos++;
                if(ntC())
                {
                    pos++;
                    if(ntDB())
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
        public static bool ntC()
        {
            if(ntD())
            {
                pos++;
                if(ntDC())
                {
                    return true;
                }
            }
            return false;
        }
        public static bool ntDC()
        {
            var actual = orden[pos];
            if(actual.name == "=="||actual.name=="!=")
            {
                pos++;
                if(ntD())
                {
                    pos++;
                    if(ntDC())
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
        public static bool ntD()
        {
            if(ntE())
            {
                pos++;
                if(ntDD())
                {
                    return true;
                }
            }
            return false;
        }
        public static bool ntDD()
        {
            var actual = orden[pos];
            if (actual.name == "<"|| actual.name == ">")
            {
                pos++;
                if(ntD())
                {
                    pos++;
                    if(ntDF())
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
        public static bool ntDDF()
        {
            var actual = orden[pos];
            if (ntE())
            {
                pos++;
                if(ntDD())
                {
                    return true;
                }
            }
            else if(actual.name == "=")
            {
                pos++;
                if(ntE())
                {
                    return true;
                }
            }
            return false;
        }
        public static bool ntE()
        {
            if(ntF())
            {
                pos++;
                if(ntDE())
                {
                    return true;
                }
            }
            return false;
        }
        public static bool ntDE()
        {
            var actual = orden[pos];
            if (actual.name == "+" || actual.name == "-")
            {
                pos++;
                if (ntF())
                {
                    pos++;
                    if (ntDE())
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
        public static bool ntF()
        {
            if(ntG())
            {
                pos++;
                if(ntDF())
                {
                    return true;
                }
            }
            return false;
        }
        public static bool ntDF()
        {
            var actual = orden[pos];
            if(actual.name == "*"|| actual.name == "/"|| actual.name == "%")
            {
                pos++;
                if(ntG())
                {
                    pos++;
                    if(ntDF())
                    {
                        pos++;
                        return true;
                    }
                }
                return false;
            }            
            return true;
        }
        public static bool ntG()
        {
            var actual = orden[pos];
            if (actual.name == "!")
            {
                pos++;
                if(ntH())
                {
                    pos++;
                    return true;
                }
            }
            else if (actual.name == "-")
            {
                pos++;
                if(ntH())
                {
                    pos++;
                    return true;
                }
            }
            else if(ntH())
            {
                pos++;
                return true;
            }
            return false;
        }
        public static bool ntH()
        {
            var actual = orden[pos];
            if(actual.name == "[")
            {
                pos++;
                if(ntExpr())
                {
                    pos++;
                    actual = orden[pos];
                    if (actual.name == "]")
                    {
                        pos++;
                        return true;
                    }
                }
            }
            else if (ntConstant() || ntDLValue())
            {
                return true;
            }
            else if(actual.name == "this")
            {
                return true;
            }
            //duda en esta si va junto o separado
            else if(actual.name == "New")
            {
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
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static bool ntLValue()
        {
            var actual = orden[pos];
            if(actual.name == "ident")
            {
                pos++;
                return true;
            }
            else if(ntExpr())
            {
                pos++;
                if(ntDLValue())
                {
                    pos++;
                    return true;
                }
            }
            return false;
        }
        public static bool ntDLValue()
        {
            var actual = orden[pos];
            if (actual.name == ".")
            {
                pos++;
                if (orden[pos].tipo == 6)
                {
                    pos++;
                    return true;
                }
            }
            else if (actual.name=="[")
            {
                pos++;
                if(ntExpr())
                {
                    pos++;
                    actual = orden[pos];
                    if (actual.name == "]")
                    {
                        pos++;
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool ntConstant()
        {
            var actual = orden[pos];
            if (actual.name == "int" || actual.name == "double" || actual.name == "boolean" || actual.name == "string")
            {
                pos++;
                return ntConstant();
            }
            else if(actual.name == "null")
            {
                pos++;
                return true;
            }
            return true;
        }
    }
}
