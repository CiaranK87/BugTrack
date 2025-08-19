import { makeAutoObservable, runInAction } from "mobx";
import { User, UserFormValues } from "../models/user";
import agent from "../api/agent";
import { store } from "./store";
import { router } from "../router/Routes";
import { Profile } from "../models/profile";

export default class UserStore {
  user: User | null = null;
  profile: Profile | null = null;
  userRegistry = new Map<string, User>(); // Key: username
  loadingProfile = false;

  constructor() {
    makeAutoObservable(this);
  }

  get isLoggedIn() {
    return !!this.user;
  }

  login = async (creds: UserFormValues) => {
    const user = await agent.Account.login(creds);
    store.commonStore.setToken(user.token);
    runInAction(() => {
      this.user = user;
      this.userRegistry.set(user.username, user); // Add to registry on login
    });
    router.navigate("/dashboard");
    store.modalStore.closeModal();
  };

  register = async (creds: UserFormValues) => {
    const user = await agent.Account.register(creds);
    store.commonStore.setToken(user.token);
    runInAction(() => {
      this.user = user;
      this.userRegistry.set(user.username, user); // Add to registry
    });
    router.navigate("/projects");
    store.modalStore.closeModal();
  };

  logout = () => {
    store.commonStore.setToken(null);
    this.user = null;
    this.userRegistry.clear();
    router.navigate("/");
  };

  getUser = async () => {
    try {
      const user = await agent.Account.current();
      runInAction(() => {
        this.user = user;
        this.userRegistry.set(user.username, user); // Ensure current user is in registry
      });
    } catch (error) {
      console.log(error);
    }
  };

  // ✅ Get user directly from Map by username
  getUserById(username: string): User | undefined {
    return this.userRegistry.get(username);
  }

  // 🔴 Remove or keep only if you add `id` later
  // We're removing the old `getUserMap` that assumed `user.id` exists
  // It's redundant — `userRegistry` is already the map

  loadProfile = async (username: string) => {
    this.loadingProfile = true;
    try {
      const profile = await agent.Profiles.get(username);
      runInAction(() => {
        this.profile = profile;
        this.loadingProfile = false;
      });
    } catch (error) {
      console.log(error);
      runInAction(() => {
        this.loadingProfile = false;
      });
    }
  };

  updateProfile = async (profile: Partial<Profile>) => {
    try {
      await agent.Profiles.updateProfile(profile);
      runInAction(() => {
        if (this.profile) {
          this.profile.displayName = profile.displayName || this.profile.displayName;
          this.profile.bio = profile.bio || this.profile.bio;
        }
        // If user updated displayName, reflect in User object
        if (profile.displayName && this.user) {
          this.user.displayName = profile.displayName;
          this.userRegistry.set(this.user.username, this.user); // Update registry
        }
      });
    } catch (error: any) {
      console.log("Profile update error:", error.response?.data || error.message);
    }
  };

  isCurrentUser = (username: string) => {
    return this.user?.username === username;
  };
}
