using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estructuras_datos_EP.Estructuras;

public class NodoSimple<T>
{
    public T Dato { get; set; }
    public NodoSimple<T>? Siguiente { get; set; }
    public NodoSimple(T dato) { Dato = dato; }
}
public class NodoDoble<T>
{
    public T Dato { get; set; }
    public NodoDoble<T>? Siguiente { get; set; }
    public NodoDoble<T>? Anterior { get; set; }
    public NodoDoble(T dato) { Dato = dato; }
}
public class NodoArbol<T>
{
    public T Dato { get; set; }
    public NodoArbol<T>? Izquierdo { get; set; }
    public NodoArbol<T>? Derecho { get; set; }
    public int Nivel { get; set; }
    public int Columna { get; set; }
    public NodoArbol(T dato) { Dato = dato; }
}