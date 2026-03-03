import { User } from "@core/types/User";
import { atom } from "jotai";

export type ChatRoom = {
    id: string;
    name: string;
    owner: User;
    description?: string;
    members: User[];
    messages: ChatMessage[];
}

export type ChatMessage = {
  id: string;
  roomId: string;
  sender: User;
  content: string;
  createdAt: string;
};

export const chatRoomsAtom = atom<ChatRoom[]>([]);
chatRoomsAtom.debugLabel = 'Chat Rooms Atom';

export const allchatRoomsAtom = atom<ChatRoom[]>([]);
allchatRoomsAtom.debugLabel = 'All Chat Rooms Atom';