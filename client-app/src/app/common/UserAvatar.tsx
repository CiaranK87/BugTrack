interface Props {
  image?: string | null;
  displayName: string;
  size?: 'mini' | 'tiny' | 'small';
  className?: string;
  style?: React.CSSProperties;
}

const SIZE_PX: Record<string, number> = { mini: 28, tiny: 35, small: 58 };

export default function UserAvatar({ image, displayName, size = 'mini', className, style }: Props) {
  const px = SIZE_PX[size] ?? 28;

  const containerStyle: React.CSSProperties = {
    width: px,
    height: px,
    borderRadius: '50%',
    overflow: 'hidden',
    display: 'inline-flex',
    alignItems: 'center',
    justifyContent: 'center',
    flexShrink: 0,
    ...style,
  };

  if (image) {
    const baseUrl = (import.meta.env.VITE_API_URL || 'http://localhost:5000/api').replace(/\/api$/, '');
    return (
      <div className={className} style={containerStyle}>
        <img
          src={`${baseUrl}${image}`}
          alt={displayName}
          style={{ width: '100%', height: '100%', objectFit: 'cover' }}
        />
      </div>
    );
  }

  const initials = displayName
    .split(' ')
    .filter(Boolean)
    .map(n => n[0].toUpperCase())
    .slice(0, 2)
    .join('') || '?';

  return (
    <div
      className={className}
      style={{
        ...containerStyle,
        backgroundColor: '#00b5ad',
        color: 'white',
        fontSize: Math.round(px * 0.38),
        fontWeight: 'bold',
      }}
    >
      {initials}
    </div>
  );
}
