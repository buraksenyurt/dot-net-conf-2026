import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/Home.vue'
import VehicleList from '../views/VehicleList.vue'
import VehicleForm from '../views/VehicleForm.vue'
import VehicleOptionList from '../views/VehicleOptionList.vue'
import VehicleOptionForm from '../views/VehicleOptionForm.vue'
import VehicleOptionSummaryList from '../views/VehicleOptionSummaryList.vue'
import AdvisorLogin from '../views/AdvisorLogin.vue'
import AdvisorDashboard from '../views/AdvisorDashboard.vue'
import { useAuth } from '../composables/useAuth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/login',                      name: 'login',                    component: AdvisorLogin },
    { path: '/',                           name: 'home',                     component: Home,                    meta: { requiresAuth: true } },
    { path: '/vehicles',                   name: 'vehicles',                 component: VehicleList,             meta: { requiresAuth: true } },
    { path: '/vehicles/new',               name: 'vehicle-new',              component: VehicleForm,             meta: { requiresAuth: true } },
    { path: '/vehicle-options',            name: 'vehicle-options',          component: VehicleOptionList,       meta: { requiresAuth: true } },
    { path: '/vehicle-options/new',        name: 'vehicle-option-new',       component: VehicleOptionForm,       meta: { requiresAuth: true } },
    { path: '/vehicle-options/summary',    name: 'vehicle-option-summary',   component: VehicleOptionSummaryList, meta: { requiresAuth: true } },
    { path: '/advisor/dashboard',          name: 'advisor-dashboard',        component: AdvisorDashboard,        meta: { requiresAuth: true } }
  ]
})

// Navigation guard — every route except /login requires an authenticated advisor
router.beforeEach((to) => {
  if (to.meta.requiresAuth) {
    const { isLoggedIn } = useAuth()
    if (!isLoggedIn.value) return { name: 'login' }
  }
})

export default router
