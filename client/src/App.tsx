
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import LoginPage from './ui/pages/LoginPage';
import HomePage from './ui/pages/HomePage';
import { useSse } from './utils/useSse';
import Layout from './ui/pages/Layout';
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
  return <RouterProvider router={router} />;
}

export default App;
