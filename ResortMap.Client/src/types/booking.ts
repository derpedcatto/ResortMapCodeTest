import type { GridCoords } from "@/types/map";

export type Booking = { room: string; guestName: string };

export type AddBookingRequest = { coords: GridCoords; booking: Booking };
