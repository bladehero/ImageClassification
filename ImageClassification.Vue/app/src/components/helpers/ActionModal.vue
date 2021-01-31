<template>
  <v-row justify="center" v-if="isModalOpen">
    <v-dialog v-model="isModalOpen" persistent max-width="300">
      <v-card>
        <v-card-title class="headline"> {{ modalOptions.title || 'Confirm action' }} </v-card-title>
        <v-card-text class="text-center" v-if="!!modalOptions.text" v-html="modalOptions.text"></v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn text @click="execute(modalOptions.cancelAction)">
            {{ modalOptions.cancelText || 'Cancel' }}
          </v-btn>
          <v-btn
            color="error"
            text
            @click="execute(modalOptions.confirmAction)"
          >
            {{ modalOptions.confirmText || 'OK' }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-row>
</template>

<script>
import { mapActions, mapState } from 'vuex'

export default {
  methods: {
    ...mapActions(['closeModal']),
    execute (func) {
      this.closeModal('actionModal')

      if (typeof func === 'function') {
        func()
      }
    }
  },
  computed: {
    ...mapState({
      isModalOpen: state => state.modals.actionModal.isModalOpen,
      modalOptions: state => state.modals.actionModal.options
    })
  }
}
</script>

<style>
</style>
