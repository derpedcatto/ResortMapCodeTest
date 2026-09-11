import { useEffect, useRef, useState, type SubmitEvent } from "react";
import { Button } from "@/components/atoms/Button/Button";
import { cabanaLabel } from "@/lib/map/cabanaLabel";
import type { Booking } from "@/types/booking";
import type { GridCoords } from "@/types/map";
import styles from "./BookingModal.module.scss";

type BookingModalProps = {
  coords: GridCoords;
  pending?: boolean;
  error?: string | null;
  onSubmit: (booking: Booking) => void;
  onClose: () => void;
};

export function BookingModal({
  coords,
  pending,
  error,
  onSubmit,
  onClose,
}: BookingModalProps) {
  const [room, setRoom] = useState("");
  const [guestName, setGuestName] = useState("");

  const pressedOnBackdrop = useRef(false);

  const cabana = cabanaLabel(coords);
  const canSubmit = room.trim() !== "" && guestName.trim() !== "" && !pending;
  const errorId = "booking-modal-error";
  const titleId = "booking-modal-title";

  useEffect(() => {
    function handleKeyDown(e: KeyboardEvent) {
      if (e.key === "Escape") {
        onClose();
      }
    }

    document.addEventListener("keydown", handleKeyDown);
    return () => document.removeEventListener("keydown", handleKeyDown);
  }, [onClose]);

  function handleSubmit(e: SubmitEvent<HTMLFormElement>) {
    e.preventDefault();
    if (!canSubmit) return;

    onSubmit({ room: room.trim(), guestName: guestName.trim() });
  }

  return (
    <div
      className={styles.backdrop}
      data-testid="booking-modal-backdrop"
      onClick={(e) => {
        if (pressedOnBackdrop.current && e.target === e.currentTarget) {
          onClose();
        }
      }}
      onPointerDown={(e) => {
        pressedOnBackdrop.current = e.target === e.currentTarget;
      }}
    >
      <form
        role="dialog"
        aria-modal="true"
        aria-labelledby={titleId}
        className={styles.modal}
        onSubmit={handleSubmit}
      >
        <h2 id={titleId}>Book Cabana {cabana}</h2>

        <div className={styles.field}>
          <label className={styles.label} htmlFor="booking-modal-room">
            Room number
          </label>

          <input
            id="booking-modal-room"
            className={styles.input}
            type="text"
            inputMode="numeric"
            value={room}
            onChange={(e) => setRoom(e.target.value)}
            placeholder="101"
            disabled={pending}
            autoFocus
            aria-invalid={error ? true : undefined}
            aria-describedby={error ? errorId : undefined}
          />
        </div>

        <div className={styles.field}>
          <label className={styles.label} htmlFor="booking-modal-guest">
            Guest name
          </label>

          <input
            id="booking-modal-guest"
            className={styles.input}
            type="text"
            value={guestName}
            onChange={(e) => setGuestName(e.target.value)}
            placeholder="John Doe"
            disabled={pending}
            aria-invalid={error ? true : undefined}
            aria-describedby={error ? errorId : undefined}
          />
        </div>

        {error && (
          <p id={errorId} className={styles.error} role="alert">
            {error}
          </p>
        )}

        <div className={styles.buttonRow}>
          <Button variant="secondary" onClick={onClose} disabled={pending}>
            Cancel
          </Button>

          <Button type="submit" disabled={!canSubmit}>
            {pending ? "Booking..." : "Book"}
          </Button>
        </div>
      </form>
    </div>
  );
}
