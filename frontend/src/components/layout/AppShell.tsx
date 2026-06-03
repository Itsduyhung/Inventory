import { useState } from 'react';
import { Link, Outlet, useLocation } from 'react-router-dom';
import { useAuth } from '../../common/context/AuthContext';
import { CreateStaffModal } from '../admin/CreateStaffModal';
import { Button } from '../ui/Button';

export function AppShell() {
  const { user, logout } = useAuth();
  const location = useLocation();
  const [staffModalOpen, setStaffModalOpen] = useState(false);

  return (
    <div className="app-layout app-layout--admin">
      <header className="app-header app-header--admin">
        <div className="app-header-left">
          <span className="app-header-title">Admin Dashboard</span>
          <nav className="admin-nav">
            <Link
              to="/admin"
              className={location.pathname === '/admin' ? 'active' : ''}
            >
              Inventory
            </Link>
            <Link
              to="/admin/sales"
              className={location.pathname === '/admin/sales' ? 'active' : ''}
            >
              Staff sales
            </Link>
          </nav>
        </div>        <div className="app-header-right">
          <Button variant="secondary" size="sm" onClick={() => setStaffModalOpen(true)}>
            + Staff account
          </Button>
          <span className="user-badge">
            <span className="user-name">{user?.fullName}</span>
            <span className="user-role">{user?.role}</span>
          </span>
          <Button variant="outline" size="sm" onClick={logout}>
            Logout
          </Button>
        </div>
      </header>

      <main className="main-content">
        <Outlet />
      </main>

      <CreateStaffModal
        isOpen={staffModalOpen}
        onClose={() => setStaffModalOpen(false)}
      />
    </div>
  );
}
