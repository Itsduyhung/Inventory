import { useState } from 'react';
import { adminUsersApi } from '../../api/adminUsersApi';
import type { CreateStaffPayload } from '../../common/types/user.types';
import { getApiErrorMessage } from '../../common/utils/apiError';
import { Button } from '../ui/Button';
import { FormField, FormInput } from '../ui/FormField';

interface CreateStaffModalProps {
  isOpen: boolean;
  onClose: () => void;
  onCreated?: () => void;
}

export function CreateStaffModal({ isOpen, onClose, onCreated }: CreateStaffModalProps) {
  const [fullName, setFullName] = useState('');
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);

  if (!isOpen) return null;

  const reset = () => {
    setFullName('');
    setUsername('');
    setEmail('');
    setPassword('');
    setConfirmPassword('');
    setError('');
    setSuccess('');
  };

  const handleClose = () => {
    reset();
    onClose();
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    if (!fullName.trim() || !username.trim() || !password) {
      setError('Full name, username, and password are required.');
      return;
    }
    if (password !== confirmPassword) {
      setError('Passwords do not match.');
      return;
    }

    const payload: CreateStaffPayload = {
      username: username.trim(),
      password,
      fullName: fullName.trim(),
      email: email.trim() || undefined,
    };

    setLoading(true);
    try {
      const created = await adminUsersApi.createStaff(payload);
      setSuccess(`Staff account "${created.username}" created. They can sign in at the login page.`);
      onCreated?.();
    } catch (err) {
      setError(getApiErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="modal-overlay" onClick={handleClose}>
      <div className="modal" onClick={(e) => e.stopPropagation()}>
        <header className="modal-header">
          <div>
            <h3>Create staff account</h3>
            <p className="modal-subtitle">Only admins can register staff. Staff sign in with these credentials.</p>
          </div>
          <button type="button" className="btn-icon" onClick={handleClose} aria-label="Close">
            ×
          </button>
        </header>

        {error && <div className="form-banner form-banner--error">{error}</div>}
        {success && <div className="form-banner form-banner--success">{success}</div>}

        <form onSubmit={handleSubmit} noValidate>
          <FormField label="Full name" required>
            <FormInput
              value={fullName}
              placeholder="Staff display name"
              onChange={(e) => setFullName(e.target.value)}
            />
          </FormField>
          <FormField label="Username" required>
            <FormInput
              value={username}
              placeholder="3–32 characters"
              autoComplete="off"
              onChange={(e) => setUsername(e.target.value)}
            />
          </FormField>
          <FormField label="Email" hint="Required for forgot-password email">
            <FormInput
              type="email"
              value={email}
              placeholder="staff@dealership.com"
              onChange={(e) => setEmail(e.target.value)}
            />
          </FormField>
          <FormField label="Temporary password" required>
            <FormInput
              type="password"
              value={password}
              placeholder="Min. 6 characters"
              autoComplete="new-password"
              onChange={(e) => setPassword(e.target.value)}
            />
          </FormField>
          <FormField label="Confirm password" required>
            <FormInput
              type="password"
              value={confirmPassword}
              autoComplete="new-password"
              onChange={(e) => setConfirmPassword(e.target.value)}
            />
          </FormField>

          <div className="modal-actions">
            <Button type="button" variant="ghost" onClick={handleClose} disabled={loading}>
              {success ? 'Close' : 'Cancel'}
            </Button>
            <Button type="submit" variant="primary" disabled={loading || !!success}>
              {loading ? 'Creating...' : 'Create staff'}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}
