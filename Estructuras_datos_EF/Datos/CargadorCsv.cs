using Estructuras_datos_EP.Estructuras;
using Estructuras_datos_EP.Modelos;
using System.Globalization;
using System.Text;

namespace Estructuras_datos_EP.Datos;

// Lee los dos CSV exportados desde Excel 

public static class CargadorCsv
{
    public static Grafo Cargar(string ruta_distribuidores, string ruta_conexiones)
    {
        List<string[]> filas_distribuidores = LeerFilas(ruta_distribuidores);
        int col_nombre = Columna(filas_distribuidores[0], "nombre", "distribuidor");
        if (col_nombre < 0)
            throw new InvalidDataException("distribuidores.csv: la primera fila debe tener una columna llamada «nombre».");

        List<Distribuidor> lista = new List<Distribuidor>();
        for (int i = 1; i < filas_distribuidores.Count; i++)
        {
            string nombre = Campo(filas_distribuidores[i], col_nombre);
            if (nombre == "") continue;
            if (lista.Any(existente => string.Equals(existente.Nombre, nombre, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidDataException($"distribuidores.csv: el distribuidor «{nombre}» está repetido.");
            lista.Add(new Distribuidor { Nombre = nombre });
        }
        if (lista.Count == 0) throw new InvalidDataException("distribuidores.csv: no hay distribuidores.");

        Grafo grafo = new Grafo(lista.ToArray());

        List<string[]> filas_conexiones = LeerFilas(ruta_conexiones);
        string[] encabezado = filas_conexiones[0];
        int col_origen = Columna(encabezado, "origen", "desde");
        int col_destino = Columna(encabezado, "destino", "hasta");
        int col_km = Columna(encabezado, "km", "kil", "distancia");
        int col_min = Columna(encabezado, "min", "tiempo");
        if (col_origen < 0 || col_destino < 0 || col_km < 0)
            throw new InvalidDataException("conexiones.csv: las columnas deben llamarse origen, destino y km (minutos es opcional).");

        for (int i = 1; i < filas_conexiones.Count; i++)
        {
            string[] fila = filas_conexiones[i];
            string origen = Campo(fila, col_origen);
            string destino = Campo(fila, col_destino);
            int a = grafo.IndiceDe(origen);
            int b = grafo.IndiceDe(destino);
            if (a < 0) throw new InvalidDataException($"conexiones.csv, fila {i + 1}: «{origen}» no está en distribuidores.csv.");
            if (b < 0) throw new InvalidDataException($"conexiones.csv, fila {i + 1}: «{destino}» no está en distribuidores.csv.");
            if (!TryNumero(Campo(fila, col_km), out double km) || km <= 0)
                throw new InvalidDataException($"conexiones.csv, fila {i + 1}: el valor de km no es válido.");

            double? minutos = null;
            if (col_min >= 0 && TryNumero(Campo(fila, col_min), out double valor_min) && valor_min > 0) minutos = valor_min;

            if (!grafo.AgregarArista(a, b, km, minutos))
                throw new InvalidDataException($"conexiones.csv, fila {i + 1}: conexión repetida o de un distribuidor consigo mismo.");
        }
        return grafo;
    }

    private static List<string[]> LeerFilas(string ruta)
    {
        if (!File.Exists(ruta)) throw new FileNotFoundException("No se encontró el archivo: " + ruta);
        string[] lineas = File.ReadAllLines(ruta, Encoding.UTF8);
        List<string> utiles = lineas.Where(linea => !string.IsNullOrWhiteSpace(linea)).ToList();
        if (utiles.Count == 0) throw new InvalidDataException(Path.GetFileName(ruta) + " está vacío.");

        char separador = utiles[0].Contains(';') ? ';' : ',';
        List<string[]> filas = new List<string[]>();
        foreach (string linea in utiles)
        {
            string[] campos = Separar(linea, separador);
            if (campos.All(campo => campo == "")) continue;
            filas.Add(campos);
        }
        if (filas.Count == 0) throw new InvalidDataException(Path.GetFileName(ruta) + " está vacío.");
        return filas;
    }

    private static string[] Separar(string linea, char separador)
    {
        List<string> campos = new List<string>();
        StringBuilder actual = new StringBuilder();
        bool entre_comillas = false;
        foreach (char caracter in linea)
        {
            if (caracter == '"') entre_comillas = !entre_comillas;
            else if (caracter == separador && !entre_comillas) { campos.Add(actual.ToString().Trim()); actual.Clear(); }
            else actual.Append(caracter);
        }
        campos.Add(actual.ToString().Trim());
        return campos.ToArray();
    }

    private static int Columna(string[] encabezado, params string[] alias)
    {
        for (int i = 0; i < encabezado.Length; i++)
        {
            string texto = encabezado[i].Trim().ToLowerInvariant();
            foreach (string nombre in alias)
                if (texto.StartsWith(nombre, StringComparison.Ordinal)) return i;
        }
        return -1;
    }

    private static string Campo(string[] fila, int columna) => columna >= 0 && columna < fila.Length ? fila[columna].Trim() : "";

    private static bool TryNumero(string texto, out double valor) =>
        double.TryParse(texto.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out valor);
}
