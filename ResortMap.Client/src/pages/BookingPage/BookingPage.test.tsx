import { render, screen, act } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BookingPage } from "./BookingPage";
import * as mapApi from "@/api/mapApi";
import * as bookingApi from "@/api/bookingApi";
import type { ResortMapDto } from "@/types/map";
import type { GridCoords } from "@/types/map";
import { tileTypeToChar as char } from "@/lib/map/decodeMapGrid";

vi.mock("@/api/mapApi");
vi.mock("@/api/bookingApi");

vi.mock("@/lib/map/spritesAssets", () => ({
  SPRITES: {
    cabana: { src: "/cabana.png", alt: "Cabana" },
    pool: { src: "/pool.png", alt: "Pool" },
    pathStraight: { src: "/pathStraight.png", alt: "Path" },
    background: { src: "/parchmentBasic", alt: "" },
    empty: { src: "", alt: "" },
  },
}));

const MAP_DTO: ResortMapDto = {
  grid: [`${char.cabana}${char.empty}`, `${char.empty}${char.empty}`],
};
const BOOKED: GridCoords[] = [];

function setup() {
  const user = userEvent.setup({ advanceTimers: vi.advanceTimersByTime });
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
      mutations: { retry: false },
    },
  });

  render(
    <QueryClientProvider client={queryClient}>
      <BookingPage />
    </QueryClientProvider>,
  );

  return user;
}

beforeEach(() => {
  vi.useFakeTimers({ shouldAdvanceTime: true });
  vi.mocked(mapApi.fetchMap).mockResolvedValue(MAP_DTO);
  vi.mocked(bookingApi.fetchBookedCabanas).mockResolvedValue(BOOKED);
  vi.mocked(bookingApi.addBooking).mockResolvedValue(undefined);
});

afterEach(() => {
  vi.useRealTimers();
  vi.restoreAllMocks();
});

const findCabana = () => screen.findByRole("button", { name: /cabana 0-0/i });

const fillDataAndBook = async (user: ReturnType<typeof userEvent.setup>) => {
  await user.type(screen.getByLabelText("Room number"), "101");
  await user.type(screen.getByLabelText("Guest name"), "Alice");
  await user.click(screen.getByRole("button", { name: "Book" }));
};

describe("BookingPage", () => {
  it("shows loading state while map queries are pending", () => {
    vi.mocked(mapApi.fetchMap).mockReturnValue(new Promise(() => {}));
    vi.mocked(bookingApi.fetchBookedCabanas).mockReturnValue(
      new Promise(() => {}),
    );

    setup();

    expect(screen.getByText("Loading the resort map...")).toBeInTheDocument();
  });

  it("shows error state on query fail", async () => {
    vi.mocked(mapApi.fetchMap).mockRejectedValue(new Error("Network down"));

    setup();

    expect(
      await screen.findByText(/Could not load the resort map/i),
    ).toBeInTheDocument();
    expect(screen.getByText(/Network down/i)).toBeInTheDocument();
  });

  it("renders map after data load", async () => {
    setup();

    expect(
      await screen.findByText("Cabana 0-0 is available"),
    ).toBeInTheDocument();
  });

  it("clicking a cabana opens the modal with correct label", async () => {
    const user = setup();

    await user.click(await findCabana());

    expect(await screen.findByText("Book Cabana 0-0")).toBeInTheDocument();
  });

  it("on booking submit - modal closes, success banner appears and disappears", async () => {
    const user = setup();

    await user.click(await findCabana());
    await fillDataAndBook(user);

    expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
    expect(screen.getByText("Cabana 0-0 is booked.")).toBeInTheDocument();

    await act(async () => {
      vi.advanceTimersByTime(2000);
    });

    expect(screen.queryByText("Cabana 0-0 is booked.")).not.toBeInTheDocument();
  });

  it("on booking fail - modal stays open, error message shown", async () => {
    vi.mocked(bookingApi.addBooking).mockRejectedValue(
      new Error("Server error"),
    );

    const user = setup();

    await user.click(await findCabana());
    await fillDataAndBook(user);

    expect(await screen.findByRole("dialog")).toBeInTheDocument();
    expect(screen.getByText("Server error")).toBeInTheDocument();
  });

  it("on cancel while mutation is pending - modal stays open", async () => {
    vi.mocked(bookingApi.addBooking).mockReturnValue(new Promise(() => {}));

    const user = setup();

    await user.click(await findCabana());
    await fillDataAndBook(user);

    expect(screen.getByText("Booking...")).toBeInTheDocument();

    await user.click(screen.getByRole("button", { name: "Cancel" }));
    expect(screen.getByRole("dialog")).toBeInTheDocument();
  });

  it("selecting a new cabana after a failed booking clears previous error", async () => {
    vi.mocked(bookingApi.addBooking).mockRejectedValue(
      new Error("Server error"),
    );

    const user = setup();
    const cabana = await findCabana();

    await user.click(cabana);
    await fillDataAndBook(user);

    expect(await screen.findByText("Server error")).toBeInTheDocument();

    await user.click(screen.getByRole("button", { name: "Cancel" }));
    await user.click(cabana);

    expect(await screen.findByRole("dialog")).toBeInTheDocument();
    expect(screen.queryByText("Server error")).not.toBeInTheDocument();
  });
});
