using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estructuras_datos_EP.Estructuras;

public class ListaCircular<T> : IEnumerable<T> where T : class
{
    private NodoSimple<T>? ultimo;
    private NodoSimple<T>? actual;
    private NodoSimple<T>? previo;

    public int Cantidad { get; private set; }
    public bool EstaVacia => ultimo == null;
    public T? Actual => actual?.Dato;

    public void Agregar(T dato)
    {
        NodoSimple<T> nuevo_nodo = new NodoSimple<T>(dato);
        if (ultimo == null)
        {
            nuevo_nodo.Siguiente = nuevo_nodo;
            ultimo = nuevo_nodo; actual = nuevo_nodo; previo = nuevo_nodo;
        }
        else
        {
            nuevo_nodo.Siguiente = ultimo.Siguiente;
            ultimo.Siguiente = nuevo_nodo;
            ultimo = nuevo_nodo;
            if (ReferenceEquals(actual, nuevo_nodo.Siguiente)) previo = nuevo_nodo;
        }
        Cantidad++;
    }

    public void Avanzar()
    {
        if (actual == null) throw new InvalidOperationException("La lista circular está vacía.");
        previo = actual;
        actual = actual.Siguiente;
    }

    public T RemoverActual()
    {
        if (actual == null) throw new InvalidOperationException("La lista circular está vacía.");
        T dato = actual.Dato;
        if (Cantidad == 1)
        {
            ultimo = null; actual = null; previo = null;
        }
        else
        {
            previo!.Siguiente = actual.Siguiente;
            if (ReferenceEquals(actual, ultimo)) ultimo = previo;
            actual = previo.Siguiente;
        }
        Cantidad--;
        return dato;
    }

    public IEnumerator<T> GetEnumerator()
    {
        NodoSimple<T>? nodo = actual;
        for (int i = 0; i < Cantidad && nodo != null; i++, nodo = nodo.Siguiente) yield return nodo.Dato;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
