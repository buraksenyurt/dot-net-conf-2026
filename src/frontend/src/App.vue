<template>
  <div class="min-h-screen bg-gray-50">
    <header class="bg-white shadow">
      <nav class="max-w-7xl mx-auto px-4 py-4 flex justify-between items-center">
        <router-link to="/" class="text-2xl font-bold text-blue-600">Araç Envanter</router-link>
        <div class="space-x-4">
          <router-link to="/" class="text-gray-700 hover:text-blue-600">Ana Sayfa</router-link>
          <router-link to="/vehicles" class="text-gray-700 hover:text-blue-600">Araçlar</router-link>
        </div>
      </nav>
    </header>
    <main class="max-w-7xl mx-auto px-4 py-8">
      <router-view />
    </main>
  </div>
</template>

<script setup lang="ts">
// App root component with router
</script>
