const duration = 650
export default {
  bind: function (el, binding, vNode) {
    // Make sure expression provided is a function
    if (typeof binding.value !== 'function') {
      // Fetch name of component
      const compName = vNode.context.name
      // pass warning to console
      let warn = `[longpress:] provided expression '${binding.expression}' is not a function, but has to be`
      if (compName) { warn += `Found in component '${compName}' ` }

      console.warn(warn)
    }

    // Define variable
    let pressTimer = null

    // Define funtion handlers
    // Create timeout ( run function after duration )
    const start = (e) => {
      if (e.type === 'click' && e.button !== 0) {
        return
      }

      if (pressTimer === null) {
        pressTimer = setTimeout(() => {
          // Run function
          handler(e)
        }, duration)
      }
    }

    // Cancel Timeout
    const cancel = (e) => {
      // Check if timer has a value or not
      if (pressTimer !== null) {
        clearTimeout(pressTimer)
        pressTimer = null
      }
    }
    // Run Function
    const handler = (e) => {
      binding.value(e)
    }

    // Add Event listeners
    el.addEventListener('touchstart', start)
    // Cancel timeouts if this events happen
    el.addEventListener('click', cancel)
    el.addEventListener('touchend', cancel)
    el.addEventListener('touchcancel', cancel)
  }
}