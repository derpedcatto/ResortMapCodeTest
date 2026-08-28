import type { Booking } from "@/types/booking";
import type { GridCoords } from "@/types/map";

// modal window to input booking data
type BookingModalProps = {
  coords: GridCoords;
  pending?: boolean;
  error?: string | null;
  onSubmit: (booking: Booking) => void; // { room, guestName }
  onClose: () => void;
};

export function BookingModal({}: BookingModalProps) {
  return <></>;
}
