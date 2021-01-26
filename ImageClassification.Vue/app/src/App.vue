<template>
  <v-app>
    <TopBar />
    <v-main> </v-main>
  </v-app>
</template>

<script>
import TopBar from '@/components/layouts/TopBar'

export default {
  name: 'App',

  components: {
    TopBar
  },

  data: () => ({})
}
</script>
