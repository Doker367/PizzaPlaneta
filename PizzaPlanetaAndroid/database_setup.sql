-- SQL Scripts para Configurar Datos de Prueba - ManyBox Chofer App
-- Ejecutar en tu base de datos ManyBox

-- =====================================================
-- 1. VERIFICAR ESTRUCTURA EXISTENTE
-- =====================================================

-- Verificar tabla de roles
SELECT * FROM roles WHERE nombre LIKE '%hofer%';

-- Verificar empleados existentes
SELECT TOP 5 * FROM empleados;

-- Verificar usuarios existentes
SELECT TOP 5 u.*, r.nombre as rol_nombre 
FROM usuarios u 
LEFT JOIN roles r ON u.rol_id = r.id;

-- =====================================================
-- 2. CREAR DATOS DE PRUEBA
-- =====================================================

-- Crear empleado de prueba si no existe
IF NOT EXISTS (SELECT 1 FROM empleados WHERE nombre = 'Juan' AND apellido = 'TestChofer')
BEGIN
    INSERT INTO empleados (nombre, apellido, correo, telefono, sucursal_id, fecha_nacimiento) 
    VALUES ('Juan', 'TestChofer', 'juan.chofer@manybox.com', '555-0123', 1, '1990-01-01');
    PRINT '✅ Empleado de prueba creado';
END
ELSE
    PRINT '⚠️ Empleado de prueba ya existe';

-- Obtener ID del empleado creado
DECLARE @empleadoId INT = (SELECT TOP 1 id FROM empleados WHERE nombre = 'Juan' AND apellido = 'TestChofer');
PRINT '🔍 Empleado ID: ' + CAST(@empleadoId AS VARCHAR(10));

-- Obtener ID del rol Chofer (ajustar nombre según tu BD)
DECLARE @rolChoferId INT = (SELECT TOP 1 id FROM roles WHERE nombre LIKE '%hofer%' OR nombre LIKE '%Chofer%');
IF @rolChoferId IS NULL
BEGIN
    -- Crear rol si no existe
    INSERT INTO roles (nombre) VALUES ('Chofer');
    SET @rolChoferId = (SELECT TOP 1 id FROM roles WHERE nombre = 'Chofer');
    PRINT '✅ Rol Chofer creado';
END
PRINT '🔍 Rol Chofer ID: ' + CAST(@rolChoferId AS VARCHAR(10));

-- Crear usuario de prueba
-- NOTA: Ajustar el hash de contraseña según tu sistema
-- Este es un ejemplo - necesitas generar el hash correcto para "123456"
DECLARE @passwordHash VARCHAR(255) = 'TU_HASH_AQUI'; -- Cambiar por el hash real

IF NOT EXISTS (SELECT 1 FROM usuarios WHERE username = 'chofer1')
BEGIN
    INSERT INTO usuarios (username, nombre, apellido, password_hash, rol_id, empleado_id, activo, fecha_creacion)
    VALUES ('chofer1', 'Juan', 'TestChofer', @passwordHash, @rolChoferId, @empleadoId, 1, GETDATE());
    PRINT '✅ Usuario chofer1 creado';
END
ELSE
    PRINT '⚠️ Usuario chofer1 ya existe';

-- =====================================================
-- 3. CREAR ENVÍOS DE PRUEBA
-- =====================================================

-- Verificar que existan ventas (necesarias para los envíos)
IF NOT EXISTS (SELECT 1 FROM ventas)
BEGIN
    PRINT '⚠️ No hay ventas en la base de datos. Crear algunas ventas primero.';
    -- Crear ventas de ejemplo si es necesario
    INSERT INTO ventas (fecha, total, cliente_id, sucursal_id) 
    VALUES 
        (GETDATE(), 100.00, 1, 1),
        (GETDATE(), 150.00, 1, 1),
        (GETDATE(), 200.00, 1, 1);
    PRINT '✅ Ventas de prueba creadas';
END

-- Obtener IDs de ventas para los envíos
DECLARE @venta1 INT = (SELECT TOP 1 id FROM ventas ORDER BY id);
DECLARE @venta2 INT = (SELECT TOP 1 id FROM ventas WHERE id > @venta1 ORDER BY id);
DECLARE @venta3 INT = (SELECT TOP 1 id FROM ventas WHERE id > @venta2 ORDER BY id);

