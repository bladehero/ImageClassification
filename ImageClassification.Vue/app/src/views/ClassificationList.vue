<template>
  <div>
    <v-row>
      <v-col
        v-for="item in classificationsByFolder(this.name)"
        :key="item.classification"
        class="col-6 col-sm-4 col-md-2 col-lg-1 text-left text-sm-center"
      >
        <Classification :value="item" />
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
          color="success"
          rounded
          id="upload-btn"
          v-bind="attrs"
          v-on="on"
        >
          <v-icon left> mdi-upload </v-icon>
          Upload
        </v-btn>
      </template>
      <v-card>
        <v-card-title class="text-h6">
          Select an image
        </v-card-title>
        <v-card-text>
          <v-row align="center" class="mx-5">
            <v-text-field class="file-label" label="Label" v-model="modalData.label"></v-text-field>
            <v-file-input label="File" v-model="modalData.files" prepend-icon="" small-chips multiple></v-file-input>
          </v-row>
        </v-card-text>
        <v-card-actions>
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
            @click="saveImage"
          >
            Upload
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script>
import Classification from '@/components/storage/classifications/Classification'
import { mapActions, mapMutations, mapGetters } from 'vuex'

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
    return {
      dialog: false,
      compiledResult: '',
      modalData: {
        files: null,
        label: null
      }
    }
  },
  computed: {
    ...mapGetters(['allFolders', 'classificationsByFolder'])
  },
  methods: {
    ...mapActions(['fetchStorageFolder', 'uploadImage']),
    ...mapMutations(['setLoading']),
    closeModal () {
      this.modalData = {
        files: null,
        label: null
      }
      this.dialog = false
    },
    async saveImage () {
      await this.uploadImage({
        folder: this.name,
        classification: this.modalData.label,
        files: this.modalData.files
      })
      this.closeModal()
      await this.updateClassificationList()
    },
    async updateClassificationList () {
      this.setLoading(true)
      await this.fetchStorageFolder(this.name)
      this.setLoading(false)
    }
  },
  async created () {
    await this.updateClassificationList()
  }
}
</script>

<style scoped>
#upload-btn {
  position: absolute;
  right: 25px;
  top: 12px;
  z-index: 5;
}
</style>
