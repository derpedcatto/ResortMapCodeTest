import { TileSprite } from "@/components/atoms/TileSprite/TileSprite";
import { SPRITES } from "@/lib/map/spritesAssets";
import type { GridCoords, VisualTile } from "@/types/map";
import { memo } from "react";
import styles from "./MapTile.module.scss";
import { Tooltip } from "@/components/atoms/Tooltip/Tooltip";
import classNames from "classnames";

type MapTileProps = {
  tile: VisualTile;
  booked?: boolean;
  onSelect?: (coords: GridCoords) => void;
  className?: string;
};

function MapTileBase({
  tile,
  booked = false,
  onSelect,
  className,
}: MapTileProps) {
  const { type, coords, sprite, rotation, edges } = tile;
  const spriteAsset = SPRITES[sprite];
  const isCabana = type === "cabana";

  const tooltipLabel = isCabana
    ? `Cabana ${coords.row}-${coords.col} is ${booked ? "booked" : "available"}`
    : "";

  const tileClassName = classNames(
    styles.tile,
    styles[type],
    booked && styles.booked,
    edges && {
      [styles.openTop]: !edges.top,
      [styles.openRight]: !edges.right,
      [styles.openBottom]: !edges.bottom,
      [styles.openLeft]: !edges.left,
    },
    className,
  );

  const content = (
    <>
      {spriteAsset.src && (
        <TileSprite
          src={spriteAsset.src}
          alt={spriteAsset.alt}
          rotation={rotation}
          className={styles.sprite}
        />
      )}
      {tooltipLabel && (
        <Tooltip label={tooltipLabel} className={styles.tooltip} />
      )}
    </>
  );

  if (!isCabana) {
    return (
      <div aria-hidden="true" className={tileClassName}>
        {content}
      </div>
    );
  }

  function handleClick() {
    if (booked) return;
    onSelect?.(coords);
  }

  return (
    <button
      type="button"
      onClick={handleClick}
      aria-label={tooltipLabel}
      aria-disabled={booked || undefined}
      data-row={coords.row}
      data-col={coords.col}
      className={tileClassName}
    >
      {content}
    </button>
  );
}

export const MapTile = memo(MapTileBase);
