import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/Home.vue'
import VehicleList from '../views/VehicleList.vue'
import VehicleForm from '../views/VehicleForm.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/', name: 'home', component: Home },
    { path: '/vehicles', name: 'vehicles', component: VehicleList },
    { path: '/vehicles/new', name: 'vehicle-new', component: VehicleForm }
  ]
})

export default router
