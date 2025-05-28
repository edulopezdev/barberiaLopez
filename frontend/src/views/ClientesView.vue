<template>
  <div class="clientes-container">
    <Card>
      <template #title>
        <div class="encabezado-acciones">
          <h4>Listado de Clientes</h4>
          <div class="botones-acciones">
            <Button
              label="Filtros"
              icon="pi pi-filter"
              class="boton-filtros"
              @click="mostrarFiltros = !mostrarFiltros"
            />
            <Button
              label="Nuevo Cliente"
              icon="pi pi-plus"
              class="boton-nuevo-cliente"
              @click="crearCliente"
            />
          </div>
        </div>
      </template>

      <template #content>
        <DataTable
          v-model:filters="filters"
          :value="clientes"
          :filterDisplay="mostrarFiltros ? 'row' : 'none'"
          :globalFilterFields="['nombre', 'email', 'telefono']"
          lazy
          paginator
          :rows="pageSize"
          :first="first"
          :totalRecords="totalClients"
          tableStyle="min-width: 100%"
          :loading="loading"
          @page="onPageChange"
          @sort="onSort"
          @filter="onFilter"
        >
          <Column field="nombre" sortable>
            <template #header>
              <span class="titulo-columna">Nombre</span>
            </template>
            <template #filter="{ filterModel, filterCallback }">
              <InputText
                v-model="filterModel.value"
                @input="filterCallback()"
                placeholder="Buscar por nombre"
              />
            </template>
          </Column>

          <Column field="email" header="Email" sortable>
            <template #filter="{ filterModel, filterCallback }">
              <InputText
                v-model="filterModel.value"
                @input="filterCallback()"
                placeholder="Buscar por email"
              />
            </template>
          </Column>

          <Column field="telefono" header="Teléfono" sortable>
            <template #filter="{ filterModel, filterCallback }">
              <InputText
                v-model="filterModel.value"
                @input="filterCallback()"
                placeholder="Buscar por teléfono"
              />
            </template>
          </Column>

          <Column field="activo" header="Estado">
            <template #body="slotProps">
              <Tag
                :value="slotProps.data.activo ? 'Activo' : 'Inactivo'"
                :severity="slotProps.data.activo ? 'success' : 'danger'"
              />
            </template>
            <template #filter="{ filterModel, filterCallback }">
              <Dropdown
                v-model="filterModel.value"
                @change="filterCallback()"
                :options="[
                  { label: 'Activo', value: true },
                  { label: 'Inactivo', value: false },
                ]"
                optionLabel="label"
                placeholder="Seleccionar estado"
                showClear
              />
            </template>
          </Column>

          <Column header="Acciones" style="min-width: 180px">
            <template #body="slotProps">
              <div class="acciones-botones">
                <Button
                  icon="pi pi-eye"
                  severity="info"
                  text
                  rounded
                  v-tooltip.bottom="'Ver detalles'"
                  @click="verDetalles(slotProps.data)"
                />
                <Button
                  icon="pi pi-pencil"
                  severity="warning"
                  text
                  rounded
                  v-tooltip.bottom="'Editar cliente'"
                  @click="editarCliente(slotProps.data)"
                />
                <Button
                  icon="pi pi-trash"
                  severity="danger"
                  text
                  rounded
                  v-tooltip.bottom="'Eliminar cliente'"
                  @click="eliminarCliente(slotProps.data)"
                />
              </div>
            </template>
          </Column>
        </DataTable>
      </template>
    </Card>
    <Dialog
      v-model:visible="mostrarModal"
      :header="clienteSeleccionado?.id ? 'Editar Cliente' : 'Nuevo Cliente'"
      :modal="true"
      :closeOnEscape="false"
      :closeOnBackdropClick="false"
      :closable="false"
      style="width: 450px"
    >
      <ClienteForm
        :cliente="clienteSeleccionado"
        @guardar="guardarCliente($event)"
        @cerrar="cerrarModal"
      />
    </Dialog>
    <Dialog
      v-model:visible="mostrarDetalleModal"
      header="Detalle del Cliente"
      :modal="true"
      :closable="false"
      style="width: 450px"
    >
      <ClienteDetalle
        :cliente="clienteSeleccionado"
        @cerrar="mostrarDetalleModal = false"
      />
    </Dialog>
  </div>
</template>

