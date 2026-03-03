import { ChatRoom } from "@core/atoms/chatAtoms";
import { api } from "@utils/api";

const endpoint = '/api/chat';

export const chatApi = {
  sendMessage: async (roomId: string, content: string): Promise<void> => {
    return await api(`${endpoint}/messages`, {
      init: {
        method: 'POST',
        body: JSON.stringify({ roomId, content }),
      },
    }).then(() => {
      console.log('Message sent successfully');
    }).catch((err: any) => {
      console.error('Failed to send message:', err);
      throw new Error(err.message || 'Failed to send message');
    });
  },

  getMyRooms: async (): Promise<ChatRoom[]> => {
    return await api(`${endpoint}/my-rooms`).then((value) => {
      const rooms = value as ChatRoom[];
      console.log('Fetched chat rooms:', rooms);
      return rooms;
    }).catch((err: any) => {
      console.error('Failed to fetch chat rooms:', err);
      throw new Error(err.message || 'Failed to fetch chat rooms');
    });
  },

  searchRooms: async (name: string): Promise<ChatRoom[]> => {
    return await api(`${endpoint}/rooms/search?name=${encodeURIComponent(name)}`).then((value) => {
      const rooms = value as ChatRoom[];
      console.log('Fetched search results:', rooms);
      return rooms;
    }).catch((err: any) => {
      console.error('Failed to search chat rooms:', err);
      throw new Error(err.message || 'Failed to search chat rooms');
    });
  }

  // Additional chat-related API methods can be added here
};  