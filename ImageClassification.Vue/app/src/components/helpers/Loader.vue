<template>
  <div class="vld-parent">
    <Loading
      :active.sync="getIsLoading"
      :can-cancel="false"
      :is-full-page="true"
    />
  </div>
</template>

<script>
import Loading from 'vue-loading-overlay'
import { mapGetters } from 'vuex'
import 'vue-loading-overlay/dist/vue-loading.css'

export default {
  data () {
    return {
      fullPage: false
    }
  },
  components: {
    Loading
  },
  computed: {
    ...mapGetters(['getIsLoading'])
  }
}
</script>