<script>
import ClienteService from "../services/ClienteService";
import DataTable from "primevue/datatable";
import Column from "primevue/column";
import Card from "primevue/card";
import Tag from "primevue/tag";
import Button from "primevue/button";
import InputText from "primevue/inputtext";
import Dropdown from "primevue/dropdown";
import { FilterMatchMode } from "primevue/api";
import Dialog from "primevue/dialog";
import ClienteForm from "../components/ClienteForm.vue";
import ClienteDetalle from "../components/ClienteDetalle.vue";

export default {
  components: {
    DataTable,
    Column,
    Card,
    Tag,
    Button,
    InputText,
    Dropdown,
    Dialog,
    ClienteForm,
    ClienteDetalle,
  },
  data() {
    return {
      mostrarFiltros: false,
      clientes: [],
      totalClients: 0,
      currentPage: 1,
      pageSize: 10,
      first: 0,
      sortField: null,
      sortOrder: null,
      loading: false,
      filters: {
        nombre: { value: null, matchMode: FilterMatchMode.STARTS_WITH },
        email: { value: null, matchMode: FilterMatchMode.CONTAINS },
        telefono: { value: null, matchMode: FilterMatchMode.CONTAINS },
        activo: { value: null, matchMode: FilterMatchMode.EQUALS },
      },

      mostrarModal: false,
      clienteSeleccionado: null,
      mostrarDetalleModal: false,
    };
  },
  mounted() {
    this.obtenerClientes();
  },
  methods: {
    // Modal: nuevo cliente
    abrirModalNuevo() {
      console.log("Abrir modal para nuevo cliente");
      this.clienteSeleccionado = null;
      this.mostrarModal = true;
      document.body.classList.add("modal-open");
    },

    // Modal: editar cliente
    abrirModalEditar(cliente) {
      console.log("Abrir modal para editar cliente:", cliente);
      this.clienteSeleccionado = { ...cliente };
      this.mostrarModal = true;
    },

    cerrarModal() {
      this.mostrarModal = false;
      document.body.classList.remove("modal-open");
    },

    // Guardar cliente: nuevo o editado
    guardarCliente(clienteActualizado) {
      const datos = {
        nombre: clienteActualizado.nombre,
        email: clienteActualizado.email,
        telefono: clienteActualizado.telefono,
        rolId: clienteActualizado.rolId,
        accedeAlSistema: clienteActualizado.accedeAlSistema,
      };

      console.log("guardarCliente - datos recibidos:", datos);

      if (clienteActualizado.id) {
        console.log("Editar cliente existente");
        ClienteService.actualizarCliente(clienteActualizado.id, datos)
          .then(() => {
            this.obtenerClientes();
            this.cerrarModal();
          })
          .catch((error) => {
            console.error("Error al editar cliente:", error);
          });
      } else {
        console.log("Crear nuevo cliente");
        ClienteService.crearCliente(datos)
          .then(() => {
            this.obtenerClientes();
            this.cerrarModal();
          })
          .catch((error) => {
            console.error("Error al crear cliente:", error);
          });
      }
    },
    // Acciones de UI
    crearCliente() {
      this.abrirModalNuevo();
    },

    verDetalles(cliente) {
      ClienteService.getCliente(cliente.id)
        .then((res) => {
          console.log("Detalle completo recibido:", res.data.usuario);
          this.clienteSeleccionado = res.data.usuario;
          this.mostrarDetalleModal = true;
        })
        .catch((err) => {
          console.error("Error al obtener detalles del cliente:", err);
          alert("No se pudo cargar el detalle del cliente.");
        });
    },
    editarCliente(cliente) {
      this.clienteSeleccionado = { ...cliente };
      this.abrirModalEditar(cliente);
    },

    eliminarCliente(cliente) {
      console.log("Intentando eliminar cliente:", cliente);
      if (
        confirm(`¿Seguro que deseas eliminar al cliente ${cliente.nombre}?`)
      ) {
        ClienteService.eliminarCliente(cliente.id)
          .then(() => {
            console.log("Cliente eliminado correctamente");
            this.obtenerClientes(this.currentPage, this.pageSize);
          })
          .catch((err) => {
            console.error("Error al eliminar cliente:", err);
            alert("Error al eliminar cliente");
          });
      }
    },

    // Carga de clientes con paginación, filtros y ordenamiento
    obtenerClientes(page = 1, pageSize = 10) {
      console.log(
        "Obteniendo clientes: página",
        page,
        "tamaño página",
        pageSize
      );
      this.loading = true;

      const filtrosAplicados = {};
      Object.keys(this.filters).forEach((key) => {
        let val = this.filters[key]?.value;
        if (val !== null && val !== undefined && val !== "") {
          if (typeof val === "object" && val.hasOwnProperty("value")) {
            val = val.value;
          }
          filtrosAplicados[key] = val;
        }
      });

      if (this.sortField && this.sortOrder) {
        filtrosAplicados.ordenarPor = this.sortField;
        filtrosAplicados.ordenDescendente = this.sortOrder === -1;
      }

      ClienteService.getClientes(page, pageSize, filtrosAplicados)
        .then((res) => {
          this.clientes = res.data.clientes;
          console.log(
            "Respuesta del backend - clientes recibidos:",
            this.clientes
          );
          this.totalClients = res.data.pagination.total;
          this.pageSize = pageSize;
          this.currentPage = page;
          this.first = (page - 1) * pageSize;
        })
        .catch((err) => {
          console.error("Error al cargar clientes:", err);
        })
        .finally(() => {
          this.loading = false;
          console.log("Carga de clientes finalizada");
        });
    },

    // Eventos de la tabla
    onPageChange(event) {
      console.log("Evento paginación:", event);
      const newPage = event.page + 1;
      const newPageSize = event.rows;
      this.first = event.first;

      this.obtenerClientes(newPage, newPageSize);
    },

    onSort(event) {
      console.log("Evento orden:", event);
      this.sortField = event.sortField;
      this.sortOrder = event.sortOrder;
      this.obtenerClientes(1, this.pageSize);
    },

    onFilter() {
      console.log("Evento filtro aplicado, filtros actuales:", this.filters);
      this.obtenerClientes(1, this.pageSize);
    },

    aplicarFiltros() {
      this.obtenerClientes(1, this.pageSize);
    },
  },
};
</script>

