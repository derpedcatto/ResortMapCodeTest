import "@testing-library/jest-dom/vitest";

// needed for BookingPage.test.tsx
// "jsdom has no ResizeObserver, and react-zoom-pan-pinch needs it"
class ResizeObserverStub {
  observe() {}
  unobserve() {}
  disconnect() {}
}

globalThis.ResizeObserver ??= ResizeObserverStub;
