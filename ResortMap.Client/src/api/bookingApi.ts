import type { AddBookingRequest, BookedCabanasResponse } from "@/types/map";
import { apiRequest } from "./apiClient";

export function fetchBookedCabanas(
  signal?: AbortSignal,
): Promise<BookedCabanasResponse> {
  return apiRequest<BookedCabanasResponse>("/api/booking", { signal });
}

export function addBooking(request: AddBookingRequest): Promise<void> {
  return apiRequest<void>("/api/booking", { method: "POST", body: request });
}
