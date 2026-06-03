import { Button } from './Button';

interface PaginationProps {
  page: number;
  totalPages: number;
  totalCount?: number;
  onPageChange: (page: number) => void;
}

export function Pagination({ page, totalPages, totalCount, onPageChange }: PaginationProps) {
  if (totalPages <= 1) return null;

  return (
    <nav className="pagination" aria-label="Pagination">
      <Button
        variant="outline"
        size="sm"
        disabled={page <= 1}
        onClick={() => onPageChange(page - 1)}
      >
        ← Previous
      </Button>
      <div className="pagination-info">
        <span className="pagination-page">
          Page <strong>{page}</strong> of <strong>{totalPages}</strong>
        </span>
        {totalCount !== undefined && (
          <span className="pagination-total">{totalCount} total</span>
        )}
      </div>
      <Button
        variant="outline"
        size="sm"
        disabled={page >= totalPages}
        onClick={() => onPageChange(page + 1)}
      >
        Next →
      </Button>
    </nav>
  );
}
