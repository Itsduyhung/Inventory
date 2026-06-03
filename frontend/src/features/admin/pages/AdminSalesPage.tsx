import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { adminSalesApi } from '../../../api/adminSalesApi';
import { adminUsersApi } from '../../../api/adminUsersApi';
import { Pagination } from '../../../components/ui/Pagination';
import { VehicleDetailModal } from '../../../components/ui/VehicleDetailModal';
import { VehicleImage } from '../../../components/ui/VehicleImage';
import type { VehicleDetail } from '../../../common/types/vehicle.types';
import { getApiErrorMessage } from '../../../common/utils/apiError';
import { formatCurrency, formatDate } from '../../../common/utils/formatters';
import { Button } from '../../../components/ui/Button';

export function AdminSalesPage() {
  const [page, setPage] = useState(1);
  const [staffUserId, setStaffUserId] = useState('');
  const [detail, setDetail] = useState<VehicleDetail | null>(null);

  const { data: staffList } = useQuery({
    queryKey: ['admin-staff-list'],
    queryFn: adminUsersApi.listStaff,
  });

  const { data, isLoading, isError, error, refetch } = useQuery({
    queryKey: ['admin-sales', page, staffUserId],
    queryFn: () => adminSalesApi.getSales({
      page,
      pageSize: 10,
      staffUserId: staffUserId || undefined,
    }),
  });

  return (
    <div className="dashboard">
      <header className="page-header">
        <div>
          <p className="eyebrow">Admin · Sales</p>
          <h1>Staff sales log</h1>
          <p className="subtitle">
            See which staff member sold which vehicle and when.
          </p>
        </div>
        <div className="admin-sales-stat">
          <span className="admin-sales-stat-value">{data?.totalCount ?? '—'}</span>
          <span className="admin-sales-stat-label">total sales</span>
        </div>
      </header>

      <section className="filters-section admin-sales-filters">
        <div className="filter-group">
          <label htmlFor="staff-filter">Filter by staff</label>
          <select
            id="staff-filter"
            className="filter-select"
            value={staffUserId}
            onChange={(e) => { setStaffUserId(e.target.value); setPage(1); }}
          >
            <option value="">All staff</option>
            {staffList?.map((s) => (
              <option key={s.id} value={s.id}>
                {s.fullName} (@{s.username})
              </option>
            ))}
          </select>
        </div>
      </section>

      <section className="inventory-section">
        <div className="section-header">
          <h2>Sales records</h2>
        </div>

        {isError ? (
          <div className="form-banner form-banner--error admin-sales-error">
            <p>{getApiErrorMessage(error)}</p>
            <Button variant="outline" size="sm" onClick={() => refetch()}>
              Retry
            </Button>
          </div>
        ) : isLoading ? (
          <p className="table-loading">Loading sales...</p>
        ) : (
          <>
            <div className="table-wrapper">
              <table className="vehicle-table admin-sales-table">
                <thead>
                  <tr>
                    <th>Photo</th>
                    <th>Staff</th>
                    <th>Vehicle</th>
                    <th>VIN</th>
                    <th>Price</th>
                    <th>Sold at</th>
                    <th>Qty left</th>
                    <th />
                  </tr>
                </thead>
                <tbody>
                  {data?.items.map((sale) => (
                    <tr key={sale.id}>
                      <td>
                        <VehicleImage
                          src={sale.vehicleImageUrl}
                          alt={sale.vehicleTitle}
                          size="thumb"
                        />
                      </td>
                      <td>
                        <div className="admin-sales-staff">
                          <strong>{sale.staffName}</strong>
                          <span className="muted">@{sale.staffUsername}</span>
                        </div>
                      </td>
                      <td><strong>{sale.vehicleTitle}</strong></td>
                      <td className="mono">{sale.vin}</td>
                      <td>{formatCurrency(sale.salePrice)}</td>
                      <td>{formatDate(sale.soldAt)}</td>
                      <td>{sale.remainingQuantityAfterSale}</td>
                      <td>
                        <button
                          type="button"
                          className="btn-text"
                          onClick={async () => {
                            const res = await adminSalesApi.getSaleDetail(sale.id);
                            setDetail(res.vehicle);
                          }}
                        >
                          Vehicle detail
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            {!data?.items.length && (
              <p className="table-empty">
                No sales recorded yet. Staff must use <strong>Record sale</strong> on the Inventory tab,
                or restart the backend to load sample seed sales.
              </p>
            )}

            {data && data.totalPages > 1 && (
              <Pagination
                page={data.page}
                totalPages={data.totalPages}
                onPageChange={setPage}
              />
            )}
          </>
        )}
      </section>

      <VehicleDetailModal vehicle={detail} isOpen={!!detail} onClose={() => setDetail(null)} />
    </div>
  );
}
