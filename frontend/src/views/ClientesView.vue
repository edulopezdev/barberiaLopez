<script>
import ClienteService from "../services/ClienteService";
import DataTable from "primevue/datatable";
import Column from "primevue/column";

export default {
  components: {
    DataTable,
    Column,
  },
  data() {
    return {
      clientes: [],
      totalClients: 0,
      currentPage: 1,
      pageSize: 10,
      first: 0,
    };
  },
  mounted() {
    this.obtenerClientes();
  },
  methods: {
    obtenerClientes(page = 1, pageSize = 10) {
      ClienteService.getClientes(page, pageSize)
        .then((res) => {
          console.log("Datos recibidos:", res.data);

          this.clientes = res.data.clientes;
          this.totalClients = res.data.pagination.total;
          this.pageSize = pageSize;
          this.currentPage = page;
          this.first = (page - 1) * pageSize;
        })
        .catch((err) => {
          console.error("Error al cargar clientes:", err);
          alert("No se pudieron cargar los clientes");
        });
    },
    onPageChange(event) {
      const newPage = event.page + 1; // PrimeVue usa index 0-based
      const newPageSize = event.rows;
      this.first = event.first;

      this.obtenerClientes(newPage, newPageSize);
    },
  },
};
</script>

<template>
    <h2>Listado de Clientes</h2>

    <DataTable
      :value="clientes"
      lazy
      paginator
      :rows="pageSize"
      :first="first"
      :rowsPerPageOptions="[5, 10, 20]"
      :totalRecords="totalClients"
      tableStyle="min-width: 100%"
      class="p-datatable-sm p-datatable-gridlines p-datatable-striped"
      @page="onPageChange"
    >
      <Column field="nombre" header="Nombre" sortable></Column>
      <Column field="email" header="Email" sortable></Column>
      <Column field="telefono" header="Teléfono" sortable></Column>
      <Column field="activo" header="¿Activo?">
        <template #body="slotProps">
          {{ slotProps.data.activo ? "Sí" : "No" }}
        </template>
      </Column>
    </DataTable>
</template>

<style scoped>
</style>
