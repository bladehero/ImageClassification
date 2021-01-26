<template>
  <v-hover v-slot="{ hover }">
    <div @click="openFolder">
      <v-icon :color="hover ? 'primary' : 'accent'" large> mdi-folder </v-icon>
      <span class="ml-2 ml-sm-0 d-sm-block font-weight-light">{{ folder.name }}</span>
    </div>
  </v-hover>
</template>

<script>
export default {
  props: {
    folder: {
      type: Object,
      required: true
    }
  },
  methods: {
    openFolder () {
      this.$router.push({ name: 'classificationList', params: { name: this.folder.name } })
      this.$emit('opened', this.folder)
    }
  }
}
</script>

<style>
</style>
