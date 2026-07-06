// src/devAdministrator/idleLogout.js
import { auth } from '../firebase/firebaseManager.js'

// Keep track of the logout timer globally in this module
let logoutTimer

/**
 * Starts an idle timer that will automatically log out the user
 * after 10 minutes of inactivity.
 *
 * @param {object} router - Vue Router instance, used to redirect to login page
 */
export function startIdleTimer(router) {
  // Function to reset the inactivity timer
  const resetTimer = () => {
    // Clear any existing timer so we don't stack multiple timeouts
    clearTimeout(logoutTimer)

    // Start a new timer: if no activity occurs within 10 minutes,
    // sign the user out and redirect them to the login page
    logoutTimer = setTimeout(() => {
      auth.signOut()                // Firebase logout
      router.push('/admin/login')   // Redirect to login page
      alert("You have been logged out due to inactivity.") // Notify user
    }, 10 * 60 * 1000) // 10 minutes in milliseconds
  }

  // Attach event listeners to detect user activity
  // Any mouse movement, key press, or click will reset the timer
  window.addEventListener('mousemove', resetTimer)
  window.addEventListener('keydown', resetTimer)
  window.addEventListener('click', resetTimer)

  // Initialize the timer immediately when the function is called
  resetTimer()
}
