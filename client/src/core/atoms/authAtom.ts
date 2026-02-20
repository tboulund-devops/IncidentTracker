import { atom } from 'jotai';

export type AuthState = {
	email?: string;
	name?: string;
	isAuthenticated: boolean;
};

export const authAtom = atom<AuthState>({
	email: undefined,
	name: undefined,
	isAuthenticated: false,
});

authAtom.debugLabel = 'Authentication State Atom';