<template>
  <v-app>
    <TopBar />
    <v-main>
      <router-view></router-view>
    </v-main>
    <BottomBar />
  </v-app>
</template>

<script>
import TopBar from '@/components/layouts/TopBar'
import BottomBar from '@/components/layouts/BottomBar'

export default {
  name: 'App',

  components: {
    TopBar,
    BottomBar
  },

  data: () => ({})
}
</script>
