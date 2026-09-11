import type { GridCoords } from "@/types/map";

export type CabanaLabel = `${number}-${number}`;

export const cabanaLabel = (c: GridCoords): CabanaLabel =>
  `${c.row}-${c.col}`;
