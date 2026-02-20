import { authAtom } from '@core/atoms/authAtom';
import { useAtom } from 'jotai';

export default function Header() {
  const [auth] = useAtom(authAtom);

  return (
    <header style={{ padding: 16, backgroundColor: '#282c34', color: 'white' }}>
      <h1>Incident Tracker</h1>
      {!auth.isAuthenticated && <a href="/login">Login</a>}
    </header>
  );
}