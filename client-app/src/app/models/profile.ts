import { User } from "./user";

export interface IProfile {
  username: string;
  displayName: string;
  bio?: string;
  email?: string;
  joinDate?: string;
}

export class Profile implements IProfile {
  constructor(user: User) {
    this.username = user.username;
    this.displayName = user.displayName;
  }
  username: string;
  displayName: string;
  bio?: string;
  email?: string;
  joinDate?: string;
}
