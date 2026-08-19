import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { fetchBookedCabanas, addBooking } from "@/api/bookingApi";
import type { AddBookingRequest } from "@/types/map";

const bookedCabanasKey = ["bookedCabanas"];

export function useBookedCabanas() {
  return useQuery({
    queryKey: bookedCabanasKey,
    queryFn: ({ signal }) => fetchBookedCabanas(signal),
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
