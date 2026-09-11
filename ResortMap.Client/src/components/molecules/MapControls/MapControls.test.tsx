import { describe, it, expect, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MapControls } from "@/components/molecules/MapControls/MapControls";

const defaultProps = {
  onZoomIn: vi.fn(),
  onZoomOut: vi.fn(),
  onReset: vi.fn(),
  scale: 1.5,
  minScale: 0.5,
  maxScale: 3,
};

function setup(overrides = {}) {
  const user = userEvent.setup();
  const props = { ...defaultProps, ...overrides };

  return { user, props, ...render(<MapControls {...props} />) };
}

describe("MapControls", () => {
  it("calls all available map controls on their button clicks", async () => {
    const { user, props } = setup();

    await user.click(screen.getByRole("button", { name: /zoom in/i }));
    expect(props.onZoomIn).toHaveBeenCalledOnce();

    await user.click(screen.getByRole("button", { name: /zoom out/i }));
    expect(props.onZoomOut).toHaveBeenCalledOnce();

    await user.click(screen.getByRole("button", { name: /reset view/i }));
    expect(props.onReset).toHaveBeenCalledOnce();
  });

  it("disables Zoom In when scale >= maxScale", async () => {
    const { user, props } = setup({ scale: 3, onZoomIn: vi.fn() });

    const button = screen.getByRole("button", { name: /zoom in/i });
    expect(button).toHaveAttribute("aria-disabled", "true");

    await user.click(button);
    expect(props.onZoomIn).not.toHaveBeenCalled();
  });

  it("disables Zoom Out when scale <= minScale", async () => {
    const { user, props } = setup({ scale: 0.5, onZoomOut: vi.fn() });

    const btn = screen.getByRole("button", { name: /zoom out/i });
    expect(btn).toHaveAttribute("aria-disabled", "true");

    await user.click(btn);
    expect(props.onZoomOut).not.toHaveBeenCalled();
  });

  it("disables Reset View when scale === 1", async () => {
    const { user, props } = setup({ scale: 1, onReset: vi.fn() });

    const btn = screen.getByRole("button", { name: /reset view/i });
    expect(btn).toHaveAttribute("aria-disabled", "true");

    await user.click(btn);
    expect(props.onReset).not.toHaveBeenCalled();
  });
});
