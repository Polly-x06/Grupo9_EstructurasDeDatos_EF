using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estructuras_datos_EP.Estructuras;

public class Pila<T> : IEnumerable<T> where T : class
{
    private NodoSimple<T>? cima;

    public int Cantidad { get; private set; }
    public bool EstaVacia => cima == null;

    public void Apilar(T dato)
    {
        cima = new NodoSimple<T>(dato) { Siguiente = cima };
        Cantidad++;
    }

    public T Desapilar()
    {
        if (cima == null) throw new InvalidOperationException("La pila está vacía: no hay abonos para revertir.");
        T dato = cima.Dato;
        cima = cima.Siguiente;
        Cantidad--;
        return dato;
    }

    public T Cima()
    {
        if (cima == null) throw new InvalidOperationException("La pila está vacía.");
        return cima.Dato;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (NodoSimple<T>? nodo = cima; nodo != null; nodo = nodo.Siguiente) yield return nodo.Dato;
    }
    public void Limpiar()
    {
        cima = null;
        Cantidad = 0;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}