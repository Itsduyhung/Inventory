import { useEffect, useState } from 'react';
import { Navigate, useSearchParams } from 'react-router-dom';
import { useAuth } from '../../../common/context/AuthContext';
import { getApiErrorMessage } from '../../../common/utils/apiError';
import { Button } from '../../../components/ui/Button';
import { FormField, FormInput } from '../../../components/ui/FormField';

type AuthMode = 'signin' | 'forgot' | 'reset';

export function AuthPage() {
  const { login, forgotPassword, resetPassword, isAuthenticated, user } = useAuth();
  const [searchParams] = useSearchParams();
  const [mode, setMode] = useState<AuthMode>('signin');

  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [resetToken, setResetToken] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');

  const [error, setError] = useState('');
  const [info, setInfo] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const token = searchParams.get('reset');
    if (token) {
      setResetToken(token);
      setMode('reset');
      setInfo('Enter your new password below.');
    }
  }, [searchParams]);

  if (isAuthenticated) {
    return <Navigate to={user?.role === 'Admin' ? '/admin' : '/staff'} replace />;
  }

  const switchMode = (next: AuthMode) => {
    setMode(next);
    setError('');
    setInfo('');
  };

  const handleSignIn = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    if (!username.trim() || !password) {
      setError('Username and password are required.');
      return;
    }
    setLoading(true);
    try {
      await login({ username: username.trim(), password });
    } catch {
      setError('Invalid username or password.');
    } finally {
      setLoading(false);
    }
  };

  const handleForgot = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setInfo('');
    if (!username.trim()) {
      setError('Enter your username or email.');
      return;
    }
    setLoading(true);
    try {
      const res = await forgotPassword({ usernameOrEmail: username.trim() });
      setInfo(res.message);
      if (res.resetToken) {
        setResetToken(res.resetToken);
        setMode('reset');
        setInfo(`${res.message} Use the reset token below (demo — no email).`);
      }
    } catch {
      setError('Could not process request.');
    } finally {
      setLoading(false);
    }
  };

  const handleReset = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setInfo('');
    if (!resetToken.trim() || !newPassword) {
      setError('Reset token and new password are required.');
      return;
    }
    if (newPassword !== confirmPassword) {
      setError('Passwords do not match.');
      return;
    }
    setLoading(true);
    try {
      const msg = await resetPassword({ token: resetToken.trim(), newPassword });
      setInfo(msg);
      setMode('signin');
      setPassword('');
      setNewPassword('');
      setConfirmPassword('');
      setResetToken('');
    } catch (err: unknown) {
      setError(getApiErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  const tabs: { id: AuthMode; label: string }[] = [
    { id: 'signin', label: 'Sign In' },
    { id: 'forgot', label: 'Forgot password' },
  ];

  return (
    <div className="login-page">
      <div className="login-bg-shape login-bg-shape--1" />
      <div className="login-bg-shape login-bg-shape--2" />
      <div className="login-bg-shape login-bg-shape--3" />

      <div className="login-layout">
        <aside className="login-hero">
          <p className="login-hero-eyebrow">Supply · Dealership Inventory</p>
          <h1 className="login-hero-title">Intelligent Inventory Dashboard</h1>
          <p className="login-hero-desc">
            Staff sign in and reset passwords here. New staff accounts are created by an administrator.
          </p>
          <ul className="login-hero-features">
            <li>Sign in for admin &amp; staff</li>
            <li>Forgot password via email</li>
            <li>Staff accounts created by admin only</li>
          </ul>
        </aside>

        <div className="login-card">
          {mode !== 'reset' && (
            <nav className="auth-tabs" aria-label="Authentication">
              {tabs.map((tab) => (
                <button
                  key={tab.id}
                  type="button"
                  className={`auth-tab${mode === tab.id ? ' auth-tab--active' : ''}`}
                  onClick={() => switchMode(tab.id)}
                >
                  {tab.label}
                </button>
              ))}
            </nav>
          )}

          {mode === 'signin' && (
            <form onSubmit={handleSignIn} noValidate>
              <h2 className="login-card-title">Sign in</h2>
              <p className="login-card-subtitle">Use credentials provided by your administrator</p>

              <FormField label="Username" error={error && !password ? error : undefined}>
                <FormInput
                  value={username}
                  placeholder="Enter username"
                  autoComplete="username"
                  autoFocus
                  onChange={(e) => { setUsername(e.target.value); setError(''); }}
                />
              </FormField>

              <FormField label="Password" error={password ? error : undefined}>
                <FormInput
                  type="password"
                  value={password}
                  placeholder="Enter password"
                  autoComplete="current-password"
                  onChange={(e) => { setPassword(e.target.value); setError(''); }}
                />
              </FormField>

              <button
                type="button"
                className="auth-link"
                onClick={() => switchMode('forgot')}
              >
                Forgot password?
              </button>

              <Button type="submit" variant="primary" fullWidth disabled={loading}>
                {loading ? 'Signing in...' : 'Sign In'}
              </Button>

              <div className="login-hint">
                <div className="login-hint-row">
                  <span className="login-hint-label">Admin</span>
                  <span className="login-hint-value">admin / admin123</span>
                </div>
                <div className="login-hint-row">
                  <span className="login-hint-label">Staff</span>
                  <span className="login-hint-value">staff / staff123</span>
                </div>
              </div>
            </form>
          )}

          {mode === 'forgot' && (
            <form onSubmit={handleForgot} noValidate>
              <h2 className="login-card-title">Forgot password</h2>
              <p className="login-card-subtitle">We will email a reset link if the account exists</p>

              {info && <p className="auth-info">{info}</p>}
              {error && <div className="form-banner form-banner--error">{error}</div>}

              <FormField label="Username or email">
                <FormInput
                  value={username}
                  placeholder="username or email"
                  onChange={(e) => { setUsername(e.target.value); setError(''); }}
                />
              </FormField>

              <Button type="submit" variant="primary" fullWidth disabled={loading}>
                {loading ? 'Sending...' : 'Send reset link'}
              </Button>

              <button type="button" className="auth-link" onClick={() => switchMode('signin')}>
                Back to sign in
              </button>
            </form>
          )}

          {mode === 'reset' && (
            <form onSubmit={handleReset} noValidate>
              <h2 className="login-card-title">Reset password</h2>
              <p className="login-card-subtitle">Paste your reset token and choose a new password</p>

              {info && <p className="auth-info">{info}</p>}
              {error && <div className="form-banner form-banner--error">{error}</div>}

              <FormField label="Reset token">
                <FormInput
                  value={resetToken}
                  placeholder="Paste token from email"
                  onChange={(e) => { setResetToken(e.target.value); setError(''); }}
                />
              </FormField>

              <FormField label="New password">
                <FormInput
                  type="password"
                  value={newPassword}
                  placeholder="Min. 6 characters"
                  autoComplete="new-password"
                  onChange={(e) => { setNewPassword(e.target.value); setError(''); }}
                />
              </FormField>

              <FormField label="Confirm password">
                <FormInput
                  type="password"
                  value={confirmPassword}
                  placeholder="Repeat password"
                  onChange={(e) => { setConfirmPassword(e.target.value); setError(''); }}
                />
              </FormField>

              <Button type="submit" variant="primary" fullWidth disabled={loading}>
                {loading ? 'Saving...' : 'Reset password'}
              </Button>

              <button type="button" className="auth-link" onClick={() => switchMode('signin')}>
                Back to sign in
              </button>
            </form>
          )}
        </div>
      </div>
    </div>
  );
}
