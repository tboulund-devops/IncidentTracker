import { atom } from 'jotai';
import { User } from '../types/User';
import { atomWithStorage } from 'jotai/utils';

export type AuthState =
  | { status: "loading" }
  | { status: "authenticated"; user: User }
  | { status: "unauthenticated" }

export const authAtom = atom<AuthState>({
  status: "loading"
})
authAtom.debugLabel = 'Authentication State Atom';