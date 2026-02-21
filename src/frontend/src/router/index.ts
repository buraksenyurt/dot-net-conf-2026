import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/Home.vue'
import VehicleList from '../views/VehicleList.vue'
import VehicleForm from '../views/VehicleForm.vue'
import VehicleOptionList from '../views/VehicleOptionList.vue'
import VehicleOptionForm from '../views/VehicleOptionForm.vue'
import AdvisorLogin from '../views/AdvisorLogin.vue'
import AdvisorDashboard from '../views/AdvisorDashboard.vue'
import { useAuth } from '../composables/useAuth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/',                     name: 'home',               component: Home },
    { path: '/vehicles',             name: 'vehicles',           component: VehicleList },
    { path: '/vehicles/new',         name: 'vehicle-new',        component: VehicleForm },
    { path: '/vehicle-options',      name: 'vehicle-options',    component: VehicleOptionList },
    { path: '/vehicle-options/new',  name: 'vehicle-option-new', component: VehicleOptionForm },
    { path: '/login',                name: 'login',              component: AdvisorLogin },
    { path: '/advisor/dashboard',    name: 'advisor-dashboard',  component: AdvisorDashboard, meta: { requiresAuth: true } }
  ]
})

// Navigation guard — protect advisor routes
router.beforeEach((to) => {
  if (to.meta.requiresAuth) {
    const { isLoggedIn } = useAuth()
    if (!isLoggedIn.value) return { name: 'login' }
  }
})

export default router
