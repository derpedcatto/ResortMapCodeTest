import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { fetchBookedCabanas, addBooking } from "@/api/bookingApi";
import { cellKey } from "@/lib/map/cellKey";
import type { AddBookingRequest } from "@/types/booking";
import type { BookedCabanaSet, GridCoords } from "@/types/map";

const bookedCabanasKey = ["bookedCabanas"];

const toBookedSet = (coords: GridCoords[]): BookedCabanaSet =>
  new Set(coords.map(cellKey));

export function useBookedCabanas() {
  return useQuery({
    queryKey: bookedCabanasKey,
    queryFn: ({ signal }) => fetchBookedCabanas(signal),
    select: toBookedSet,
  });
}

export function useAddBooking() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: AddBookingRequest) => addBooking(request),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: bookedCabanasKey }),
  });
}
