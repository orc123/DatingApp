export type Message = {
  id: string;
  content: string;
  dateRead?: string;
  messageSent: string;
  senderDeteled: boolean;
  recipientDeleted: boolean;
  senderId: string;
  senderDisplayName: string;
  senderImageUrl: string;
  recipientId: string;
  recipientDisplayName: string;
  recipientImageUrl: string;
  currentUserSender?: boolean;
};
