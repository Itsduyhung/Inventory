import type { VehicleDetail } from '../../common/types/vehicle.types';
import { formatCurrency, formatDate } from '../../common/utils/formatters';
import { VehicleImage } from './VehicleImage';

interface VehicleDetailModalProps {
  vehicle: VehicleDetail | null;
  isOpen: boolean;
  onClose: () => void;
}

export function VehicleDetailModal({ vehicle, isOpen, onClose }: VehicleDetailModalProps) {
  if (!isOpen || !vehicle) return null;

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal modal-lg" onClick={(e) => e.stopPropagation()}>
        <header className="modal-header">
          <h3>
            {vehicle.year} {vehicle.make} {vehicle.model}
          </h3>
          <button type="button" className="btn-icon" onClick={onClose} aria-label="Close">
            ×
          </button>
        </header>

        <div className="detail-hero-image">
          <VehicleImage
            src={vehicle.imageUrl}
            alt={`${vehicle.year} ${vehicle.make} ${vehicle.model}`}
            size="detail"
          />
        </div>

        <div className="detail-grid">
          <DetailItem label="VIN" value={vehicle.vin} />
          <DetailItem label="Price" value={formatCurrency(vehicle.price)} />
          <DetailItem label="Quantity" value={String(vehicle.quantity)} />
          <DetailItem label="Status" value={vehicle.isDisabled ? 'Disabled' : 'Active'} />
          <DetailItem label="Added to inventory" value={formatDate(vehicle.dateAddedToInventory)} />
          <DetailItem label="Days on lot" value={`${vehicle.daysInInventory} days`} />
          <DetailItem label="Color" value={vehicle.color} />
          <DetailItem label="Exterior" value={vehicle.exteriorColor} />
          <DetailItem label="Interior" value={vehicle.interiorColor} />
          <DetailItem label="Mileage" value={`${vehicle.mileage.toLocaleString()} mi`} />
          <DetailItem label="Fuel" value={vehicle.fuelType} />
          <DetailItem label="Transmission" value={vehicle.transmission} />
          <DetailItem label="Engine" value={vehicle.engine} />
          <DetailItem label="Body type" value={vehicle.bodyType} />
        </div>

        {vehicle.note && (
          <div className="detail-note">
            <strong>Note</strong>
            <p>{vehicle.note}</p>
          </div>
        )}

        <div className="detail-description">
          <strong>Description</strong>
          <p>{vehicle.description}</p>
        </div>
      </div>
    </div>
  );
}

function DetailItem({ label, value }: { label: string; value: string }) {
  return (
    <div className="detail-item">
      <span className="detail-label">{label}</span>
      <span className="detail-value">{value}</span>
    </div>
  );
}
