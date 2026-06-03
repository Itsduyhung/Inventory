interface VehicleImageProps {
  src?: string | null;
  alt: string;
  className?: string;
  size?: 'thumb' | 'card' | 'detail';
}

export function VehicleImage({ src, alt, className = '', size = 'thumb' }: VehicleImageProps) {
  const sizeClass = `vehicle-image vehicle-image--${size}`;

  if (!src?.trim()) {
    return (
      <div className={`${sizeClass} vehicle-image--placeholder ${className}`} aria-hidden>
        <span>No photo</span>
      </div>
    );
  }

  return (
    <img
      src={src}
      alt={alt}
      className={`${sizeClass} ${className}`}
      loading="lazy"
    />
  );
}
