using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Estructuras_datos_EP.Estructuras;

public class ListaSimple<T> : IEnumerable<T> where T : class
{
    private NodoSimple<T>? cabeza;
    private NodoSimple<T>? cola;

    public int Cantidad { get; private set; }
    public bool EstaVacia => cabeza == null;

    public void AgregarFinal(T dato)
    {
        NodoSimple<T> nuevo_nodo = new NodoSimple<T>(dato);
        if (cabeza == null) { cabeza = nuevo_nodo; cola = nuevo_nodo; }
        else { cola!.Siguiente = nuevo_nodo; cola = nuevo_nodo; }
        Cantidad++;
    }

    public void AgregarInicio(T dato)
    {
        NodoSimple<T> nuevo_nodo = new NodoSimple<T>(dato) { Siguiente = cabeza };
        cabeza = nuevo_nodo;
        if (cola == null) cola = nuevo_nodo;
        Cantidad++;
    }

    public int NodosVisitados { get; private set; }

    public T? Buscar(Predicate<T> criterio)
    {
        NodosVisitados = 0;
        for (NodoSimple<T>? nodo = cabeza; nodo != null; nodo = nodo.Siguiente)
        {
            NodosVisitados++;
            if (criterio(nodo.Dato)) return nodo.Dato;
        }
        return null;
    }

    public bool Eliminar(Predicate<T> criterio)
    {
        NodoSimple<T>? previo = null;
        for (NodoSimple<T>? nodo = cabeza; nodo != null; previo = nodo, nodo = nodo.Siguiente)
        {
            if (!criterio(nodo.Dato)) continue;
            if (previo == null) cabeza = nodo.Siguiente; else previo.Siguiente = nodo.Siguiente;
            if (nodo == cola) cola = previo;
            Cantidad--;
            return true;
        }
        return false;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (NodoSimple<T>? nodo = cabeza; nodo != null; nodo = nodo.Siguiente) yield return nodo.Dato;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
