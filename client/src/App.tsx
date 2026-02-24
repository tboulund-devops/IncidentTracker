
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import LoginPage from './ui/pages/LoginPage';
import HomePage from './ui/pages/HomePage';
import { useSse } from './utils/useSse';
import Layout from './ui/pages/Layout';
import { useEffect } from 'react';
import { useAtom } from 'jotai';
import { authAtom } from '@core/atoms/authAtom';
import { api } from '@utils/api';
import { User } from '@core/types/User';
import { authApi } from '@core/controllers/authApi';
// import './App.css';


const router = createBrowserRouter([
  {
    path: '/',
    element: <Layout />,
    children: [
      {
        path: '/',
        element: <HomePage />,
      },
    ],
  },
  {
    path: '/login',
    element: <LoginPage />,
  },
]);

function App() {
  const [, setAuth] = useAtom(authAtom)

  useEffect(() => {
    const checkAuth = async () => {
      try {
        const user = await authApi.me();
        setAuth({ status: "authenticated", user: user });
      } catch {
        setAuth({ status: "unauthenticated" })
      }
    }

    checkAuth()
  }, []);
  
  return <RouterProvider router={router} />;
}

export default App;
