# 🧪 Plan de Pruebas Automatizadas: Proyecto Pinbox

**Framework:** Playwright + NUnit (C#)
**Arquitectura:** Page Object Model (POM) / Domain-Driven
**Entorno:** Capacitación / QA

---

## 1. Objetivo General
Establecer un framework de pruebas automatizadas escalable, mantenible y robusto para la plataforma Pinbox, asegurando la integridad estructural de la navegación y validando los flujos de negocio críticos mediante simulaciones End-to-End (E2E).

## 2. Enfoque Estratégico y Metodología ("Clean Code & Scalability")

### Metodología: E2E de Caja Negra (Black-Box End-to-End Testing)
Dado que la automatización se realiza sin acceso al código fuente de la plataforma, la estrategia se fundamenta estrictamente en **Pruebas de Caja Negra (Black-Box)**.
* **Simulación Real:** Se utiliza Microsoft Playwright como motor para simular de forma exacta la experiencia y el comportamiento de un usuario real operando el sistema desde el navegador. Tolerancia dinámica a la carga del DOM (`WaitForAsync`).
* **Validación Integral (E2E):** El framework valida el flujo completo de la aplicación de principio a fin. Al examinar la funcionalidad desde el exterior y evaluar la interfaz, se asegura que la orquestación de todos los componentes internos funcione correctamente.
* **Trazabilidad y Evidencia:** Implementación de `Tracing` global forense y segregación automática de evidencias (Videos, Traces, Logs, Network HAR) organizadas por carpetas según el módulo.

### Arquitectura de Pruebas (Separation of Concerns)
El proyecto divide el esfuerzo de automatización en dos capas:
* **Pruebas de Humo (Smoke Tests) Estructurales:** Pruebas de navegación rápidas enfocadas en la disponibilidad de la interfaz, menús y ruteo. Detectan errores 500 o bloqueos de UI rápidamente.
* **Pruebas Profundas (Deep Dives) Funcionales:** Pruebas dedicadas exclusivamente a la simulación de reglas de negocio en módulos críticos. Se centran en el ingreso de datos, manipulación de acordeones, validación de iframes, intercepción de pop-ups y descargas.

## 3. Fases de Implementación
* **Fase 1: Core, Autenticación y Página Principal (✅ Completada)**
* **Fase 2: Navegación, Enrutamiento y Deep Dives Secundarios (🔄 En Progreso):** Estabilización de los módulos de "Tablero" y "Ayudas" con las nuevas aserciones de tolerancia de red.
* **Fase 3: Deep Dive Funcional - Cotizador y Clientes (🎯 Siguiente Objetivo):** Automatización exhaustiva de los módulos críticos definidos por Sistemas (Actualmente en 🔄 Cuarentena/Ignorados para revisión de logs).

## 4. Stack Tecnológico
* **Lenguaje:** C# (.NET 10.0)
* **Framework de Pruebas:** NUnit (Aserciones, Data-Driven Testing, Filtros `[Ignore]`).
* **Automatización UI:** Microsoft Playwright.
* **Gestor de Entorno:** Archivo único de credenciales globales (`Global_Credentials.json`) para identidad centralizada.

---

# 📊 Matriz de Ejecución de Pruebas

## 📖 Glosario de Nomenclatura (Prefijos y Estados)
Para mantener la escalabilidad, los Casos de Prueba (TC) utilizan un sistema de prefijos basado en las consonantes del módulo:
* **QA-LGN-** : Login (Autenticación)
* **QA-PRN-** : Página Principal (Cabecera, KPIs, Pestañas, Filtros)
* **QA-SMK-** : Smoke Tests (Pruebas barredoras de menús laterales)
* **QA-CTZ-** : Cotizador
* **QA-CLN-** : Cliente Nuevo
* **QA-CNT-** : Contratos
* **QA-TBL-** : Tablero (Deep Dives)
* **QA-GST-** : Gestión Operativa (Deep Dives)
* **QA-AYD-** : Ayudas y Herramientas (Deep Dives)

**Estados de Ejecución:**
* ✅ **Automatizado y Estable:** Ejecución regular en verde.
* 🔄 **En Desarrollo / Cuarentena:** Pausado por Bug del sistema o silenciado temporalmente para validación de red y refactorización.
* ❌ **Pendiente de Desarrollo:** Mapeado para futuras iteraciones.

---

## 🔐 Módulo 1: Login
*Validación de acceso y seguridad del portal.*

### Módulo 1.1: Autenticación
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-LGN-01** | Login Exitoso (Redirección a Página Principal) | ✅ |
| **QA-LGN-02** | Login Denegado (Contraseña Incorrecta) | 🔄 |
| **QA-LGN-03** | Login Denegado (Usuario Inexistente) | ✅ |

---

## 🏠 Módulo 2: Página Principal
*Validación de los elementos fijos, interactividad de KPIs y filtros iniciales en el Dashboard.*

### Módulo 2.1: Botones KPI (Circulares)
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-PRN-01** | Navegación y retorno: Contratos en OLBC | ✅ |
| **QA-PRN-02** | Navegación y retorno: Contratos rechazados | ✅ |
| **QA-PRN-03** | Navegación y retorno: Contratos en revisión | ✅ |
| **QA-PRN-04** | Navegación y retorno: Contratos en ingreso | ✅ |
| **QA-PRN-05** | Navegación y retorno: Estación IC | ✅ |
| **QA-PRN-06** | Navegación y retorno: Contratos en fulfillment | ✅ |
| **QA-PRN-07** | Navegación y retorno: Contratos publicados | ✅ |
| **QA-PRN-08** | Navegación y retorno: Casos | ✅ |
| **QA-PRN-09** | Navegación y retorno: Al día | ✅ |
| **QA-PRN-10** | Navegación y retorno: Cambios y correcciones | ✅ |

### Módulo 2.2: Pestañas de Navegación Central y Buscador
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-PRN-11** | Navegación por pestañas (Dashboard, Gestión, Ayudas) | ✅ |
| **QA-PRN-12** | Renderizado e intercepción de alertas en Buscador Vacío | ✅ |

### Módulo 2.3: Filtros de Sinergia y Tarjetas de Estado
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-PRN-13** | Integridad de Cabecera (Inyección correcta de Identidad del Agente) | ✅ |
| **QA-PRN-14** | Validar tarjetas de estado interactivo (No Elaborado, Pendiente) | ✅ |
| **QA-PRN-15** | Aplicación de filtro dinámico: Comercial, Residencial, Ambos | ✅ |

---

## 🧭 Módulo 3: Menús Laterales (Smoke Tests)
*Pruebas estructurales que confirman que el clic en el enlace lateral abre la pantalla correcta y no genera errores de servidor.*

### Módulo 3.1: Comportamiento Base
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-SMK-01** | Apertura y Cierre de Menú Toggle (Mismo Botón Hamburguesa) | ✅ |

### Módulo 3.2: Barredores Dinámicos
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-SMK-02** | Barredor dinámico de enlaces en Tablero (6 enlaces) | 🔄 |
| **QA-SMK-03** | Barredor dinámico de enlaces en Gestión (~26 enlaces) | ✅ |
| **QA-SMK-04** | Barredor dinámico de enlaces en Ayudas (11 enlaces) | 🔄 |

---

## 💼 Módulo 4: Gestión (Deep Dives Funcionales Core)
*Pruebas End-to-End de los flujos críticos de captura de datos y cotizaciones.*

### Módulo 4.1: Cotizador
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-CTZ-01** | Carga inicial del formulario y validación de combos | 🔄 |
| **QA-CTZ-02** | Creación completa de cotización exitosa (E2E) | 🔄 |
| **QA-CTZ-03** | Validación de campos obligatorios vacíos | 🔄 |

### Módulo 4.2: Cliente Nuevo
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-CLN-01** | Carga de formulario, validación de RFC y prevención de duplicados | 🔄 |
| **QA-CLN-02** | Alta exitosa de cliente comercial (E2E) | 🔄 |

### Módulo 4.3: Contratos Creados
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-CNT-01** | Búsqueda y visualización de detalle de contrato | ❌ |
| **QA-CNT-02** | Descarga/Exportación de reportes | ❌ |

---

## 📈 Módulo 5: Tablero (Deep Dives Funcionales)
*Pruebas profundas de interacción dentro de las pantallas del menú Tablero (Comisiones y Reportes).*

### Módulo 5.1: Comisiones y Reportes
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-TBL-01** | Tablero: Descarga de reporte e integridad de entorno | 🔄 |
| **QA-TBL-02** | Comisiones: Integridad de datos en todas las comisiones | 🔄 |
| **QA-TBL-03** | Comisiones: Flujo de descarga Excel y Aclaración | 🔄 |
| **QA-TBL-04** | Cuánto falta para mis comisiones: Validar Textos Informativos | 🔄 |
| **QA-TBL-05** | Valida Dominio: Flujos válidos (.com, .com.mx) e inválidos | 🔄 |
| **QA-TBL-06** | Navegación a Certificación Ventas | 🔄 |
| **QA-TBL-07** | Validación de Bono Sinergia | 🔄 |

---

## 📚 Módulo 6: Ayudas (Deep Dives Funcionales)
*Pruebas de interacción, formularios anidados y herramientas visuales dentro del menú Ayudas.*

### Módulo 6.1: Herramientas y Documentación
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-AYD-01** | AppPinbox: Intercepción y validación de descarga de APK | 🔄 |
| **QA-AYD-02** | Glosario: Navegación y filtro alfabético dinámico | 🔄 |
| **QA-AYD-03** | Infografías: Manipulación de selectores y carga de imágenes | 🔄 |
| **QA-AYD-04** | Mapa Mental: Carga de visualizadores complejos | 🔄 |
| **QA-AYD-05** | Notiventas: Manejo y validación de nueva pestaña (Target='_blank') | 🔄 |
| **QA-AYD-06** | Performance Sitios ADN: Renderizado de Iframe de PowerBI | 🔄 |
| **QA-AYD-07** | Preparador Digital: Llenado de formularios y validaciones | 🔄 |
| **QA-AYD-08** | Proceso de Ventas: Interacción con pestañas/acordeones anidados | 🔄 |
| **QA-AYD-09** | Productos: Funcionalidad de filtros Checkbox iterativos | 🔄 |
| **QA-AYD-10** | Herramientas: Visibilidad de enlaces de red (Evitando clicks externos) | 🔄 |
| **QA-AYD-11** | Velocidad de páginas: Redirección a servicios externos (PageSpeed) | 🔄 |

**Leyenda de Estados:**
* ✅ **Automatizado y Estable:** Ejecución regular en verde.
* 🔄 **En Desarrollo / Cuarentena:** Construyéndose o pausado por bug del sistema.
* ❌ **Pendiente de Desarrollo:** Mapeado para futuras iteraciones.

---
---

## 📋 PLANTILLA VACÍA PARA NUEVOS MÓDULOS

## [Icono] Módulo X: [Nombre del Módulo]
*[Breve descripción de lo que cubre este módulo]*

### Módulo X.1: [Sub-módulo]
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-XXX-01** | [Descripción de la prueba 1] | ❌ |
| **QA-XXX-02** | [Descripción de la prueba 2] | ❌ |