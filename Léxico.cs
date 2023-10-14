

namespace Hulk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

public class AnalizadorLéxico
{
        string codigoFuente;
    int indice;
    private List<string> diagnostics = new List<string>();
    
     Dictionary <string, TipoToken> palabrasReservadas = new Dictionary<string, TipoToken>
        {
            { "print", TipoToken.PalabraReservada },
            { "log", TipoToken.PalabraReservada },
            { "function", TipoToken.PalabraReservada },
            { "let", TipoToken.PalabraReservada },
            { "in", TipoToken.PalabraReservada },
            { "if", TipoToken.PalabraReservada },
            { "else", TipoToken.PalabraReservada }
            // Agrega las demás palabras reservadas del lenguaje HULK
        };
   
    /* Dictionary<string, TipoToken> Math;*/
     Dictionary<string, TipoToken> palarasReservadas;

    public AnalizadorLéxico(string codigoFuente)
    {
        this.codigoFuente = codigoFuente;
        indice = 0;
        

      
       
      /*  Math = new Dictionary<string, TipoToken>
        {
            { "sin", TipoToken.Math},
            { "cos", TipoToken.Math},
            { "tan", TipoToken.Math},
        };*/
    }

    

    public List<Token> ObtenerTokens()
    {
        List<Token> tokens = new List<Token>();

        while (indice < codigoFuente.Length)
        {
            char caracterActual = codigoFuente[indice];
            string _caracterActual = codigoFuente[indice].ToString();

            // Omitir espacios en blanco y caracteres de nueva línea
            if (caracterActual == ' ' || caracterActual == '\n' || caracterActual == '\r')
            {
                indice++;
                continue;
            }
            
            // Detectar delimitadores
            if (caracterActual == '('  )
            {
                tokens.Add(new Token(TipoToken.DelimitadorAbierto, caracterActual.ToString()));
                indice++;
                continue;
            }
            if ( caracterActual == ')' )
            {
                tokens.Add(new Token(TipoToken.DelimitadorCerrado, caracterActual.ToString()));
                indice++;
                continue;
            }

             if ( caracterActual == '"')
            {
                tokens.Add(new Token(TipoToken.Comillas, caracterActual.ToString()));
                indice++;
                continue;
            }

             if ( caracterActual == ';' )
            {
                tokens.Add(new Token(TipoToken.PuntoComa, caracterActual.ToString()));
                indice++;
                continue;
            }
             if ( caracterActual == ',' )
            {
                tokens.Add(new Token(TipoToken.Coma, caracterActual.ToString()));
                indice++;
                continue;
            }

            // Detectar números
            if (char.IsDigit(caracterActual))
            {
                string numero = "";
                while (indice < codigoFuente.Length && char.IsDigit(codigoFuente[indice]))
                {
                    numero += codigoFuente[indice];
                    indice++;
                }
                tokens.Add(new Token(TipoToken.Numero, numero));
                continue;
            }

            // Detectar palabras reservadas o identificadores
            if (char.IsLetter(caracterActual) || caracterActual == '_')
            {
                string palabra = "";
                while (indice < codigoFuente.Length && (char.IsLetterOrDigit(codigoFuente[indice]) || codigoFuente[indice] == '_'))
                {
                    palabra += codigoFuente[indice];
                    indice++;
                }

                if (palabrasReservadas.ContainsKey(palabra))
                {
                    tokens.Add(new Token(palabrasReservadas[palabra], palabra));
                }
               /* else if (Math.ContainsKey(palabra))
                {
                    tokens.Add(new Token(Math[palabra], palabra));
                }/*/
                else
                {
                    tokens.Add(new Token(TipoToken.Identificador, palabra));
                }
                continue;
            }

            // Detectar operadores aritméticos
            if (caracterActual == '+' || caracterActual == '-' || caracterActual == '*' 
                ||caracterActual == '^' || caracterActual == '%'
                || caracterActual == '/')
            {
                tokens.Add(new Token(TipoToken.OperadorAritmético, caracterActual.ToString()));
                indice++;
                continue; 
            }
            // Detectar operadores lógicos
             if (
                     caracterActual == '>' || caracterActual == '<' )
            {
                tokens.Add(new Token(TipoToken.OperadorLógico, caracterActual.ToString()));
                indice++;
                continue; 
            }


           

            
            if (caracterActual == '=')
            {
                if (indice + 1 < codigoFuente.Length && codigoFuente[indice + 1] == '=')
                    {  
                        tokens.Add(new Token(TipoToken.OperadorLógico, "=="));
                        indice += 2;
                        continue;
                    }

                else if (indice + 1 < codigoFuente.Length && codigoFuente[indice + 1] == '>')
                    {  
                        tokens.Add(new Token(TipoToken.Flecha, "=>"));
                        indice += 2;
                        continue;
                    }    
                    else
                 {
                     tokens.Add(new Token(TipoToken.OperadorAsignación, "="));
                    indice++;
                     continue;
                }
            }
             if (caracterActual == '!')
            {
                if (indice + 1 < codigoFuente.Length && codigoFuente[indice + 1] == '=')
                    {  
                        tokens.Add(new Token(TipoToken.OperadorLógico, "!="));
                        indice += 2;
                        continue;
                    }
                    else
                 {
                     tokens.Add(new Token(TipoToken.Unknown, caracterActual.ToString()));        
                     // Si no se reconoce el carácter, generar error léxico 
                    diagnostics.Add("Non nkown char at "+ indice);
                    indice++;
                    continue;
                }
            }
             tokens.Add(new Token(TipoToken.Unknown, caracterActual.ToString()));        
            // Si no se reconoce el carácter, generar error léxico 
            diagnostics.Add("Caracter desconocido en : "+ indice);
            indice++;

            
        

        }      
       return tokens;     
             
    }
       
    
}