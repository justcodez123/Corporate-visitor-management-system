export interface Visitor {
  id?: number;
  fullName: string;
  email: string;
  phoneNumber: string;
  company?: string;
  hostName: string;
  purpose: string;
  checkInTime?: string; // ISO Date String
  checkOutTime?: string | null; // ISO Date String or null
  badgeNumber?: string;
}

export interface VisitorCheckInDto {
  fullName: string;
  email: string;
  phoneNumber: string;
  company?: string;
  hostName: string;
  purpose: string;
  badgeNumber?: string;
}
