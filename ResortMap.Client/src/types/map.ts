export type Map = { grid: string[] };
export type MapCoords = { row: number; col: number };
export type Booking = { room: string; guestName: string };

export type AddBookingRequest = { coords: MapCoords; booking: Booking };
