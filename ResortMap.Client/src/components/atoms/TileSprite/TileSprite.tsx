import classNames from "classnames";
import type { SpriteRotation } from "@/types/sprites";
import styles from "./TileSprite.module.scss";

type TileSpriteProps = {
  src: string;
  rotation: SpriteRotation;
  alt?: string;
  className?: string;
};

export function TileSprite({
  src,
  rotation = 0,
  alt = "",
  className,
}: TileSpriteProps) {
  return (
    <img
      src={src}
      alt={alt}
      draggable={false}
      decoding="async"
      style={rotation ? { transform: `rotate(${rotation}deg)` } : undefined}
      className={classNames(styles.sprite, className)}
    />
  );
}
