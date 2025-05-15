import { createApp } from 'vue';
import App from './App.vue';
import router from './router/router.js';
import { library } from '@fortawesome/fontawesome-svg-core';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import {
  faHouse,
  faCalendar,
  faRightToBracket,
  faScissors,
  faUser,
  faBox,
} from '@fortawesome/free-solid-svg-icons';

library.add(faHouse, faCalendar, faRightToBracket, faScissors, faUser, faBox);

const app = createApp(App);

app.use(router);
app.component('font-awesome-icon', FontAwesomeIcon); 
app.mount('#app');
