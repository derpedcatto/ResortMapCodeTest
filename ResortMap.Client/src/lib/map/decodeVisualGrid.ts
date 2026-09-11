import type {
  TileEdges,
  TileGrid,
  TileType,
  VisualTile,
  VisualTileGrid,
} from "@/types/map";
import type { SpriteKey, SpriteRotation } from "@/types/sprites";
import { cellKey } from "./cellKey";

export function decodeVisualGrid(grid: TileGrid) {
  const resultGrid: VisualTileGrid = [];

  for (let row = 0; row < grid.length; row++) {
    const resultRow: VisualTile[] = [];

    for (let col = 0; col < grid[row].length; col++) {
      const tile = grid[row][col];

      const result: VisualTile = {
        type: tile,
        coords: { row, col },
        key: cellKey({ row, col }),
        rotation: 0,
        sprite: "empty",
      };

      switch (tile) {
        case "empty":
          break;
        case "cabana":
          result.sprite = "cabana";
          break;
        case "chalet":
          result.sprite = "chalet";
          break;
        case "pool":
          result.sprite = "water";
          result.edges = decodePool(grid, row, col);
          break;
        case "path": {
          const pathResult = decodePath(grid, row, col);
          result.sprite = pathResult.sprite;
          result.rotation = pathResult.rotation;
          break;
        }
      }

      resultRow.push(result);
    }

    resultGrid.push(resultRow);
  }

  return resultGrid;
}

function getTile(grid: TileGrid, row: number, col: number): TileType | null {
  return grid[row]?.[col] ?? null;
}

function decodePool(grid: TileGrid, row: number, col: number): TileEdges {
  return {
    top: getTile(grid, row - 1, col) === "pool",
    right: getTile(grid, row, col + 1) === "pool",
    bottom: getTile(grid, row + 1, col) === "pool",
    left: getTile(grid, row, col - 1) === "pool",
  };
}

function decodePath(
  grid: TileGrid,
  row: number,
  col: number,
): { sprite: SpriteKey; rotation: SpriteRotation } {
  const up = getTile(grid, row - 1, col) === "path";
  const right = getTile(grid, row, col + 1) === "path";
  const down = getTile(grid, row + 1, col) === "path";
  const left = getTile(grid, row, col - 1) === "path";
  const count = +up + +right + +down + +left;

  switch (count) {
    case 4:
      return { sprite: "pathCrossing", rotation: 0 };

    case 3: {
      if (!left) return { sprite: "pathSplit", rotation: 0 };
      if (!up) return { sprite: "pathSplit", rotation: 90 };
      if (!right) return { sprite: "pathSplit", rotation: 180 };
      return { sprite: "pathSplit", rotation: 270 };
    }

    case 2: {
      if (up && down) return { sprite: "pathStraight", rotation: 0 };
      if (left && right) return { sprite: "pathStraight", rotation: 90 };
      if (up && right) return { sprite: "pathCorner", rotation: 0 };
      if (right && down) return { sprite: "pathCorner", rotation: 90 };
      if (down && left) return { sprite: "pathCorner", rotation: 180 };
      return { sprite: "pathCorner", rotation: 270 };
    }

    case 1: {
      if (down) return { sprite: "pathEnd", rotation: 0 };
      if (left) return { sprite: "pathEnd", rotation: 90 };
      if (up) return { sprite: "pathEnd", rotation: 180 };
      return { sprite: "pathEnd", rotation: 270 };
    }

    default:
      return { sprite: "pathStraight", rotation: 0 };
  }
}
