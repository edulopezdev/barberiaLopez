import { createRouter, createWebHistory } from 'vue-router';
import PaginaInicio from '../views/PaginaInicio.vue';
import GestionTurnos from '../views/GestionTurnos.vue';
import UserLogin from '../views/UserLogin.vue';

const routes = [
    { path: '/', component: PaginaInicio },
    { path: '/turnos', component: GestionTurnos },
    { path: '/login', component: UserLogin }
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

export default router;