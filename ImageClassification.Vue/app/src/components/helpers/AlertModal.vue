<template>
  <v-row justify="center" v-if="isModalOpen">
    <v-dialog :value="isModalOpen" max-width="275">
      <v-card>
        <v-card-title class="headline">
          {{ modalOptions.title }}
        </v-card-title>
        <v-layout class="d-flex justify-center mb-2">
          <v-icon class="huge-icon" :color="modalOptions.iconColor || 'info'">
            mdi-{{ modalOptions.icon || "information" }}
          </v-icon>
        </v-layout>
        <v-card-text
          class="text-center body-1"
          v-if="!!modalOptions.text"
          v-html="modalOptions.text || 'Successfully done!'"
        >
        </v-card-text>
      </v-card>
    </v-dialog>
  </v-row>
</template>

<script>
import { mapActions, mapState } from 'vuex'

export default {
  methods: {
    ...mapActions(['closeModal'])
  },
  computed: {
    ...mapState({
      isModalOpen: (state) => state.modals.alertModal.isModalOpen,
      modalOptions: (state) => state.modals.alertModal.options
    })
  },
  watch: {
    isModalOpen (newValue) {
      if (newValue) {
        const vm = this
        setTimeout(() => {
          vm.closeModal('alertModal')
        }, vm.modalOptions.duration || 2000)
      }
    }
  }
}
</script>

<style scoped>
.huge-icon {
  font-size: 4rem;
}
</style>
