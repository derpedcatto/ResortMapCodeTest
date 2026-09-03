import { useCallback, useEffect, useState } from "react";
import { cabanaLabel } from "@/lib/map/cabanaLabel";
import { BookingLayout } from "@/components/templates/BookingLayout/BookingLayout";
import { BookingModal } from "@/components/organisms/BookingModal/BookingModal";
import { ResortMap } from "@/components/organisms/ResortMap/ResortMap";
import { useAddBooking, useBookedCabanas } from "@/hooks/useBooking";
import { useResortMap } from "@/hooks/useResortMap";
import type { Booking } from "@/types/booking";
import type { GridCoords, VisualTileGrid } from "@/types/map";

const NO_TILES: VisualTileGrid = [];

export function BookingPage() {
  const mapQuery = useResortMap();
  const bookedQuery = useBookedCabanas();
  const addBooking = useAddBooking();

  const [selected, setSelected] = useState<GridCoords | null>(null);
  const [justBooked, setJustBooked] = useState<GridCoords | null>(null);

  const isLoading = mapQuery.isPending || bookedQuery.isPending;
  const loadError = mapQuery.error ?? bookedQuery.error;

  useEffect(() => {
    if (!justBooked) return;

    const id = setTimeout(() => setJustBooked(null), 2000);
    return () => clearTimeout(id);
  }, [justBooked]);

  const handleSelectCabana = useCallback(
    (coords: GridCoords) => {
      // drop the result of the previous booking before the form opens again
      addBooking.reset();
      setSelected(coords);
    },
    [addBooking],
  );

  function handleSubmit(booking: Booking) {
    if (!selected) return;

    addBooking.mutate(
      { coords: selected, booking },
      {
        onSuccess: (_, vars) => {
          setSelected(null);
          setJustBooked(vars.coords);
        },
      },
    );
  }

  function statusText(): string | null {
    if (loadError) return `Could not load the resort map. ${loadError.message}`;
    if (isLoading) return "Loading the resort map…";
    if (justBooked) {
      return `Cabana ${cabanaLabel(justBooked)} is booked.`;
    }

    return null;
  }

  return (
    <BookingLayout
      title="Resort Cabana Booking"
      map={
        <ResortMap
          tiles={mapQuery.data ?? NO_TILES}
          status={statusText()}
          bookedCabanas={bookedQuery.data}
          onSelectCabana={handleSelectCabana}
        />
      }
      modal={
        selected && (
          <BookingModal
            coords={selected}
            pending={addBooking.isPending}
            error={addBooking.error?.message ?? null}
            onSubmit={handleSubmit}
            onClose={() => setSelected(null)}
          />
        )
      }
    />
  );
}
