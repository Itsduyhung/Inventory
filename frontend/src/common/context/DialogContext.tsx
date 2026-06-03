import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useRef,
  useState,
  type ReactNode,
} from 'react';
import { Button } from '../../components/ui/Button';

export type DialogTone = 'default' | 'warning' | 'danger' | 'success' | 'info';

export interface ConfirmOptions {
  title?: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  tone?: DialogTone;
}

export interface AlertOptions {
  title?: string;
  message: string;
  confirmText?: string;
  tone?: DialogTone;
}

interface DialogState {
  kind: 'confirm' | 'alert';
  title: string;
  message: string;
  confirmText: string;
  cancelText?: string;
  tone: DialogTone;
}

interface DialogContextValue {
  confirm: (options: ConfirmOptions) => Promise<boolean>;
  alert: (options: AlertOptions | string) => Promise<void>;
}

const DialogContext = createContext<DialogContextValue | null>(null);

const toneIcon: Record<DialogTone, string> = {
  default: 'ℹ️',
  warning: '⚠️',
  danger: '⛔',
  success: '✅',
  info: 'ℹ️',
};

export function DialogProvider({ children }: { children: ReactNode }) {
  const [dialog, setDialog] = useState<DialogState | null>(null);
  const resolverRef = useRef<((value: boolean) => void) | null>(null);

  const close = useCallback((result: boolean) => {
    resolverRef.current?.(result);
    resolverRef.current = null;
    setDialog(null);
  }, []);

  const confirm = useCallback((options: ConfirmOptions) => {
    return new Promise<boolean>((resolve) => {
      resolverRef.current = resolve;
      setDialog({
        kind: 'confirm',
        title: options.title ?? 'Confirm',
        message: options.message,
        confirmText: options.confirmText ?? 'OK',
        cancelText: options.cancelText ?? 'Cancel',
        tone: options.tone ?? 'default',
      });
    });
  }, []);

  const alert = useCallback((options: AlertOptions | string) => {
    const opts = typeof options === 'string' ? { message: options } : options;
    return new Promise<boolean>((resolve) => {
      resolverRef.current = resolve;
      setDialog({
        kind: 'alert',
        title: opts.title ?? 'Notice',
        message: opts.message,
        confirmText: opts.confirmText ?? 'OK',
        tone: opts.tone ?? 'info',
      });
    }).then(() => undefined);
  }, []);

  const value = useMemo(() => ({ confirm, alert }), [confirm, alert]);

  return (
    <DialogContext.Provider value={value}>
      {children}
      {dialog && (
        <div className="app-dialog-overlay" role="presentation">
          <div
            className={`app-dialog app-dialog--${dialog.tone}`}
            role="dialog"
            aria-modal="true"
            aria-labelledby="app-dialog-title"
          >
            <div className="app-dialog-icon">{toneIcon[dialog.tone]}</div>
            <h2 id="app-dialog-title" className="app-dialog-title">
              {dialog.title}
            </h2>
            <p className="app-dialog-message">{dialog.message}</p>
            <div className="app-dialog-actions">
              {dialog.kind === 'confirm' && (
                <Button variant="ghost" onClick={() => close(false)}>
                  {dialog.cancelText}
                </Button>
              )}
              <Button
                variant={dialog.tone === 'danger' ? 'danger' : 'primary'}
                onClick={() => close(true)}
              >
                {dialog.confirmText}
              </Button>
            </div>
          </div>
        </div>
      )}
    </DialogContext.Provider>
  );
}

export function useDialog() {
  const ctx = useContext(DialogContext);
  if (!ctx) throw new Error('useDialog must be used within DialogProvider');
  return ctx;
}
