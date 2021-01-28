<template>
    <v-bottom-navigation v-model="value" dark grow fixed>
      <v-btn value="storage" @click="$router.replace('/')">
        <span>Storage</span>
        <v-icon>mdi-home</v-icon>
      </v-btn>

      <v-btn value="classification" @click="$router.replace('/classification')">
        <span>Classification</span>
        <v-icon>mdi-cube-scan</v-icon>
      </v-btn>

      <v-btn value="classifiers" @click="$router.replace('/classifiers')">
        <span>Classifiers</span>
        <v-icon>mdi-lightbulb-on-outline</v-icon>
      </v-btn>
    </v-bottom-navigation>
</template>

<script>
import { mapMutations } from 'vuex'

export default {
  data: () => ({ value: 'storage' }),

  methods: {
    ...mapMutations(['setBarSettings'])
  },

  created () {
    const name = this.$router.history?.pending?.name
    if (name) {
      this.value = name
    }
  },

  mounted () {
    const bottomBarHeight = this.$el.clientHeight
    this.setBarSettings({ bottomBarHeight })
  }
}
</script>
