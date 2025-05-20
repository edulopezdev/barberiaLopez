<template>
  <div>
    <h2>Listado de Clientes</h2>
    <DataTable :value="clientes" tableStyle="min-width: 60rem">
      <Column field="nombre" header="Nombre"></Column>
      <Column field="email" header="Email"></Column>
      <Column field="telefono" header="Teléfono"></Column>
    </DataTable>
  </div>
</template>

<script>
export default {
  data() {
    return {
      clientes: [
        { id: 1, nombre: 'Juan Pérez', email: 'juan@example.com', telefono: '1234567890' },
        { id: 2, nombre: 'María López', email: 'maria@example.com', telefono: '0987654321' }
      ]
    }
  }
}
</script>