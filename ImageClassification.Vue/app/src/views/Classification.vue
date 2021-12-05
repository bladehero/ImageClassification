<template>
  <div class="mx-auto container">
    <div class="justify-center text-center">
      <v-btn v-if="(hasTaken || !!image) && selectedClassifier" color="blue-grey lighten-2 mb-5">
        <v-icon>
          mdi-image-filter-center-focus
        </v-icon>
        Recognize
      </v-btn>
      <p class="font-weight-light subtitle-1 text-center  ">Select classifier</p>
      <v-combobox prepend-icon="mdi-square-circle" v-model="selectedClassifier" :items="this.allClassifiers.map(x => x.name)"
                  label="Classifier" clearable/>
      <p class="font-weight-bold subtitle-1 text-center">AND</p>
      <p class="font-weight-light subtitle-1 text-center  ">Select image</p>
      <v-file-input accept="image/*" label="Image" prepend-icon="mdi-camera" flat v-model="image" />
      <p class="font-weight-bold subtitle-1 text-center">OR</p>
      <v-dialog
        v-model="dialog"
        fullscreen
      >
        <template v-slot:activator="{ on, attrs }">
          <div class="text-center">
            <v-btn
              color="secondary"
              dark
              v-bind="attrs"
              v-on="on"
              @click="openModal"
            >
              Take photo
            </v-btn>
          </div>
        </template>
        <v-card id="screenshot">
          <video autoplay></video>
          <img v-show="hasTaken" alt="" src="">
          <canvas style="display:none;"></canvas>
          <v-btn
            outlined
            fab
            small
            color="yellow"
            id="screenshot-button"
            @click="takePhoto"
            v-if="!hasTaken"
          >
            <v-icon large>mdi-circle</v-icon>
          </v-btn>
          <div v-if="hasTaken" id="photo-buttons">
            <v-btn
              fab
              small
              color="success"
              @click="usePhoto"
              v-if="hasTaken"
            >
              <v-icon>mdi-target</v-icon>
            </v-btn>
            <v-btn
              fab
              small
              color="orange darken-1"
              @click="resetPhoto"
            >
              <v-icon>mdi-refresh</v-icon>
            </v-btn>
          </div>
          <v-btn
            outlined
            fab
            x-small
            color="red darken-1"
            id="close-button"
            @click="dialog = false"
          >
            <v-icon small>mdi-close</v-icon>
          </v-btn>
        </v-card>
      </v-dialog>
    </div>
  </div>
</template>

<script>
import { mapActions, mapGetters, mapMutations } from 'vuex'

export default {
  data () {
    return {
      image: null,
      dialog: false,
      hasTaken: false,
      selectedClassifier: null
    }
  },
  computed: {
    ...mapGetters(['allClassifiers'])
  },
  methods: {
    ...mapActions(['fetchClassifiers', 'openModal']),
    ...mapMutations(['setLoading']),
    takePhoto () {
      const img = document.querySelector('#screenshot img')
      const video = document.querySelector('#screenshot video')
      const canvas = document.createElement('canvas')

      canvas.width = video.videoWidth
      canvas.height = video.videoHeight
      canvas.getContext('2d').drawImage(video, 0, 0)
      img.src = canvas.toDataURL('image/webp')
      this.hasTaken = true
    },
    usePhoto () {
      const video = document.querySelector('#screenshot video')
      const canvas = document.createElement('canvas')

      canvas.width = video.videoWidth
      canvas.height = video.videoHeight
      canvas.getContext('2d').drawImage(video, 0, 0)
      canvas.toBlob(blob => {
        this.image = new File([blob], 'image.png')
      })

      this.dialog = false
      this.hasTaken = false
    },
    resetPhoto () {
      this.image = null
      this.hasTaken = false
    },
    openModal () {
      this.dialog = true
      this.$nextTick(() => {
        const video = document.querySelector('#screenshot video')

        navigator.mediaDevices
          .getUserMedia({
            video: {
              width: {
                min: 1280
              },
              height: {
                min: 720
              },
              facingMode: 'environment'
            }
          })
          .then(stream => {
            video.srcObject = stream
          })
      })
    }
  },
  async mounted () {
    this.setLoading(true)
    await this.fetchClassifiers()
    this.setLoading(false)
  }
}
</script>

<style scoped>
.container {
  max-width: 300px;
}

#screenshot {
  position: relative;
  height: 0;
}

#screenshot-button, #photo-buttons {
  position: absolute;
  bottom: 1rem;
  margin-left: auto;
  margin-right: auto;
  left: 0;
  right: 0;
  text-align: center;
}
#photo-buttons button {
  margin-left: 0.25rem;
  margin-right: 0.25rem;
}

#close-button {
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
}

#screenshot video {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
}

#screenshot img {
  height: 100%;
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
}
</style>
