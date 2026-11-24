PROTOTIPO EF - ESTRUCTURAS DE DATOS (CIIN1190P)
Bodega "Chabelita", Trujillo - observación del 12/10/2026, 6:00 a 7:30 p. m.

Grupo 9
Integrantes:
  - Paredes Pacherre, Carlos Adrian
  - Alva Plasencia, Dario Leonel
  - Sare Aranda, Alexia Camila
  - Esquivel Gonzales, Elmer Antony
  - Contreras Siccha, Fabricio Andre


1. QUE HACE
	Simula el flujo de pagos de una bodega con Yape (cola, registro, reintentos y
	reversa del último abono), indexa las operaciones en un árbol binario de
	búsqueda y modela a los distribuidores como un grafo: rutas más cortas, árbol
	de expansión mínima, compras fiadas, pago de deudas y plan de visita óptimo.


2. REQUISITOS
	- Windows 10 u 11
	- Visual Studio con la carga "Desarrollo de escritorio con .NET"
	- .NET 8 SDK


3. COMO EJECUTAR
	1. Descomprimir el ZIP.
	2. Abrir Estructuras_datos_EF.slnx con Visual Studio.
	3. Verificar que el proyecto de inicio sea Estructuras_datos_EF y presionar F5.
	Los archivos distribuidores.csv y conexiones.csv (carpeta Datos) se copian solos
	junto al ejecutable y el grafo se carga al abrir la ventana.


4. ESTRUCTURAS (carpeta Estructuras)
	Registro de operaciones     ->  Lista simple    ->  ListaSimple.cs
	Navegación del historial    ->  Lista doble     ->  ListaDoble.cs
	Gestión de reintentos       ->  Lista circular  ->  ListaCircular.cs
	Reversión del último abono  ->  Pila            ->  Pila.cs
	Orden de arribo             ->  Cola            ->  Cola.cs
	Búsqueda por código         ->  Árbol binario de búsqueda  ->  ArbolBinarioBusqueda.cs
	Red de distribuidores       ->  Grafo (matriz y lista de adyacencia, Floyd-Warshall,
	                                Prim, plan de visita)       ->  Grafo.cs

	Todas las estructuras están hechas con nodos propios (sin usar las clases
	List, Queue ni Stack de .NET en las estructuras).


5. COMO USAR LA VENTANA
	Panel izquierdo y pestañas Cola, Lista simple, Lista doble, Reintentos, Pila:
	igual que en la EP (encolar, atender, atender SIN señal, reintentos, revertir,
	buscar, anular, limpiar cola y pila).
	Pestaña "Árbol": dibujo del árbol, recorridos inorden/preorden/postorden y
	comparación de búsqueda lista simple vs árbol.
	Pestaña "Grafo": dibujo del grafo con las distancias en km; "Ruta más corta"
	(origen y destino, Floyd-Warshall, se resalta en rojo); "Floyd-Warshall" (matriz
	de distancias mínimas); "Prim" (árbol mínimo, en verde); "Matriz y lista".
	Pestaña "Deudas": "Comprar fiado" (todos menos Makro), "Pagar deuda" (crea una
	transferencia que entra a la COLA; al atenderla pasa por reintentos y pila; si se
	revierte, la deuda vuelve a subir), "Plan de visita óptimo" (marcar los
	distribuidores a visitar), "Cargar 40 simulados", "Atender toda la cola" y
	"Probar volumen de 10 000".
	Ejecutar pruebas: corre los casos y guarda resultados_pruebas.txt
	(junto al ejecutable, en bin\Debug\net8.0-windows).


6. FORMATO DE LOS CSV (carpeta Datos)
	distribuidores.csv   ->  una columna "nombre"
	conexiones.csv       ->  columnas "origen", "destino", "km" y, opcional, "minutos"
	El peso del grafo es la distancia en km (es lo único completo en las 8 conexiones;
	el tiempo solo está en 3). Se aceptan separador ; o , y decimales con coma o punto.


7. VALIDACIONES
	Código: formato OP-001 (3 a 6 dígitos), no vacío y único.
	Fecha y hora: no puede ser futura.
	Monto: mayor que 0 y hasta S/ 500.
	Origen y destino: no vacíos y distintos.
	Intentos: de 0 a 3.
	Compra fiada: monto mayor que 0; Makro no vende fiado.
	Pago de deuda: no puede superar el saldo pendiente.
	CSV: columnas obligatorias, distribuidores sin repetir, conexiones solo entre
	distribuidores existentes, km mayor que 0, sin conexiones repetidas.


8. DATOS DE PRUEBA
	- 12 operaciones reales de la bodega (Datos/DatosCampo.cs).
	- 40 operaciones simuladas (botón "Cargar 40 simulados": semilla fija, siempre las mismas).
	- 10 000 operaciones para el volumen (botón "Probar volumen de 10 000").
	- Grafo de ejemplo de 5 puntos, calculado a mano, para las pruebas del grafo.


9. CASOS DE PRUEBA (botón "Ejecutar pruebas": 28 casos)
	1 a 9 y Extra: estructuras de la EP.
	10 a 18: árbol, anulación y eliminación en listas.
	19. Grafo: matriz y lista de adyacencia
	20. Floyd-Warshall: distancias y rutas
	21. Prim: árbol de expansión mínima
	22. Plan de visita óptimo
	23. Carga desde CSV
	24. Compras fiadas (todos menos Makro)
	25. Pago de deuda por cola, reintentos y pila
	26. 40 operaciones simuladas
	27. Volumen de 10 000 operaciones (lista simple vs árbol)


10. ALCANCE
	Evaluación Final: incluye listas, pila, cola, árbol binario de búsqueda y grafo.