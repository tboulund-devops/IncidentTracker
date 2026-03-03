import React from 'react';
import { Outlet } from 'react-router-dom';
import Header from '../components/Header';

const Layout: React.FC = () => {
  return (
    <>
      <Header />
      <main style={{ padding: '1rem' }}>
        <Outlet />
      </main>
    </>
  );
};

export default Layout;