<style scoped>
/* ===========================
   CONTENEDOR GENERAL
=========================== */
.clientes-container {
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 2rem;
  color: #e0e0e0;
}

/* ===========================
   ENCABEZADO DE TABLA Y BOTONES
=========================== */
.encabezado-acciones {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: -2.5rem;
  margin-top: -1rem;
}

.boton-filtros {
  background-color: transparent !important;
  color: white !important;
  border: none !important;
  box-shadow: none !important;
  padding: 0.3rem 0.6rem !important;
  margin-right: 1rem !important;
}
.boton-filtros:focus,
.boton-filtros:focus-visible,
.boton-filtros:active {
  outline: none !important;
  box-shadow: none !important;
  border-color: transparent !important;
}
:deep(.boton-filtros:focus-visible) {
  outline: none !important;
  box-shadow: 0 0 0 2px rgba(40, 167, 69, 0.4) !important;
  border-color: transparent !important;
}

.boton-nuevo-cliente {
  background-color: #28a745;
  color: white;
  font-weight: normal;
  padding: 0.5rem 1rem;
  border-radius: 6px;
  font-size: 0.7rem;
  width: auto !important;
  height: auto !important;
  min-width: 120px !important;
}
.boton-nuevo-cliente:hover {
  background-color: #218838;
}

/* ===========================
   DATATABLE ESTILOS GENERALES
=========================== */
:deep(.p-datatable) {
  background-color: #121212;
  color: #eee;
  border-radius: 10px;
  border: none;
  box-shadow: 0 3px 8px rgb(0 0 0 / 0.5);
  font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
  font-size: 0.95rem;
}

:deep(.p-datatable-tbody > tr > td),
:deep(.p-datatable-thead > tr > th) {
  padding: 0.4rem 0.75rem !important;
  line-height: 1.2rem;
  border-bottom: 1px solid #333 !important;
  vertical-align: middle;
  text-align: center !important;
}

:deep(.p-datatable-thead > tr > th) {
  background-color: #2a2a2a;
  color: #ffffff;
  font-weight: 600;
  text-align: center !important;
}

:deep(.p-datatable-tbody > tr:hover) {
  background-color: #1e3a1e;
}

:deep(.p-sortable-column .p-column-header-content) {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.4rem;
}

:deep(.p-datatable-thead > tr > th:nth-child(1) .p-column-header-content) {
  gap: 0.1rem !important;
}

