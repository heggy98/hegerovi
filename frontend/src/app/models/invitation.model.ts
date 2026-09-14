export type InvitationStatus = 'Pending' | 'Confirmed' | 'Declined';

export interface RsvpInfo {
  status: InvitationStatus;
  peopleCount: number;
  menuPreference?: string;
  allergies?: string;
  guestName?: string;
  plusOneAllowed: boolean;
}

export interface RsvpRequest {
  attending: boolean;
  peopleCount: number;
  menuPreference?: string;
  allergies?: string;
}

export interface InvitationOverview {
  id: string;
  token: string;
  status: InvitationStatus;
  peopleCount: number;
  menuPreference?: string;
  allergies?: string;
  respondedAt?: string;
  guestName?: string;
}
