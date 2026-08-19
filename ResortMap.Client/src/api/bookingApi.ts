import type { ResortMapGridCoords } from "@/types/map";
import type { AddBookingRequest } from "@/types/booking";
import { apiRequest } from "./apiClient";

export function fetchBookedCabanas(
  signal?: AbortSignal,
): Promise<ResortMapGridCoords[]> {
  return apiRequest<ResortMapGridCoords[]>("/api/booking", { signal });
}

export function addBooking(request: AddBookingRequest): Promise<void> {
  return apiRequest<void>("/api/booking", { method: "POST", body: request });
}
