<template>
  <div>
    <v-row>
      <v-col
        v-for="classifier in this.allClassifiers"
        :key="classifier.name"
        class="col-12 col-sm-6 col-md-3 col-lg-2 text-left text-sm-center"
      >
        <Classifier :value="classifier"/>
      </v-col>
    </v-row>
    <v-dialog
      v-model="dialog"
      persistent
      max-width="290"
    >
      <template v-slot:activator="{ on, attrs }">
        <v-btn
          elevation="1"
          small
          color="orange"
          rounded
          id="prepare-classifier-btn"
          v-bind="attrs"
          v-on="on"
        >
          Prepare
          <v-icon right> mdi-head-sync-outline</v-icon>
        </v-btn>
      </template>
      <v-card>
        <div>
          <v-card-title class="text-h6" v-if="!modalData.isStarted">
            Choose classifier
          </v-card-title>
          <v-card-text v-if="!modalData.isStarted">
            <v-row align="center" class="mx-5">
              <v-combobox v-model="modalData.folder" :items="this.allFolders.map(x => x.name)" label="Folder"/>
              <v-text-field label="Classifier" v-model="modalData.classifier"/>
            </v-row>
          </v-card-text>
          <v-card-actions v-if="!modalData.isStarted">
            <v-spacer></v-spacer>
            <v-btn
              color="red"
              text
              @click="closeModal"
            >
              Cancel
            </v-btn>
            <v-btn
              color="green darken-1"
              text
              @click="startPreparing"
            >
              Start
            </v-btn>
          </v-card-actions>
          <div class="pa-5 text-center" v-if="modalData.isStarted">
            <p>Preparing...</p>
            <v-progress-linear color="orange accent-4" indeterminate rounded height="6"/>
          </div>
        </div>
      </v-card>
    </v-dialog>
  </div>
</template>

<script>
import Classifier from '../components/classifiers/Classifier'
import { mapActions, mapGetters, mapMutations } from 'vuex'

export default {
  components: { Classifier },
  data () {
    return {
      dialog: false,
      modalData: {
        folder: null,
        classifier: null,
        isStarted: false
      }
    }
  },
  computed: {
    ...mapGetters(['allClassifiers', 'allFolders'])
  },
  methods: {
    ...mapActions(['fetchClassifiers', 'fetchStorage', 'prepareClassifier', 'openModal']),
    ...mapMutations(['setLoading']),
    closeModal () {
      this.dialog = false
      this.modalData = {
        folder: null,
        classifier: null,
        isStarted: false
      }
    },
    async startPreparing () {
      const classifier = this.modalData.classifier
      this.modalData.isStarted = true
      const success = await this.prepareClassifier(this.modalData)
      this.closeModal()
      if (success) {
        this.openModal({
          type: 'alertModal',
          opts: {
            text: `Successfully created '<b>${classifier}</b>'!`,
            icon: 'checkbox-marked-circle',
            iconColor: 'success'
          }
        })
      }
      await this.fetchClassifiers()
    }
  },
  async mounted () {
    this.setLoading(true)
    await this.fetchStorage()
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
