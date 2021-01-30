<template>
  <v-main class="vld-parent">
    <div class="px-5 py-3">
      <Loader />
      <Breadcrumbs />
      <v-divider class="mb-5"></v-divider>
      <vue-page-transition name="fade">
        <router-view
          class="scrollable"
          :style="{ height: `${viewHeight}vh` }"
        ></router-view>
      </vue-page-transition>
    </div>
  </v-main>
</template>

<script>
import Breadcrumbs from '@/components/helpers/Breadcrumbs'
import Loader from '@/components/helpers/Loader'
import { mapGetters } from 'vuex'

export default {
  computed: {
    ...mapGetters(['getBarSettings']),
    viewHeight () {
      const { topBarHeight, bottomBarHeight } = this.getBarSettings

      const framingHeight = topBarHeight + bottomBarHeight
      const windowHeight = window.screen.height

      const bottomPadding = Math.ceil((windowHeight - framingHeight) * 0.01)

      const mainHeight = windowHeight - framingHeight - bottomPadding

      return Math.floor((mainHeight / windowHeight) * 90)
    }
  },
  components: {
    Breadcrumbs,
    Loader
  }
}
</script>

<style scoped>
.scrollable {
  overflow-y: auto;
  overflow-x: hidden;
}
</style>
