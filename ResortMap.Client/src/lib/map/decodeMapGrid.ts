import type { TileGrid, TileType } from "@/types/map";

type TileTypeChar = "W" | "p" | "#" | "c" | ".";

const tileCharMapping: Record<TileTypeChar, TileType> = {
  W: "cabana",
  p: "pool",
  "#": "path",
  c: "chalet",
  ".": "empty",
};

export function decodeMapGrid(grid: string[]): TileGrid {
  return grid.map((line) =>
    [...line].map((char) => tileCharMapping[char as TileTypeChar]),
  );
}
