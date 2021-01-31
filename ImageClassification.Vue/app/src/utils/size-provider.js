export default {
  getMargins (el) {
    const computedStyle = window.getComputedStyle(el)
    return {
      marginTop: parseInt(
        computedStyle.getPropertyValue('margin-top')
      ),
      marginRight: parseInt(
        computedStyle.getPropertyValue('margin-right')
      ),
      marginBottom: parseInt(
        computedStyle.getPropertyValue('margin-bottom')
      ),
      marginLeft: parseInt(
        computedStyle.getPropertyValue('margin-left')
      )
    }
  },
  getPaddings (el) {
    const computedStyle = window.getComputedStyle(el)
    return {
      paddingTop: parseInt(
        computedStyle.getPropertyValue('padding-top')
      ),
      paddingRight: parseInt(
        computedStyle.getPropertyValue('padding-right')
      ),
      paddingBottom: parseInt(
        computedStyle.getPropertyValue('padding-bottom')
      ),
      paddingLeft: parseInt(
        computedStyle.getPropertyValue('padding-left')
      )
    }
  },
  getFullHeight (el) {
    const margins = this.getMargins(el)
    const paddings = this.getPaddings(el)

    const elementHeight = el.clientHeight
    const marginHeight = margins.marginTop + margins.marginBottom
    const paddingHeight = paddings.paddingTop + paddings.paddingBottom

    return elementHeight + marginHeight + paddingHeight
  },
  getFullWidth (el) {
    const margins = this.getMargins(el)
    const paddings = this.getPaddings(el)

    const elementWidth = el.clientWidth
    const marginWidth = margins.marginLeft + margins.marginRight
    const paddingWidth = paddings.paddingLeft + paddings.paddingRight

    return elementWidth + marginWidth + paddingWidth
  },
  getFullSize (el) {
    const height = this.getFullHeight(el)
    const width = this.getFullWidth(el)

    return {
      height,
      width
    }
  }
}
