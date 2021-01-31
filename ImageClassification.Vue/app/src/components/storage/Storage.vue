<template>
  <div>
    <v-row>
      <v-col
        v-for="folder in allFolders"
        :key="folder.name"
        class="col-12 col-sm-6 col-md-3 col-lg-2 text-left text-sm-center"
      >
        <Folder :folder="folder" />
      </v-col>
    </v-row>
    <v-btn
      elevation="1"
      small
      color="secondary"
      rounded
      id="add-folder-btn"
      @click="createFolder"
      :disabled="hasNewFolder"
    >
      <v-icon left> mdi-plus </v-icon>
      Add
    </v-btn>
  </div>
</template>

<script>
import Folder from './Folder'
import { mapActions, mapGetters, mapMutations } from 'vuex'

export default {
  components: {
    Folder
  },
  computed: {
    ...mapGetters(['allFolders', 'hasNewFolder'])
  },
  methods: {
    ...mapActions(['fetchStorage']),
    ...mapMutations(['setLoading', 'createNewFolder']),
    createFolder () {
      this.createNewFolder()
    }
  },
  async mounted () {
    this.setLoading(true)
    await this.fetchStorage()
    this.setLoading(false)
  }
}
</script>

<style scoped>
#add-folder-btn {
  position: absolute;
  right: 25px;
  top: 12px;
  z-index: 5;
}
</style>
