using Estructuras_datos_EP.Estructuras;
using Estructuras_datos_EP.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estructuras_datos_EP.Servicios;

public class GestorTransacciones
{
    private readonly Func<DateTime> reloj;

    public Cola<Operacion> Llegadas { get; } = new();
    public ListaSimple<Operacion> Registro { get; } = new();
    public ListaDoble<Operacion> Historial { get; } = new();
    public ListaCircular<Operacion> Reintentos { get; } = new();
    public Pila<Operacion> Abonos { get; } = new();
    public ArbolBinarioBusqueda<Operacion> Indice { get; } = new(operacion => operacion.Numero);

    public GestorTransacciones(Func<DateTime>? reloj_externo = null)
    {
        reloj = reloj_externo ?? (() => DateTime.Now);
    }

    public bool ExisteCodigo(string codigo)
    {
        if (!Operacion.TryObtenerNumero(codigo, out int numero)) return false;
        return Registro.Buscar(operacion => operacion.Numero == numero) != null
            || Llegadas.Any(operacion => operacion.Numero == numero)
            || Reintentos.Any(operacion => operacion.Numero == numero);
    }

    public List<string> Llegar(Operacion operacion)
    {
        operacion.Codigo = (operacion.Codigo ?? "").Trim().ToUpperInvariant();
        operacion.Origen = (operacion.Origen ?? "").Trim();
        operacion.Destino = (operacion.Destino ?? "").Trim();

        List<string> errores = Validador.Validar(operacion, reloj());
        if (errores.Count == 0 && ExisteCodigo(operacion.Codigo))
            errores.Add($"Ya existe una operación con el código {operacion.Codigo}.");
        if (errores.Count > 0) return errores;

        operacion.Estado = EstadoOperacion.Pendiente;
        operacion.Intentos = 0;
        Llegadas.Enqueue(operacion);
        return errores;
    }

    public List<string> CargarLlegadas(IEnumerable<Operacion> operaciones)
    {
        List<string> lista_errores = new List<string>();
        foreach (Operacion operacion in operaciones)
        {
            string codigo_operacion = operacion.Codigo;
            foreach (string error in Llegar(operacion)) lista_errores.Add($"{codigo_operacion}: {error}");
        }
        return lista_errores;
    }

    public Operacion Atender(bool hay_senal = true)
    {
        Operacion operacion = Llegadas.Dequeue();
        Registro.AgregarFinal(operacion);
        Historial.AgregarFinal(operacion);
        Indice.Insertar(operacion);
        operacion.Intentos = 1;
        if (hay_senal) CompletarConExito(operacion);
        else
        {
            operacion.Estado = EstadoOperacion.Pendiente;
            Reintentos.Agregar(operacion);
        }
        return operacion;
    }

    private void CompletarConExito(Operacion operacion)
    {
        operacion.Estado = EstadoOperacion.Procesada;
        if (operacion.EsAbono) Abonos.Apilar(operacion);
    }

    public List<string> Reintentar(Func<Operacion, bool> hay_exito)
    {
        List<string> lista_eventos = new List<string>();
        int limite_visitas = Reintentos.Cantidad * Validador.MaxIntentos;
        int visitas = 0;

        while (!Reintentos.EstaVacia)
        {
            if (++visitas > limite_visitas)
            {
                lista_eventos.Add("Corte de seguridad: se alcanzó el límite de visitas.");
                break;
            }
            Operacion operacion = Reintentos.Actual!;
            operacion.Intentos++;
            if (hay_exito(operacion))
            {
                CompletarConExito(operacion);
                Reintentos.RemoverActual();
                lista_eventos.Add($"{operacion.Codigo}: procesada en el intento {operacion.Intentos}.");
            }
            else if (operacion.Intentos >= Validador.MaxIntentos)
            {
                operacion.Estado = EstadoOperacion.Fallida;
                Reintentos.RemoverActual();
                lista_eventos.Add($"{operacion.Codigo}: FALLIDA tras {operacion.Intentos} intentos (máximo alcanzado).");
            }
            else
            {
                lista_eventos.Add($"{operacion.Codigo}: intento {operacion.Intentos} sin éxito; pasa a la siguiente.");
                Reintentos.Avanzar();
            }
        }
        return lista_eventos;
    }

    public Operacion RevertirUltimoAbono()
    {
        Operacion operacion = Abonos.Desapilar();
        operacion.Estado = EstadoOperacion.Revertida;
        return operacion;
    }

    public Operacion ConsultarFrente() => Llegadas.Peek();

    public Operacion? BuscarPorCodigo(string codigo)
    {
        if (!Operacion.TryObtenerNumero(codigo, out int numero)) return null;
        return Indice.Buscar(numero)
            ?? Llegadas.FirstOrDefault(operacion => operacion.Numero == numero);
    }

    public (Operacion? Encontrada, int ComparacionesLista, int ComparacionesArbol) CompararBusqueda(string codigo)
    {
        if (!Operacion.TryObtenerNumero(codigo, out int numero)) return (null, 0, 0);
        Operacion? en_lista = Registro.Buscar(operacion => operacion.Numero == numero);
        int comparaciones_lista = Registro.NodosVisitados;
        Operacion? en_arbol = Indice.Buscar(numero);
        int comparaciones_arbol = Indice.Comparaciones;
        return (en_arbol ?? en_lista, comparaciones_lista, comparaciones_arbol);
    }

    public int LimpiarCola()
    {
        int descartadas = Llegadas.Cantidad;
        Llegadas.Limpiar();
        return descartadas;
    }

    public int LimpiarPila()
    {
        int confirmados = Abonos.Cantidad;
        Abonos.Limpiar();
        return confirmados;
    }

    public Operacion AnularOperacion(string codigo)
    {
        Operacion? operacion = Operacion.TryObtenerNumero(codigo, out int numero) ? Indice.Buscar(numero) : null;
        if (operacion == null)
            throw new InvalidOperationException($"No existe una operación registrada con el código '{(codigo ?? "").Trim()}'.");
        if (operacion.Estado != EstadoOperacion.Fallida && operacion.Estado != EstadoOperacion.Revertida)
            throw new InvalidOperationException("Solo se pueden anular operaciones Fallidas o Revertidas. Para deshacer un abono procesado use «Revertir último abono».");

        Registro.Eliminar(registrada => ReferenceEquals(registrada, operacion));
        Historial.Eliminar(registrada => ReferenceEquals(registrada, operacion));
        Indice.Eliminar(operacion.Numero);
        return operacion;
    }
}