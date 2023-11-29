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

    Desconocido,

    Flecha,

    Cadena,

    Concatenador,

   
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
    public double Valor { get; }

    public ValorNumerico(double value)
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
    
    object valor = subexpresion.Evaluar(entorno);

    
    if (valor is int)
    {
       
        return - Convert.ToDouble(valor);
    }
    else if (valor is double)
    {
        
        return - (double)valor;
    }
    else
    {
      
        throw new Exception("Error: se intentó negar un valor no numérico.");
    }
}

}

public class Cadena : AST
{
    public string Valor { get; }

    public Cadena(string valor)
    {
        Valor = valor;
    }

    public override object Evaluar(Entorno entorno)
    {
        // Para una cadena, simplemente devolvemos su valor
        return Valor;
    }
}





public class FuncionLog : AST
{
    public AST BaseLog { get; }
    public AST ArgumentoLog { get; }

    public FuncionLog(AST baseLog, AST argumentoLog)
    {
        BaseLog = baseLog;
        ArgumentoLog = argumentoLog;
    }

    public override object Evaluar(Entorno entorno)
    {
         double valorBase = Convert.ToDouble(BaseLog.Evaluar(entorno));
        double valorArgumento = Convert.ToDouble(ArgumentoLog.Evaluar(entorno));
       

       
        return Math.Log(valorBase,valorArgumento);
    }
}

public class FuncionSin : AST
{
    public AST Argumento { get; }

    public FuncionSin(AST argumento)
    {
        Argumento = argumento;
    }

    public override object Evaluar(Entorno entorno)
    {
        
        double valorArgumento = Convert.ToDouble(Argumento.Evaluar(entorno));

       
        return Math.Sin(valorArgumento);
    }
}
public class FuncionCos : AST
{
    public AST Argumento { get; }

    public FuncionCos(AST argumento)
    {
        Argumento = argumento;
    }

    public override object Evaluar(Entorno entorno)
    {
        
        double valorArgumento = Convert.ToDouble(Argumento.Evaluar(entorno));

       
        return Math.Cos(valorArgumento);
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
   
    var valorIzquierdo = OperandoIzquierdo.Evaluar(entorno);
    var valorDerecho = OperandoDerecho.Evaluar(entorno);

   
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
   
    var variable = entorno.BuscarVariable(Nombre);
    if (variable != null)
    {
        
        return variable.Value;
    }
    else
    {
       
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
    
    var valor = ValorInicial.Evaluar(entorno);

    
    entorno.DefinirVariable(new Variable(Nombre, valor));

   
    return null;
}

    
}

public class Concatenacion : AST
{
    public AST Izquierdo { get; }
    public AST Derecho { get; }

    public Concatenacion(AST izquierdo, AST derecho)
    {
        Izquierdo = izquierdo;
        Derecho = derecho;
    }

    public override object Evaluar(Entorno entorno)
    {
        
        object valorIzquierdo = Izquierdo.Evaluar(entorno);
        object valorDerecho = Derecho.Evaluar(entorno);

        
        return valorIzquierdo.ToString() + " " + valorDerecho.ToString();
    }
}
public class LetInExpression : AST
{
    public Entorno Entorno { get; }
    public AST Cuerpo { get; }

    public LetInExpression(Entorno entorno, AST cuerpo)
    {
        Entorno = entorno;
        Cuerpo = cuerpo;
    }

   public override object Evaluar(Entorno entorno)
{
    
    foreach (var variable in this.Entorno.variables)
    {
        entorno.DefinirVariable(variable.Value);
    }

  
    return this.Cuerpo.Evaluar(entorno);
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
  
    bool condicion = Convert.ToBoolean(Condicion.Evaluar(entorno));

   
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
   
    entorno.DefinirFuncion(this);

    
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
        var funcion = entorno.BuscarFuncion(Nombre);

        if (funcion == null)
        {
            throw new Exception($"Error: Función no definida '{Nombre}'.");
        }

        var entornoFuncion = new Entorno(); 

        
        foreach (var variable in entorno.variables)
        {
            entornoFuncion.DefinirVariable(variable.Value);
        }
        foreach (var funcionInline in entorno.funciones)
        {
            entornoFuncion.DefinirFuncion(funcion);
        }

        for (int i = 0; i < Argumentos.Count; i++)
        {
            var valor = Argumentos[i].Evaluar(entorno); 
            var variable = new Variable(funcion.Parametros[i], valor);
            entornoFuncion.DefinirVariable(variable);
        }

        return funcion.Cuerpo.Evaluar(entornoFuncion);
    }
}


public class Entorno
{
    public Dictionary<string, Variable> variables = new Dictionary<string, Variable>();

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

    public Dictionary<string, FuncionInline> funciones = new Dictionary<string, FuncionInline>();

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
    public string Name { get; private set;}
    public object Value { get; private set;} 

    public Variable(string name, object value)
    {
       Name = name;
       Value = value;
    }

    
}









    










 

    











