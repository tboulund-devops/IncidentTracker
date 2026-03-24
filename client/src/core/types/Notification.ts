export type NotificationType = 'Poke' | 'NewMessage';

export type Notification = {
    id: string;
    type: NotificationType;
    payload: string;
    isRead: boolean;
    createdAt: string;
    // Display fields resolved on the frontend
    displayTitle: string;
    displayPreview?: string;
};