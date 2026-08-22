import classNames from "classnames";
import styles from "./TileSprite.module.scss";

// export type TileRotation = 0 | 90 | 180 | 270;
// style={rotation ? { transform: `rotate(${rotation}deg)` } : undefined}

type TileSpriteProps = {
  src: string;
  alt?: string;
  className?: string;
};

export function TileSprite({ src, alt = "", className }: TileSpriteProps) {
  return (
    <img
      src={src}
      alt={alt}
      draggable={false}
      decoding="async"
      className={classNames(styles.sprite, className)}
    />
  );
}