-- Crear envíos asignados al chofer de prueba
IF NOT EXISTS (SELECT 1 FROM envios WHERE empleado_id = @empleadoId)
BEGIN
    INSERT INTO envios (guia_rastreo, fecha_entrega, empleado_id, venta_id)
    VALUES 
        ('MB' + RIGHT('000000' + CAST(@venta1 AS VARCHAR), 6), NULL, @empleadoId, @venta1),
        ('MB' + RIGHT('000000' + CAST(@venta2 AS VARCHAR), 6), NULL, @empleadoId, @venta2),
        ('MB' + RIGHT('000000' + CAST(@venta3 AS VARCHAR), 6), NULL, @empleadoId, @venta3);
    PRINT '✅ Envíos de prueba creados';
END
ELSE
    PRINT '⚠️ Ya existen envíos para este chofer';

-- =====================================================
-- 4. CREAR SEGUIMIENTOS DE EJEMPLO
-- =====================================================

-- Obtener IDs de envíos recién creados
DECLARE @envio1 INT = (SELECT TOP 1 id FROM envios WHERE empleado_id = @empleadoId ORDER BY id);
DECLARE @envio2 INT = (SELECT TOP 1 id FROM envios WHERE empleado_id = @empleadoId AND id > @envio1 ORDER BY id);
DECLARE @envio3 INT = (SELECT TOP 1 id FROM envios WHERE empleado_id = @empleadoId AND id > @envio2 ORDER BY id);

-- Crear seguimientos con diferentes estados
IF NOT EXISTS (SELECT 1 FROM seguimiento_paquete WHERE envio_id IN (@envio1, @envio2, @envio3))
BEGIN
    INSERT INTO seguimiento_paquete (envio_id, status, fecha_status, descripcion)
    VALUES 
        -- Envío 1: En preparación
        (@envio1, 'Asignado', DATEADD(hour, -2, GETDATE()), 'Paquete asignado al chofer'),
        
        -- Envío 2: En camino
        (@envio2, 'Asignado', DATEADD(hour, -3, GETDATE()), 'Paquete asignado al chofer'),
        (@envio2, 'En camino', DATEADD(hour, -1, GETDATE()), 'Chofer en ruta de entrega'),
        
        -- Envío 3: Entregado
        (@envio3, 'Asignado', DATEADD(day, -1, GETDATE()), 'Paquete asignado al chofer'),
        (@envio3, 'En camino', DATEADD(hour, -5, GETDATE()), 'Chofer en ruta de entrega'),
        (@envio3, 'Último tramo', DATEADD(hour, -2, GETDATE()), 'Chofer próximo a entregar'),
        (@envio3, 'Entregado', DATEADD(hour, -1, GETDATE()), 'Paquete entregado exitosamente');
    
    PRINT '✅ Seguimientos de prueba creados';
END
ELSE
    PRINT '⚠️ Ya existen seguimientos para estos envíos';

-- =====================================================
-- 5. VERIFICAR CONFIGURACIÓN
-- =====================================================

PRINT '';
PRINT '🔍 RESUMEN DE CONFIGURACIÓN:';
PRINT '================================';

-- Mostrar datos creados
SELECT 
    'Usuario: ' + u.username + ' | Empleado ID: ' + CAST(u.empleado_id AS VARCHAR) + ' | Rol: ' + r.nombre as Configuracion
FROM usuarios u 
JOIN roles r ON u.rol_id = r.id 
WHERE u.username = 'chofer1';

SELECT 
    'Envíos asignados: ' + CAST(COUNT(*) AS VARCHAR) as Envios
FROM envios 
WHERE empleado_id = @empleadoId;

SELECT 
    'Total seguimientos: ' + CAST(COUNT(*) AS VARCHAR) as Seguimientos
FROM seguimiento_paquete sp
JOIN envios e ON sp.envio_id = e.id
WHERE e.empleado_id = @empleadoId;

PRINT '';
PRINT '✅ Configuración completada!';
PRINT '📱 Usar credenciales: chofer1 / 123456';
PRINT '⚠️ IMPORTANTE: Actualizar password_hash con el hash correcto de "123456"';

-- Query para verificar el endpoint de la app
PRINT '';
PRINT '🔍 PREVIEW DEL ENDPOINT /api/envios/asignados/empleado/' + CAST(@empleadoId AS VARCHAR) + ':';
SELECT 
    e.id as EnvioId,
    e.guia_rastreo as GuiaRastreo,
    COALESCE(c.nombre, 'Cliente Test') as Destinatario,
    e.venta_id as VentaId,
    COALESCE((
        SELECT TOP 1 status 
        FROM seguimiento_paquete sp 
        WHERE sp.envio_id = e.id 
        ORDER BY fecha_status DESC
    ), 'Asignado') as EstadoActual
FROM envios e
LEFT JOIN ventas v ON e.venta_id = v.id
LEFT JOIN clientes c ON v.cliente_id = c.id
WHERE e.empleado_id = @empleadoId;