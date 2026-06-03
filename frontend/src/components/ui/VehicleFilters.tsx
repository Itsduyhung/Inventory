interface VehicleFiltersProps {
  make: string;
  model: string;
  minAgeDays: string;
  maxAgeDays: string;
  agingOnly: boolean;
  onMakeChange: (value: string) => void;
  onModelChange: (value: string) => void;
  onMinAgeChange: (value: string) => void;
  onMaxAgeChange: (value: string) => void;
  onAgingOnlyChange: (value: boolean) => void;
  onReset: () => void;
}

export function VehicleFilters({
  make,
  model,
  minAgeDays,
  maxAgeDays,
  agingOnly,
  onMakeChange,
  onModelChange,
  onMinAgeChange,
  onMaxAgeChange,
  onAgingOnlyChange,
  onReset,
}: VehicleFiltersProps) {
  return (
    <section className="filters-panel">
      <div className="filters-header">
        <h2>Filters</h2>
        <button type="button" className="btn-text" onClick={onReset}>
          Reset
        </button>
      </div>
      <div className="filters-grid">
        <label>
          Make
          <input
            type="text"
            placeholder="e.g. BM → BMW"
            value={make}
            onChange={(e) => onMakeChange(e.target.value)}
          />
        </label>
        <label>
          Model
          <input
            type="text"
            placeholder="e.g. Cam (partial match)"
            value={model}
            onChange={(e) => onModelChange(e.target.value)}
          />
        </label>
        <label>
          Min Age (days)
          <input
            type="number"
            min={0}
            placeholder="0"
            value={minAgeDays}
            onChange={(e) => onMinAgeChange(e.target.value)}
          />
        </label>
        <label>
          Max Age (days)
          <input
            type="number"
            min={0}
            placeholder="365"
            value={maxAgeDays}
            onChange={(e) => onMaxAgeChange(e.target.value)}
          />
        </label>
      </div>
      <label className="checkbox-label">
        <input
          type="checkbox"
          checked={agingOnly}
          onChange={(e) => onAgingOnlyChange(e.target.checked)}
        />
        Show aging stock only (&gt; 90 days)
      </label>
      <p className="filter-hint">Make &amp; Model use partial match (e.g. &quot;BM&quot; finds BMW).</p>
    </section>
  );
}
