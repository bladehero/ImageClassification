<template>
  <v-main>
    <div class="px-5 py-3">
      <Breadcrumbs />
      <v-divider class="mb-5"></v-divider>
      <vue-page-transition name="fade">
        <router-view />
      </vue-page-transition>
    </div>
  </v-main>
</template>

<script>
import Breadcrumbs from '@/components/helpers/Breadcrumbs'
export default {
  components: {
    Breadcrumbs
  }
}
</script>
