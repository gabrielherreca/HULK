namespace Hulk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

public class Consola
{
    
    public static Entorno entorno = new Entorno();

    
        
    

    public  void Ejecutar()
    {
        while (true)
        {
            Console.Write("> ");
            string linea = Console.ReadLine();
            if (string.IsNullOrEmpty(linea))
                continue;
            if (linea.ToLower() == "salir")
                break;

            try
            {   AnalizadorLéxico lex = new AnalizadorLéxico(linea);
                List<Token> tokens = lex.ObtenerTokens();
                AnalizadorSintáctico sintaxis = new AnalizadorSintáctico(tokens);
                AST ast = sintaxis.Analizar();
                object resultado = ast.Evaluar(entorno); 
                Console.WriteLine(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    
}

class Program
{
    static void Main(string[] args)
    {
        Consola consola = new Consola();
        consola.Ejecutar();
    }
}
