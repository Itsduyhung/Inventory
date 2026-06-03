import { Navigate } from 'react-router-dom';
import { useAuth } from '../../common/context/AuthContext';
import type { UserRole } from '../../common/types/auth.types';

interface ProtectedRouteProps {
  children: React.ReactNode;
  role?: UserRole;
}

export function ProtectedRoute({ children, role }: ProtectedRouteProps) {
  const { user, isAuthenticated } = useAuth();

  if (!isAuthenticated) return <Navigate to="/login" replace />;
  if (role && user?.role !== role) {
    return <Navigate to={user?.role === 'Admin' ? '/admin' : '/staff'} replace />;
  }

  return <>{children}</>;
}
