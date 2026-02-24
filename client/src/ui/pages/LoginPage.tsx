import React, { useState } from 'react';
import { api } from '../../utils/api';
import { useSetAtom, useAtom } from 'jotai';
import { authAtom, type AuthState } from '../../core/atoms/authAtom';
import { Navigate } from 'react-router-dom';
import { User } from '@core/types/User';
import { RESET } from 'jotai/utils';
import { authApi } from '@core/controllers/authApi';

const LoginPage: React.FC = () => {
  const [auth, setAuth] = useAtom(authAtom);
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);


  const handleSubmit = async (e: React.SubmitEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    await authApi.login(email, password)
     .then((user) => {
        setAuth({ status: "authenticated", user });
      })
      .catch((err: any) => {
        setAuth({ status: "unauthenticated" });
        setError(err.message || 'Login failed');
      })
      .finally(() => {
        setLoading(false);
      });
  };

  if (auth.status === "authenticated") {
    return <Navigate to="/" replace />;
  }
  return (
    <div className="login-page" style={{ maxWidth: 400, margin: 'auto', padding: 32 }}>
      <h2>Login</h2>
      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: 16 }}>
          <label htmlFor="email">Email</label>
          <input
            id="email"
            type="email"
            value={email}
            onChange={e => setEmail(e.target.value)}
            required
            style={{ width: '100%', padding: 8, marginTop: 4 }}
          />
        </div>
        <div style={{ marginBottom: 16 }}>
          <label htmlFor="password">Password</label>
          <input
            id="password"
            type="password"
            value={password}
            onChange={e => setPassword(e.target.value)}
            required
            style={{ width: '100%', padding: 8, marginTop: 4 }}
          />
        </div>
        {error && <div style={{ color: 'red', marginBottom: 16 }}>{error}</div>}
        <button type="submit" disabled={loading} style={{ width: '100%', padding: 10 }}>
          {loading ? 'Logging in...' : 'Login'}
        </button>
      </form>
    </div>
  );
};

export default LoginPage;
