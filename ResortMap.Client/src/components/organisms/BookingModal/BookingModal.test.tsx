import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, it, expect, vi } from "vitest";
import { BookingModal } from "./BookingModal";

const defaultProps = {
  coords: { row: 2, col: 5 },
  onSubmit: vi.fn(),
  onClose: vi.fn(),
};

function setup(overrides = {}) {
  const user = userEvent.setup();
  const props = { ...defaultProps, ...overrides };

  return { user, props, ...render(<BookingModal {...props} />) };
}

describe("BookingModal", () => {
  it("submits trimmed down booking data", async () => {
    const { user, props } = setup();

    await user.type(screen.getByLabelText("Room number"), "  101   ");
    await user.type(screen.getByLabelText("Guest name"), "  Alice   ");
    await user.click(screen.getByRole("button", { name: "Book" }));

    expect(props.onSubmit).toHaveBeenCalledWith({
      room: "101",
      guestName: "Alice",
    });
  });

  it("doesnt submit when fields are empty", async () => {
    const { user, props } = setup();

    await user.click(screen.getByRole("button", { name: "Book" }));
    expect(props.onSubmit).not.toHaveBeenCalled();
  });

  it("closes on cancel button clicked", async () => {
    const { user, props } = setup();

    await user.click(screen.getByRole("button", { name: "Cancel" }));
    expect(props.onClose).toHaveBeenCalled();
  });

  it("disables inputs on pending state", async () => {
    setup({ pending: true });

    expect(screen.getByLabelText("Room number")).toBeDisabled();
    expect(screen.getByLabelText("Guest name")).toBeDisabled();
    expect(screen.getByRole("button", { name: "Booking..." })).toBeDisabled();
  });

  it("renders error text", async () => {
    setup({ error: "Failed to book cabana" });

    expect(screen.getByText("Failed to book cabana")).toBeInTheDocument();
  });

  it("calls onClose on backdrop click but not on modal body click", async () => {
    const { user, props } = setup();

    const modal = screen.getByRole("dialog");
    const backdrop = modal.parentElement!;

    await user.click(modal);
    expect(props.onClose).not.toHaveBeenCalled();

    await user.click(backdrop);
    expect(props.onClose).toHaveBeenCalledOnce();
  });
});
