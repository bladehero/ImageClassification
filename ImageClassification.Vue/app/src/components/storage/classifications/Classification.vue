<template>
  <v-hover v-slot="{ hover }">
    <div>
      <v-icon :color="hover ? 'primary' : 'accent'" large>
        mdi-tag-text-outline
      </v-icon>
      <div class="ml-2 ml-sm-0 d-sm-block font-weight-light">
        {{ item.classification }}
        <br />
        <span class="caption">{{ item.fileCount }}pcs.</span>
      </div>
    </div>
  </v-hover>
</template>

<script>
export default {
  props: {
    item: {
      type: Object,
      required: true
    }
  }
}
</script>

<style>
</style>
