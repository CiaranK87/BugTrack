import { makeAutoObservable, reaction } from "mobx";
import { ServerError } from "../models/serverError";

export default class CommonStore {
  error: ServerError | null = null;
  token: string | null = localStorage.getItem("jwt");
  appLoaded = false;
  darkMode: boolean = localStorage.getItem("darkMode") === "true";

  constructor() {
    makeAutoObservable(this);

    reaction(
      () => this.token,
      (token) => {
        if (token) {
          localStorage.setItem("jwt", token);
        } else {
          localStorage.removeItem("jwt");
        }
      }
    );

    reaction(
      () => this.darkMode,
      (darkMode) => {
        localStorage.setItem("darkMode", darkMode.toString());
        if (darkMode) {
          document.body.classList.add("dark-mode");
        } else {
          document.body.classList.remove("dark-mode");
        }
      }
    );
  }

  setServerError(error: ServerError) {
    this.error = error;
  }

  setToken(token: string | null) {
    this.token = token;
  }

  setAppLoaded = () => {
    this.appLoaded = true;
  };

  setDarkMode = (value: boolean) => {
    this.darkMode = value;
  };

  toggleDarkMode = () => {
    this.darkMode = !this.darkMode;
  };
}
