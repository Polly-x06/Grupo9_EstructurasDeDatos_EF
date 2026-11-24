using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estructuras_datos_EP.Estructuras;

public class Cola<T> : IEnumerable<T> where T : class
{
    private NodoSimple<T>? frente;
    private NodoSimple<T>? final;

    public int Cantidad { get; private set; }
    public bool EstaVacia => frente == null;

    public void Enqueue(T dato)
    {
        NodoSimple<T> nuevo_nodo = new NodoSimple<T>(dato);
        if (final == null) { frente = nuevo_nodo; final = nuevo_nodo; }
        else { final.Siguiente = nuevo_nodo; final = nuevo_nodo; }
        Cantidad++;
    }


    public T Dequeue()
    {
        if (frente == null) throw new InvalidOperationException("La cola está vacía: no hay transacciones por atender.");
        T dato = frente.Dato;
        frente = frente.Siguiente;
        if (frente == null) final = null;
        Cantidad--;
        return dato;
    }


    public T Peek()
    {
        if (frente == null) throw new InvalidOperationException("La cola está vacía.");
        return frente.Dato;
    }


    public IEnumerator<T> GetEnumerator()
    {
        for (NodoSimple<T>? nodo = frente; nodo != null; nodo = nodo.Siguiente) yield return nodo.Dato;
    }
    public void Limpiar()
    {
        frente = null;
        final = null;
        Cantidad = 0;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}