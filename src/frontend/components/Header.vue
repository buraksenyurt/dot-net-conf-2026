<template>
  <header class="bg-white shadow-sm border-b border-gray-200">
    <nav class="container mx-auto px-4 py-4">
      <div class="flex items-center justify-between">
        <div class="flex items-center space-x-8">
          <NuxtLink to="/" class="text-2xl font-bold text-blue-600">
            🚗 Araç Envanter
          </NuxtLink>
          <div class="hidden md:flex space-x-6">
            <NuxtLink 
              to="/vehicles" 
              class="text-gray-700 hover:text-blue-600 transition-colors"
            >
              Araç Listesi
            </NuxtLink>
            <NuxtLink 
              to="/vehicles/new" 
              class="text-gray-700 hover:text-blue-600 transition-colors"
            >
              Yeni Araç Ekle
            </NuxtLink>
          </div>
        </div>
        <div class="text-sm text-gray-500">
          .NET Conference 2026 Demo
        </div>
      </div>
    </nav>
  </header>
</template>

<script setup lang="ts">
// Header component
</script>
