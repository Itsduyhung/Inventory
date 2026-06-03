import type { Vehicle } from '../../common/types/vehicle.types';
import { formatCurrency } from '../../common/utils/formatters';
import { Button } from '../ui/Button';
import { VehicleImage } from '../ui/VehicleImage';

interface StaffVehicleCardProps {
  vehicle: Vehicle;
  onDetail: () => void;
  onSale: () => void;
  salePending?: boolean;
}

export function StaffVehicleCard({ vehicle, onDetail, onSale, salePending }: StaffVehicleCardProps) {
  const aging = vehicle.isAgingStock;
  const lowStock = vehicle.isLowStock || vehicle.quantity <= 2;

  return (
    <article className={`staff-vehicle-card${aging ? ' staff-vehicle-card--aging' : ''}`}>
      <VehicleImage
        src={vehicle.imageUrl}
        alt={`${vehicle.year} ${vehicle.make} ${vehicle.model}`}
        size="card"
        className="staff-vehicle-card-photo"
      />
      <div className="staff-vehicle-card-top">
        <div>
          <p className="staff-vehicle-year">{vehicle.year}</p>
          <h3 className="staff-vehicle-title">
            {vehicle.make} {vehicle.model}
          </h3>
          <p className="staff-vehicle-trim">{vehicle.bodyType || vehicle.color || '—'}</p>
        </div>
        <div className="staff-vehicle-badges">
          {aging && <span className="staff-badge staff-badge--warn">Aging</span>}
          {lowStock && <span className="staff-badge staff-badge--qty">Qty {vehicle.quantity}</span>}
        </div>
      </div>

      <dl className="staff-vehicle-meta">
        <div>
          <dt>Price</dt>
          <dd>{formatCurrency(vehicle.price)}</dd>
        </div>
        <div>
          <dt>In stock</dt>
          <dd>{vehicle.quantity}</dd>
        </div>
        <div>
          <dt>Age</dt>
          <dd>{vehicle.daysInInventory}d</dd>
        </div>
        <div>
          <dt>VIN</dt>
          <dd className="mono">{vehicle.vin.slice(-8)}</dd>
        </div>
      </dl>

      <div className="staff-vehicle-actions">
        <Button variant="outline" size="sm" onClick={onDetail}>
          Details
        </Button>
        <Button variant="primary" size="sm" onClick={onSale} disabled={salePending}>
          {salePending ? 'Processing...' : 'Record sale'}
        </Button>
      </div>
    </article>
  );
}
