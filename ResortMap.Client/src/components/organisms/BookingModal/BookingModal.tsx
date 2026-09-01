import { useState, type SubmitEvent } from "react";
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

  const cabana = cabanaLabel(coords);
  const canSubmit = room.trim() !== "" && guestName.trim() !== "" && !pending;

  function handleSubmit(e: SubmitEvent<HTMLFormElement>) {
    e.preventDefault();
    if (!canSubmit) return;

    onSubmit({ room: room.trim(), guestName: guestName.trim() });
  }

  return (
    <div className={styles.backdrop} onClick={onClose}>
      <form
        className={styles.modal}
        onClick={(e) => e.stopPropagation()}
        onSubmit={handleSubmit}
      >
        <h2>Book Cabana {cabana}</h2>

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
          />
        </div>

        {error && <p className={styles.error}>{error}</p>}

        <div className={styles.buttonRow}>
          <Button variant="secondary" onClick={onClose} disabled={pending}>
            Cancel
          </Button>

          <Button type="submit" disabled={!canSubmit}>
            {pending ? "Booking…" : "Book"}
          </Button>
        </div>
      </form>
    </div>
  );
}
