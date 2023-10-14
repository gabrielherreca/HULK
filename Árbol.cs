namespace Hulk;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

public enum TipoToken
{
    PalabraReservada,

    Funcion,
    Identificador,
    Numero,
    OperadorAritmético,
    OperadorLógico,
    OperadorAsignación,
    DelimitadorAbierto,
    DelimitadorCerrado,
    Math,
    Comillas,
    PuntoComa,
    Coma,

    Unknown,

    Flecha,

   
}






 public class Token 
{    
      
    



    public Token(TipoToken tipo, string valor)
    {
        Tipo = tipo;
        Valor = valor;
    }
    public  TipoToken Tipo { get; }
    public string Valor { get; }  

    
} 


 
  
public abstract class AST
{
    public abstract object Evaluar(Entorno entorno);
     
}

public class ValorNumerico : AST
{
    public int Valor { get; }

    public ValorNumerico(int value)
    {
        Valor = value;
    }

    public override object Evaluar(Entorno entorno)
    {
       
        return Valor;
    }

   
}

public class OperacionAritmetica : AST
{
    public AST OperandoIzquierdo { get; }
    public AST OperandoDerecho { get; }
    public string Operador { get; }

    public OperacionAritmetica(AST operandoIzquierdo, AST operandoDerecho, string operador)
    {
        OperandoIzquierdo = operandoIzquierdo;
        OperandoDerecho = operandoDerecho;
        Operador = operador;
    }

    public override object Evaluar(Entorno entorno)
{
    
    double valorIzquierdo = Convert.ToDouble(OperandoIzquierdo.Evaluar(entorno));
    double valorDerecho = Convert.ToDouble(OperandoDerecho.Evaluar(entorno));

    
    switch (Operador)
    {
        case "+":
            return valorIzquierdo + valorDerecho;
        case "-":
            return valorIzquierdo - valorDerecho;
        case "*":
            return valorIzquierdo * valorDerecho;
         case "%":
            return valorIzquierdo % valorDerecho;    
          case "^":
            return Math.Pow(valorIzquierdo,valorDerecho);   
        case "/":
            if (valorDerecho != 0)
            {
                return valorIzquierdo / valorDerecho;
            }
            else
            {
                throw new Exception("Error: División por cero.");
            }
        default:
            throw new Exception($"Error: Operador desconocido '{Operador}'.");
    }

    
}

  

}

public class Negacion : AST
{
    private AST subexpresion;

    public Negacion(AST subexpresion)
    {
        this.subexpresion = subexpresion;
    }

    public override object Evaluar(Entorno entorno)
{
    // Primero evaluamos la subexpresión.
    object valor = subexpresion.Evaluar(entorno);

    // Luego, comprobamos que el valor sea un número.
    if (valor is int)
    {
        // Si es un int, lo convertimos a double y luego lo negamos.
        return - Convert.ToDouble(valor);
    }
    else if (valor is double)
    {
        // Si ya es un double, simplemente lo negamos.
        return - (double)valor;
    }
    else
    {
        // Si no es un número, lanzamos una excepción.
        throw new Exception("Error: se intentó negar un valor no numérico.");
    }
}

}




public class FuncionLog : AST
{
    public AST Argumento { get; }

    public FuncionLog(AST argumento)
    {
        Argumento = argumento;
    }

    public override object Evaluar(Entorno entorno)
    {
        // Primero, evaluamos el argumento.
        double valorArgumento = Convert.ToDouble(Argumento.Evaluar(entorno));

        // Luego, calculamos y devolvemos el logaritmo del argumento.
        return Math.Log(valorArgumento);
    }
}





 

    public class OperadorLogico : AST
{   


    public AST OperandoIzquierdo { get; }
    public AST OperandoDerecho { get; }
    public string Operador { get; }

    public OperadorLogico(AST operandoIzquierdo, AST operandoDerecho, string operador)
    {
        OperandoIzquierdo = operandoIzquierdo;
        OperandoDerecho = operandoDerecho;
        Operador = operador;
    }

    public override object Evaluar(Entorno entorno)
{
    // Primero, evaluamos los operandos.
    var valorIzquierdo = OperandoIzquierdo.Evaluar(entorno);
    var valorDerecho = OperandoDerecho.Evaluar(entorno);

    // Luego, realizamos la operación lógica.
    switch (Operador)
    {
        case "==":
            return Convert.ToDouble(valorIzquierdo) == Convert.ToDouble(valorDerecho);
        case "!=":
            return Convert.ToDouble(valorIzquierdo) != Convert.ToDouble(valorDerecho);;
        case ">":
            return Convert.ToDouble(valorIzquierdo) > Convert.ToDouble(valorDerecho);
        case "<":
            return Convert.ToDouble(valorIzquierdo) < Convert.ToDouble(valorDerecho);
        case "&&":
            return Convert.ToBoolean(valorIzquierdo) && Convert.ToBoolean(valorDerecho);
        case "||":
            return Convert.ToBoolean(valorIzquierdo) || Convert.ToBoolean(valorDerecho);
        default:
            throw new Exception($"Error: Operador desconocido '{Operador}'.");
    }
}

}

public class Identificador : AST
{
    public string Nombre { get; }

    public Identificador(string nombre)
    {
        Nombre = nombre;
    }
    public override object Evaluar(Entorno entorno)
{
    // Buscamos el valor actual del identificador en el entorno.
    var variable = entorno.BuscarVariable(Nombre);
    if (variable != null)
    {
        // Si la variable existe, devolvemos su valor.
        return variable.Value;
    }
    else
    {
        // Si la variable no existe, lanzamos una excepción.
        throw new Exception($"Error: Variable no definida '{Nombre}'.");
    }
}

}

