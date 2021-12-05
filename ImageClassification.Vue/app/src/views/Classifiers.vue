<template>
  <div>
    <v-row>
      <v-col
        v-for="classifier in allClassifiers"
        :key="classifier.name"
        class="col-12 col-sm-6 col-md-3 col-lg-2 text-left text-sm-center"
      >
        <Classifier :value="classifier" />
      </v-col>
    </v-row>
    <v-btn
      elevation="1"
      small
      color="yellow"
      rounded
      id="prepare-classifier-btn"
    >
      Prepare
      <v-icon right> mdi-head-sync-outline </v-icon>
    </v-btn>
  </div>
</template>

<script>
import Classifier from '../components/classifiers/Classifier'
import { mapActions, mapGetters, mapMutations } from 'vuex'

export default {
  components: { Classifier },
  computed: {
    ...mapGetters(['allClassifiers'])
  },
  methods: {
    ...mapActions(['fetchClassifiers']),
    ...mapMutations(['setLoading'])
  },
  async mounted () {
    debugger
    this.setLoading(true)
    await this.fetchClassifiers()
    this.setLoading(false)
  }
}
</script>

<style scoped>
#prepare-classifier-btn {
  position: absolute;
  right: 25px;
  top: 12px;
  z-index: 5;
}
</style>
