<template>
  <v-app>
    <TopBar />
    <Main />
    <BottomBar />
  </v-app>
</template>

<script>
import TopBar from '@/components/layouts/TopBar'
import Main from '@/components/layouts/Main'
import BottomBar from '@/components/layouts/BottomBar'

export default {
  name: 'App',

  components: {
    TopBar,
    Main,
    BottomBar
  },

  data: () => ({})
}
</script>

<style>
html {
  overflow-y: auto;
}
</style>
