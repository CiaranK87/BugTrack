export interface User {
  username: string;
  displayName: string;
  token: string;
  globalRole: string;
  projectRoles?: string[];
}

export interface UserFormValues {
  email: string;
  password: string;
  displayName?: string;
  username?: string;
}

export interface UserSearchDto {
  id: string;
  name: string;
  username: string;
}

export interface UserDto {
  id: string;
  username: string;
  displayName: string;
  email: string;
  globalRole: string;
  jobTitle?: string;
  bio?: string;
}

export interface DecodedToken {
  unique_name: string;
  globalrole?: string;
  role?: string[];
  projectrole?: string[];
  exp: number;
}
