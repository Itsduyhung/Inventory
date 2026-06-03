import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useMemo, useState } from 'react';
import { adminVehicleApi } from '../../../api/adminVehicleApi';
import type { CreateVehiclePayload, Vehicle, VehicleDetail } from '../../../common/types/vehicle.types';
import { useDialog } from '../../../common/context/DialogContext';
import { getApiErrorMessage } from '../../../common/utils/apiError';
import { Button } from '../../../components/ui/Button';
import { Pagination } from '../../../components/ui/Pagination';
import { SummaryCards } from '../../../components/ui/SummaryCards';
import { VehicleDetailModal } from '../../../components/ui/VehicleDetailModal';
import { VehicleFilters } from '../../../components/ui/VehicleFilters';
import { VehicleFormModal } from '../../../components/ui/VehicleFormModal';
import { VehicleTable } from '../../../components/ui/VehicleTable';

export function InventoryDashboardPage() {
  const dialog = useDialog();
  const queryClient = useQueryClient();
  const [page, setPage] = useState(1);
  const [make, setMake] = useState('');
  const [model, setModel] = useState('');
  const [minAgeDays, setMinAgeDays] = useState('');
  const [maxAgeDays, setMaxAgeDays] = useState('');
  const [agingOnly, setAgingOnly] = useState(false);
  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState<VehicleDetail | null>(null);
  const [detail, setDetail] = useState<VehicleDetail | null>(null);
  const [formError, setFormError] = useState('');

  const filter = useMemo(() => {
    const params: Record<string, string | number | boolean> = { page, pageSize: 10 };
    if (make.trim()) params.make = make.trim();
    if (model.trim()) params.model = model.trim();
    if (minAgeDays) params.minAgeDays = Number(minAgeDays);
    if (maxAgeDays) params.maxAgeDays = Number(maxAgeDays);
    if (agingOnly) params.agingOnly = true;
    return params;
  }, [make, model, minAgeDays, maxAgeDays, agingOnly, page]);

  const { data: summary, isLoading: summaryLoading } = useQuery({
    queryKey: ['inventory-summary'],
    queryFn: adminVehicleApi.getSummary,
  });

  const { data, isLoading } = useQuery({
    queryKey: ['admin-vehicles', filter],
    queryFn: () => adminVehicleApi.getAll(filter),
  });

  const invalidate = () => {
    queryClient.invalidateQueries({ queryKey: ['admin-vehicles'] });
    queryClient.invalidateQueries({ queryKey: ['inventory-summary'] });
  };

  const createMutation = useMutation({
    mutationFn: (payload: CreateVehiclePayload) => adminVehicleApi.create(payload),
    onSuccess: () => {
      invalidate();
      setFormOpen(false);
      setFormError('');
    },
    onError: (err) => setFormError(getApiErrorMessage(err)),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, payload }: { id: string; payload: CreateVehiclePayload }) =>
      adminVehicleApi.update(id, {
        ...payload,
        dateAddedToInventory: payload.dateAddedToInventory ?? new Date().toISOString().split('T')[0],
      }),
    onSuccess: () => {
      invalidate();
      setEditing(null);
      setFormError('');
    },
    onError: (err) => setFormError(getApiErrorMessage(err)),
  });

  const disableMutation = useMutation({
    mutationFn: adminVehicleApi.disable,
    onSuccess: invalidate,
  });

  const addQtyMutation = useMutation({
    mutationFn: (id: string) => adminVehicleApi.addQuantity(id, 1),
    onSuccess: invalidate,
  });

  const openDetail = async (v: Vehicle) => setDetail(await adminVehicleApi.getById(v.id));
  const openEdit = async (v: Vehicle) => setEditing(await adminVehicleApi.getById(v.id));

  const handleReset = () => {
    setMake('');
    setModel('');
    setMinAgeDays('');
    setMaxAgeDays('');
    setAgingOnly(false);
    setPage(1);
  };

  return (
    <div className="dashboard">
      <header className="page-header">
        <div>
          <p className="eyebrow">Supply · Dealership Inventory</p>
          <h1>Intelligent Inventory Dashboard</h1>
          <p className="subtitle">
            Real-time overview of vehicle stock with aging stock identification and actionable insights.
          </p>
        </div>
        <Button variant="primary" onClick={() => { setFormError(''); setFormOpen(true); }}>
          + Add Vehicle
        </Button>
      </header>

      <SummaryCards summary={summary} isLoading={summaryLoading} />

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

      <section className="inventory-section">
        <div className="section-header">
          <h2>Vehicle Inventory</h2>
          <span className="result-count">{data?.totalCount ?? 0} vehicles</span>
        </div>
        <VehicleTable
          vehicles={data?.items ?? []}
          isLoading={isLoading}
          mode="admin"
          onDetail={openDetail}
          onEdit={openEdit}
          onDisable={async (v) => {
            const ok = await dialog.confirm({
              title: 'Disable vehicle',
              message: 'Disable this vehicle? Quantity will be set to 0.',
              tone: 'warning',
              confirmText: 'Disable',
            });
            if (ok) disableMutation.mutate(v.id);
          }}
          onAddQuantity={(v) => addQtyMutation.mutate(v.id)}
        />
        {data && (
          <Pagination
            page={data.page}
            totalPages={data.totalPages}
            totalCount={data.totalCount}
            onPageChange={setPage}
          />
        )}
      </section>

      {formError && <div className="form-banner form-banner--error page-error">{formError}</div>}

      <VehicleFormModal
        isOpen={formOpen}
        isSaving={createMutation.isPending}
        onClose={() => { setFormOpen(false); setFormError(''); }}
        onSubmit={async (p) => { await createMutation.mutateAsync(p); }}
      />

      <VehicleFormModal
        isOpen={!!editing}
        vehicle={editing}
        isSaving={updateMutation.isPending}
        onClose={() => { setEditing(null); setFormError(''); }}
        onSubmit={async (p) => {
          if (!editing) throw new Error('No vehicle selected');
          await updateMutation.mutateAsync({ id: editing.id, payload: p });
        }}
      />

      <VehicleDetailModal vehicle={detail} isOpen={!!detail} onClose={() => setDetail(null)} />
    </div>
  );
}
