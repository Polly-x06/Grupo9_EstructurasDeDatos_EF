using System;
using System.Collections;
using System.Collections.Generic;

namespace Estructuras_datos_EP.Estructuras;

public class ArbolBinarioBusqueda<T> : IEnumerable<T> where T : class
{
    private readonly Func<T, int> obtener_clave;
    private NodoArbol<T>? raiz;

    public ArbolBinarioBusqueda(Func<T, int> obtener_clave)
    {
        this.obtener_clave = obtener_clave;
    }

    public int Cantidad { get; private set; }
    public bool EstaVacio => raiz == null;
    public NodoArbol<T>? Raiz => raiz;
    public int Comparaciones { get; private set; }

    public bool Insertar(T dato)
    {
        Comparaciones = 0;
        int clave = obtener_clave(dato);
        NodoArbol<T> nuevo_nodo = new NodoArbol<T>(dato);
        if (raiz == null) { raiz = nuevo_nodo; Cantidad++; return true; }

        NodoArbol<T> actual = raiz;
        while (true)
        {
            Comparaciones++;
            int clave_actual = obtener_clave(actual.Dato);
            if (clave == clave_actual) return false;
            if (clave < clave_actual)
            {
                if (actual.Izquierdo == null) { actual.Izquierdo = nuevo_nodo; break; }
                actual = actual.Izquierdo;
            }
            else
            {
                if (actual.Derecho == null) { actual.Derecho = nuevo_nodo; break; }
                actual = actual.Derecho;
            }
        }
        Cantidad++;
        return true;
    }

    public T? Buscar(int clave)
    {
        Comparaciones = 0;
        NodoArbol<T>? actual = raiz;
        while (actual != null)
        {
            Comparaciones++;
            int clave_actual = obtener_clave(actual.Dato);
            if (clave == clave_actual) return actual.Dato;
            actual = clave < clave_actual ? actual.Izquierdo : actual.Derecho;
        }
        return null;
    }

    public bool Eliminar(int clave)
    {
        Comparaciones = 0;
        NodoArbol<T>? padre = null;
        NodoArbol<T>? actual = raiz;
        while (actual != null)
        {
            Comparaciones++;
            int clave_actual = obtener_clave(actual.Dato);
            if (clave == clave_actual) break;
            padre = actual;
            actual = clave < clave_actual ? actual.Izquierdo : actual.Derecho;
        }
        if (actual == null) return false;

        if (actual.Izquierdo != null && actual.Derecho != null)
        {
            NodoArbol<T> padre_sucesor = actual;
            NodoArbol<T> sucesor = actual.Derecho!;
            while (sucesor.Izquierdo != null) { padre_sucesor = sucesor; sucesor = sucesor.Izquierdo; }
            actual.Dato = sucesor.Dato;
            padre = padre_sucesor;
            actual = sucesor;
        }

        NodoArbol<T>? hijo = actual.Izquierdo ?? actual.Derecho;
        if (padre == null) raiz = hijo;
        else if (padre.Izquierdo == actual) padre.Izquierdo = hijo;
        else padre.Derecho = hijo;
        Cantidad--;
        return true;
    }

    public void Limpiar()
    {
        raiz = null;
        Cantidad = 0;
        Comparaciones = 0;
    }

    public ListaSimple<NodoArbol<T>> NodosPreOrden()
    {
        ListaSimple<NodoArbol<T>> resultado = new ListaSimple<NodoArbol<T>>();
        if (raiz == null) return resultado;
        Pila<NodoArbol<T>> pendientes = new Pila<NodoArbol<T>>();
        pendientes.Apilar(raiz);
        while (!pendientes.EstaVacia)
        {
            NodoArbol<T> nodo = pendientes.Desapilar();
            resultado.AgregarFinal(nodo);
            if (nodo.Derecho != null) pendientes.Apilar(nodo.Derecho);
            if (nodo.Izquierdo != null) pendientes.Apilar(nodo.Izquierdo);
        }
        return resultado;
    }

    public ListaSimple<NodoArbol<T>> NodosInOrden()
    {
        ListaSimple<NodoArbol<T>> resultado = new ListaSimple<NodoArbol<T>>();
        Pila<NodoArbol<T>> pendientes = new Pila<NodoArbol<T>>();
        NodoArbol<T>? actual = raiz;
        while (actual != null || !pendientes.EstaVacia)
        {
            while (actual != null) { pendientes.Apilar(actual); actual = actual.Izquierdo; }
            actual = pendientes.Desapilar();
            resultado.AgregarFinal(actual);
            actual = actual.Derecho;
        }
        return resultado;
    }

    public ListaSimple<NodoArbol<T>> NodosPostOrden()
    {
        ListaSimple<NodoArbol<T>> resultado = new ListaSimple<NodoArbol<T>>();
        if (raiz == null) return resultado;
        Pila<NodoArbol<T>> primera = new Pila<NodoArbol<T>>();
        Pila<NodoArbol<T>> segunda = new Pila<NodoArbol<T>>();
        primera.Apilar(raiz);
        while (!primera.EstaVacia)
        {
            NodoArbol<T> nodo = primera.Desapilar();
            segunda.Apilar(nodo);
            if (nodo.Izquierdo != null) primera.Apilar(nodo.Izquierdo);
            if (nodo.Derecho != null) primera.Apilar(nodo.Derecho);
        }
        while (!segunda.EstaVacia) resultado.AgregarFinal(segunda.Desapilar());
        return resultado;
    }

    public ListaSimple<T> PreOrden() => Datos(NodosPreOrden());
    public ListaSimple<T> InOrden() => Datos(NodosInOrden());
    public ListaSimple<T> PostOrden() => Datos(NodosPostOrden());

    private static ListaSimple<T> Datos(ListaSimple<NodoArbol<T>> nodos)
    {
        ListaSimple<T> datos = new ListaSimple<T>();
        foreach (NodoArbol<T> nodo in nodos) datos.AgregarFinal(nodo.Dato);
        return datos;
    }

    public int Altura()
    {
        if (raiz == null) return 0;
        int mayor_nivel = 0;
        Pila<NodoArbol<T>> pendientes = new Pila<NodoArbol<T>>();
        raiz.Nivel = 0;
        pendientes.Apilar(raiz);
        while (!pendientes.EstaVacia)
        {
            NodoArbol<T> nodo = pendientes.Desapilar();
            if (nodo.Nivel > mayor_nivel) mayor_nivel = nodo.Nivel;
            if (nodo.Derecho != null) { nodo.Derecho.Nivel = nodo.Nivel + 1; pendientes.Apilar(nodo.Derecho); }
            if (nodo.Izquierdo != null) { nodo.Izquierdo.Nivel = nodo.Nivel + 1; pendientes.Apilar(nodo.Izquierdo); }
        }
        return mayor_nivel + 1;
    }

    public int AsignarDisposicion()
    {
        int altura = Altura();
        int columna = 0;
        foreach (NodoArbol<T> nodo in NodosInOrden()) nodo.Columna = columna++;
        return altura;
    }

    public IEnumerator<T> GetEnumerator() => InOrden().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}