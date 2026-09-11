import type { SpriteKey, SpriteRotation } from "./sprites";

export type ResortMapDto = { grid: string[] };

export type CellKey = `${number},${number}`;
export type BookedCabanaSet = ReadonlySet<CellKey>;

export type GridCoords = { row: number; col: number };

export type TileType = "cabana" | "pool" | "path" | "chalet" | "empty";
export type TileGrid = TileType[][];

export type TileEdges = {
  top: boolean;
  right: boolean;
  bottom: boolean;
  left: boolean;
};

export type VisualTile = {
  type: TileType;
  coords: GridCoords;
  key: CellKey;
  sprite: SpriteKey;
  rotation: SpriteRotation;
  edges?: TileEdges;
};
export type VisualTileGrid = VisualTile[][];
