import type { GridCoords } from "@/types/map";
import { apiRequest } from "./apiClient";
import type { AddBookingRequest } from "@/types/booking";

export function fetchBookedCabanas(
  signal?: AbortSignal,
): Promise<GridCoords[]> {
  return apiRequest<GridCoords[]>("/api/booking", { signal });
}

export function addBooking(request: AddBookingRequest): Promise<void> {
  return apiRequest<void>("/api/booking", { method: "POST", body: request });
}
