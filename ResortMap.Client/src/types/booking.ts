import type { ResortMapGridCoords } from "./map";

export type Booking = { room: string; guestName: string };

export type AddBookingRequest = {
  coords: ResortMapGridCoords;
  booking: Booking;
};
