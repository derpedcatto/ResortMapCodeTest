import type { GridCoords } from "@/types/map";
import type { AddBookingRequest } from "@/types/booking";
import { http } from "./http";

export function fetchBookedCabanas(
  signal?: AbortSignal,
): Promise<GridCoords[]> {
  return http.get("/api/booking", { signal }).json<GridCoords[]>();
}

export async function addBooking(request: AddBookingRequest): Promise<void> {
  await http.post("/api/booking", { json: request });
}
