import type { BookedCabanaSet, GridCoords, VisualTileGrid } from "@/types/map";
import { SPRITES } from "@/lib/map/spritesAssets";
import { memo, type CSSProperties } from "react";
import classNames from "classnames";
import { MapTile } from "@/components/molecules/MapTile/MapTile";
import styles from "./MapGrid.module.scss";

type MapGridProps = {
  tiles: VisualTileGrid;
  bookedCabanas?: BookedCabanaSet;
  onSelectCabana?: (coords: GridCoords) => void;
  className?: string;
};

function MapGridBase({
  tiles,
  bookedCabanas,
  onSelectCabana,
  className,
}: MapGridProps) {
  const rows = tiles.length;
  const cols = tiles[0].length;

  const gridStyle = {
    "--map-rows": rows,
    "--map-cols": cols,
    "--map-background": `url("${SPRITES.background.src}")`,
  } as CSSProperties;

  return (
    <div
      role="group"
      aria-label="Resort Map"
      style={gridStyle}
      className={classNames(styles.grid, className)}
    >
      {tiles.flatMap((row) =>
        row.map((tile) => (
          <MapTile
            key={tile.key}
            tile={tile}
            booked={bookedCabanas?.has(tile.key)}
            onSelect={onSelectCabana}
          />
        )),
      )}
    </div>
  );
}

export const MapGrid = memo(MapGridBase);
