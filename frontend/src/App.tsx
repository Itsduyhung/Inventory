import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { AuthProvider } from './common/context/AuthContext';
import { DialogProvider } from './common/context/DialogContext';
import { AppShell } from './components/layout/AppShell';
import { ProtectedRoute } from './components/layout/ProtectedRoute';
import { StaffLayout } from './components/layout/StaffLayout';
import { AdminSalesPage } from './features/admin/pages/AdminSalesPage';
import { InventoryDashboardPage } from './features/inventory/pages/InventoryDashboardPage';
import { AuthPage } from './features/auth/pages/AuthPage';
import { StaffHistoryPage } from './features/staff/pages/StaffHistoryPage';
import { StaffInventoryPage } from './features/staff/pages/StaffInventoryPage';

function App() {
  return (
    <AuthProvider>
      <DialogProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<AuthPage />} />
          <Route
            element={
              <ProtectedRoute role="Admin">
                <AppShell />
              </ProtectedRoute>
            }
          >
            <Route path="/admin" element={<InventoryDashboardPage />} />
            <Route path="/admin/sales" element={<AdminSalesPage />} />
          </Route>
          <Route
            element={
              <ProtectedRoute role="Staff">
                <StaffLayout />
              </ProtectedRoute>
            }
          >
            <Route path="/staff" element={<StaffInventoryPage />} />
            <Route path="/staff/history" element={<StaffHistoryPage />} />
          </Route>
          <Route path="*" element={<Navigate to="/login" replace />} />
        </Routes>
      </BrowserRouter>
      </DialogProvider>
    </AuthProvider>
  );
}

export default App;
