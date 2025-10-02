import { makeAutoObservable, runInAction } from "mobx";
import { User, UserFormValues, UserSearchDto } from "../models/user";
import agent from "../api/agent";
import { store } from "./store";
import { router } from "../router/Routes";
import { Profile } from "../models/profile";

export default class UserStore {
  user: User | null = null;
  profile: Profile | null = null;
  userRegistry = new Map<string, User>();
  loadingProfile = false;
  userSearchResults: UserSearchDto[] = [];
  loadingUsers = false;

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
    this.userRegistry.set(user.username, user);
  });
  
  store.projectStore.clear();
  store.ticketStore.clear();
  
  router.navigate("/dashboard");
  store.modalStore.closeModal();
};


  register = async (creds: UserFormValues) => {
    const user = await agent.Account.register(creds);
    store.commonStore.setToken(user.token);
    runInAction(() => {
      this.user = user;
      this.userRegistry.set(user.username, user);
    });
    router.navigate("/dashboard");
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
        this.userRegistry.set(user.username, user);
      });
    } catch (error) {
      console.log(error);
    }
  };

  searchUsers = async (query: string) => {
  this.loadingUsers = true;
  try {
    const users = await agent.Users.search(query);
    runInAction(() => {
      this.userSearchResults = users;
    });
  } catch (error) {
    console.error(error);
  } finally {
    runInAction(() => {
      this.loadingUsers = false;
    });
  }
};

  
  
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
        
        if (profile.displayName && this.user) {
          this.user.displayName = profile.displayName;
          this.userRegistry.set(this.user.username, this.user);
        }
      });
    } catch (error: any) {
      console.log("Profile update error:", error.response?.data || error.message);
    }
  };

  get projectRoles() {
    if (!this.user?.projectRoles) return {};
    const roles: Record<string, string> = {};
    
    this.user.projectRoles.forEach(claim => {
      const [projectPart, role] = claim.split("=");
      const projectId = projectPart.split(":")[1];
      roles[projectId] = role;
    });
  
  return roles;
  }
  
  getUserById(username: string): User | undefined {
    return this.userRegistry.get(username);
  }

  isCurrentUser = (username: string) => {
    return this.user?.username === username;
  };
}
