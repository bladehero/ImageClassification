<template>
  <v-row>
    <v-col
      v-for="item in classificationsByFolder(this.name)"
      :key="item.classification"
      class="col-6 col-sm-4 col-md-2 col-lg-1 text-left text-sm-center"
    >
      <Classification :item="item" />
    </v-col>
  </v-row>
</template>

<script>
import Classification from '@/components/storage/classifications/Classification'
import { mapActions, mapGetters } from 'vuex'

export default {
  components: {
    Classification
  },
  props: {
    name: {
      type: String,
      required: true
    }
  },
  data () {
    return { compiledResult: '' }
  },
  computed: {
    ...mapGetters(['allFolders', 'classificationsByFolder'])
  },
  methods: {
    ...mapActions(['fetchStorageFolder'])
  },
  async created () {
    await this.fetchStorageFolder(this.name)
  }
}
</script>

<style>
</style>
