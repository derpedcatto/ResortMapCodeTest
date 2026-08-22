import type { CellKey, GridCoords } from "@/types/map";

export const cellKey = (c: GridCoords): CellKey => `${c.row},${c.col}`;
