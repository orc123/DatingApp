export type User = {
  id: string;
  displayName: string;
  email: string;
  token: string;
  imageUrl?: string;
  roles: string[];
};

export type LoginCreds = {
  email: string;
  password: string;
};

export type RegisterCreds = {
  displayName: string;
  email: string;
  password: string;
  gender: string;
  dateOfBirth: string;
  city: string;
  country: string;
};

export class MemberParams {
  gender?: string;
  minAge = 18;
  maxAge = 100;
  pageNumber = 1;
  pageSize = 10;
  orderBy = 'lastActive';
}
