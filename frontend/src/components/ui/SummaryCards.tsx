import type { InventorySummary } from '../../common/types/vehicle.types';

interface SummaryCardsProps {
  summary?: InventorySummary;
  isLoading: boolean;
}

export function SummaryCards({ summary, isLoading }: SummaryCardsProps) {
  if (isLoading) {
    return (
      <div className="summary-grid">
        {[1, 2, 3].map((i) => (
          <div key={i} className="summary-card skeleton" />
        ))}
      </div>
    );
  }

  return (
    <div className="summary-grid">
      <article className="summary-card">
        <span className="summary-label">Total Inventory</span>
        <strong className="summary-value">{summary?.totalVehicles ?? 0}</strong>
        <span className="summary-hint">Units in stock (active)</span>
      </article>
      <article className="summary-card summary-card--alert">
        <span className="summary-label">Aging Stock</span>
        <strong className="summary-value">{summary?.agingStockCount ?? 0}</strong>
        <span className="summary-hint">&gt; 90 days on lot</span>
      </article>
      <article className="summary-card summary-card--success">
        <span className="summary-label">With Note</span>
        <strong className="summary-value">{summary?.vehiclesWithNote ?? 0}</strong>
        <span className="summary-hint">Vehicles with manager note</span>
      </article>
    </div>
  );
}
