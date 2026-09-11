import { describe, it, expect } from "vitest";
import { decodeVisualGrid } from "./decodeVisualGrid";
import { decodeMapGrid, tileTypeToChar as char } from "./decodeMapGrid";
import type { SpriteKey } from "@/types/sprites";

const sprite = <const T extends SpriteKey>(spr: T) => spr;

function tileAt(lines: string[], row: number, col: number) {
  const mapGrid = decodeMapGrid(lines);
  return decodeVisualGrid(mapGrid)[row][col];
}

describe("decodeVisualGrid", () => {
  it("Empty tile -> sprite empty, rotation 0", () => {
    const tile = tileAt([char.empty], 0, 0);

    expect(tile.sprite).toBe(sprite("empty"));
    expect(tile.rotation).toBe(0);
  });

  it("Cabana tile - sprite cabana, rotation 0", () => {
    const tile = tileAt([char.cabana], 0, 0);

    expect(tile.sprite).toBe(sprite("cabana"));
    expect(tile.rotation).toBe(0);
  });

  it("Pool surrounded by pools - all edges true", () => {
    const p = char.pool;
    const map = [`${p}${p}${p}`, `${p}${p}${p}`, `${p}${p}${p}`];
    const tile = tileAt(map, 1, 1);

    expect(tile.sprite).toBe(sprite("water"));
    expect(tile.edges).toEqual({
      top: true,
      right: true,
      bottom: true,
      left: true,
    });
  });

  it("Pool at grid corner - only adjacent pool edges true, out-of-bounds edges false", () => {
    const map = [`${char.pool}${char.pool}`, `${char.pool}${char.empty}`];
    const tile = tileAt(map, 0, 0);

    expect(tile.edges).toEqual({
      top: false,
      right: true,
      bottom: true,
      left: false,
    });
  });

  it("Single path tile - sprite pathStraight, rotation 0", () => {
    const e = char.empty;
    const map = [`${e}${e}${e}`, `${e}${char.path}${e}`, `${e}${e}${e}`];
    const tile = tileAt(map, 1, 1);

    expect(tile.sprite).toBe(sprite("pathStraight"));
    expect(tile.rotation).toBe(0);
  });

  it("1-way path down - sprite pathEnd, rotation 0", () => {
    const e = char.empty;
    const p = char.path;
    const map = [`${e}${e}${e}`, `${e}${p}${e}`, `${e}${p}${e}`];
    const tile = tileAt(map, 1, 1);

    expect(tile.sprite).toBe(sprite("pathEnd"));
    expect(tile.rotation).toBe(0);
  });

  it("1-way path left - sprite pathEnd, rotation 90", () => {
    const e = char.empty;
    const p = char.path;
    const map = [`${e}${e}${e}`, `${p}${p}${e}`, `${e}${e}${e}`];
    const tile = tileAt(map, 1, 1);

    expect(tile.sprite).toBe(sprite("pathEnd"));
    expect(tile.rotation).toBe(90);
  });

  it("1-way path up - sprite pathEnd, rotation 180", () => {
    const e = char.empty;
    const p = char.path;
    const map = [`${e}${p}${e}`, `${e}${p}${e}`, `${e}${e}${e}`];
    const tile = tileAt(map, 1, 1);

    expect(tile.sprite).toBe(sprite("pathEnd"));
    expect(tile.rotation).toBe(180);
  });

  it("1-way path right - sprite pathEnd, rotation 270", () => {
    const e = char.empty;
    const p = char.path;
    const map = [`${e}${e}${e}`, `${e}${p}${p}`, `${e}${e}${e}`];
    const tile = tileAt(map, 1, 1);

    expect(tile.sprite).toBe(sprite("pathEnd"));
    expect(tile.rotation).toBe(270);
  });

  it("2-way path with neighbors up+down - pathStraight, rotation 0", () => {
    const e = char.empty;
    const p = char.path;
    const map = [`${e}${p}${e}`, `${e}${p}${e}`, `${e}${p}${e}`];
    const tile = tileAt(map, 1, 1);

    expect(tile.sprite).toBe(sprite("pathStraight"));
    expect(tile.rotation).toBe(0);
  });

  it("2-way path with neighbors left+right - pathStraight, rotation 90", () => {
    const e = char.empty;
    const p = char.path;
    const map = [`${e}${e}${e}`, `${p}${p}${p}`, `${e}${e}${e}`];
    const tile = tileAt(map, 1, 1);

    expect(tile.sprite).toBe(sprite("pathStraight"));
    expect(tile.rotation).toBe(90);
  });

  it("2-way path with neighbors up+right - pathCorner, rotation 0", () => {
    const e = char.empty;
    const p = char.path;
    const map = [`${e}${p}${e}`, `${e}${p}${p}`, `${e}${e}${e}`];
    const tile = tileAt(map, 1, 1);

    expect(tile.sprite).toBe(sprite("pathCorner"));
    expect(tile.rotation).toBe(0);
  });

  it("2-way path with neighbors right+down - pathCorner, rotation 90", () => {
    const e = char.empty;
    const p = char.path;
    const map = [`${e}${e}${e}`, `${e}${p}${p}`, `${e}${p}${e}`];
    const tile = tileAt(map, 1, 1);

    expect(tile.sprite).toBe(sprite("pathCorner"));
    expect(tile.rotation).toBe(90);
  });

  it("2-way path with neighbors down+left - pathCorner, rotation 180", () => {
    const e = char.empty;
    const p = char.path;
    const map = [`${e}${e}${e}`, `${p}${p}${e}`, `${e}${p}${e}`];
    const tile = tileAt(map, 1, 1);

    expect(tile.sprite).toBe(sprite("pathCorner"));
    expect(tile.rotation).toBe(180);
  });

  it("2-way path with neighbors left+up - pathCorner, rotation 270", () => {
    const e = char.empty;
    const p = char.path;
    const map = [`${e}${p}${e}`, `${p}${p}${e}`, `${e}${e}${e}`];
    const tile = tileAt(map, 1, 1);

    expect(tile.sprite).toBe(sprite("pathCorner"));
    expect(tile.rotation).toBe(270);
  });

  it("3-way path with neighbors up+right+down - pathSplit, rotation 0", () => {
    const e = char.empty;
    const p = char.path;
    const map = [`${e}${p}${e}`, `${e}${p}${p}`, `${e}${p}${e}`];
    const tile = tileAt(map, 1, 1);

    expect(tile.sprite).toBe(sprite("pathSplit"));
    expect(tile.rotation).toBe(0);
  });

  it("3-way path with neighbors right+down+left - pathSplit, rotation 90", () => {
    const e = char.empty;
    const p = char.path;
    const map = [`${e}${e}${e}`, `${p}${p}${p}`, `${e}${p}${e}`];
    const tile = tileAt(map, 1, 1);

    expect(tile.sprite).toBe(sprite("pathSplit"));
    expect(tile.rotation).toBe(90);
  });

  it("3-way path with neighbors up+down+left - pathSplit, rotation 180", () => {
    const e = char.empty;
    const p = char.path;
    const map = [`${e}${p}${e}`, `${p}${p}${e}`, `${e}${p}${e}`];
    const tile = tileAt(map, 1, 1);

    expect(tile.sprite).toBe(sprite("pathSplit"));
    expect(tile.rotation).toBe(180);
  });

  it("3-way path with neighbors up+right+left - pathSplit, rotation 270", () => {
    const e = char.empty;
    const p = char.path;
    const map = [`${e}${p}${e}`, `${p}${p}${p}`, `${e}${e}${e}`];
    const tile = tileAt(map, 1, 1);

    expect(tile.sprite).toBe(sprite("pathSplit"));
    expect(tile.rotation).toBe(270);
  });

  it("4-way path - pathCrossing, rotation 0", () => {
    const e = char.empty;
    const p = char.path;
    const map = [`${e}${p}${e}`, `${p}${p}${p}`, `${e}${p}${e}`];
    const tile = tileAt(map, 1, 1);

    expect(tile.sprite).toBe(sprite("pathCrossing"));
    expect(tile.rotation).toBe(0);
  });
});
