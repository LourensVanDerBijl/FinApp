// Import the functions you need
import { initializeApp } from "firebase/app";
import { getAuth, setPersistence, browserLocalPersistence } from "firebase/auth";

// Your web app's Firebase configuration
const firebaseConfig = {
  apiKey: "AIzaSyDW5zV4ebC-B2VIlEVu2_ROdI-33viYpj0",
  authDomain: "finbineadmin.firebaseapp.com",
  projectId: "finbineadmin",
  storageBucket: "finbineadmin.appspot.com",
  messagingSenderId: "469407609300",
  appId: "1:469407609300:web:88a8e766bd17fe92ec6641"
};

// Initialize Firebase
const app = initializeApp(firebaseConfig);

// Export Auth instance with persistence
export const auth = getAuth(app);
setPersistence(auth, browserLocalPersistence);
