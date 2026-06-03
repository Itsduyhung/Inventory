import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useMemo, useState } from 'react';
import { staffApi } from '../../../api/staffApi';
import { useDialog } from '../../../common/context/DialogContext';
import { StaffVehicleCard } from '../../../components/staff/StaffVehicleCard';
import { Pagination } from '../../../components/ui/Pagination';
import { VehicleDetailModal } from '../../../components/ui/VehicleDetailModal';
import { VehicleFilters } from '../../../components/ui/VehicleFilters';
import type { Vehicle, VehicleDetail } from '../../../common/types/vehicle.types';

export function StaffInventoryPage() {
  const dialog = useDialog();
  const queryClient = useQueryClient();
  const [page, setPage] = useState(1);
  const [make, setMake] = useState('');
  const [model, setModel] = useState('');
  const [minAgeDays, setMinAgeDays] = useState('');
  const [maxAgeDays, setMaxAgeDays] = useState('');
  const [agingOnly, setAgingOnly] = useState(false);
  const [detail, setDetail] = useState<VehicleDetail | null>(null);
  const [saleError, setSaleError] = useState('');
  const [pendingSaleId, setPendingSaleId] = useState<string | null>(null);

  const filter = useMemo(() => {
    const params: Record<string, string | number | boolean> = { page, pageSize: 9 };
    if (make.trim()) params.make = make.trim();
    if (model.trim()) params.model = model.trim();
    if (minAgeDays) params.minAgeDays = Number(minAgeDays);
    if (maxAgeDays) params.maxAgeDays = Number(maxAgeDays);
    if (agingOnly) params.agingOnly = true;
    return params;
  }, [make, model, minAgeDays, maxAgeDays, agingOnly, page]);

  const { data, isLoading } = useQuery({
    queryKey: ['staff-inventory', filter],
    queryFn: () => staffApi.getInventory(filter),
  });

  const saleMutation = useMutation({
    mutationFn: staffApi.processSale,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['staff-inventory'] });
      queryClient.invalidateQueries({ queryKey: ['staff-history'] });
      setSaleError('');
      setPendingSaleId(null);
    },
    onError: (err: unknown) => {
      const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message;
      setSaleError(msg ?? 'Sale failed.');
      setPendingSaleId(null);
    },
  });

  const handleSale = async (vehicle: Vehicle) => {
    const title = vehicle.quantity === 1 ? 'Last unit warning' : 'Confirm sale';
    const message =
      vehicle.quantity === 1
        ? `Last unit (${vehicle.year} ${vehicle.make} ${vehicle.model}). Vehicle will be disabled after sale. Continue?`
        : `Confirm sale: ${vehicle.year} ${vehicle.make} ${vehicle.model}?`;

    const ok = await dialog.confirm({
      title,
      message,
      tone: vehicle.quantity === 1 ? 'warning' : 'default',
      confirmText: 'Confirm',
    });
    if (!ok) return;

    setPendingSaleId(vehicle.id);
    await saleMutation.mutateAsync(vehicle.id);
  };

  const handleReset = () => {
    setMake('');
    setModel('');
    setMinAgeDays('');
    setMaxAgeDays('');
    setAgingOnly(false);
    setPage(1);
  };

  return (
    <div className="staff-page">
      <header className="staff-page-header">
        <div>
          <p className="staff-page-eyebrow">Today&apos;s floor</p>
          <h1>Available inventory</h1>
          <p className="staff-page-desc">
            Browse, filter, and record sales. Out-of-stock units are hidden automatically.
          </p>
        </div>
        <div className="staff-stat-pill">
          <span className="staff-stat-value">{data?.totalCount ?? '—'}</span>
          <span className="staff-stat-label">vehicles</span>
        </div>
      </header>

      <div className="staff-filters-panel">
        <VehicleFilters
          make={make}
          model={model}
          minAgeDays={minAgeDays}
          maxAgeDays={maxAgeDays}
          agingOnly={agingOnly}
          onMakeChange={(v) => { setMake(v); setPage(1); }}
          onModelChange={(v) => { setModel(v); setPage(1); }}
          onMinAgeChange={(v) => { setMinAgeDays(v); setPage(1); }}
          onMaxAgeChange={(v) => { setMaxAgeDays(v); setPage(1); }}
          onAgingOnlyChange={(v) => { setAgingOnly(v); setPage(1); }}
          onReset={handleReset}
        />
      </div>

      {saleError && <p className="form-error staff-alert">{saleError}</p>}

      {isLoading ? (
        <p className="staff-loading">Loading inventory...</p>
      ) : (
        <>
          <div className="staff-card-grid">
            {data?.items.map((vehicle) => (
              <StaffVehicleCard
                key={vehicle.id}
                vehicle={vehicle}
                salePending={pendingSaleId === vehicle.id}
                onDetail={async () => setDetail(await staffApi.getVehicleDetail(vehicle.id))}
                onSale={() => handleSale(vehicle)}
              />
            ))}
          </div>
          {!data?.items.length && (
            <p className="staff-empty">No vehicles match your filters.</p>
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
