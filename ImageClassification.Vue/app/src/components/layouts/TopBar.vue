<template>
  <v-app-bar app color="primary" dark>
    <div class="d-flex align-center">
      <img id="logo" src="@/assets/logo.svg" alt="Logo" />
      <span class="font-weight-light headline">Image Classification</span>
    </div>
    <v-spacer />
    <v-tooltip left>
      <template v-slot:activator="{ on, attrs }">
        <div v-bind="attrs" v-on="on">
          <v-switch class="ml-2 mt-5" color="secondary" v-model="isDark" />
        </div>
      </template>
      <span>Dark mode</span>
    </v-tooltip>
  </v-app-bar>
</template>

<script>
import { mapMutations } from 'vuex'

export default {
  data () {
    return {
      isDark: false
    }
  },

  methods: {
    ...mapMutations(['setBarSettings'])
  },

  watch: {
    isDark (newValue) {
      this.$vuetify.theme.isDark = newValue
    }
  },

  mounted () {
    const topBarHeight = this.$el.clientHeight
    this.setBarSettings({ topBarHeight })
  }
}
</script>

<style scoped>
#logo {
  width: 2rem !important;
  height: 2rem !important;
  margin-right: 0.5rem;
}
</style>
