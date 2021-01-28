<template>
    <Loading
      :active.sync="getIsLoading"
      :can-cancel="false"
      :is-full-page="false"
      loader="bars"
      :lock-scroll="true"
    />
</template>

<script>
import Loading from 'vue-loading-overlay'
import { mapGetters } from 'vuex'
import 'vue-loading-overlay/dist/vue-loading.css'

export default {
  components: {
    Loading
  },
  computed: {
    ...mapGetters(['getIsLoading'])
  }
}
</script>
