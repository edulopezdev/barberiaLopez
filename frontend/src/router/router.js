import { createRouter, createWebHistory } from 'vue-router';

// Layout y vistas
import DashboardLayout from '@/layouts/DashboardLayout.vue';
import PaginaInicio from '@/views/PaginaInicio.vue';        // será el home del dashboard
import GestionTurnos from '@/views/GestionTurnos.vue';
import GestionClientes from '@/views/GestionClientes.vue';        // agrégala si no la tienes aún
import UserLogin from '@/views/UserLogin.vue';

const routes = [
  {
    path: '/',
    component: DashboardLayout,
    children: [
      { path: '', component: PaginaInicio },        // Muestra PaginaInicio.vue dentro del dashboard
      { path: 'turnos', component: GestionTurnos },
      { path: 'clientes', component: GestionClientes }, // opcional
    ],
  },
  {
    path: '/login',
    component: UserLogin, // Ruta pública, fuera del layout
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
