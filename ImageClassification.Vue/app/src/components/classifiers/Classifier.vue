<template>
  <v-hover v-slot="{ hover }">
    <div>
      <v-icon :color="hover ? 'success' : 'secondary'" large>
        mdi-square-circle
      </v-icon>
      <div class="ml-2 ml-sm-0 d-sm-block font-weight-light">
        {{ value.name }}
      </div>
    </div>
  </v-hover>
</template>

<script>
export default {
  name: 'Classifier',
  props: {
    value: {
      type: Object,
      required: true
    }
  }
}
</script>

<style scoped>

</style>
