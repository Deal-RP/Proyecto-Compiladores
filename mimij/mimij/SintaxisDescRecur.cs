﻿using System;
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
        static int pos, contReturn, contPrint, contExpr, contValue;
        static string msg = string.Empty;
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
                    //Logica saltarnos a la siguiente linea
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
                    msg = "ERROR: SE ESPERABA UN ';'";
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
                else { msg = "ERROR: SE ESPERABA UN IDENTIFICADOR"; }
            }
            return false;
        }
        static bool ntType()
        {
            var actual = orden[pos];
            if (actual.name == "int" || actual.name == "double" || actual.name == "boolean" || actual.name == "string" || actual.tipo == 6)
            {
                pos++;
                return ntDType();
            }
            msg = "ERROR: FALTAN TIPO DE VARIABLE";
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
                else { msg = "ERROR: FALTAN ']'"; return false; }
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
                                var bandera = ntDFunction();
                                while (bandera)
                                {
                                    bandera = ntDFunction();
                                }
                                return bandera;
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
                                var bandera = ntDFunction();
                                while (bandera)
                                {
                                    bandera = ntDFunction();
                                }
                                return bandera;
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
        static bool ntDFunction()
        {
            contExpr = 0;
            contReturn = 0;
            if (!ntReturnStmt())
            {
                if (contReturn > 0) { return false; }
                contExpr = 0;
                contPrint = 0;
                if (!ntPrintStmt())
                {
                    if (contPrint > 0) { return false; }
                    contExpr = 0;
                    if (!ntExpr()) { return contExpr > 0 ? false : true; }
                }
            }
            return true;
        }
        static bool ntFormals()
        {
            //Variable+
            var actual = orden[pos];
            if (actual.name == ",") { pos++; }
            else
            {
                msg = "ERROR FALTA ','";
                return false;
            }
            return true;
        }
        static bool ntStmt()
        {
            if (ntReturnStmt()||ntPrintStmt())
            {
                return true;
            }
            else if(ntExpr())
            {
                var actual = orden[pos];
                if (actual.name ==";")
                {
                    pos++;
                    return true;
                }
            }
            return false;
        }
        static bool ntReturnStmt()
        {
            var actual = orden[pos];
            if (actual.name == "return")
            {
                contReturn++;
                pos++;
                if (pos == orden.Count)
                {
                    return true;
                }
                if (!ntExpr() && contExpr > 0) { return false; }
                return true;
            }
            return false;
        }
        static bool ntPrintStmt()
        {
            var actual = orden[pos];
            if(actual.name == "Print")
            {
                contPrint++;
                pos++;
                if(actual.name == "(")
                {
                    pos++;
                    //Agregar Expr+
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
        static bool ntExpr()
        {
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
        static bool ntA()
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
        static bool ntDA()
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
        static bool ntB()
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
        static bool ntDB()
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
        static bool ntC()
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
        static bool ntDC()
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
        static bool ntD()
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
        static bool ntDD()
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
        static bool ntDDF()
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
        static bool ntE()
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
        static bool ntDE()
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
        static bool ntF()
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
        static bool ntDF()
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
        static bool ntG()
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
        static bool ntH()
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
        static bool ntLValue()
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
        static bool ntDLValue()
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
        static bool ntConstant()
        {
            var actual = orden[pos];
            if (actual.tipo == 2 || actual.tipo == 3 || actual.tipo == 4 || actual.tipo == 7 || actual.name == "null")
            {
                pos++;
                return true;
            }
            return false;
        }
    }
}
