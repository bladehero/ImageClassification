<template>
  <v-main class="vld-parent">
    <div class="px-5 py-3" id="main-framer">
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
    <AlertModal />
    <ActionModal />
  </v-main>
</template>

<script>
import Breadcrumbs from '@/components/helpers/Breadcrumbs'
import Loader from '@/components/helpers/Loader'
import AlertModal from '@/components/helpers/AlertModal'
import ActionModal from '@/components/helpers/ActionModal'
import { mapGetters, mapMutations } from 'vuex'
import sizeProvider from '@/utils/size-provider'

export default {
  components: {
    Breadcrumbs,
    Loader,
    AlertModal,
    ActionModal
  },
  methods: {
    ...mapMutations(['setBarSettings']),
    changeHeight () {
      if (!this._isMounted) {
        return
      }
      const framer = this.$el.querySelector('#main-framer')
      const divider = framer.querySelector('hr.v-divider')

      const mainHeight = sizeProvider.getFullHeight(framer) - framer.clientHeight
      const dividerHeight = sizeProvider.getFullHeight(divider)

      this.setBarSettings({
        mainHeight,
        dividerHeight
      })
    }
  },
  computed: {
    ...mapGetters(['getBarSettings', 'getFullBarHeight']),
    viewHeight () {
      const framingHeight = this.getFullBarHeight
      if (isNaN(framingHeight)) {
        return
      }
      const windowHeight = window.innerHeight

      const bottomPadding = 7

      const mainHeight = windowHeight - framingHeight - bottomPadding

      return Math.floor((mainHeight / windowHeight) * 100)
    }
  },
  mounted () {
    const vm = this
    window.addEventListener('resize', () => vm.changeHeight())
  }
}
</script>

<style scoped>
.scrollable {
  overflow-y: auto;
  overflow-x: hidden;
}
</style>
