#!/bin/bash

# Script de verificación para Android Studio
echo "=== Verificación de Proyecto ManyBox Android ==="

# Verificar estructura del proyecto
echo "1. Verificando estructura del proyecto..."
if [ -f "build.gradle" ] && [ -f "settings.gradle" ] && [ -d "app" ]; then
    echo "✅ Estructura del proyecto correcta"
else
    echo "❌ Estructura del proyecto incompleta"
    exit 1
fi

# Verificar archivos principales
echo "2. Verificando archivos principales..."
files=(
    "app/build.gradle"
    "app/src/main/AndroidManifest.xml"
    "app/src/main/java/com/manybox/chofer/MainActivity.kt"
    "app/src/main/java/com/manybox/chofer/LoginActivity.kt"
)

for file in "${files[@]}"; do
    if [ -f "$file" ]; then
        echo "✅ $file"
    else
        echo "❌ $file - FALTANTE"
    fi
done

# Verificar dependencias principales
echo "3. Verificando dependencias en build.gradle..."
if grep -q "androidx.navigation" app/build.gradle; then
    echo "✅ Navigation Component"
else
    echo "❌ Navigation Component no encontrado"
fi

if grep -q "retrofit2" app/build.gradle; then
    echo "✅ Retrofit"
else
    echo "❌ Retrofit no encontrado"
fi

if grep -q "material" app/build.gradle; then
    echo "✅ Material Design"
else
    echo "❌ Material Design no encontrado"
fi

echo ""
echo "=== INSTRUCCIONES ==="
echo "1. Abre Android Studio"
echo "2. Selecciona 'Open an existing Android Studio project'"
echo "3. Navega a esta carpeta: $(pwd)"
echo "4. Espera la sincronización de Gradle"
echo "5. Configura un AVD (Android Virtual Device)"
echo "6. Presiona el botón Run (▶️)"
echo ""
echo "Credenciales de prueba:"
echo "Usuario: chofer@test.com"
echo "Contraseña: 123456"
echo ""
echo "¡Listo para probar! 🚀"