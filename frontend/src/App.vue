<template>
  <div class="app-container">
    <SidebarMenu />
    <div class="content">
      <h1>Bienvenido a la Barbería</h1>
      <router-view />
    </div>
  </div>
</template>

<script>
import SidebarMenu from './components/SidebarMenu.vue';

export default {
  name: 'App',
  components: { SidebarMenu },
};
</script>

<style>
:root {
  --sidebar-width: 240px;
}

.app-container {
  display: flex;
}

.content {
  margin-left: var(--sidebar-width);
  flex-grow: 1;
  padding: 20px;
  min-height: 100vh;
  background-color: #f9fafb; /* color suave para contraste */
}
</style>
