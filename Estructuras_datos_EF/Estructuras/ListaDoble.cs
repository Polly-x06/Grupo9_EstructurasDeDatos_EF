using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estructuras_datos_EP.Estructuras;

public class ListaDoble<T> : IEnumerable<T> where T : class
{
    private NodoDoble<T>? cabeza;
    private NodoDoble<T>? cola;
    private NodoDoble<T>? actual;

    public int Cantidad { get; private set; }
    public bool EstaVacia => cabeza == null;
    public T? Actual => actual?.Dato;

    public void AgregarFinal(T dato)
    {
        NodoDoble<T> nuevo_nodo = new NodoDoble<T>(dato);
        if (cabeza == null) { cabeza = nuevo_nodo; cola = nuevo_nodo; }
        else { nuevo_nodo.Anterior = cola; cola!.Siguiente = nuevo_nodo; cola = nuevo_nodo; }
        actual = nuevo_nodo;
        Cantidad++;
    }

    public void IrAlInicio() { actual = cabeza; }
    public void IrAlFinal() { actual = cola; }

    public bool Siguiente()
    {
        if (actual?.Siguiente == null) return false;
        actual = actual.Siguiente;
        return true;
    }

    public bool Anterior()
    {
        if (actual?.Anterior == null) return false;
        actual = actual.Anterior;
        return true;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (NodoDoble<T>? nodo = cabeza; nodo != null; nodo = nodo.Siguiente) yield return nodo.Dato;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
