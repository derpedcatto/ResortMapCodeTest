export type ResortMapGrid = { grid: string[] };
export type ResortMapGridCoords = { row: number; col: number };
export type Booking = { room: string; guestName: string };

export type AddBookingRequest = {
  coords: ResortMapGridCoords;
  booking: Booking;
};
export type BookedCabanasResponse = ResortMapGridCoords[];
