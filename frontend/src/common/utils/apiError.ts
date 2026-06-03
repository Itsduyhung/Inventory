import axios from 'axios';

export function getApiErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as {
      message?: string;
      title?: string;
      errors?: Record<string, string[]>;
    };

    if (data?.message) return data.message;

    if (data?.errors) {
      const messages = Object.values(data.errors).flat();
      if (messages.length > 0) return messages.join(' ');
    }

    if (error.response?.status === 401) return 'Session expired. Please sign in again.';
    if (error.response?.status === 400) return data?.title ?? 'Invalid data. Please check the form.';
    if (error.response?.status === 404) {
      return 'API not found (404). Restart the backend after updating: cd backend/src/InventoryDashboard.API && dotnet run';
    }
    if (error.response?.status === 500) return data?.message ?? data?.title ?? 'Server error. Please try again.';
    if (!error.response) {
      return 'Cannot reach the API. Start the backend (http://localhost:5272) and try again.';
    }
  }

  if (error instanceof Error && error.message) return error.message;
  return 'Something went wrong. Please try again.';
}
