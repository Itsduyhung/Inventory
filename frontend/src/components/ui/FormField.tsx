import type { InputHTMLAttributes, ReactNode, TextareaHTMLAttributes } from 'react';

interface FormFieldProps {
  label: string;
  error?: string;
  hint?: string;
  required?: boolean;
  children: ReactNode;
  className?: string;
}

export function FormField({ label, error, hint, required, children, className = '' }: FormFieldProps) {
  return (
    <label className={`form-field ${error ? 'form-field--error' : ''} ${className}`.trim()}>
      <span className="form-field-label">
        {label}
        {required && <span className="form-required">*</span>}
      </span>
      {children}
      {hint && !error && <span className="form-hint">{hint}</span>}
      {error && <span className="form-field-error">{error}</span>}
    </label>
  );
}

type InputProps = InputHTMLAttributes<HTMLInputElement> & { hasError?: boolean };

export function FormInput({ hasError, className = '', ...props }: InputProps) {
  return (
    <input
      className={`form-input ${hasError ? 'form-input--error' : ''} ${className}`.trim()}
      {...props}
    />
  );
}

type TextAreaProps = TextareaHTMLAttributes<HTMLTextAreaElement> & { hasError?: boolean };

export function FormTextarea({ hasError, className = '', ...props }: TextAreaProps) {
  return (
    <textarea
      className={`form-input form-textarea ${hasError ? 'form-input--error' : ''} ${className}`.trim()}
      {...props}
    />
  );
}
