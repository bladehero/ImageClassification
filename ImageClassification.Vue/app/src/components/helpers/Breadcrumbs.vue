<template>
  <v-breadcrumbs class="px-2 py-0" :items="crumbs">
    <template v-slot:divider>
      <v-icon>mdi-chevron-right</v-icon>
    </template>
    <template v-slot:item="{ item }">
      <v-breadcrumbs-item :href="item.to" :disabled="isLastCrumb(item)" @click.prevent="goto(item)">
        <span class="overline">{{ item.text }}</span>
      </v-breadcrumbs-item>
    </template>
  </v-breadcrumbs>
</template>

<script>
export default {
  methods: {
    isLastCrumb (crumb) {
      const _crumbs = this.crumbs
      const last = _crumbs[_crumbs.length - 1]
      return crumb === last
    },
    goto (crumb) {
      this.$router.push(crumb.to)
    }
  },
  computed: {
    crumbs () {
      const _route = this.$route
      const pathArray = _route.path.split('/')
      pathArray.shift()
      const breadcrumbs = pathArray.reduce((breadcrumbArray, path, idx) => {
        breadcrumbArray.push({
          path: path,
          to: breadcrumbArray[idx - 1]
            ? '/' + breadcrumbArray[idx - 1].path + '/' + path
            : '/' + path,
          text: decodeURI(path) || _route.name
        })
        return breadcrumbArray
      }, [])
      return breadcrumbs
    }
  }
}
</script>

<style>
</style>
