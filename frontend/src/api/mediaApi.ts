import { apiClient } from './client';

export const mediaApi = {
  uploadVehicleImage: async (file: File): Promise<string> => {
    const form = new FormData();
    form.append('file', file);
    const { data } = await apiClient.post<{ url: string }>('/api/media/upload-image', form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return data.url;
  },
};
