PROTOTIPO EP - ESTRUCTURAS DE DATOS (CIIN1190P)
Bodega "Chabelita", Trujillo - observación del 12/10/2026, 6:00 a 7:30 p. m.

Grupo 9
Integrantes:
  - Paredes Pacherre, Carlos Adrian
  - Alva Plasencia, Dario Leonel
  - Sare Aranda, Alexia Camila
  - Esquivel Gonzales, Elmer Antony
  - Contreras Siccha, Fabricio Andre


1. QUE HACE
	Simula el flujo de pagos de una bodega con Yape: las operaciones llegan a una
	cola, se atienden en orden, se registran, se reintentan si no hay señal y el
	ultimo abono se puede revertir.


2. REQUISITOS
	- Windows 10 u 11
	- Visual Studio con la carga "Desarrollo de escritorio con .NET"
	- .NET 8 SDK


3. COMO EJECUTAR
	1. Descomprimir el ZIP.
	2. Abrir Estructuras_datos_EP.slnx con Visual Studio.
	3. Verificar que el proyecto de inicio sea Estructuras_datos_EP y presionar F5.


4. ESTRUCTURAS (carpeta Estructuras)
	Registro de operaciones     ->  Lista simple    ->  ListaSimple.cs
	Navegación del historial    ->  Lista doble     ->  ListaDoble.cs
	Gestión de reintentos       ->  Lista circular  ->  ListaCircular.cs
	Reversión del ultimo abono  ->  Pila            ->  Pila.cs
	Orden de arribo             ->  Cola            ->  Cola.cs

	Operaciones de la cola: Enqueue (encolar), Dequeue (desencolar) y
	Peek (consultar el frente sin sacarlo).

	Clase común: Operación (carpeta Modelos): código, fecha y hora, tipo, monto,
	origen, destino, estado e intentos.
	Todas las estructuras están hechas con nodos propios (sin usar las clases
	List, Queue ni Stack de .NET).


5. COMO USAR LA VENTANA
	Cargar datos de campo: pone en la cola las 12 operaciones observadas.
	Atender (desencolar): procesa la primera de la cola.
	Atender SIN señal (casilla): manda la operación a reintentos.
	Ejecutar reintentos: recorre la lista circular (se detiene a los 3 intentos).
	Revertir ultimo abono: usa la pila.
	Buscar operación: busca por código (ejemplo: OP-001).
	Pestana "Lista doble": botones Inicio, Anterior, Siguiente y Final.
	Ejecutar pruebas: corre los casos de prueba y guarda resultados_pruebas.txt
	(junto al ejecutable, en bin\Debug\net8.0-windows).
	Reiniciar: vacía las 5 estructuras.


6. VALIDACIONES
	Código: formato OP-001, no vacío y único.
	Fecha y hora: no puede ser futura.
	Monto: mayor que 0 y hasta S/ 500.
	Origen y destino: no vacíos y distintos.
	Intentos: de 0 a 3.


7. DATOS DE PRUEBA
	12 operaciones reales de la bodega (18:03 a 19:24), todas procesadas al
	primer intento. Total S/ 78.60. Están en el archivo bitacora.xlsx (en la
	misma carpeta que este README) y en el código, en
	Estructuras_datos_EP/Datos/DatosCampo.cs.
	En campo no hubo fallas ni reversas: esas pruebas se hacen ingresando
 	casos manualmente.


8. CASOS DE PRUEBA (botón "Ejecutar pruebas")
	1. Estructura vacía
	2. Registro de una operación
	3. Registro de varias operaciones
	4. Navegación hacia adelante y atrás
	5. Recorrido circular con condición de parada
	6. Reversa del ultimo abono
	7. Búsqueda de una operación inexistente
	8. Control de datos inválidos
	9. Cola: encolar, atender y consultar el frente


9. ALCANCE
	Esta entrega es la EP: listas simple, doble y circular, pila y cola.
	El árbol de búsqueda binaria y el grafo quedan para la Evaluación Final.