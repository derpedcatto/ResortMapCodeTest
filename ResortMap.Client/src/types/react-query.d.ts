import type { ApiError } from "@/api/apiError";

declare module "@tanstack/react-query" {
  interface Register {
    defaultError: ApiError;
  }
}