:deep(.p-datatable-thead > tr > th:nth-child(4)) /* Estado */ {
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
}

:deep(.p-datatable-thead > tr > th:nth-child(5)) /* Acciones */ {
  text-align: center !important;
  vertical-align: middle !important;
}

:deep(.p-datatable-thead > tr > th:nth-child(5) .p-column-header-content) {
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
  gap: 0.4rem;
}

/* ===========================
   ESTILOS DE FILTROS EN COLUMNAS
=========================== */
:deep(.p-column-filter .p-inputtext) {
  height: 28px !important;
  padding: 0.25rem 0.5rem !important;
  font-size: 0.85rem !important;
  margin: 0 !important;
  border-radius: 4px !important;
}

:deep(.p-column-filter .p-dropdown) {
  height: 28px !important;
  font-size: 0.85rem !important;
}

:deep(.p-column-filter .p-dropdown .p-dropdown-label) {
  line-height: 28px !important;
  padding: 0 0.5rem !important;
}

:deep(.p-column-filter) {
  padding: 0.15rem 0.3rem !important;
  margin: 0 !important;
  min-height: 40px;
}

:deep(.p-column-filter-menu-button) {
  background: transparent !important;
  border: none !important;
  padding: 0 !important;
  width: 14px;
  height: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
}

:deep(.p-column-filter-clear-button) {
  background: transparent !important;
  border: none !important;
  padding: 0 !important;
  width: auto !important;
  height: auto !important;
  min-width: 0 !important;
  min-height: 0 !important;
  display: inline-flex !important;
  align-items: center !important;
  justify-content: center !important;
  box-shadow: none !important;
  outline: none !important;
}

:deep(.p-column-filter-clear-button > svg) {
  width: 16px !important;
  height: 16px !important;
  pointer-events: auto;
  fill: currentColor;
}

:deep(.p-column-filter-menu-button:focus),
:deep(.p-column-filter-menu-button:active) {
  outline: none !important;
  box-shadow: none !important;
}

/* ===========================
   PAGINADOR
=========================== */
:deep(.p-datatable .p-paginator) {
  background-color: transparent;
  border-top: 1px solid #444;
  padding-top: 0.4rem;
  font-size: 0.85rem;
  color: #ccc;
}

:deep(.p-datatable .p-paginator .p-highlight) {
  background-color: #28a745 !important;
  color: white !important;
  border-radius: 4px;
  font-weight: 600;
}

:deep(.p-paginator .p-paginator-page:focus),
:deep(.p-paginator .p-paginator-page:focus-visible),
:deep(.p-paginator .p-paginator-page.p-highlight) {
  outline: none !important;
  box-shadow: none !important;
  border-color: transparent !important;
}

/* ===========================
   BOTONES DE ACCIÓN EN FILA
=========================== */
.acciones-botones {
  display: flex;
  gap: 0.25rem;
  justify-content: center;
  align-items: center;
}

:deep(.p-button) {
  margin: 0 !important;
  padding: 0.25rem 0.35rem !important;
  font-size: 0.85rem !important;
  width: 30px;
  height: 30px;
  min-width: 30px !important;
  border-radius: 6px !important;
  line-height: 1;
}

:deep(.p-button:last-child) {
  margin-right: 0;
}

/* ===========================
   TAGS DE ESTADO (ACTIVO / INACTIVO)
=========================== */
:deep(.p-tag-success) {
  background-color: #27ae60 !important;
  color: #e0f2f1 !important;
  font-weight: 600;
  border-radius: 12px !important;
  padding: 0.2rem 0.6rem !important;
}

:deep(.p-tag-danger) {
  background-color: #c0392b !important;
  color: #fdecea !important;
  font-weight: 600;
  border-radius: 12px !important;
  padding: 0.2rem 0.6rem !important;
}

/* ===========================
   TÍTULOS Y FILTROS AVANZADOS
=========================== */
.titulo-columna {
  font-weight: 600;
  color: white;
}

.columna-nombre-header {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: flex-start;
  gap: 0.3rem;
}

.icono-filtro {
  position: absolute;
  left: -5rem;
  top: 50%;
  transform: translateY(-50%);
  color: white;
  font-size: 1rem;
  cursor: pointer;
  transition: transform 0.2s ease, color 0.2s ease;
  z-index: 10;
}
.icono-filtro:hover {
  transform: scale(1.2);
  color: #28a745;
}
</style>
