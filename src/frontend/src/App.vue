<template>
  <!-- Login sayfası: tam ekran, sidebar yok -->
  <router-view v-if="$route.path === '/login'" />

  <!-- Ana layout: sidebar + header + content -->
  <div v-else class="d-flex min-vh-100">
    <!-- Sidebar -->
    <aside class="bg-dark text-white d-flex flex-column flex-shrink-0" style="width: 260px;">
      <div class="p-3 border-bottom border-secondary">
        <router-link to="/" class="d-flex align-items-center text-white text-decoration-none">
          <i class="bi bi-car-front-fill fs-4 me-2 text-primary"></i>
          <span class="fs-5 fw-bold">Araç Envanter</span>
        </router-link>
      </div>
      
      <div class="p-3 flex-grow-1 overflow-y-auto">
        <ul class="nav nav-pills flex-column mb-auto gap-2">
          <li class="nav-item">
            <router-link to="/" class="nav-link text-white d-flex align-items-center" active-class="active">
              <i class="bi bi-speedometer2 me-2"></i>
              Dashboard
            </router-link>
          </li>
          <li class="nav-item">
            <router-link to="/vehicles" class="nav-link text-white d-flex align-items-center" active-class="active" exact-active-class="active">
              <i class="bi bi-list-ul me-2"></i>
              Araç Listesi
            </router-link>
          </li>
          <li class="nav-item">
            <router-link to="/vehicles/new" class="nav-link text-white d-flex align-items-center" active-class="active">
              <i class="bi bi-plus-circle me-2"></i>
              Yeni Araç Ekle
            </router-link>
          </li>

          <!-- Opsiyonlama bölümü -->
          <li class="nav-item mt-2">
            <div class="text-white-50 px-2 mb-1" style="font-size:0.7rem;text-transform:uppercase;letter-spacing:.05em">Opsiyonlama</div>
          </li>
          <li class="nav-item">
            <router-link to="/vehicle-options" class="nav-link text-white d-flex align-items-center" active-class="active">
              <i class="bi bi-bookmark-star me-2"></i>
              Opsiyonlar
            </router-link>
          </li>
          <li class="nav-item">
            <router-link to="/vehicle-options/new" class="nav-link text-white d-flex align-items-center" active-class="active">
              <i class="bi bi-bookmark-plus me-2"></i>
              Yeni Opsiyon
            </router-link>
          </li>

          <!-- Servis Danışmanı bölümü -->
          <li class="nav-item mt-2">
            <div class="text-white-50 px-2 mb-1" style="font-size:0.7rem;text-transform:uppercase;letter-spacing:.05em">Servis Danışmanı</div>
          </li>
          <li v-if="isLoggedIn" class="nav-item">
            <router-link to="/advisor/dashboard" class="nav-link text-white d-flex align-items-center" active-class="active">
              <i class="bi bi-bar-chart-line me-2"></i>
              Danışman Panosu
            </router-link>
          </li>
          <li v-if="!isLoggedIn" class="nav-item">
            <router-link to="/login" class="nav-link text-white d-flex align-items-center" active-class="active">
              <i class="bi bi-box-arrow-in-right me-2"></i>
              Danışman Girişi
            </router-link>
          </li>
        </ul>
      </div>
      
      <!-- Sidebar Alt: Kullanıcı Bilgisi -->
      <div class="p-3 border-top border-secondary bg-black bg-opacity-25">
        <!-- Danışman oturumu açıksa -->
        <div v-if="isLoggedIn" class="d-flex align-items-center justify-content-between">
          <div class="d-flex align-items-center">
            <div class="bg-success rounded-circle d-flex align-items-center justify-content-center text-white me-2" style="width: 32px; height: 32px;">
              <i class="bi bi-person-fill"></i>
            </div>
            <div>
              <div class="small fw-bold">{{ advisor?.firstName }} {{ advisor?.lastName }}</div>
              <div class="small text-white-50" style="font-size: 0.7rem;">{{ advisor?.department }}</div>
            </div>
          </div>
          <button class="btn btn-sm btn-outline-danger border-0 p-1" title="Çıkış" @click="handleLogout">
            <i class="bi bi-box-arrow-right"></i>
          </button>
        </div>
        <!-- Standart admin bilgisi -->
        <div v-else class="d-flex align-items-center">
          <div class="bg-primary rounded-circle d-flex align-items-center justify-content-center text-white me-2" style="width: 32px; height: 32px;">
            <i class="bi bi-person-fill"></i>
          </div>
          <div>
            <div class="small fw-bold">Admin User</div>
            <div class="small text-white-50" style="font-size: 0.75rem;">Yönetici</div>
          </div>
        </div>
      </div>
    </aside>

    <!-- Main Wrapper -->
    <div class="d-flex flex-column flex-grow-1 bg-light">
      <!-- Header -->
      <header class="bg-white shadow-sm border-bottom py-3 px-4">
        <div class="d-flex justify-content-between align-items-center">
             <h5 class="m-0 text-secondary">
               <span v-if="$route.path === '/'">Dashboard</span>
               <span v-else-if="$route.path === '/vehicles'">Araç Listesi</span>
               <span v-else-if="$route.path === '/vehicles/new'">Yeni Araç Ekle</span>
               <span v-else-if="$route.path === '/vehicle-options'">Opsiyonlar</span>
               <span v-else-if="$route.path === '/vehicle-options/new'">Yeni Opsiyon</span>
               <span v-else-if="$route.path === '/advisor/dashboard'">Danışman Panosu</span>
               <span v-else>Araç Envanter Sistemi</span>
             </h5>
             
             <div class="d-flex align-items-center gap-3">
                <button class="btn btn-light position-relative border-0">
                  <i class="bi bi-bell fs-5 text-secondary"></i>
                  <span class="position-absolute top-25 start-75 translate-middle p-1 bg-danger border border-light rounded-circle">
                    <span class="visually-hidden">New alerts</span>
                  </span>
                </button>
             </div>
        </div>
      </header>

      <!-- Main Content -->
      <main class="flex-grow-1 p-4 overflow-y-scroll">
         <div class="container-fluid p-0"> 
            <router-view></router-view>
         </div>
      </main>

      <!-- Footer -->
      <footer class="bg-white border-top py-3 text-center text-muted mt-auto">
        <div class="container-fluid">
          <small>
             AI Destekli Legacy Modernizasyonu Demo | .NET Conference 2026
          </small>
        </div>
      </footer>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useAuth } from './composables/useAuth'

const router = useRouter()
const { advisor, isLoggedIn, logout } = useAuth()

function handleLogout() {
  logout()
  router.push('/login')
}
</script>
