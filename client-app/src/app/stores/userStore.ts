import { makeAutoObservable, runInAction } from "mobx";
import { User, UserFormValues, UserSearchDto, UserDto } from "../models/user";
import agent from "../api/agent";
import { store } from "./store";
import { router } from "../router/Routes";
import { Profile } from "../models/profile";
import { logger } from "../utils/logger";

export default class UserStore {
  user: User | null = null;
  profile: Profile | null = null;
  userRegistry = new Map<string, User>();
  loadingProfile = false;
  userSearchResults: UserSearchDto[] = [];
  loadingUsers = false;
  users: UserDto[] = [];
  loadingUserList = false;
  updatingUserRole = false;

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

  adminRegister = async (creds: UserFormValues) => {
    try {
      const user = await agent.Account.adminRegister(creds);
      runInAction(() => {
        this.loadUsers();
      });
      return user;
    } catch (error) {
      logger.error("Failed to create user", error);
      throw error;
    }
  };

  logout = () => {
    runInAction(() => {
      store.commonStore.setToken(null);
      this.user = null;
      this.userRegistry.clear();
      
      store.projectStore.clear();
      store.ticketStore.clear();
    });
    
    router.navigate("/");
  };

  getUser = async () => {
    try {
      const user = await agent.Account.current();
      runInAction(() => {
        this.user = user;
        this.userRegistry.set(user.username, user);
        
      });
    } catch (error: any) {
      logger.error("Failed to get current user", error);
      if (error?.response?.status === 401) {
        runInAction(() => {
          store.commonStore.setToken(null);
          this.user = null;
          this.userRegistry.clear();
        });
      }
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
    logger.error("Failed to search users", error);
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
      logger.error("Failed to load profile", error);
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
      logger.error("Profile update error", error.response?.data || error.message);
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

  get isAdmin() {
    return this.user?.globalRole === "Admin";
  }

  get isProjectManager() {
    return this.user?.globalRole === "ProjectManager";
  }

  get canCreateProjects() {
    return this.isAdmin || this.isProjectManager;
  }

  loadUsers = async () => {
    this.loadingUserList = true;
    try {
      const users = await agent.Users.list();
      runInAction(() => {
        this.users = users;
      });
    } catch (error) {
      logger.error("Failed to load users", error);
      throw error;
    } finally {
      runInAction(() => {
        this.loadingUserList = false;
      });
    }
  };

  updateUserRole = async (userId: string, role: string) => {
    this.updatingUserRole = true;
    try {
      const updatedUser = await agent.Users.updateRole(userId, role);
      runInAction(() => {
        const index = this.users.findIndex(u => u.id === userId);
        if (index !== -1) {
          this.users[index] = updatedUser;
        }
      });
      return updatedUser;
    } catch (error) {
      logger.error("Failed to update user role", error);
      throw error;
    } finally {
      runInAction(() => {
        this.updatingUserRole = false;
      });
    }
  };

  updateUser = async (userId: string, user: Partial<UserDto>) => {
    try {
      const updatedUser = await agent.Users.update(userId, user);
      runInAction(() => {
        const index = this.users.findIndex(u => u.id === userId);
        if (index !== -1) {
          this.users[index] = updatedUser;
        }
      });
      return updatedUser;
    } catch (error) {
      logger.error("Failed to update user", error);
      throw error;
    }
  };

  deleteUser = async (userId: string) => {
    try {
      await agent.Users.delete(userId);
      runInAction(() => {
        this.users = this.users.filter(u => u.id !== userId);
      });
    } catch (error) {
      logger.error("Failed to delete user", error);
      throw error;
    }
  };
  
  getUserById(username: string): User | undefined {
    return this.userRegistry.get(username);
  }

  isCurrentUser = (username: string) => {
    return this.user?.username === username;
  };
}
