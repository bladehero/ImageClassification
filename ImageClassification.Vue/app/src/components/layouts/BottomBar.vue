<template>
  <v-bottom-navigation v-model="value" dark grow>
    <v-btn value="Storage">
      <span>Storage</span>
      <v-icon>mdi-home</v-icon>
    </v-btn>

    <v-btn value="Classification">
      <span>Classification</span>
      <v-icon>mdi-cube-scan</v-icon>
    </v-btn>

    <v-btn value="Classifiers">
      <span>Classifiers</span>
      <v-icon>mdi-lightbulb-on-outline</v-icon>
    </v-btn>
  </v-bottom-navigation>
</template>

<script>
export default {
  data: () => ({ value: 'Storage' })
}
</script>

<style lang="stylus"></style>
