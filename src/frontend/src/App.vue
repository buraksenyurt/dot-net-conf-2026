<template>
  <div class="d-flex min-vh-100">
    <!-- Sidebar -->
    <aside class="bg-dark text-white d-flex flex-column flex-shrink-0" style="width: 260px;">
      <div class="p-3 border-bottom border-secondary">
        <router-link to="/" class="d-flex align-items-center text-white text-decoration-none">
          <i class="bi bi-car-front-fill fs-4 me-2 text-primary"></i>
          <span class="fs-5 fw-bold">Araç Envanter</span>
        </router-link>
      </div>
      
      <div class="p-3">
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
        </ul>
      </div>
      
      <div class="mt-auto p-3 border-top border-secondary bg-black bg-opacity-25">
         <div class="d-flex align-items-center">
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
      <main class="flex-grow-1 p-4 overflow-auto">
         <!-- Use container-fluid to maximize width but keep bootstrap grid available -->
         <div class="container-fluid p-0"> 
            <router-view v-slot="{ Component }">
              <transition name="fade" mode="out-in">
                <component :is="Component" />
              </transition>
            </router-view>
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
// App layout with fixed Sidebar and Header
</script>

<style scoped>
.nav-link {
  color: rgba(255, 255, 255, 0.85);
  transition: all 0.2s;
}
.nav-link:hover {
  background-color: rgba(255, 255, 255, 0.1);
  color: white;
  padding-left: 1.25rem;
}
.nav-link.active {
  background-color: var(--bs-primary);
  color: white;
  box-shadow: 0 4px 6px rgba(0,0,0,0.1);
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
