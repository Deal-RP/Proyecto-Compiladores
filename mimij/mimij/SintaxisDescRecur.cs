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
        //Agregar contadores program ....
        // Agregar if de que pos no sea mayor a cantidad de lista Tokens
        public static List<Token> orden = new List<Token>();
        public static bool ntProgram()
        {
            return ntDecl() && ntDProgram();
        }
        public static bool ntDProgram()
        {
            //Agregar validacion
            return ntProgram();
        }
        public static bool ntDecl()
        {
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
            //Validacion
            ntStmt();
            ntDFunctionDecl();
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
            //contFormals = 0;
            //var bandera = ntVariable();
            //if (!bandera && contFormals != 0)
            //{
            //    return true;
            //}
            //return false;
            return ntVariable() && ntDVariable();
        }
        public static bool ntStmt()
        {
            var actual = orden[pos];
            return true;
        }
        public static bool ntDVariable()
        {
            var actual = orden[pos];
            return true;
        }
        //returnStmt' //Validacion
        // printStmt' //Validacion
        //Sumas de contadores ver imagen que te envio por WhatSapp
    }
}
