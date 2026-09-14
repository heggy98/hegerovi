export interface Guest {
  id: string;
  name: string;
  email?: string;
  phone?: string;
  group: string;
  plusOneAllowed: boolean;
  note?: string;
}
