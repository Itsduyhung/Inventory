export interface CreateStaffPayload {
  username: string;
  password: string;
  fullName: string;
  email?: string;
}

export interface StaffUser {
  id: string;
  username: string;
  fullName: string;
  email: string | null;
  role: string;
  createdAt: string;
}
