import type { Vehicle } from '../../common/types/vehicle.types';
import { formatCurrency, formatDate, getAgeBadgeClass } from '../../common/utils/formatters';
import { Button } from './Button';
import { VehicleImage } from './VehicleImage';

export type VehicleTableMode = 'admin' | 'staff';

interface VehicleTableProps {
  vehicles: Vehicle[];
  isLoading: boolean;
  mode: VehicleTableMode;
  onDetail?: (vehicle: Vehicle) => void;
  onEdit?: (vehicle: Vehicle) => void;
  onDisable?: (vehicle: Vehicle) => void;
  onAddQuantity?: (vehicle: Vehicle) => void;
  onSale?: (vehicle: Vehicle) => void;
}

export function VehicleTable({
  vehicles,
  isLoading,
  mode,
  onDetail,
  onEdit,
  onDisable,
  onAddQuantity,
  onSale,
}: VehicleTableProps) {
  if (isLoading) {
    return <div className="table-loading">Loading inventory...</div>;
  }

  if (vehicles.length === 0) {
    return <div className="table-empty">No vehicles match your filters.</div>;
  }

  return (
    <div className="table-wrapper">
      <table className="vehicle-table">
        <thead>
          <tr>
            <th>Photo</th>
            <th>Vehicle</th>
            <th>VIN</th>
            <th>Price</th>
            <th>Qty</th>
            <th>Added</th>
            <th>Age</th>
            <th>Note</th>
            <th />
          </tr>
        </thead>
        <tbody>
          {vehicles.map((vehicle) => (
            <tr
              key={vehicle.id}
              className={
                vehicle.isDisabled
                  ? 'row-disabled'
                  : vehicle.isAgingStock
                    ? 'row-aging'
                    : undefined
              }
            >
              <td>
                <VehicleImage
                  src={vehicle.imageUrl}
                  alt={`${vehicle.year} ${vehicle.make} ${vehicle.model}`}
                  size="thumb"
                />
              </td>
              <td>
                <div className="vehicle-name">
                  {vehicle.year} {vehicle.make} {vehicle.model}
                </div>
                {vehicle.isAgingStock && !vehicle.isDisabled && (
                  <span className="aging-tag">Aging Stock</span>
                )}
                {vehicle.isDisabled && (
                  <span className="aging-tag">Disabled</span>
                )}
                {mode === 'staff' && vehicle.isLowStock && (
                  <span className="aging-tag">Last unit!</span>
                )}
              </td>
              <td className="mono">{vehicle.vin}</td>
              <td>{formatCurrency(vehicle.price)}</td>
              <td>
                <span className={vehicle.quantity === 0 ? 'badge badge-aging' : ''}>
                  {vehicle.quantity}
                </span>
              </td>
              <td>{formatDate(vehicle.dateAddedToInventory)}</td>
              <td>
                <span className={`badge ${getAgeBadgeClass(vehicle.daysInInventory)}`}>
                  {vehicle.daysInInventory} days
                </span>
              </td>
              <td>{vehicle.note ?? <span className="muted">—</span>}</td>
              <td className="actions-cell">
                {mode === 'admin' && (
                  <div className="actions-cell-inner">
                    <Button variant="ghost" size="sm" onClick={() => onDetail?.(vehicle)}>Detail</Button>
                    <Button variant="ghost" size="sm" onClick={() => onEdit?.(vehicle)}>Edit</Button>
                    <Button variant="secondary" size="sm" onClick={() => onAddQuantity?.(vehicle)}>+ Qty</Button>
                    {!vehicle.isDisabled && vehicle.quantity > 0 && (
                      <Button variant="danger" size="sm" onClick={() => onDisable?.(vehicle)}>Disable</Button>
                    )}
                  </div>
                )}
                {mode === 'staff' && (
                  <div className="actions-cell-inner">
                    <Button variant="ghost" size="sm" onClick={() => onDetail?.(vehicle)}>Detail</Button>
                    <Button variant="primary" size="sm" onClick={() => onSale?.(vehicle)}>Sale</Button>
                  </div>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
