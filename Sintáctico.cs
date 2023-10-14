namespace Hulk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Xml;

public class AnalizadorSintáctico
{
    private List<Token> tokens;
    private int indice;
    private Token tokenActual;

    public AnalizadorSintáctico(List<Token> tokens)
    {
        this.tokens = tokens;
        indice = 0;
        if (tokens.Count != 0) {
            tokenActual = tokens[0];
        }
    }

    public void siguienteToken() {
        if (indice < tokens.Count - 1) {
            indice++;
            tokenActual = tokens[indice];
        }
    }

    public static Entorno entorno = new Entorno();
    public AST Analizar()
    
    
{    if (tokenActual.Tipo == TipoToken.PalabraReservada && tokenActual.Valor == "if")
    {  
        return AnalizarIfElse();
    }    

     if (tokenActual.Tipo == TipoToken.PalabraReservada && tokenActual.Valor == "let")
       {
        return AnalizarLetIn();
       }
      if (tokenActual.Tipo == TipoToken.PalabraReservada && tokenActual.Valor == "print")
    {
             return AnalizarPrint();
    }           

     if (tokenActual.Tipo == TipoToken.PalabraReservada && tokenActual.Valor == "function")
    {  
        return AnalizarFuncionInline();
    }
   

    if (tokenActual.Tipo == TipoToken.Cadena)
    {
    AST nodo = new Cadena(tokenActual.Valor);
     siguienteToken();
            return nodo;
    
    }
    if(tokenActual.Tipo == TipoToken.Identificador)
    {   
        if ((FuncionDefinida(tokenActual.Valor) == true))
        {   Console.WriteLine("hola");
            return AnalizarLlamadaFuncion();
        }

        return AnalizarIdentificador();
    }
    if(tokenActual.Tipo == TipoToken.DelimitadorAbierto)
    {   siguienteToken();
        return Analizar();
    }
    
    

    AST result = AnalizarExpresion();

    while (tokenActual != null && tokenActual.Tipo == TipoToken.DelimitadorAbierto)
    {
        siguienteToken();
        AST derecho = Analizar();
     

        if (tokenActual.Tipo != TipoToken.DelimitadorCerrado)
            throw new Exception("Error: Se esperaba un paréntesis cerrado.");

        siguienteToken();

        if (tokenActual != null && esOperadorAritmetico(tokenActual.Valor))
        {
            Token operador = tokenActual;
            siguienteToken();
            result = new OperacionAritmetica(result, derecho, operador.Valor);
        }
    }

    return result;
}

public AST AnalizarIfElse() {
   
   siguienteToken();

    
    siguienteToken(); 
    AST condicion = Analizar();
    siguienteToken(); 

    
    AST expresionIf = Analizar();

   
    AST expresionElse = null;
    if (tokenActual.Valor == "else") 
    {
        siguienteToken();
        expresionElse = Analizar();
    }
    else
    {
        throw new Exception("Error: Se esperaba la condicion Else.");
    }

  
    return new IfElseExpression(condicion, expresionIf, expresionElse);
}


public bool FuncionDefinida(string nombre)
{   var funcion = entorno.BuscarFuncion(nombre);
    if (funcion == null)
    {
        return false;
    }
    else
    {
       return true;
    }
}

private AST AnalizarLlamadaFuncion()
{
    string nombreFuncion = tokenActual.Valor;
    siguienteToken();

    if (tokenActual.Tipo != TipoToken.DelimitadorAbierto)
        throw new Exception("Error: Se esperaba un paréntesis abierto.");

    siguienteToken();

    List<AST> argumentos = new List<AST>();
    while (tokenActual.Tipo != TipoToken.DelimitadorCerrado)
    {
        AST argumento = AnalizarExpresion();
        argumentos.Add(argumento);

        if (tokenActual.Tipo == TipoToken.Coma)
            siguienteToken();
        else if (tokenActual.Tipo != TipoToken.DelimitadorCerrado)
            throw new Exception("Error: Se esperaba una coma o un paréntesis cerrado.");
    }

    siguienteToken();

    return new LlamadaFuncion(nombreFuncion, argumentos);
}

public AST AnalizarIdentificador()
{
   
    AST nodo = new Identificador(tokenActual.Valor);

    
    siguienteToken();

    
    if (tokenActual != null && tokenActual.Tipo == TipoToken.Concatenador)
    {
       
        siguienteToken();

        
        AST derecho = Analizar();

       
        nodo = new Concatenacion(nodo, derecho);
       

    }
    
    

    return nodo;
}

public AST AnalizarLetIn()
{
    

    siguienteToken();

    
    List<Variable> variables = new List<Variable>();
    do
    {
        
        if (tokenActual.Tipo != TipoToken.Identificador)
            throw new Exception("Error: Se esperaba un identificador.");

        string nombre = tokenActual.Valor;

        siguienteToken();

        
        if (tokenActual.Tipo != TipoToken.OperadorAsignación)
            throw new Exception("Error: Se esperaba '='.");

        siguienteToken();

       
        AST valor = Analizar();

      
        Entorno ent = new Entorno();
        variables.Add(new Variable(nombre, valor.Evaluar(ent)));

      
        if (tokenActual.Tipo == TipoToken.Coma)
            siguienteToken();
    }
    while ( tokenActual.Valor != "in");

    siguienteToken();

    
    AST cuerpo = Analizar();

   
    Entorno entornoLetIn = new Entorno();
    foreach (Variable variable in variables)
        entornoLetIn.DefinirVariable(variable);

    
    return new LetInExpression(entornoLetIn, cuerpo);
}


private AST AnalizarPrint()
{
     siguienteToken();  

   
    if (tokenActual.Tipo != TipoToken.DelimitadorAbierto)
        throw new Exception("Error: Se esperaba un paréntesis abierto.");

    siguienteToken();  

   
    AST expresion = Analizar();
    


    
    if (tokenActual.Tipo != TipoToken.DelimitadorCerrado)
       
          throw new Exception($"Error: Token inesperado '{tokenActual.Valor}'.");
    siguienteToken();  

   
    AST nodo = new PrintExpression(expresion);
    return nodo;
   
    }


private AST AnalizarFuncionInline()
{
    siguienteToken(); 

    if (tokenActual.Tipo != TipoToken.Identificador)
        throw new Exception("Error: Se esperaba un identificador.");

    string nombreFuncion = tokenActual.Valor;
    siguienteToken();

    if (tokenActual.Tipo != TipoToken.DelimitadorAbierto)
        throw new Exception("Error: Se esperaba un paréntesis abierto.");

    siguienteToken();

    List<string> parametros = new List<string>();
    while (tokenActual.Tipo != TipoToken.DelimitadorCerrado)
    {
        if (tokenActual.Tipo != TipoToken.Identificador)
            throw new Exception("Error: Se esperaba un identificador.");

        parametros.Add(tokenActual.Valor);
        siguienteToken();

        if (tokenActual.Tipo == TipoToken.Coma)
            siguienteToken();
        else if (tokenActual.Tipo != TipoToken.DelimitadorCerrado)
            throw new Exception("Error: Se esperaba una coma o un paréntesis cerrado.");
    }

    siguienteToken();

    if (tokenActual.Tipo != TipoToken.Flecha)
        throw new Exception("Error: Se esperaba una flecha.");

    siguienteToken();

    AST cuerpo = AnalizarExpresion();

    return new FuncionInline(nombreFuncion, parametros, cuerpo);
}


private AST AnalizarExpresion()
{
    AST result = AnalizarOperacionAritmetica();
    while (tokenActual != null && esOperadorLogico(tokenActual.Valor))
    {
        Token operador = tokenActual;
        siguienteToken();
        AST derecho = AnalizarOperacionAritmetica();
        result = new OperadorLogico(result, derecho, operador.Valor);
    }
    return result;
}

private AST AnalizarOperacionAritmetica()
{
    AST result = AnalizarTermino();
    while (tokenActual != null && (tokenActual.Valor == "+" || tokenActual.Valor == "-"))
    {
        Token operador = tokenActual;
        siguienteToken();
        AST derecho = AnalizarTermino();
        result = new OperacionAritmetica(result, derecho, operador.Valor);
    }
    return result;
}

private AST AnalizarTermino()
{
    AST result = AnalizarFactor();
    while (tokenActual != null && (tokenActual.Valor == "*" || tokenActual.Valor == "/" || tokenActual.Valor == "%"))
    {
        Token operador = tokenActual;
        siguienteToken();
        AST derecho = AnalizarFactor();
        result = new OperacionAritmetica(result, derecho, operador.Valor);
    }
    return result;
}

private AST AnalizarFactor()
{   
    AST result = AnalizarExponente();
    while (tokenActual != null && tokenActual.Valor == "^")
    {
        Token operador = tokenActual;
        siguienteToken();
        AST derecho = AnalizarExponente();
        result = new OperacionAritmetica(result, derecho, operador.Valor);
    }
    return result;
}
private AST AnalizarExponente()
{
    
    if (tokenActual.Tipo == TipoToken.Numero)
    {
        int valor = int.Parse(tokenActual.Valor);
        AST nodo = new ValorNumerico(valor);
        siguienteToken();
        return nodo;
    }

    
    
    else if (tokenActual.Tipo == TipoToken.DelimitadorAbierto)
    {
        siguienteToken();
        AST nodo = AnalizarExpresion();
        
        if (tokenActual.Tipo != TipoToken.DelimitadorCerrado)
            throw new Exception("Error: Se esperaba un paréntesis cerrado.");
            
        siguienteToken();
        return nodo;
    }
    
    else if (tokenActual.Valor == "-")
    {
        siguienteToken();
        
        if (tokenActual.Tipo == TipoToken.Numero)
        {
            int valor = -int.Parse(tokenActual.Valor);
            AST nodo = new ValorNumerico(valor);
            siguienteToken();
            return nodo;
        }
        
        else if (tokenActual.Tipo == TipoToken.DelimitadorAbierto)
        {
            siguienteToken();
            AST nodo = AnalizarExpresion();
            
            if (tokenActual.Tipo != TipoToken.DelimitadorCerrado)
                throw new Exception("Error: Se esperaba un paréntesis cerrado.");
                
            siguienteToken();
            
           
            return new Negacion(nodo);
        }
        
        else
        {
            throw new Exception("Error: Se esperaba un número o una expresión entre paréntesis después del signo '-'.");
        }
    }
    
    else
    {
        throw new Exception($"Error: Token inesperado '{tokenActual.Valor}'.");
    }
}

private bool esOperadorAritmetico(string valor)
{
    return valor == "+" || valor == "-" || valor == "*" || valor == "/" || valor == "%";
}



    private AST AnalizarOperacionLogica()
{
    AST result = AnalizarExpresion();
    while (tokenActual != null && esOperadorLogico(tokenActual.Valor))
    {
        Token operador = tokenActual;
        siguienteToken();
        AST derecho = AnalizarExpresion();
        result = new OperadorLogico(result, derecho, operador.Valor);
    }
    return result;
}

private bool esOperadorLogico(string valor)
{   
    return valor == "==" || valor == "!=" || valor == ">" || valor == "<" || valor == "&&" || valor == "||";
}


}
