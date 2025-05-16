<template>
  <div class="dashboard">
    <SidebarMenu :collapsed="isSidebarCollapsed" @toggle-collapse="handleSidebarToggle" />
    
    <div class="main-content" :class="{ collapsed: isSidebarCollapsed }">
      <TopNavBar />
      
      <div class="page-container">
        <div class="content-wrapper">
          <router-view />
        </div>
      </div>
      
    </div>
  </div>
</template>

<script>
import SidebarMenu from '@/components/SidebarMenu.vue';
import TopNavBar from '@/components/TopNavBar.vue';

export default {
  name: 'DashboardLayout',
  components: {
    SidebarMenu,
    TopNavBar,
  },
  data() {
    return {
      isSidebarCollapsed: false,
    };
  },
  methods: {
    handleSidebarToggle(val) {
      this.isSidebarCollapsed = val;
    },
  },
};
</script>

<style scoped>
.dashboard {
  display: flex;
  width: 100%;
  height: 100vh;
  background-image: url('@/assets/fondo.webp');
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
}

.main-content {
  margin-left: 270px;
  flex: 1;
  display: flex;
  flex-direction: column;
  transition: margin-left 0.3s ease;
}

.main-content.collapsed {
  margin-left: 85px;
}

/* Contenedor principal del contenido (debajo del TopNavBar) */
.page-container {
  flex: 1;
  display: flex;
  justify-content: center;
  padding-top: 90px; /* 50px debajo del TopNavBar */
  padding-bottom: 1rem;
  overflow-y: auto;
}

/* Contenedor blanco con bordes redondeados y sombra sutil */
.content-wrapper {
  background-color: white;
  color: #2e2e2e; /* gris oscuro */
  padding: 2rem;
  border-radius: 12px;
  width: 90%;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
  transition: box-shadow 0.3s ease;
}

h1, h2, h3 {
  color: #222;
  font-weight: 600;
  margin-bottom: 1rem;
}

.section {
  background-color: #f5f5f5;
  padding: 1rem;
  border-radius: 8px;
}

.content-wrapper:hover {
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
}
</style>
