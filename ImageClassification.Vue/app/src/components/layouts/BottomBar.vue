<template>
  <v-bottom-navigation v-model="value" dark grow>
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
export default {
  data: () => ({ value: 'storage' }),

  created () {
    const name = this.$router.history?.pending?.name
    if (name) {
      this.value = name
    }
  }
}
</script>
