import { useEffect, useRef, useState } from 'react';
import { mediaApi } from '../../api/mediaApi';
import type { CreateVehiclePayload, VehicleDetail } from '../../common/types/vehicle.types';
import { getApiErrorMessage } from '../../common/utils/apiError';
import { VehicleImage } from './VehicleImage';
import {
  emptyVehicleFormValues,
  formValuesToPayload,
  todayDateInputValue,
  validateVehicleForm,
  vehicleDetailToFormValues,
  type FormErrors,
  type VehicleFormValues,
} from '../../common/utils/vehicleFormValidation';
import { Button } from './Button';
import { FormField, FormInput, FormTextarea } from './FormField';

interface VehicleFormModalProps {
  isOpen: boolean;
  vehicle?: VehicleDetail | null;
  isSaving: boolean;
  onClose: () => void;
  onSubmit: (payload: CreateVehiclePayload) => Promise<unknown>;
}

export function VehicleFormModal({
  isOpen,
  vehicle,
  isSaving,
  onClose,
  onSubmit,
}: VehicleFormModalProps) {
  const [form, setForm] = useState<VehicleFormValues>(emptyVehicleFormValues);
  const [errors, setErrors] = useState<FormErrors>({});
  const [apiError, setApiError] = useState('');
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [imagePreview, setImagePreview] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const isEdit = !!vehicle;

  useEffect(() => {
    if (!isOpen) return;
    setErrors({});
    setApiError('');
    setImageFile(null);
    setForm(vehicle ? vehicleDetailToFormValues(vehicle) : emptyVehicleFormValues());
    setImagePreview(vehicle?.imageUrl ?? null);
  }, [isOpen, vehicle]);

  useEffect(() => {
    return () => {
      if (imagePreview?.startsWith('blob:')) URL.revokeObjectURL(imagePreview);
    };
  }, [imagePreview]);

  if (!isOpen) return null;

  const setField = <K extends keyof VehicleFormValues>(key: K, value: VehicleFormValues[K]) => {
    setForm((prev) => ({ ...prev, [key]: value }));
    setErrors((prev) => ({ ...prev, [key]: undefined, form: undefined }));
    setApiError('');
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const validation = validateVehicleForm(form);
    if (Object.keys(validation).length > 0) {
      setErrors(validation);
      return;
    }

    try {
      let imageUrl = form.imageUrl?.trim() || undefined;
      if (imageFile) {
        imageUrl = await mediaApi.uploadVehicleImage(imageFile);
      }
      const payload = { ...formValuesToPayload(form), imageUrl };
      await onSubmit(payload);
      onClose();
    } catch (err) {
      setApiError(getApiErrorMessage(err));
    }
  };

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    if (!file.type.startsWith('image/')) {
      setApiError('Please choose an image file (JPEG, PNG, WebP, GIF).');
      return;
    }
    if (file.size > 10 * 1024 * 1024) {
      setApiError('Image must be 10 MB or smaller.');
      return;
    }
    setApiError('');
    setImageFile(file);
    if (imagePreview?.startsWith('blob:')) URL.revokeObjectURL(imagePreview);
    setImagePreview(URL.createObjectURL(file));
  };

  const err = (key: keyof FormErrors) => errors[key];

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal modal-lg vehicle-form-modal" onClick={(e) => e.stopPropagation()}>
        <header className="modal-header">
          <div>
            <h3>{isEdit ? 'Edit Vehicle' : 'Add Vehicle'}</h3>
            <p className="modal-subtitle">All fields marked * are required</p>
          </div>
          <button type="button" className="btn-icon" onClick={onClose} aria-label="Close">
            ×
          </button>
        </header>

        {apiError && <div className="form-banner form-banner--error">{apiError}</div>}

        <form className="vehicle-form" onSubmit={handleSubmit} noValidate>
          <section className="form-section">
            <h4>Vehicle photo</h4>
            <div className="vehicle-image-upload">
              <VehicleImage
                src={imagePreview}
                alt="Vehicle preview"
                size="detail"
              />
              <div className="vehicle-image-upload-actions">
                <input
                  ref={fileInputRef}
                  type="file"
                  accept="image/jpeg,image/png,image/webp,image/gif"
                  className="sr-only"
                  onChange={handleImageChange}
                />
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  onClick={() => fileInputRef.current?.click()}
                >
                  {imagePreview ? 'Change photo' : 'Upload photo'}
                </Button>
                {imagePreview && (
                  <Button
                    type="button"
                    variant="ghost"
                    size="sm"
                    onClick={() => {
                      setImageFile(null);
                      setImagePreview(null);
                      setField('imageUrl', '');
                      if (fileInputRef.current) fileInputRef.current.value = '';
                    }}
                  >
                    Remove
                  </Button>
                )}
              </div>
            </div>
          </section>

          <section className="form-section">
            <h4>Basic Info</h4>
            <div className="form-grid">
              <FormField label="Make" required error={err('make')}>
                <FormInput
                  hasError={!!err('make')}
                  value={form.make}
                  placeholder="e.g. BMW"
                  onChange={(e) => setField('make', e.target.value)}
                />
              </FormField>
              <FormField label="Model" required error={err('model')}>
                <FormInput
                  hasError={!!err('model')}
                  value={form.model}
                  placeholder="e.g. X5"
                  onChange={(e) => setField('model', e.target.value)}
                />
              </FormField>
              <FormField label="Year" required error={err('yearText')}>
                <FormInput
                  hasError={!!err('yearText')}
                  inputMode="numeric"
                  value={form.yearText}
                  placeholder="2024"
                  onChange={(e) => setField('yearText', e.target.value.replace(/\D/g, ''))}
                />
              </FormField>
              <FormField label="VIN" required error={err('vin')} hint="11–17 characters">
                <FormInput
                  hasError={!!err('vin')}
                  value={form.vin}
                  placeholder="Vehicle identification number"
                  onChange={(e) => setField('vin', e.target.value.toUpperCase())}
                />
              </FormField>
              <FormField label="Price (USD)" required error={err('priceText')}>
                <FormInput
                  hasError={!!err('priceText')}
                  inputMode="decimal"
                  className="input-no-spinner"
                  value={form.priceText}
                  placeholder="e.g. 28500"
                  onChange={(e) => setField('priceText', e.target.value.replace(/[^0-9.]/g, ''))}
                />
              </FormField>
              <FormField label="Quantity" required error={err('quantityText')}>
                <FormInput
                  hasError={!!err('quantityText')}
                  inputMode="numeric"
                  className="input-no-spinner"
                  value={form.quantityText}
                  onChange={(e) => setField('quantityText', e.target.value.replace(/\D/g, ''))}
                />
              </FormField>
              <FormField
                label="Date Added"
                required
                error={err('dateAddedToInventory')}
                hint="Cannot select a future date"
                className="span-2"
              >
                <FormInput
                  type="date"
                  hasError={!!err('dateAddedToInventory')}
                  max={todayDateInputValue()}
                  value={form.dateAddedToInventory ?? ''}
                  onChange={(e) => setField('dateAddedToInventory', e.target.value)}
                />
              </FormField>
            </div>
          </section>

          <section className="form-section">
            <h4>Details</h4>
            <div className="form-grid">
              <FormField label="Color">
                <FormInput value={form.color} onChange={(e) => setField('color', e.target.value)} />
              </FormField>
              <FormField label="Exterior">
                <FormInput value={form.exteriorColor} onChange={(e) => setField('exteriorColor', e.target.value)} />
              </FormField>
              <FormField label="Interior">
                <FormInput value={form.interiorColor} onChange={(e) => setField('interiorColor', e.target.value)} />
              </FormField>
              <FormField label="Mileage" error={err('mileageText')}>
                <FormInput
                  hasError={!!err('mileageText')}
                  inputMode="numeric"
                  className="input-no-spinner"
                  value={form.mileageText}
                  onChange={(e) => setField('mileageText', e.target.value.replace(/\D/g, ''))}
                />
              </FormField>
              <FormField label="Fuel Type">
                <FormInput value={form.fuelType} onChange={(e) => setField('fuelType', e.target.value)} />
              </FormField>
              <FormField label="Transmission">
                <FormInput value={form.transmission} onChange={(e) => setField('transmission', e.target.value)} />
              </FormField>
              <FormField label="Engine">
                <FormInput value={form.engine} onChange={(e) => setField('engine', e.target.value)} />
              </FormField>
              <FormField label="Body Type">
                <FormInput value={form.bodyType} onChange={(e) => setField('bodyType', e.target.value)} />
              </FormField>
              <FormField label="Note" className="span-2" hint="Manager notes (optional)">
                <FormTextarea rows={2} value={form.note} onChange={(e) => setField('note', e.target.value)} />
              </FormField>
              <FormField label="Description" required error={err('description')} className="span-2">
                <FormTextarea
                  hasError={!!err('description')}
                  rows={3}
                  value={form.description}
                  placeholder="Describe condition, features, history..."
                  onChange={(e) => setField('description', e.target.value)}
                />
              </FormField>
            </div>
          </section>

          <div className="modal-actions">
            <Button variant="ghost" onClick={onClose} disabled={isSaving}>
              Cancel
            </Button>
            <Button type="submit" variant="primary" disabled={isSaving}>
              {isSaving ? 'Saving...' : isEdit ? 'Update Vehicle' : 'Add Vehicle'}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}
