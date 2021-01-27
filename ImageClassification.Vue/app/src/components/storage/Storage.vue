<template>
  <v-row>
    <v-col
      v-for="folder in allFolders"
      :key="folder.name"
      class="col-12 col-sm-6 col-md-3 col-lg-2 text-left text-sm-center"
    >
      <Folder :folder="folder" />
    </v-col>
  </v-row>
</template>

<script>
import Folder from './Folder'
import { mapActions, mapGetters, mapMutations } from 'vuex'

export default {
  components: {
    Folder
  },
  computed: mapGetters(['allFolders']),
  methods: {
    ...mapActions(['fetchStorage']),
    ...mapMutations(['setLoading'])
  },
  async mounted () {
    this.setLoading(true)
    await this.fetchStorage()
    this.setLoading(false)
  }
}
</script>

<style scoped>
</style>