public class DeclaracionVariable : AST
{
    public string Nombre { get; }
    public AST ValorInicial { get; }

    public DeclaracionVariable(string nombre, AST valorInicial)
    {
        Nombre = nombre;
        ValorInicial = valorInicial;
    }

    public override object Evaluar(Entorno entorno)
{
    // Primero, evaluamos el valor inicial de la variable.
    var valor = ValorInicial.Evaluar(entorno);

    // Luego, definimos una nueva variable en el entorno con el nombre y el valor inicial.
    entorno.DefinirVariable(new Variable(Nombre, valor));

    // La declaración de una variable no tiene un valor por sí misma, por lo que devolvemos null.
    return null;
}

    
}

public class LetInExpression : AST
{
    public List<DeclaracionVariable> Declaraciones { get; }
    public AST Cuerpo { get; }

    public LetInExpression(List<DeclaracionVariable> declaraciones, AST cuerpo)
    {
        Declaraciones = declaraciones;
        Cuerpo = cuerpo;
    }

    public override object Evaluar(Entorno entorno)
{
    // Primero, definimos las nuevas variables en el entorno.
    foreach (var declaracion in Declaraciones)
    {
        declaracion.Evaluar(entorno);
    }

    // Luego, evaluamos y devolvemos el resultado del cuerpo.
    return Cuerpo.Evaluar(entorno);
}

    
}




public class IfElseExpression : AST
{
    public AST Condicion { get; }
    public AST ExpresionIf { get; }
    public AST ExpresionElse { get; }

    public IfElseExpression(AST condicion, AST expresionIf, AST expresionElse)
    {
        Condicion = condicion;
        ExpresionIf = expresionIf;
        ExpresionElse = expresionElse;
    }

    public override object Evaluar(Entorno entorno)
{
    // Primero, evaluamos la condición.
    bool condicion = Convert.ToBoolean(Condicion.Evaluar(entorno));

    // Luego, en función del valor de la condición, evaluamos y devolvemos el resultado de ExpresionIf o ExpresionElse.
    if (condicion)
    {
        return ExpresionIf.Evaluar(entorno);
    }
    else
    {
        return ExpresionElse.Evaluar(entorno);
    }
}

}

public class PrintExpression : AST
{
    public AST Expresion { get; }

    public PrintExpression(AST expresion)
    {
        Expresion = expresion;
    }

    public override object Evaluar(Entorno entorno)
{
    // Evaluamos la expresión y la convertimos en una cadena.
    var valor = Expresion.Evaluar(entorno);
    return valor.ToString();
}
}

public class FuncionInline : AST
{
    public string Nombre { get; }
    public List<string> Parametros { get; }
    public AST Cuerpo { get; }

    public FuncionInline(string nombre, List<string> parametros, AST cuerpo)
    {
        Nombre = nombre;
        Parametros = parametros;
        Cuerpo = cuerpo;
    }
    public override object Evaluar(Entorno entorno)
{
    // Definimos una nueva función en el entorno.
    entorno.DefinirFuncion(this);

    // La definición de una función no tiene un valor por sí misma, por lo que devolvemos null.
    return null;
}

}

public class LlamadaFuncion : AST
{
    public string Nombre { get; }
    public List<AST> Argumentos { get; }

    public LlamadaFuncion(string nombre, List<AST> argumentos)
    {
        Nombre = nombre;
        Argumentos = argumentos;
    }
    public override object Evaluar(Entorno entorno)
{
    // Buscamos la definición de la función en el entorno.
    var funcion = entorno.BuscarFuncion(Nombre);
    if (funcion == null)
    {
        throw new Exception($"Error: Función no definida '{Nombre}'.");
    }

    // Creamos un nuevo entorno para la llamada a la función.
    var entornoFuncion = new Entorno();

    // Evaluamos los argumentos y los definimos en el entorno de la función.
    for (int i = 0; i < Argumentos.Count; i++)
    {
        var valor = Argumentos[i].Evaluar(entorno);
        var variable = new Variable(funcion.Parametros[i], valor);
        entornoFuncion.DefinirVariable(variable);
    }

    // Evaluamos el cuerpo de la función en el entorno de la función y devolvemos el resultado.
    return funcion.Cuerpo.Evaluar(entornoFuncion);
} 

} 

public class Entorno
{
    private Dictionary<string, Variable> variables = new Dictionary<string, Variable>();

    public void DefinirVariable(Variable variable)
    {
        variables[variable.Name] = variable;
    }

    public Variable BuscarVariable(string nombre)
    {
        if (variables.TryGetValue(nombre, out var variable))
        {
            return variable;
        }
        else
        {
            return null;
        }
    }

    private Dictionary<string, FuncionInline> funciones = new Dictionary<string, FuncionInline>();

    public void DefinirFuncion(FuncionInline funcion)
    {
        funciones[funcion.Nombre] = funcion;
    }

    public FuncionInline BuscarFuncion(string nombre)
    {
        if (funciones.TryGetValue(nombre, out var funcion))
        {
            return funcion;
        }
       else
        {
            return null;
        }
    }    
}

public class Variable
{
    private string name;
    private object value;

    public Variable(string name, object value)
    {
        this.name = name;
        this.value = value;
    }

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public object Value
    {
        get { return value; }
        set { this.value = value; }
    }
}









    










 

    











