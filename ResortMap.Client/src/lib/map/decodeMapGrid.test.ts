import { describe, it, expect } from "vitest";
import { decodeMapGrid, tileTypeToChar as char } from "./decodeMapGrid";

const unknownCharExample = "`";

describe("decodeMapGrid", () => {
  it("maps all valid types correctly", () => {
    const input = `${char.cabana}${char.pool}${char.path}${char.chalet}${char.empty}`;

    expect(decodeMapGrid([input])).toEqual([
      ["cabana", "pool", "path", "chalet", "empty"],
    ]);
  });

  it("decodes a 2d grid correctly", () => {
    const result = decodeMapGrid([
      `${char.cabana}${char.pool}${char.path}`,
      `${char.chalet}${char.empty}${char.cabana}`,
    ]);

    expect(result).toEqual([
      ["cabana", "pool", "path"],
      ["chalet", "empty", "cabana"],
    ]);
  });

  it("returns empty array for an empty grid", () => {
    expect(decodeMapGrid([])).toEqual([]);
  });

  it("returns null for unknown character", () => {
    expect(decodeMapGrid([unknownCharExample])).toEqual([[null]]);
  });

  it("mixes known and unknown characters", () => {
    const result = decodeMapGrid([
      `${char.cabana}${unknownCharExample}${char.pool}`,
    ]);

    expect(result).toEqual([["cabana", null, "pool"]]);
  });
});
