export interface User {
  username: string;
  displayName: string;
  token: string;
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

export interface DecodedToken {
  unique_name: string;
  role?: string[];
  projectrole?: string[];
  exp: number;
}
