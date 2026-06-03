import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { staffApi } from '../../../api/staffApi';
import { Pagination } from '../../../components/ui/Pagination';
import { VehicleDetailModal } from '../../../components/ui/VehicleDetailModal';
import type { VehicleDetail } from '../../../common/types/vehicle.types';
import { formatCurrency, formatDate } from '../../../common/utils/formatters';

export function StaffHistoryPage() {
  const [page, setPage] = useState(1);
  const [detail, setDetail] = useState<VehicleDetail | null>(null);

  const { data, isLoading } = useQuery({
    queryKey: ['staff-history', page],
    queryFn: () => staffApi.getHistory(page, 10),
  });

  return (
    <div className="staff-page">
      <header className="staff-page-header">
        <div>
          <p className="staff-page-eyebrow">Your activity</p>
          <h1>Sales history</h1>
          <p className="staff-page-desc">
            Every sale you recorded — tap a card for full vehicle details.
          </p>
        </div>
        <div className="staff-stat-pill">
          <span className="staff-stat-value">{data?.totalCount ?? '—'}</span>
          <span className="staff-stat-label">sales</span>
        </div>
      </header>

      {isLoading ? (
        <p className="staff-loading">Loading history...</p>
      ) : (
        <>
          <div className="staff-history-list">
            {data?.items.map((s) => (
              <article key={s.id} className="staff-history-card">
                <div className="staff-history-card-date">
                  <span className="staff-history-day">{formatDate(s.soldAt)}</span>
                </div>
                <div className="staff-history-card-body">
                  <h3>{s.vehicleTitle}</h3>
                  <p className="staff-history-vin mono">{s.vin}</p>
                  <div className="staff-history-metrics">
                    <span><strong>{formatCurrency(s.salePrice)}</strong></span>
                    <span>Qty sold: {s.quantitySold}</span>
                    <span>Remaining: {s.remainingQuantityAfterSale}</span>
                  </div>
                </div>
                <button
                  type="button"
                  className="staff-history-detail-btn"
                  onClick={async () => {
                    const sale = await staffApi.getHistoryDetail(s.id);
                    setDetail(sale.vehicle);
                  }}
                >
                  View vehicle
                </button>
              </article>
            ))}
          </div>

          {!data?.items.length && (
            <p className="staff-empty">No sales yet — record your first sale from Inventory.</p>
          )}

          {data && data.totalPages > 1 && (
            <Pagination page={data.page} totalPages={data.totalPages} onPageChange={setPage} />
          )}
        </>
      )}

      <VehicleDetailModal vehicle={detail} isOpen={!!detail} onClose={() => setDetail(null)} />
    </div>
  );
}
