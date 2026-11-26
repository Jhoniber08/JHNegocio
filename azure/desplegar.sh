#!/usr/bin/env bash
# ============================================================
#  JH Negocio — Publicar la API en Azure App Service
#  Autor: Jhoniber De la Cruz
#  Uso (desde la raíz del repositorio, en Codespaces):
#      bash azure/desplegar.sh
#  La primera vez crea los recursos; las siguientes solo actualiza.
# ============================================================
set -uo pipefail
cd "$(dirname "$0")/.."

GRUPO="jhnegocio-rg"
PLAN="jhnegocio-plan"
ARCHIVO_NOMBRE="azure/.nombre-app"
REGIONES="eastus2 eastus centralus westus2 southcentralus westus3 brazilsouth canadacentral"

paso() { echo; echo "==> $1"; }
falla() { echo; echo "XX  $1"; echo "    Toma una captura de esta pantalla y envíala."; exit 1; }

# 1. Azure CLI
if ! command -v az >/dev/null 2>&1; then
  paso "Instalando Azure CLI (1-2 minutos)..."
  curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash >/dev/null || falla "No se pudo instalar Azure CLI."
fi

# 2. Iniciar sesión
if ! az account show >/dev/null 2>&1; then
  paso "Inicia sesión: abre https://microsoft.com/devicelogin en otra pestaña y escribe el código que aparece abajo."
  az login --use-device-code --output none || falla "No se completó el inicio de sesión."
fi
az account show --query "{Suscripcion:name, Usuario:user.name}" -o table

# 3. Registrar el proveedor de App Service (necesario en suscripciones nuevas)
paso "Preparando la suscripción..."
az provider register --namespace Microsoft.Web --wait >/dev/null 2>&1 || true

# 4. Crear recursos (solo la primera vez)
if [ -f "$ARCHIVO_NOMBRE" ]; then
  APP=$(cat "$ARCHIVO_NOMBRE")
  paso "La app ya existe: $APP (solo se actualizará el código)"
else
  APP="jhnegocio-api-$(( RANDOM % 9000 + 1000 ))"
  CREADO=""
  for R in $REGIONES; do
    paso "Probando región $R..."
    az group create -n "$GRUPO" -l "$R" -o none 2>/dev/null || { az group delete -n "$GRUPO" --yes 2>/dev/null; continue; }
    if az appservice plan create -n "$PLAN" -g "$GRUPO" -l "$R" --sku F1 --is-linux -o none 2>/tmp/plan.err; then
      CREADO="$R"; break
    fi
    echo "    No permitida aquí: $(head -c 200 /tmp/plan.err)"
    az group delete -n "$GRUPO" --yes 2>/dev/null
  done
  [ -n "$CREADO" ] || falla "Ninguna región aceptó el plan gratuito F1."
  paso "Creando la Web App $APP (.NET 10, Linux) en $CREADO..."
  az webapp create -n "$APP" -g "$GRUPO" -p "$PLAN" --runtime "DOTNETCORE:10.0" -o none || falla "No se pudo crear la Web App."
  az webapp config set -n "$APP" -g "$GRUPO" --startup-file "dotnet JHNegocio.Api.dll" -o none
  az webapp update -n "$APP" -g "$GRUPO" --https-only true -o none
  echo "$APP" > "$ARCHIVO_NOMBRE"
fi

# 5. Compilar y publicar
paso "Compilando la API en modo Release..."
rm -rf /tmp/jh-publish /tmp/jh-api.zip
dotnet publish JHNegocio.Api/JHNegocio.Api.csproj -c Release -o /tmp/jh-publish --nologo -v q || falla "Error al compilar la API."
(cd /tmp/jh-publish && python3 -m zipfile -c /tmp/jh-api.zip .) || falla "No se pudo crear el ZIP."

paso "Subiendo a Azure (1-3 minutos)..."
az webapp deploy -g "$GRUPO" -n "$APP" --src-path /tmp/jh-api.zip --type zip -o none || falla "Falló la subida."

# 6. Verificación
URL="https://$APP.azurewebsites.net"
paso "Verificando (la primera respuesta puede tardar hasta 1 minuto)..."
for i in 1 2 3 4 5 6; do
  CODIGO=$(curl -s -o /tmp/jh-resp.json -w "%{http_code}" "$URL/api/productos/resumen" || echo 000)
  [ "$CODIGO" = "200" ] && break
  sleep 10
done
echo "    GET /api/productos/resumen -> HTTP $CODIGO"
[ "$CODIGO" = "200" ] && cat /tmp/jh-resp.json && echo

echo
echo "============================================================"
echo "  API publicada en Azure"
echo "  Swagger:  $URL/swagger"
echo "  Para la app (botón Servidor):  $URL/"
echo "============================================================"
