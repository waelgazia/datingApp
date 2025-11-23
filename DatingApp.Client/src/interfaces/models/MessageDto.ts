export interface MessageDto {
  id: string,
  senderId: string,
  senderDisplayName: string,
  senderImageUrl: string,
  recipientId: string,
  recipientDisplayName: string,
  recipientImageUrl: string,
  content: string,
  readAt?: string,
  sentAt: string,
  currentUserIsSender?: boolean
}