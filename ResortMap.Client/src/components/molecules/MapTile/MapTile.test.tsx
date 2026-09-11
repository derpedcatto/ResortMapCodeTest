import { describe, it, expect, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MapTile } from "@/components/molecules/MapTile/MapTile";
import type { VisualTile } from "@/types/map";
import { cellKey } from "@/lib/map/cellKey";

vi.mock("@/lib/map/spritesAssets", () => ({
  SPRITES: {
    cabana: { src: "/cabana.png", alt: "Cabana" },
    pool: { src: "/pool.png", alt: "Pool" },
    pathStraight: { src: "/pathStraight.png", alt: "Path" },
    empty: { src: "", alt: "" },
  },
}));

const defaultCoords = { row: 1, col: 1 };

const defaultTile: VisualTile = {
  type: "cabana",
  coords: defaultCoords,
  key: cellKey(defaultCoords),
  sprite: "cabana",
  rotation: 0,
};

const defaultProps = {
  tile: defaultTile,
  onSelect: vi.fn(),
};

function setup(overrides = {}, tileOverrides: Partial<VisualTile> = {}) {
  const user = userEvent.setup();
  const props = {
    ...defaultProps,
    ...overrides,
    tile: { ...defaultTile, ...tileOverrides },
  };

  return { user, props, ...render(<MapTile {...props} />) };
}

describe("MapTile", () => {
  it("renders an available cabana as <button> and calls onSelect with correct coords", async () => {
    const { user, props } = setup();

    const button = screen.getByRole("button");
    expect(button).not.toHaveAttribute("aria-disabled");

    await user.click(button);
    expect(props.onSelect).toHaveBeenCalledOnce();
    expect(props.onSelect).toHaveBeenCalledWith({ row: 1, col: 1 });
  });

  it("renders booked cabana with aria-disabled and blocks click", async () => {
    const { user, props } = setup({ booked: true });

    const button = screen.getByRole("button");
    expect(button).toHaveAttribute("aria-disabled", "true");

    await user.click(button);
    expect(props.onSelect).not.toHaveBeenCalled();
  });

  it("renders non-cabana tile as aria-hidden div with no button", () => {
    setup({}, { type: "pool", sprite: "pool" });

    expect(screen.queryByRole("button")).not.toBeInTheDocument();

    const tile = document.querySelector("[aria-hidden='true']");
    expect(tile).toBeInTheDocument();
  });

  it("shows correct tooltip for available cabana", () => {
    setup();

    expect(screen.getByText("Cabana 1-1 is available")).toBeInTheDocument();
  });

  it("shows correct tooltip for booked cabana", () => {
    setup({ booked: true });

    expect(screen.getByText("Cabana 1-1 is booked")).toBeInTheDocument();
  });

  it("doesn't render tooltip on non-cabana tile", () => {
    setup({}, { type: "path", sprite: "pathStraight" });

    expect(
      screen.queryByText("Cabana 1-1 is available"),
    ).not.toBeInTheDocument();
    expect(screen.queryByText("Cabana 1-1 is booked")).not.toBeInTheDocument();
  });

  it("renders no img when sprite has no src", () => {
    setup({}, { sprite: "empty" });

    expect(screen.queryByRole("img")).not.toBeInTheDocument();
  });
});
