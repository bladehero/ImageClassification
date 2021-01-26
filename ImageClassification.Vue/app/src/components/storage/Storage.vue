<template>
  <v-row>
    <v-col
      v-for="folder in allFolders"
      :key="folder.name"
      class="col-12 col-sm-6 col-md-3 col-lg-2 text-left text-sm-center"
    >
      <Folder :folder="folder"/>
    </v-col>
  </v-row>
</template>

<script>
import Folder from './Folder'
import { mapActions, mapGetters } from 'vuex'

export default {
  components: {
    Folder
  },
  computed: mapGetters(['allFolders']),
  methods: mapActions(['fetchStorage']),
  async mounted () {
    await this.fetchStorage()
  }
}
</script>

<style scoped>
</style>
