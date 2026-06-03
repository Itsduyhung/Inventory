import { Link, Outlet, useLocation } from 'react-router-dom';
import { useAuth } from '../../common/context/AuthContext';
import { Button } from '../ui/Button';

export function StaffLayout() {
  const { user, logout } = useAuth();
  const location = useLocation();

  return (
    <div className="staff-app">
      <aside className="staff-sidebar">
        <div className="staff-brand">
          <span className="staff-brand-icon">◇</span>
          <div>
            <p className="staff-brand-eyebrow">Sales workspace</p>
            <strong>Inventory Hub</strong>
          </div>
        </div>

        <nav className="staff-nav">
          <Link
            to="/staff"
            className={`staff-nav-link${location.pathname === '/staff' ? ' staff-nav-link--active' : ''}`}
          >
            <span className="staff-nav-icon">▣</span>
            Inventory
          </Link>
          <Link
            to="/staff/history"
            className={`staff-nav-link${location.pathname.includes('/history') ? ' staff-nav-link--active' : ''}`}
          >
            <span className="staff-nav-icon">◷</span>
            Sales history
          </Link>
        </nav>

        <div className="staff-sidebar-footer">
          <div className="staff-user-card">
            <span className="staff-user-avatar">{user?.fullName?.charAt(0) ?? '?'}</span>
            <div>
              <p className="staff-user-name">{user?.fullName}</p>
              <p className="staff-user-role">Staff</p>
            </div>
          </div>
          <Button variant="outline" size="sm" fullWidth onClick={logout}>
            Log out
          </Button>
        </div>
      </aside>

      <div className="staff-main">
        <Outlet />
      </div>
    </div>
  );
}
